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
        public override ShippingResult GetShippingResult(string ShipMethodName, Order ThisOrder, AnonymousAddress Address)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            ShippingResult result = new ShippingResult();
            int orderNumber = int.Parse(ThisOrder.MerchantPrivateData);

            string shipping = (from o in db.orders
                               where o.order_id == orderNumber
                               select o.shipping).ToString();
            result.ShippingRate = decimal.Parse(shipping);
            result.Shippable = true;
            return result;
        }

        public override decimal GetTaxResult(Order ThisOrder, AnonymousAddress Address, decimal ShippingRate)
        {
            StoreDataClassesDataContext db = new StoreDataClassesDataContext();
            int orderNumber = int.Parse(ThisOrder.MerchantPrivateData);
            string tax = (from o in db.orders
                          where o.order_id == orderNumber
                          select o.tax).ToString();
            return decimal.Parse(tax);
        }
    }
}
