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

namespace IRTicker2 {
    public partial class Form1 : Form {

        OrderBook bidOBobj;
        OrderBook offerOBobj;
        decimal bestBid = 0;
        decimal bestOffer = 0;

        public Form1() {
            InitializeComponent();
            Connect();
        }

        private void Connect() {

            bidOBobj = new OrderBook();
            bidOBobj.side = "Bid";
            offerOBobj = new OrderBook();
            offerOBobj.side = "Offer";
            var IRWS = new WebSocket("wss://websockets.independentreserve.com");

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
        }

        private void ReadMessage(string msg) {
            if (msg.Contains("Subscriptions") || msg.Contains("Heartbeat")) {
                Debug.Print("heartbeat or subscription");
                return;
            }
            else {
                socketOBObj OBevent = JsonConvert.DeserializeObject<socketOBObj>(msg);
                //Debug.Print("Event: " + OBevent.Event);
                OrderBook OBobj;
                if (OBevent.Data.OrderType == "LimitBid") OBobj = bidOBobj;
                else if (OBevent.Data.OrderType == "LimitOffer") OBobj = offerOBobj;
                else {
                    Debug.Print(DateTime.Now + " - Ordertype not limitbid/offer, it was: " + OBevent.Data.OrderType);
                    return;
                }
                
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

                // draw it?
                printOB(OBobj, OBevent.Data.OrderType);

                //spread_dyn_label.Text = (bestOffer - bestBid).ToString();
                spread_dyn_label.Invoke((MethodInvoker)(() => spread_dyn_label.Text = (bestOffer - bestBid).ToString()));
            }
        }

        private bool printOB(OrderBook OBobj, string side) {
            IOrderedEnumerable<KeyValuePair<decimal, ConcurrentDictionary<string, socketOBObjData>>> orderedInput;

            if (side == "LimitBid") {
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
            }
            else {
                Debug.Print(DateTime.Now + " - BIG PRoblems, the snapshot for offers was empty");
            }
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
    }
}
