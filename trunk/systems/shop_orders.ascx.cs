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
        if (!IsPostBack && Page.User.Identity.IsAuthenticated && Page.User.IsInRole("Administrators"))
        {
            GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
            Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
            ordersGrid.DataSource = Orders.Orders.GetAllOrders();
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
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        Label fulfillmentLbl = (Label)LoginView.FindControl("FulfillmentLabel");
        Label financialLbl = (Label)LoginView.FindControl("FinancialLabel");
        Label totalAmountLbl = (Label)LoginView.FindControl("TotalAmountLabel");
        Label taxLbl = (Label)LoginView.FindControl("TaxLabel");
        Label chargedAmountLbl = (Label)LoginView.FindControl("ChargedAmountLabel");
        GridView orderItemsGrid = (GridView)LoginView.FindControl("OrderItemsGridView");
        Button cancelBtn = (Button)LoginView.FindControl("CancelButton");
        Panel chargingPanel = (Panel)LoginView.FindControl("ChargingPanel");
        Panel partialChargingPanel = (Panel)LoginView.FindControl("PartialChargingPanel");
        Panel refundPanel = (Panel)LoginView.FindControl("RefundPanel");
        Panel shippingPanel = (Panel)LoginView.FindControl("ShippingPanel");
        
        orderDetailsPanel.Visible = true;
        ordersGrid.Visible = false;
        partialChargingPanel.Visible = false;
        refundPanel.Visible = false;
        shippingPanel.Visible = false;
        long dataKey = (long)ordersGrid.SelectedDataKey.Value;
        List<order> source = Orders.Orders.GetOrderDetails(dataKey);
        List<order_item> items = Orders.Orders.GetItems(source[0].order_id);
        List<customer> customerDetails = Orders.Orders.GetCustomerDetails(dataKey);

        if (source[0].status == "CANCELLED" || source[0].status == "CANCELLED_BY_GOOGLE")
            cancelBtn.Enabled = false;
        else
            cancelBtn.Enabled = true;
        if (source[0].status != "CHARGEABLE")
            chargingPanel.Visible = false;
        else 
            chargingPanel.Visible = true;
        orderNumberLbl.Text = dataKey.ToString();
        financialLbl.Text = source[0].status;
        fulfillmentLbl.Text = source[0].shipping_status;
        taxLbl.Text = source[0].tax.ToString();
        totalAmountLbl.Text = source[0].total.ToString();
        chargedAmountLbl.Text = source[0].charged_amount.ToString();
        billingDetails.DataSource = customerDetails;
        billingDetails.DataBind();
        shippingDetails.DataSource = source;
        shippingDetails.DataBind();
        orderItemsGrid.DataSource = items;
        orderItemsGrid.DataBind();
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

    /// <summary>
    /// Handles the Click event of the ArchiveButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ArchiveButton_Click(object sender, EventArgs e)
    {        
        Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        GCheckout.OrderProcessing.ArchiveOrderRequest archiveReq = new GCheckout.OrderProcessing.ArchiveOrderRequest(orderNumberLbl.Text);
        archiveReq.Send();
        Orders.Orders.ArchiveOrder(orderNumberLbl.Text);
        orderDetailsPanel.Visible = false;
        Response.Redirect(Request.RawUrl);
    }

    /// <summary>
    /// Handles the Click event of the ChargeButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ChargeButton_Click(object sender, EventArgs e)
    {
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        GCheckout.OrderProcessing.ChargeOrderRequest chargeReq = new GCheckout.OrderProcessing.ChargeOrderRequest(orderNumberLbl.Text);
        chargeReq.Send();
    }

    /// <summary>
    /// Handles the Click event of the PartialChargingButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PartialChargingButton_Click(object sender, EventArgs e)
    {
        Panel chargingPanel = (Panel)LoginView.FindControl("ChargingPanel");
        Panel partialChargingPanel = (Panel)LoginView.FindControl("PartialChargingPanel");
        
        partialChargingPanel.Visible = true;
        chargingPanel.Visible = false;        
    }

    /// <summary>
    /// Handles the Click event of the ChargePartialButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ChargePartialButton_Click(object sender, EventArgs e)
    {
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        TextBox partialAmountTB = (TextBox)LoginView.FindControl("PartialAmountTextBox");
        Label totalAmountLbl = (Label)LoginView.FindControl("TotalAmountLabel");
        Label statusLabel = (Label)LoginView.FindControl("StatusLabel");
        Panel partialChargingPanel = (Panel)LoginView.FindControl("PartialChargingPanel");
        decimal total = decimal.Parse(totalAmountLbl.Text);
        decimal amount = 0;
        if (decimal.TryParse(partialAmountTB.Text, out amount) && amount != 0 && amount <= total)
        {
            GCheckout.OrderProcessing.ChargeOrderRequest chargeReq = new GCheckout.OrderProcessing.ChargeOrderRequest(orderNumberLbl.Text, "USD", amount);
            chargeReq.Send();
            statusLabel.Text = "";
            partialChargingPanel.Visible = false;
        }
        else
            statusLabel.Text = "The entered amount is not correct!";

    }

    /// <summary>
    /// Handles the Click event of the CancelPartialChargingButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CancelPartialChargingButton_Click(object sender, EventArgs e)
    {
        Panel chargingPanel = (Panel)LoginView.FindControl("ChargingPanel");
        Panel partialChargingPanel = (Panel)LoginView.FindControl("PartialChargingPanel");
        Label statusLabel = (Label)LoginView.FindControl("StatusLabel");

        partialChargingPanel.Visible = false;
        chargingPanel.Visible = true;
        statusLabel.Text = "";
    }

    /// <summary>
    /// Handles the Click event of the RefundButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void RefundButton_Click(object sender, EventArgs e)
    {
        RangeValidator rangeValidator = (RangeValidator)LoginView.FindControl("RangeValidator");
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        TextBox refundCommentTB = (TextBox)LoginView.FindControl("RefundCommentTextBox");
        TextBox refundAmountTB = (TextBox)LoginView.FindControl("RefundTextBox");
        Label chargedAmountLbl = (Label)LoginView.FindControl("ChargedAmountLabel");
        Label refundStatusLbl = (Label)LoginView.FindControl("RefundStatusLabel");
        Panel refundPanel = (Panel)LoginView.FindControl("RefundPanel");

        decimal total = decimal.Parse(chargedAmountLbl.Text);
        rangeValidator.MaximumValue = total.ToString();
        decimal amount = 0;
        if (decimal.TryParse(refundAmountTB.Text, out amount) && amount != 0 && amount <= total)
        {
            GCheckout.OrderProcessing.RefundOrderRequest refundReq = new GCheckout.OrderProcessing.RefundOrderRequest(orderNumberLbl.Text, refundCommentTB.Text, "USD", amount);
            refundReq.Send();
            refundStatusLbl.Text = "";
            refundPanel.Visible = false;

        }
        else
            refundStatusLbl.Text = "The entered amount is not correct!";
    }

    /// <summary>
    /// Handles the Click event of the RefundPanelButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void RefundPanelButton_Click(object sender, EventArgs e)
    {
        Panel refundPanel = (Panel)LoginView.FindControl("RefundPanel");
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");

        long orderNumber = Int64.Parse(orderNumberLbl.Text);
        List<order> refundOrder = Orders.Orders.GetOrderDetails(orderNumber);
        if (refundOrder[0].charged_amount != 0)
            refundPanel.Visible = true;
    }

    /// <summary>
    /// Handles the Click event of the CancelRefundButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CancelRefundButton_Click(object sender, EventArgs e)
    {
        Panel refundPanel = (Panel)LoginView.FindControl("RefundPanel");
        Label refundStatusLbl = (Label)LoginView.FindControl("RefundStatusLabel");
        refundStatusLbl.Text = "";
        refundPanel.Visible = false;
    }

    /// <summary>
    /// Handles the Click event of the ShipPanelButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ShipPanelButton_Click(object sender, EventArgs e)
    {
        Panel shippingPanel = (Panel)LoginView.FindControl("ShippingPanel");
        shippingPanel.Visible = true;
    }

    /// <summary>
    /// Handles the Click event of the SendShippingInfoButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void SendShippingInfoButton_Click(object sender, EventArgs e)
    {
        TextBox carrierTB = (TextBox)LoginView.FindControl("CarrierTextBox");
        TextBox trackingNumberTB = (TextBox)LoginView.FindControl("TrackingNumberTextBox");
        Label orderNumberLbl = (Label)LoginView.FindControl("GoogleOrderNumberLabel");
        Panel shippingPanel = (Panel)LoginView.FindControl("ShippingPanel");

        GCheckout.OrderProcessing.DeliverOrderRequest deliverReq = new GCheckout.OrderProcessing.DeliverOrderRequest(orderNumberLbl.Text, carrierTB.Text, trackingNumberTB.Text, true);
        deliverReq.Send();
        shippingPanel.Visible = false;
    }

    /// <summary>
    /// Handles the Click event of the CancelSendingShippingInfoButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CancelSendingShippingInfoButton_Click(object sender, EventArgs e)
    {
        Panel shippingPanel = (Panel)LoginView.FindControl("ShippingPanel");
        shippingPanel.Visible = false;
    }
}
