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

        /// <summary>
        /// Gets the shipping result.
        /// </summary>
        /// <param name="ShipMethodName">Name of the ship method.</param>
        /// <param name="ThisOrder">The this order.</param>
        /// <param name="Address">The address.</param>
        /// <returns>ShippingResult object</returns>
        public override ShippingResult GetShippingResult(string ShipMethodName, Order ThisOrder, AnonymousAddress Address)
        {
            string conn = Orders.GetConnectionString();
            StoreDataClassesDataContext db = new StoreDataClassesDataContext(conn);
            ShippingResult result = new ShippingResult();
            int orderNumber = int.Parse(orderId);

            string shipping = (from o in db.orders
                               where o.order_id == orderNumber
                               select o.shipping).Single().ToString();
            result.ShippingRate = decimal.Parse(shipping);
            result.Shippable = true;
            return result;
        }

        /// <summary>
        /// Gets the tax result.
        /// </summary>
        /// <param name="ThisOrder">The this order.</param>
        /// <param name="Address">The address.</param>
        /// <param name="ShippingRate">The shipping rate.</param>
        /// <returns>Tax result as decimal value</returns>
        public override decimal GetTaxResult(Order ThisOrder, AnonymousAddress Address, decimal ShippingRate)
        {
            string conn = Orders.GetConnectionString();
            StoreDataClassesDataContext db = new StoreDataClassesDataContext(conn);
            int orderNumber = int.Parse(orderId);

            string tax = (from o in db.orders
                          where o.order_id == orderNumber
                          select o.tax).Single().ToString();
            return decimal.Parse(tax);
        }
    }
}
