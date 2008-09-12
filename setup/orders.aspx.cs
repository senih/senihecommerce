using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orders;

public partial class setup_orders : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<order> source = Orders.Orders.GetALL();
        GridView1.DataSource = source;
        GridView1.DataBind();

        List<customer> source1 = Orders.Orders.GetAllCustomers();
        GridView2.DataSource = source1;
        GridView2.DataBind();

        List<order_item> source2 = Orders.Orders.GetAllOrderItems();
        GridView3.DataSource = source2;
        GridView3.DataBind();
    }
}
