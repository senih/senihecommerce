using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }

}
