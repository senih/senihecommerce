using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Orders;
using EclipseWebSolutions.DatePicker;

public partial class systems_shop_orders : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && Page.User.Identity.IsAuthenticated)
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
        DatePicker fromDP = (DatePicker)LoginView.FindControl("FromDatePicker");
        DatePicker toDP = (DatePicker)LoginView.FindControl("ToDatePicker");
        GridView orderGrid = (GridView)LoginView.FindControl("OrdersGridView");
        Label statusLbl = (Label)LoginView.FindControl("StatusLabel");
        Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
        orderDetailsPanel.Visible = false;
        string shipment;
        string payment;
        DateTime minDateTime = (DateTime)SqlDateTime.MinValue;
        DateTime maxDateTime = (DateTime)SqlDateTime.MaxValue;
        DateTime fromDate = minDateTime;
        DateTime toDate = maxDateTime;
        long googleOrderNumber =0;
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

        DateTime.TryParse(fromDP.txtDate.Text, out fromDate);
        DateTime.TryParse(toDP.txtDate.Text, out toDate);
        if (fromDate < minDateTime || fromDate > maxDateTime || toDate < minDateTime || toDate > maxDateTime)
        {
            fromDate = minDateTime;
            toDate = maxDateTime;
        }

        List<order> gridDataSource = Orders.Orders.GetOrders(payment, shipment, googleOrderNumber, fromDate, toDate);
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
    protected void OrdersGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
        DetailsView billingDetails = (DetailsView)LoginView.FindControl("BillingDetails");
        DetailsView shippingDetails = (DetailsView)LoginView.FindControl("ShippingDetails");
        Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
        orderDetailsPanel.Visible = true;
        ordersGrid.Visible = false;
        long dataKey = (long)ordersGrid.SelectedDataKey.Value;
        List<order> source = Orders.Orders.GetOrderDetails(dataKey);
        List<order_item> items = Orders.Orders.GetItems(source[0].order_id);

        shippingDetails.DataSource = source;
        shippingDetails.DataBind();
    }
}
