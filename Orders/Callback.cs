using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCheckout.MerchantCalculation;
using GCheckout.Util;

namespace Orders
{
    public class Callback : CallbackRules
    {
        string orderId;

        public Callback(){}

        public Callback(string id)
        {
            orderId = id;
        }

        public override ShippingResult GetShippingResult(string ShipMethodName, Order ThisOrder, AnonymousAddress Address)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext("Data Source=mssql401.ixwebhosting.com;Initial Catalog=karolin_ecommerce;uid=karolin_ecomm;password=ke6grty");
            ShippingResult result = new ShippingResult();
            int orderNumber = int.Parse(orderId);

            string shipping = (from o in db.orders
                               where o.order_id == orderNumber
                               select o.shipping).Single().ToString();
            result.ShippingRate = decimal.Parse(shipping);
            result.Shippable = true;
            return result;
        }

        public override decimal GetTaxResult(Order ThisOrder, AnonymousAddress Address, decimal ShippingRate)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext("Data Source=mssql401.ixwebhosting.com;Initial Catalog=karolin_ecommerce;uid=karolin_ecomm;password=ke6grty");
            int orderNumber = int.Parse(orderId);

            string tax = (from o in db.orders
                          where o.order_id == orderNumber
                          select o.tax).Single().ToString();
            return decimal.Parse(tax);
        }
    }
}
