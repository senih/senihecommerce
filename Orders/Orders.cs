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

        /// <summary>
        /// Processes the notification.
        /// </summary>
        /// <param name="xmlFile">The XML file.</param>
        /// <returns>Serial number of the notification</returns>
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
                    ///This notification tells us that Google has accepted the order
                    SerialNumber = N1.serialnumber;
                    Int64 OrderNumber1 = Int64.Parse(N1.googleordernumber);
                    int pos = N1.buyershippingaddress.contactname.IndexOf(" ");
                    string ShipToFirstName = N1.buyershippingaddress.contactname.Substring(0, pos);
                    string ShipToLatsName = N1.buyershippingaddress.contactname.Substring(pos + 1);
                    string UserName = N1.shoppingcart.merchantprivatedata.Any[0].InnerText;

                    order newOrder = new order();
                    newOrder.google_order_number = OrderNumber1;
                    newOrder.order_date = N1.timestamp;
                    newOrder.order_by = UserName;
                    newOrder.sub_total = 0;
                    newOrder.total = N1.ordertotal.Value;
                    newOrder.status = "Order placed";
                    newOrder.root_id = 1;
                    newOrder.shipping_first_name = ShipToFirstName;
                    newOrder.shipping_last_name = ShipToLatsName;
                    newOrder.shipping_address = N1.buyershippingaddress.address1;
                    newOrder.shipping_city = N1.buyershippingaddress.city;
                    newOrder.shipping_state = N1.buyershippingaddress.region;
                    newOrder.shipping_zip = N1.buyershippingaddress.postalcode;
                    newOrder.shipping_country = N1.buyerbillingaddress.countrycode;

                    db.orders.InsertOnSubmit(newOrder);
                    db.SubmitChanges();

                    List<int> list = (from o in db.orders where o.google_order_number == OrderNumber1 select o.order_id).ToList();
                    int orderId = list[0];                    

                    foreach (Item ThisItem in N1.shoppingcart.items)
                    {
                        int itemId = int.Parse(ThisItem.merchantprivateitemdata.Any[0].InnerText);
                        string desc = ThisItem.itemdescription;
                        int quantity = ThisItem.quantity;
                        decimal price = ThisItem.unitprice.Value;
                        bool tangible = false;
                        if (ThisItem.digitalcontent != null)
                            tangible = true;
                        
                        order_item newItem = new order_item();
                        newItem.item_id = itemId;
                        newItem.order_id = orderId;
                        newItem.price = price;
                        newItem.qty = quantity;
                        newItem.tangible = tangible;
                        newItem.item_desc = desc;

                        db.order_items.InsertOnSubmit(newItem);
                        db.SubmitChanges();

                    }

                    break;
                case "risk-information-notification":
                    RiskInformationNotification N2 = (RiskInformationNotification)EncodeHelper.Deserialize(requestedXml, typeof(RiskInformationNotification));
                    // This notification tells us that Google has authorized the order and it has passed the fraud check
                    SerialNumber = N2.serialnumber;
                    string contactName = EncodeHelper.GetElementValue(requestedXml, "contact-name");
                    string email = EncodeHelper.GetElementValue(requestedXml, "email");
                    string city = EncodeHelper.GetElementValue(requestedXml, "city");
                    int zip = int.Parse(EncodeHelper.GetElementValue(requestedXml, "postal-code"));
                    string country = EncodeHelper.GetElementValue(requestedXml, "country-code");

                    customer newCustomer = new customer();
                    newCustomer.google_order_number = Int64.Parse(N2.googleordernumber);
                    newCustomer.eligibility = N2.riskinformation.eligibleforprotection;
                    newCustomer.contact_name = contactName;
                    newCustomer.email = email;
                    newCustomer.address = N2.riskinformation.billingaddress.address1;
                    newCustomer.city = contactName;
                    newCustomer.zip = zip;
                    newCustomer.country = country;
                    newCustomer.avs = char.Parse(N2.riskinformation.avsresponse);
                    newCustomer.cvn = char.Parse(N2.riskinformation.cvnresponse);
                    newCustomer.cc_number = int.Parse(N2.riskinformation.partialccnumber);
                    newCustomer.ip = N2.riskinformation.ipaddress;

                    db.customers.InsertOnSubmit(newCustomer);
                    db.SubmitChanges();

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
