using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace IRTicker2 {
    class OrderBook {
        public string side = "";
        public bool changed = false;
        public ConcurrentDictionary<decimal, ConcurrentDictionary<string, Form1.socketOBObjData>> priceDict = new ConcurrentDictionary<decimal, ConcurrentDictionary<string, Form1.socketOBObjData>>();
        public ConcurrentDictionary<string, decimal> guidDict = new ConcurrentDictionary<string, decimal>();

        public OrderBook(string _side) {
            side = _side;
        }

        public bool findGuid(string oGuid) {
            if (guidDict.ContainsKey(oGuid)) {
                decimal price = guidDict[oGuid];
                Debug.Print(DateTime.Now + " - order existed at $" + price);
                if (priceDict.ContainsKey(price)) {
                    Debug.Print(DateTime.Now + " - this price does exist");
                    if (price == priceDict.Keys.Max() || price == priceDict.Keys.Min()) {
                        Debug.Print(DateTime.Now + " - the price is at the top of the order book (either min or max");
                    }
                    else {
                        Debug.Print("it appears the price isn't at the top of the orderbook.  min: " + priceDict.Keys.Min() + " and max: " + priceDict.Keys.Max());
                    }
                    if (priceDict[price].ContainsKey(oGuid)) {
                        Debug.Print("The pricedict at this price also contains this GUID.");
                        return true;
                    }
                    else {
                        Debug.Print("priceDict does not know of this guid at this price");
                        return false;
                    }
                }
                else {
                    Debug.Print("priceDict doesn't have this price at all...");
                    return false;
                }
            }
            else {
                Debug.Print("guidDict doesn't know of this guid");
                return false;
            }
        }

        public bool addEvent(Form1.socketOBObjData eventObj) {

            ///////////////////
            ////// NEW EVENT ALREADY EXISTS IN GUIDDICT
            ///////////////////
            if (guidDict.ContainsKey(eventObj.OrderGuid)) {
                decimal price = guidDict[eventObj.OrderGuid];
                Debug.Print(DateTime.Now + " (" + side +  ") - adding an event that alread exists in guidDict: " + eventObj.Price);

                if (guidDict[eventObj.OrderGuid] == eventObj.Price) Debug.Print("guidDict has the correct price...");
                else Debug.Print(DateTime.Now + " (" + side + ")  - guidDict has a different price?? " + guidDict[eventObj.OrderGuid]);

                if (priceDict.ContainsKey(price)) {  // priceDict has this price though
                    Debug.Print(DateTime.Now + " (" + side + ")  - price dict has the price (not totally surprising as it could be a different order(s)...)");
                    if (priceDict[price].ContainsKey(eventObj.OrderGuid)) {
                        Debug.Print(DateTime.Now + " (" + side + ")  - priceDict also has this guid.  this is more surprising.  Will overwrite it.");
                        priceDict[price][eventObj.OrderGuid] = eventObj;
                        return true;
                    }
                    else {
                        Debug.Print(DateTime.Now + " (" + side + ")  - priceDict at that price didn't have this guid.  let's search");
                        foreach (KeyValuePair<decimal, ConcurrentDictionary<string, Form1.socketOBObjData>> priceLevel in priceDict) {
                            foreach (KeyValuePair<string, Form1.socketOBObjData> order in priceLevel.Value) {
                                if (order.Value.OrderGuid == eventObj.OrderGuid) {
                                    Debug.Print(DateTime.Now + " (" + side + ")  - found the guid at a different price? this should never happen  " + order.Value.Price + " which should be the same as " + priceLevel.Key);
                                    priceLevel.Value.TryRemove(eventObj.OrderGuid, out Form1.socketOBObjData ignore);
                                    priceDict[price][eventObj.OrderGuid] = eventObj;
                                    return true;
                                }
                            }
                        }
                        Debug.Print(DateTime.Now + " (" + side + ")  - I guess it didn't exist anywhere else");
                        priceDict[price][eventObj.OrderGuid] = eventObj;
                        return true;
                    }
                }
                else { // priceDict doesn't have this price at all
                    Debug.Print(DateTime.Now + " (" + side + ")  - priceDict doesn't have this price.  will search for the guid");
                    foreach (KeyValuePair<decimal, ConcurrentDictionary<string, Form1.socketOBObjData>> priceLevel in priceDict) {
                        foreach (KeyValuePair<string, Form1.socketOBObjData> order in priceLevel.Value) {
                            if (order.Value.OrderGuid == eventObj.OrderGuid) {
                                Debug.Print(DateTime.Now + " (" + side + ")  - found the guid at a different price? this should never happen  " + order.Value.Price + " which should be the same as " + priceLevel.Key);
                                priceLevel.Value.TryRemove(eventObj.OrderGuid, out Form1.socketOBObjData ignore);
                                ConcurrentDictionary<string, Form1.socketOBObjData> tempDict1 = new ConcurrentDictionary<string, Form1.socketOBObjData>();
                                tempDict1[eventObj.OrderGuid] = eventObj;
                                priceDict[eventObj.Price] = tempDict1;
                                return true;
                            }
                        }
                    }
                    Debug.Print(DateTime.Now + " (" + side + ")   - nup, can't find the guid anywhere");
                    ConcurrentDictionary<string, Form1.socketOBObjData> tempDict = new ConcurrentDictionary<string, Form1.socketOBObjData>();
                    tempDict[eventObj.OrderGuid] = eventObj;
                    priceDict[eventObj.Price] = tempDict;
                    return true;
                }
            }

            /////////////////////////////////////////
            ////// guidDict doesn't contain this price, which is expected.
            /////////////////////////////////////////
            else {
                guidDict[eventObj.OrderGuid] = eventObj.Price;
                if (priceDict.ContainsKey(eventObj.Price)) {
                    if (priceDict[eventObj.Price].ContainsKey(eventObj.OrderGuid)) {
                        Debug.Print(DateTime.Now + " (" + side + ")  - guid didn't exist in guidDict, but it does exist (at " + eventObj.Price + ") in priceDict?  will overwrite");
                        priceDict[eventObj.Price][eventObj.OrderGuid] = eventObj;
                        guidDict[eventObj.OrderGuid] = eventObj.Price;
                        return true;

                    }
                    else {  // orderGuid doesn't exist, which is expected
                        if (!priceDict[eventObj.Price].TryAdd(eventObj.OrderGuid, eventObj)) {
                            Debug.Print(DateTime.Now + " (" + side + ")  - adding the new price failed for some reason... " + eventObj.Price);
                        }
                        guidDict[eventObj.OrderGuid] = eventObj.Price;
                        return true;
                    }
                }
                else {  // this is a new price
                    ConcurrentDictionary<string, Form1.socketOBObjData> tempDict1 = new ConcurrentDictionary<string, Form1.socketOBObjData>();
                    tempDict1[eventObj.OrderGuid] = eventObj;
                    priceDict[eventObj.Price] = tempDict1;
                    return true;
                }
            }
            return false;
        }

        public bool changeEvent(Form1.socketOBObjData eventObj) {

            if (guidDict.ContainsKey(eventObj.OrderGuid)) {  // this is good
                decimal price = guidDict[eventObj.OrderGuid];
                if (priceDict[price].ContainsKey(eventObj.OrderGuid)) {  // the price dict entry for this price has an orderGuid that matches.  good.
                    if (eventObj.Volume == 0) {
                        if (priceDict[price].Count == 1) {  // remove the whole price from the priceDict
                            priceDict.TryRemove(price, out ConcurrentDictionary<string, Form1.socketOBObjData> ignore);
                            return true;
                        }
                        else if (priceDict[price].Count > 1) {  // just remove the order at this price
                            priceDict[price].TryRemove(eventObj.OrderGuid, out Form1.socketOBObjData ignore);
                            return true;
                        }
                        else {
                            Debug.Print(DateTime.Now + " (" + side + ")  - changeEvente: I guess the number of orders at this price level is 0 ?? how:  " + price);
                            return false;
                        }
                    }
                    else { // vol > 0 i guess
                        priceDict[price][eventObj.OrderGuid].Volume = eventObj.Volume;
                        return true;
                    }
                }
                else {  // no order at this price?
                    Debug.Print(DateTime.Now + " (" + side + ")  - trying to change volume, but no matching order at this price... " + price);
                    return false;
                }
            }
            else {  // the guidDict has no orderGuid ??
                Debug.Print(DateTime.Now + " (" + side + ")  - changeEvent: orderGuid dictionary has no order at this guid");
                return false;
            }
            return false;
        }

        public bool removeEvent(Form1.socketOBObjData eventObj) {

            if (guidDict.ContainsKey(eventObj.OrderGuid)) {
                //decimal price = guidDict[eventObj.OrderGuid];
                if (guidDict.TryRemove(eventObj.OrderGuid, out decimal price)) {
                    if (priceDict[price].ContainsKey(eventObj.OrderGuid)) {  // good, delete it
                        if (priceDict[price].Count == 1) {  // remove the whole price from the priceDict
                            priceDict.TryRemove(price, out ConcurrentDictionary<string, Form1.socketOBObjData> ignore);
                            return true;
                        }
                        else if (priceDict[price].Count > 1) {  // just remove the order at this price
                            priceDict[price].TryRemove(eventObj.OrderGuid, out Form1.socketOBObjData ignore);
                            return true;
                        }
                        else {
                            Debug.Print(DateTime.Now + " (" + side + ")  - removeEvent: I guess the number of orders at this price level is 0 ?? how:  " + price);
                            return false;
                        }
                    }
                    else {  // we have the price, but the order doesn't appear to exist at this price :/
                        Debug.Print(DateTime.Now + " (" + side + ")  - trying to delete order, but no matching order at this price.. " + price);
                        return false;
                    }
                }
                else {
                    Debug.Print(DateTime.Now + " - couldn't remove entry from guidDict: " + price);
                }
            }
            else {  // the guidDict does not have this orderGuid
                Debug.Print(DateTime.Now + " (" + side + ")  - removeEvent: no orderguid in the guidDict?");
                return false;
            }
            return false;
        }
    }
}
