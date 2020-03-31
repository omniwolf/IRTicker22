using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;
using Websocket.Client;

namespace IRTicker2 {
    public partial class Form1 : Form {

        OrderBook bidOBobj;
        OrderBook offerOBobj;
        private bool snapShotLoaded = false;
        ConcurrentDictionary<int, socketOBObj> orderBuffer;
        int nonce;
        //WebSocket IRWS;
        WebsocketClient client;
        private decimal bestBid = 0;
        private decimal bestOffer = 0;
        private int badNonceCount = 0;
        private int sleepFreq = 400;

        public Form1() {
            InitializeComponent();
            timerSleep.Text = sleepFreq.ToString();
            Connect();
        }

        private void Connect() {

            // initialise the vars
            orderBuffer = new ConcurrentDictionary<int, socketOBObj>();
            bidOBobj = new OrderBook("Bid");
            offerOBobj = new OrderBook("Offer");
            //IRWS = new WebSocket("wss://websockets.independentreserve.com");
            //IRWS.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            nonce = -1;
            snapShotLoaded = false;
            badNonceCount = 0;
            if (!UITimer.IsBusy) UITimer.RunWorkerAsync();

            

            if (!WSClient_thread.IsBusy) WSClient_thread.RunWorkerAsync();

            string OBsnapshot = "";
            using (WebClient wc = new WebClient()) {
                OBsnapshot = wc.DownloadString("https://api.independentreserve.com/Public/GetAllOrders?primaryCurrencyCode=xbt&secondaryCurrencyCode=aud");
            }
            parseOBsnapshot(OBsnapshot);



            /*
            IRWS.OnMessage += (sender, e) => {
                //Debug.Print("got a message: " + e.Data.ToString());
                ReadMessage(e.Data);
            };

            IRWS.OnError += (sender, e) => {
                Debug.Print("error: " + e.Message.ToString());
            };

            IRWS.OnClose += (sender, e) => {
                Debug.Print("closed!");
            };

            IRWS.OnOpen += (sender, e) => {
                Debug.Print("opened!");
                string OBsnapshot = "";
                using (WebClient wc = new WebClient()) {
                    OBsnapshot = wc.DownloadString("https://api.independentreserve.com/Public/GetAllOrders?primaryCurrencyCode=xbt&secondaryCurrencyCode=aud");
                }
                parseOBsnapshot(OBsnapshot);
            };

            IRWS.Connect();
            string subscribeSTR = "{\"Event\":\"Subscribe\",\"Data\":[\"orderbook-xbt-aud\"]}";
            IRWS.Send(subscribeSTR);
            */
        }

        private void ReadMessage(string msg) {
            if (msg.Contains("Subscriptions") || msg.Contains("Heartbeat")) {
                Debug.Print("heartbeat or subscription");
                return;
            }
            else {
                socketOBObj OBevent = JsonConvert.DeserializeObject<socketOBObj>(msg);
                //Debug.Print("Event: " + OBevent.Event);

                if (!validateNonce(OBevent)) {
                    resetSocket("Couldn't add order to buffer?  Nonce: " + nonce + " and order nonce: " + OBevent.Nonce);
                }

                // so if we haven't grabbed the snapshot yet, we just build a buffer to replay over it once we have
                if (!snapShotLoaded) {
                    orderBuffer[OBevent.Nonce] =  OBevent;
                    Debug.Print("Buffering a " + OBevent.Event + " event, total: " + orderBuffer.Count);
                    return;
                }

                if (!UITimer.IsBusy) UITimer.RunWorkerAsync();

                //ApplyEventToOB(OBevent, true);

                // if the orderBuffer dict has anything in it, then it means we have buffered an out of order event due to a 
                // non -contiguous nonce.  Let's try see if the next nonce is in the buffer and apply in order.
                //if (orderBuffer.Count > 0) TryRecoverFromNonce();  
            }
        }

        private int TryRecoverFromNonce() {

            //Debug.Print(" -- Applying " + orderBuffer.Count + " orders from the buffer...");
            int numOrders = orderBuffer.Count;

            if (nonce == -1) {  // first event
                if (numOrders <= 0) {
                    return numOrders;
                }
                nonce = orderBuffer.Keys.Min() - 1;
            }

            // If our buffer contains the next nonce, then we apply it to the OB, and delete that buffer entry
            // If the buffer doesn't, we just exit this method and check the next nonce when the next event comes in.
            // if the buffer grows bigger than 4 elements (see validateNonce(...) method), we bail and start fresh.
            while (orderBuffer.ContainsKey(nonce + 1)) {
                nonce += 1;
                //Debug.Print(" -- found it!  applying to OB...");
                if (orderBuffer.TryRemove(nonce, out socketOBObj currentEvent)) {
                    ApplyEventToOB(currentEvent, false);
                }
                else {
                    resetSocket("OrderBuffer missing an event it should have?? " + nonce);
                    return -1;
                }
            }

            if (orderBuffer.Count != 0) {
                Debug.Print(DateTime.Now + " - we have a missing nonce, " + orderBuffer.Count + " orders in the buffer");
                if (orderBuffer.Count > 10) {
                    resetSocket("Too many out of order Nonces, let's reset");
                    return -1;
                }
            }
            return numOrders - orderBuffer.Count;
        }

