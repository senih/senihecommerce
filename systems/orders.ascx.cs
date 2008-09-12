using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orders;

public partial class systems_orders : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && Page.User.Identity.IsAuthenticated)
        {
            GridView ordersGrid = (GridView)LoginView.FindControl("OrdersGridView");
            Panel orderDetailsPanel = (Panel)LoginView.FindControl("OrderDetailsPanel");
            ordersGrid.DataSource = Orders.Orders.GetOrdersByUser(Page.User.Identity.Name);
            ordersGrid.DataBind();
            orderDetailsPanel.Visible = false;
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
        Label shippingLbl = (Label)LoginView.FindControl("ShippingLabel");
        Label taxLbl = (Label)LoginView.FindControl("TaxLabel");
        Label refundLbl = (Label)LoginView.FindControl("RefundLabel");
        Label chargedAmountLbl = (Label)LoginView.FindControl("ChargedAmountLabel");
        Label dateLbl = (Label)LoginView.FindControl("DateLabel");
        GridView orderItemsGrid = (GridView)LoginView.FindControl("OrderItemsGridView");
        
        orderDetailsPanel.Visible = true;
        ordersGrid.Visible = false;                
        long dataKey = (long)ordersGrid.SelectedDataKey.Value;
        List<order> source = Orders.Orders.GetOrderDetails(dataKey);
        List<order_item> items = Orders.Orders.GetItems(source[0].order_id);
        List<customer> customerDetails = Orders.Orders.GetCustomerDetails(dataKey);

        if (source[0].status != "TEMP")
        {            
            orderNumberLbl.Text = dataKey.ToString();
            financialLbl.Text = source[0].status;
            fulfillmentLbl.Text = source[0].shipping_status;
            shippingLbl.Text = source[0].shipping.ToString();
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
            if (source[0].sub_total + source[0].charged_amount != source[0].total)
            {
                decimal temp = source[0].total - Math.Abs(source[0].sub_total - source[0].charged_amount.Value);
                refundLbl.Text = temp.ToString();
            }
        }
        else
            statusLbl.Text = "No details available!";
    }

}
