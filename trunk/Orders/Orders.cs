using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GCheckout.Checkout;
using GCheckout.Util;
using GCheckout.AutoGen;
using GCheckout.OrderProcessing;


namespace Orders
{
    public static class Orders
    {

        /// <summary>
        /// Gets the item description.
        /// </summary>
        /// <param name="item_id">The item_id.</param>
        /// <returns>Description of the item wich is the summery of the page</returns>
        public static string GetItemDescription(int item_id)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();

            var d = from p in db.pages where (p.page_id == item_id && p.status == "published") select p.summary;

            List<string> description = d.ToList();

            return description[0].ToString();
        }

        public static string ProcessNotification(string xmlFile)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            string SerialNumber = "";
            ///Read XML file
            StreamReader strReader = new StreamReader(xmlFile);
            string requestedXml = strReader.ReadToEnd();
            ///Act on XML file
            switch (EncodeHelper.GetTopElement(requestedXml))
            {
                case "new-order-notification":
                    NewOrderNotification N1 = (NewOrderNotification)EncodeHelper.Deserialize(requestedXml, typeof(NewOrderNotification));
                    SerialNumber = N1.serialnumber;
                    Int64 OrderNumber1 = Int64.Parse(N1.googleordernumber);
                    string ShipToName = N1.buyershippingaddress.contactname;
                    string ShipToAddress1 = N1.buyershippingaddress.address1;
                    string ShipToAddress2 = N1.buyershippingaddress.address2;
                    string ShipToCity = N1.buyershippingaddress.city;
                    string ShipToState = N1.buyershippingaddress.region;
                    string ShipToZip = N1.buyershippingaddress.postalcode;

                    order newOrder = new order();
                    newOrder.google_order_number = OrderNumber1;
                    newOrder.order_date = N1.timestamp;
                    newOrder.order_by = "test";
                    newOrder.sub_total = 0;
                    newOrder.total = N1.ordertotal.Value;
                    newOrder.status = "Order placed";
                    newOrder.root_id = 1;

                    db.orders.InsertOnSubmit(newOrder);
                    db.SubmitChanges();

                    var d = from o in db.orders where o.google_order_number == OrderNumber1 select o.order_id;

                    foreach (Item ThisItem in N1.shoppingcart.items)
                    {
                        string Name = ThisItem.itemname;
                        int Quantity = ThisItem.quantity;
                        decimal Price = ThisItem.unitprice.Value;
                    }

                    break;
                case "risk-information-notification":
                    RiskInformationNotification N2 = (RiskInformationNotification)EncodeHelper.Deserialize(requestedXml, typeof(RiskInformationNotification));
                    // This notification tells us that Google has authorized the order and it has passed the fraud check.
                    // Use the data below to determine if you want to accept the order, then start processing it.
                    SerialNumber = N2.serialnumber;
                    string OrderNumber2 = N2.googleordernumber;
                    string AVS = N2.riskinformation.avsresponse;
                    string CVN = N2.riskinformation.cvnresponse;
                    bool SellerProtection = N2.riskinformation.eligibleforprotection;
                    break;
                case "order-state-change-notification":
                    OrderStateChangeNotification N3 = (OrderStateChangeNotification)EncodeHelper.Deserialize(requestedXml, typeof(OrderStateChangeNotification));
                    // The order has changed either financial or fulfillment state in Google's system.
                    SerialNumber = N3.serialnumber;
                    string OrderNumber3 = N3.googleordernumber;
                    string NewFinanceState = N3.newfinancialorderstate.ToString();
                    string NewFulfillmentState = N3.newfulfillmentorderstate.ToString();
                    string PrevFinanceState = N3.previousfinancialorderstate.ToString();
                    string PrevFulfillmentState = N3.previousfulfillmentorderstate.ToString();
                    break;
                case "charge-amount-notification":
                    ChargeAmountNotification N4 = (ChargeAmountNotification)EncodeHelper.Deserialize(requestedXml, typeof(ChargeAmountNotification));
                    // Google has successfully charged the customer's credit card.
                    SerialNumber = N4.serialnumber;
                    string OrderNumber4 = N4.googleordernumber;
                    decimal ChargedAmount = N4.latestchargeamount.Value;
                    break;
                case "refund-amount-notification":
                    RefundAmountNotification N5 = (RefundAmountNotification)EncodeHelper.Deserialize(requestedXml, typeof(RefundAmountNotification));
                    // Google has successfully refunded the customer's credit card.
                    SerialNumber = N5.serialnumber;
                    string OrderNumber5 = N5.googleordernumber;
                    decimal RefundedAmount = N5.latestrefundamount.Value;
                    break;
                case "chargeback-amount-notification":
                    ChargebackAmountNotification N6 = (ChargebackAmountNotification)EncodeHelper.Deserialize(requestedXml, typeof(ChargebackAmountNotification));
                    // A customer initiated a chargeback with his credit card company to get her money back.
                    SerialNumber = N6.serialnumber;
                    string OrderNumber6 = N6.googleordernumber;
                    decimal ChargebackAmount = N6.latestchargebackamount.Value;
                    break;
                default:
                    break;
            }
            strReader.Close();
            strReader.Dispose();
            return SerialNumber;
        }
    }

}