        private void resetSocket(string debugMsg) {
            Debug.Print(DateTime.Now + " - " + debugMsg);
            //IRWS.Close();
            //client.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "resetting");
            
            if (UITimer.IsBusy) UITimer.CancelAsync();
            Connect();
        }

        private bool validateNonce(socketOBObj OBevent) {

            /*if (nonce == -1) {  // first event
                nonce = OBevent.Nonce;
                return true;
            }*/
            if (orderBuffer.TryAdd(OBevent.Nonce, OBevent)) {
                return true;
            }
            else {
                Debug.Print("could not add an event to the order buffer??  current nonce: " + nonce + " and event nonce: " + OBevent.Nonce);
                return false;
            }
            if (OBevent.Nonce == nonce + 1) {  // nonce looks good
                //nonce += 1;
                return true;
            }

            Debug.Print("NONCE out of order, we have " + orderBuffer.Count + " order(s) buffered.  Received " + OBevent.Nonce + ", expected " + (nonce + 1));

            if (badNonceCount < 10) {  // if we haven't caught up by 4 bad nonces, we're not catching up.
                badNonceCount++;
                return false;
            }
            else {
                Debug.Print("Too many out of order Nonces, let's reset");
                //IRWS.Close();
                client.Stop(System.Net.WebSockets.WebSocketCloseStatus.ProtocolError, "bad nonces");

                UITimer.CancelAsync();
                Connect();
            }
            return false;
        }

        private void ApplyEventToOB(socketOBObj OBevent, bool draw) {
            OrderBook OBobj;
            if (OBevent.Data.OrderType == "LimitBid") OBobj = bidOBobj;
            else if (OBevent.Data.OrderType == "LimitOffer") OBobj = offerOBobj;
            else {
                Debug.Print(DateTime.Now + " - Ordertype not limitbid/offer, it was: " + OBevent.Data.OrderType + " Event: " + OBevent.Event);
                if (OBevent.Data.OrderType.EndsWith("Bid")) OBobj = bidOBobj;
                else if (OBevent.Data.OrderType.EndsWith("Offer")) OBobj = offerOBobj;
                else {
                    Debug.Print("what order type is this?? " + OBevent.Data.OrderType);
                    return;
                }

                OBobj.findGuid(OBevent.Data.OrderGuid);

                return;
            }

            OBobj.changed = true;

            switch (OBevent.Event) {
                case "NewOrder":
                    OBobj.addEvent(OBevent.Data);
                    break;

                case "OrderChanged":
                    OBobj.changeEvent(OBevent.Data);
                    break;

                case "OrderCanceled":
                    OBobj.removeEvent(OBevent.Data);
                    break;
            }

            if (draw) {
                // draw it?
                printOB(OBobj);
            }
        }

        private bool printOB(OrderBook OBobj) {

            if (!IsHandleCreated) return false;

            IOrderedEnumerable<KeyValuePair<decimal, ConcurrentDictionary<string, socketOBObjData>>> orderedInput;

            if (OBobj.side == "Bid") {
                orderedInput = OBobj.priceDict.OrderByDescending(key => key.Key);
                bestBid = orderedInput.FirstOrDefault().Key;

                //best_bid_dyn_label.Text = bestBid.ToString();
                best_bid_dyn_label.Invoke((MethodInvoker)(() => best_bid_dyn_label.Text = bestBid.ToString()));
            }
            else {
                orderedInput = OBobj.priceDict.OrderBy(key => key.Key);
                bestOffer = orderedInput.FirstOrDefault().Key;
                //best_offer_dyn_label.Text = bestOffer.ToString();
                best_offer_dyn_label.Invoke((MethodInvoker)(() => best_offer_dyn_label.Text = bestOffer.ToString()));
            }
            OBobj.changed = false;

            decimal spread = bestOffer - bestBid;

            //spread_dyn_label.Text = (bestOffer - bestBid).ToString();
            spread_dyn_label.Invoke((MethodInvoker)(() => spread_dyn_label.Text = spread.ToString()));
            if (spread < 0) Debug.Print(DateTime.Now + " - spread " + spread);

            return true;
        }

