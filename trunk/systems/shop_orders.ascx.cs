using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Orders;

public partial class systems_shop_orders : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
        ordersGrid.DataSource = Orders.Orders.GetAllOrders();
        ordersGrid.DataBind();
    }
}
