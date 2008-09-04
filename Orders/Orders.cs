using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GCheckout.Checkout;
using GCheckout.Util;
using GCheckout.AutoGen;
using GCheckout.OrderProcessing;
using GCheckout.MerchantCalculation;


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
            string description = (from p in db.pages where (p.page_id == item_id && p.status == "published") select p.summary).Single<string>();
            return description;
        }

        #region Notification methods
        /// <summary>
        /// Processes the notification.
        /// </summary>
        /// <param name="xmlFile">The XML file.</param>
        /// <returns>Serial number of the notification</returns>
        public static string ProcessNotification(string xmlFile)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            string SerialNumber = "";
            //Read XML file
            StreamReader strReader = new StreamReader(xmlFile);
            string requestedXml = strReader.ReadToEnd();
            //Act on XML file
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
                    int internalOrderId = int.Parse(N1.shoppingcart.merchantprivatedata.Any[1].InnerText);

                    order newOrder = db.orders.Where(o => o.order_id == internalOrderId).Single<order>();

                    newOrder.google_order_number = OrderNumber1;
                    newOrder.order_date = N1.timestamp;
                    newOrder.order_by = UserName;
                    newOrder.sub_total = 0;
                    newOrder.total = N1.ordertotal.Value;
                    newOrder.status = "NEW";
                    newOrder.root_id = 1;
                    //newOrder.shipping_first_name = ShipToFirstName;
                    //newOrder.shipping_last_name = ShipToLatsName;
                    //newOrder.shipping_address = N1.buyershippingaddress.address1;
                    //newOrder.shipping_city = N1.buyershippingaddress.city;
                    //newOrder.shipping_state = N1.buyershippingaddress.region;
                    //newOrder.shipping_zip = N1.buyershippingaddress.postalcode;
                    //newOrder.shipping_country = N1.buyerbillingaddress.countrycode;

                    db.orders.DeleteAllOnSubmit(db.orders.Where(o => (o.status == "TEMP" && o.order_by == UserName && o.order_id != internalOrderId)));
                    db.SubmitChanges();
                    
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
                        newItem.order_id = internalOrderId;
                        newItem.price = price;
                        newItem.qty = quantity;
                        newItem.tangible = tangible;
                        newItem.item_desc = desc;
                        newItem.item_name = ThisItem.itemname;

                        db.order_items.InsertOnSubmit(newItem);
                        db.SubmitChanges();

                    }

                    break;

                case "risk-information-notification":
                    RiskInformationNotification N2 = (RiskInformationNotification)EncodeHelper.Deserialize(requestedXml, typeof(RiskInformationNotification));
                    // This notification tells us that Google has authorized the order and it has passed the fraud check
                    SerialNumber = N2.serialnumber;
                    long googleOrderNumber = Int64.Parse(N2.googleordernumber);
                    string contactName = EncodeHelper.GetElementValue(requestedXml, "contact-name");
                    string email = EncodeHelper.GetElementValue(requestedXml, "email");
                    string city = EncodeHelper.GetElementValue(requestedXml, "city");
                    int zip = int.Parse(EncodeHelper.GetElementValue(requestedXml, "postal-code"));
                    string country = EncodeHelper.GetElementValue(requestedXml, "country-code");
                    bool elibible = N2.riskinformation.eligibleforprotection;
                    char cvn = char.Parse(N2.riskinformation.cvnresponse);

                    if (elibible && N2.riskinformation.cvnresponse == "M")
                    {
                        customer newCustomer = new customer();
                        newCustomer.google_order_number = googleOrderNumber;
                        newCustomer.eligibility = elibible;
                        newCustomer.contact_name = contactName;
                        newCustomer.email = email;
                        newCustomer.address = N2.riskinformation.billingaddress.address1;
                        newCustomer.city = city;
                        newCustomer.zip = zip;
                        newCustomer.country = country;
                        newCustomer.avs = char.Parse(N2.riskinformation.avsresponse);
                        newCustomer.cvn = cvn;
                        newCustomer.cc_number = int.Parse(N2.riskinformation.partialccnumber);
                        newCustomer.ip = N2.riskinformation.ipaddress;

                        db.customers.InsertOnSubmit(newCustomer);
                        db.SubmitChanges();
                    }
                    else
                    {
                        string reason = "You did not pass Google security check!";
                        string comment = "Please visis http://checkout.google.com/support/sell/bin/topic.py?topic=15055 for more information";
                        GCheckout.OrderProcessing.CancelOrderRequest cancelReq = new GCheckout.OrderProcessing.CancelOrderRequest(N2.googleordernumber, reason, comment);
                        cancelReq.Send();
                    }

                    break;

                case "order-state-change-notification":
                    OrderStateChangeNotification N3 = (OrderStateChangeNotification)EncodeHelper.Deserialize(requestedXml, typeof(OrderStateChangeNotification));
                    // The order has changed either financial or fulfillment state in Google's system.
                    SerialNumber = N3.serialnumber;
                    long googleOrderNumber1 = Int64.Parse(N3.googleordernumber);
                    string newFinanceState = N3.newfinancialorderstate.ToString();
                    string newFulfillmentState = N3.newfulfillmentorderstate.ToString();
                    string prevFinanceState = N3.previousfinancialorderstate.ToString();
                    string prevFulfillmentState = N3.previousfulfillmentorderstate.ToString();

                    order thisOrder1 = (from or in db.orders where or.google_order_number == googleOrderNumber1 select or).Single<order>();
                    if (newFinanceState == "CHARGEABLE")
                    {
                        GCheckout.OrderProcessing.ChargeOrderRequest chargeReq = new GCheckout.OrderProcessing.ChargeOrderRequest(N3.googleordernumber);
                        chargeReq.Send();
                    }
                    thisOrder1.status = newFinanceState;
                    thisOrder1.shipping_status = newFulfillmentState;
                    db.SubmitChanges();
                    break;

                case "charge-amount-notification":
                    ChargeAmountNotification N4 = (ChargeAmountNotification)EncodeHelper.Deserialize(requestedXml, typeof(ChargeAmountNotification));
                    // Google has successfully charged the customer's credit card.
                    SerialNumber = N4.serialnumber;
                    long googleOrderNumber2 = Int64.Parse(N4.googleordernumber);
                    decimal chargedAmount = N4.latestchargeamount.Value;

                    order thisOrder2 = (from or in db.orders where or.google_order_number == googleOrderNumber2 select or).Single<order>();
                    thisOrder2.charged_amount = chargedAmount;
                    db.SubmitChanges();

                    break;

                case "refund-amount-notification":
                    RefundAmountNotification N5 = (RefundAmountNotification)EncodeHelper.Deserialize(requestedXml, typeof(RefundAmountNotification));
                    // Google has successfully refunded the customer's credit card.
                    SerialNumber = N5.serialnumber;
                    long googleOrderNumber3 = Int64.Parse(N5.googleordernumber);
                    decimal refundedAmount = N5.latestrefundamount.Value;

                    order thisOrder3 = (from or in db.orders where or.google_order_number == googleOrderNumber3 select or).Single<order>();
                    thisOrder3.status = "REFUNDED";
                    thisOrder3.charged_amount -= refundedAmount;
                    db.SubmitChanges();

                    break;

                case "chargeback-amount-notification":
                    ChargebackAmountNotification N6 = (ChargebackAmountNotification)EncodeHelper.Deserialize(requestedXml, typeof(ChargebackAmountNotification));
                    // A customer initiated a chargeback with his credit card company to get her money back.
                    SerialNumber = N6.serialnumber;
                    long googleOrderNumber4 = Int64.Parse(N6.googleordernumber);
                    decimal chargebackAmount = N6.latestchargebackamount.Value;

                    order thisOrder4 = (from or in db.orders where or.google_order_number == googleOrderNumber4 select or).Single<order>();
                    thisOrder4.status = "CHARGEBACK";
                    db.SubmitChanges();

                    break;

                default:
                    break;

            } 
        
            strReader.Close();
            strReader.Dispose();
            return SerialNumber;
        }

        #endregion

        #region OrderProcessing methods

        /// <summary>
        /// Gets all orders.
        /// </summary>
        /// <returns>List of all orders</returns>
        public static List<order> GetAllOrders()
        {

            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            List<order> listOfAllOrderes = (from or in db.orders 
                                            where or.root_id != 999
                                            select or).ToList<order>();
            return listOfAllOrderes;
        }


        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="shipment">The shipment.</param>
        /// <param name="orderNumber">The order number.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>List of search results</returns>
        public static List<order> GetOrders(string payment, string shipment, long orderNumber, DateTime fromDate, DateTime toDate)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            List<order> listOfOrders;


            listOfOrders = (from or in db.orders
                            where (or.status == payment || payment == "") && (or.shipping_status == shipment || shipment == "") && (orderNumber==0 || or.google_order_number == orderNumber) && (or.order_date >= fromDate && or.order_date <= toDate)
                            orderby or.order_date descending
                            select or).ToList<order>();

            return listOfOrders;
        }

        /// <summary>
        /// Gets the order details.
        /// </summary>
        /// <param name="googleOrderNumber">The google order number.</param>
        /// <returns>Details for the order</returns>
        public static List<order> GetOrderDetails(long googleOrderNumber)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            List<order> orderDetails = (from or in db.orders
                                        where or.google_order_number == googleOrderNumber
                                        select or).ToList<order>();
            return orderDetails;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>Items from order with given orderID</returns>
        public static List<order_item> GetItems(int orderId)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            List<order_item> itemsDetails = (from i in db.order_items
                                             where i.order_id == orderId
                                             select i).ToList<order_item>();
            return itemsDetails;
        }

        /// <summary>
        /// Gets the customer details.
        /// </summary>
        /// <param name="googleOrderNumber">The google order number.</param>
        /// <returns>Customer details</returns>
        public static List<customer> GetCustomerDetails(long googleOrderNumber)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            List<customer> customerDetails = (from c in db.customers
                                              where c.google_order_number == googleOrderNumber
                                              select c).ToList<customer>();
            return customerDetails;
        }

        /// <summary>
        /// Archives the order.
        /// </summary>
        /// <param name="orderNumber">The order number.</param>
        public static void ArchiveOrder(string orderNumber)
        {
            long googleOrderNumber = Int64.Parse(orderNumber);
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            order archiveOrder = (from or in db.orders
                                        where or.google_order_number == googleOrderNumber
                                        select or).Single<order>();
            archiveOrder.root_id = 999;
            db.SubmitChanges();
        }

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        public static void DeleteOrder(int orderId)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            order deleteOrder = db.orders.Where(o => o.order_id == orderId).Single<order>();
            db.orders.DeleteOnSubmit(deleteOrder);
            db.SubmitChanges();
        }

        #endregion


    }

}
