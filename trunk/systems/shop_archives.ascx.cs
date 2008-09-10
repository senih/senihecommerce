using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Xml.Linq;
using Orders;
using EclipseWebSolutions.DatePicker;

public partial class systems_shop_archives : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && Page.User.Identity.IsAuthenticated && Page.User.IsInRole("Administrators"))
        {
            GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
            Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
            ordersGrid.DataSource = Orders.Orders.GetArchivedOrders();
            ordersGrid.DataBind();
            orderDetailsPanel.Visible = false;
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

        List<order> gridDataSource = Orders.Orders.GetArchives(payment, shipment, googleOrderNumber, fromDate, toDate);
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

    /// <summary>
    /// Handles the SelectedIndexChanged event of the OrdersGridView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void OrdersGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
        DetailsView billingDetails = (DetailsView)LoginView.FindControl("BillingDetails");
        DetailsView shippingDetails = (DetailsView)LoginView.FindControl("ShippingDetails");
        Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
        Label statusLbl = (Label)LoginView.FindControl("StatusLabel");
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        Label fulfillmentLbl = (Label)LoginView.FindControl("FulfillmentLabel");
        Label financialLbl = (Label)LoginView.FindControl("FinancialLabel");
        Label totalAmountLbl = (Label)LoginView.FindControl("TotalAmountLabel");
        Label taxLbl = (Label)LoginView.FindControl("TaxLabel");
        Label chargedAmountLbl = (Label)LoginView.FindControl("ChargedAmountLabel");
        Label dateLbl = (Label)LoginView.FindControl("DateLabel");
        GridView orderItemsGrid = (GridView)LoginView.FindControl("OrderItemsGridView");
        Button cancelBtn = (Button)LoginView.FindControl("CancelButton");
        
        orderDetailsPanel.Visible = true;
        ordersGrid.Visible = false;        
        long dataKey = (long)ordersGrid.SelectedDataKey.Value;
        List<order> source = Orders.Orders.GetOrderDetails(dataKey);
        List<order_item> items = Orders.Orders.GetItems(source[0].order_id);
        List<customer> customerDetails = Orders.Orders.GetCustomerDetails(dataKey);

        if (source[0].status != "TEMP")
        {
            if (source[0].status == "CANCELLED" || source[0].status == "CANCELLED_BY_GOOGLE")
                cancelBtn.Enabled = false;
            else
                cancelBtn.Enabled = true;
            
            orderNumberLbl.Text = dataKey.ToString();
            financialLbl.Text = source[0].status;
            fulfillmentLbl.Text = source[0].shipping_status;
            taxLbl.Text = source[0].tax.ToString();
            totalAmountLbl.Text = source[0].total.ToString();
            chargedAmountLbl.Text = source[0].charged_amount.ToString();
            dateLbl.Text = source[0].order_date.ToShortDateString();
            billingDetails.DataSource = customerDetails;
            billingDetails.DataBind();
            shippingDetails.DataSource = source;
            shippingDetails.DataBind();
            orderItemsGrid.DataSource = items;
            orderItemsGrid.DataBind();
        }
        else
            statusLbl.Text = "No details available!";
    }

    /// <summary>
    /// Handles the Click event of the UnarchiveButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ArchiveButton_Click(object sender, EventArgs e)
    {
        Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        GCheckout.OrderProcessing.UnarchiveOrderRequest archiveReq = new GCheckout.OrderProcessing.UnarchiveOrderRequest(orderNumberLbl.Text);
        archiveReq.Send();
        Orders.Orders.UnarchiveOrder(orderNumberLbl.Text);
        orderDetailsPanel.Visible = false;
        Response.Redirect(Request.RawUrl);
    }
    
    /// <summary>
    /// Handles the Click event of the CancelButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        string reason = "Cannceled by merchant!";
        string comment = "Item(s) not available in this moment";
        GCheckout.OrderProcessing.CancelOrderRequest cancelReq = new GCheckout.OrderProcessing.CancelOrderRequest(orderNumberLbl.Text, reason, comment);
        cancelReq.Send();
    }
}
