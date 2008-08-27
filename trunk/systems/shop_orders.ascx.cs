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
using System.Collections.Generic;
using Orders;

public partial class systems_shop_orders : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
            ordersGrid.DataSource = Orders.Orders.GetAllOrders();
            ordersGrid.DataBind();
        }

    }

    /// <summary>
    /// Handles the Click event of the SearchButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        DropDownList paymentDDL = (DropDownList)LoginView.FindControl("PaymentDropDownList");
        DropDownList shipmentDDL = (DropDownList)LoginView.FindControl("ShipmentDropDownList");
        TextBox orderNumberTB = (TextBox)LoginView.FindControl("OrderNumberTextBox");
        TextBox fromDateTB = (TextBox)LoginView.FindControl("FromDateTextBox");
        TextBox toDateTB = (TextBox)LoginView.FindControl("ToDateTextBox");
        GridView orderGrid = (GridView)LoginView.FindControl("OrdersGridView");
        Label statusLbl = (Label)LoginView.FindControl("StatusLabel");
        string shipment;
        string payment;
        DateTime from = DateTime.Parse("1/1/1753");
        DateTime to = DateTime.MaxValue;
        long googleOrderNumber = 0;
        if (paymentDDL.SelectedValue == "ALL_PAY")
            payment = "";
        else
            payment = paymentDDL.SelectedValue;
        if (shipmentDDL.SelectedValue == "ALL_SHIP")
            shipment = "";
        else
            shipment = shipmentDDL.SelectedValue;
        if (orderNumberTB.Text != string.Empty)
        {
            googleOrderNumber = Int64.Parse(orderNumberTB.Text);
        }
        if ((fromDateTB.Text != string.Empty) && (toDateTB.Text != string.Empty))
        {
            from = DateTime.Parse(fromDateTB.Text);
            to = DateTime.Parse(toDateTB.Text);
        }
        List<order> gridDataSource = Orders.Orders.GetOrders(payment, shipment, googleOrderNumber, from, to);
        if (gridDataSource.Count == 0)
        {
            orderGrid.Visible = false;
            statusLbl.Text = "No results found in database";
        }
        else
        {
            statusLbl.Text = "";
            orderGrid.Visible = true;
            orderGrid.DataSource = gridDataSource;
            orderGrid.DataBind();
        }
    }
}