        private void parseOBsnapshot(string snapshot) {
            snapshotRoot snapShotOB = JsonConvert.DeserializeObject<snapshotRoot>(snapshot);
            
            if (snapShotOB.BuyOrders.Count > 0) {
                foreach (snapOrder BuyO in snapShotOB.BuyOrders) {
                    socketOBObjData tempOBObj = new socketOBObjData();
                    tempOBObj.OrderGuid = BuyO.Guid;
                    tempOBObj.OrderType = "LimitBid";
                    tempOBObj.Price = BuyO.Price;
                    tempOBObj.Volume = BuyO.Volume;
                    tempOBObj.Pair = "XBT-AUD";
                    if (!bidOBobj.addEvent(tempOBObj)) {
                        Debug.Print(DateTime.Now + " (Bid) - couldn't add an order from snap shot??");
                    }
                }
                printOB(bidOBobj);
            }
            else {
                Debug.Print(DateTime.Now + " - BIG PRoblems, the snapshot for bids was empty");
            }

            if (snapShotOB.SellOrders.Count > 0) {
                foreach (snapOrder SellO in snapShotOB.SellOrders) {
                    socketOBObjData tempOBObj = new socketOBObjData();
                    tempOBObj.OrderGuid = SellO.Guid;
                    tempOBObj.OrderType = "LimitOffer";
                    tempOBObj.Price = SellO.Price;
                    tempOBObj.Volume = SellO.Volume;
                    tempOBObj.Pair = "XBT-AUD";
                    if (!offerOBobj.addEvent(tempOBObj)) {
                        Debug.Print(DateTime.Now + " (Offer) - couldn't add an order from snap shot??");
                    }
                }
                printOB(offerOBobj);
            }
            else {
                Debug.Print(DateTime.Now + " - BIG PRoblems, the snapshot for offers was empty");
            }


            if (orderBuffer.Count > 0) {
                nonce = orderBuffer.Keys.Min() - 1;  // set the nonce to the value before the first nonce value in our buffer
                Debug.Print("Start-up buffer has " + orderBuffer.Count + " order(s), will replay them now starting at nonce " + (nonce + 1));

                while (orderBuffer.ContainsKey(nonce + 1)) {
                    nonce += 1;
                    
                    if (orderBuffer.TryRemove(nonce, out socketOBObj currentEvent)) {
                        ApplyEventToOB(currentEvent, false);
                    }
                    else {
                        resetSocket("OrderBuffer missing an event it should have?  resetting...");
                    }
                }

                if (orderBuffer.Count > 0) {  // this means we have out of order nonces in the buffer.  bad news; we can't recover from this.
                    resetSocket("Trying to replay initial buffer over the OB and there is missing nonces.  reseting.");
                    return;
                }
            }
            snapShotLoaded = true;
        }


        private class socketOBObj {
            public string Channel { get; set; }
            public int Nonce { get; set; }
            public socketOBObjData Data { get; set; }
            public string Event { get; set; }
        }

        public class socketOBObjData {
            public string OrderType { get; set; }
            public string OrderGuid { get; set; }
            public string Pair { get; set; }
            public decimal Price { get; set; }
            public decimal Volume { get; set; }
        }

        public class snapOrder {
            public string Guid { get; set; }
            public decimal Price { get; set; }
            public decimal Volume { get; set; }
        }

        public class snapshotRoot {
            public List<snapOrder> BuyOrders { get; set; }
            public List<snapOrder> SellOrders { get; set; }
            public string PrimaryCurrencyCode { get; set; }
            public string SecondaryCurrencyCode { get; set; }
            public DateTime CreatedTimestampUtc { get; set; }
        }

        private void UITimer_DoWork(object sender, DoWorkEventArgs e) {
            while (true) {
                if (UITimer.CancellationPending) break;
                int numOrders = TryRecoverFromNonce();
                UITimer.ReportProgress(1, numOrders.ToString());
                Thread.Sleep(sleepFreq);
            }
        }

        private void UITimer_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (IsHandleCreated) orders_processed_dyn_label.Invoke((MethodInvoker)(() => orders_processed_dyn_label.Text = (string)e.UserState));
            if (bidOBobj.changed) printOB(bidOBobj);
            if (offerOBobj.changed) printOB(offerOBobj);
        }

        private void button1_Click(object sender, EventArgs e) {

            if (Int32.TryParse(timerSleep.Text, out int sleepFreq1) && sleepFreq1 > 0) {
                sleepFreq = sleepFreq1;
                resetSocket("========= Setting the timer to " + timerSleep.Text + "ms. ==============");
            }
            else {
                Debug.Print("timer sleep not a number??");
            }
        }

        private void WSClient_thread_DoWork(object sender, DoWorkEventArgs e) {
            var exitEvent = new ManualResetEvent(false);
            var url = new Uri("wss://websockets.independentreserve.com");

            using (client = new WebsocketClient(url)) {
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                client.ReconnectionHappened.Subscribe(info =>
                    Debug.Print($"Reconnection happened, type: {info.Type}"));

                client.MessageReceived.Subscribe(msg => ReadMessage(msg.Text));
                client.Start().Wait();

                Task.Run(() => client.Send("{\"Event\":\"Subscribe\",\"Data\":[\"orderbook-xbt-aud\"]}"));

                exitEvent.WaitOne();
            }

        }
    }
}
