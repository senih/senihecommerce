<%@ Control Language="C#" AutoEventWireup="true" CodeFile="orders.ascx.cs" Inherits="systems_orders" %>

<asp:LoginView ID="LoginView" runat="server">
    <LoggedInTemplate>
                <div>
                    <div>
                        <asp:GridView ID="OrdersGridView" runat="server" SkinID="gridOrders" 
                            AutoGenerateColumns="False" AllowSorting="true" AllowPaging="true" 
                            onselectedindexchanged="OrdersGridView_SelectedIndexChanged" 
                            DataKeyNames="google_order_number">
                            <Columns>
                                <asp:BoundField DataField="order_id" HeaderText="ID" />
                                <asp:BoundField DataField="google_order_number" HeaderText="Google #" />
                                <asp:BoundField DataField="charged_amount" HeaderText="Total" />
                                <asp:BoundField DataField="status" HeaderText="Payment" />
                                <asp:BoundField DataField="shipping_status" HeaderText="Shipment" />
                                <asp:BoundField DataField="order_by" HeaderText="User" />
                                <asp:BoundField DataField="order_date" HeaderText="Order date" />
                                <asp:CommandField ButtonType="Button" SelectText="Details" ShowSelectButton="true" />
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="StatusLabel" runat="server" ForeColor="Red"></asp:Label>
                    </div>
                    <div>
                        <asp:Panel ID="OrderDetailsPanel" runat="server">
                            <table style="width:100%">
                                <tr>
                                    <td align="center" bgcolor="Silver">
                                        <b>Billing info</b></td>
                                    <td align="center" bgcolor="Silver">
                                        <b>Shipping info</b></td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:DetailsView ID="BillingDetails" runat="server" AutoGenerateRows="false" 
                                            SkinID="detailsOrder">
                                            <Fields>
                                                <asp:BoundField DataField="contact_name" HeaderText="Contact name: " />
                                                <asp:BoundField DataField="address" HeaderText="Address: " />
                                                <asp:BoundField DataField="city" HeaderText="City: " />
                                                <asp:BoundField DataField="zip" HeaderText="ZIP: " />
                                                <asp:BoundField DataField="country" HeaderText="Country: " />
                                                <asp:BoundField DataField="email" HeaderText="E-mail: " />
                                                <asp:BoundField DataField="cc_number" HeaderText="Last 4 digits of CC: " />
                                                <asp:BoundField DataField="ip" HeaderText="IP address: " />
                                            </Fields>
                                        </asp:DetailsView>
                                    </td>
                                    <td align="center">
                                        <asp:DetailsView ID="ShippingDetails" runat="server" AutoGenerateRows="false" 
                                            SkinID="detailsOrder">
                                            <Fields>
                                                <asp:BoundField DataField="shipping_first_name" HeaderText="First name:" />
                                                <asp:BoundField DataField="shipping_last_name" HeaderText="Last name:" />
                                                <asp:BoundField DataField="shipping_company" HeaderText="Company:" />
                                                <asp:BoundField DataField="shipping_address" HeaderText="Address:" />
                                                <asp:BoundField DataField="shipping_city" HeaderText="City:" />
                                                <asp:BoundField DataField="shipping_zip" HeaderText="ZIP:" />
                                                <asp:BoundField DataField="shipping_country" HeaderText="Country:" />
                                                <asp:BoundField DataField="shipping_phone" HeaderText="Phone:" />
                                            </Fields>
                                        </asp:DetailsView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        &nbsp;</td>
                                    <td align="center">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td bgcolor="Silver" colspan="2">
                                        Order placed on:
                                        <asp:Label ID="DateLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td bgcolor="Silver" colspan="2">
                                        Order number:
                                        <asp:Label ID="GoogleOrderNumberLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td bgcolor="Silver" colspan="2">
                                        Financial state:
                                        <asp:Label ID="FinancialLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td bgcolor="Silver" colspan="2">                                        
                                        Fulfillment state:
                                        <asp:Label ID="FulfillmentLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:GridView Width="100%" ID="OrderItemsGridView" runat="server" 
                                            AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:BoundField DataField="item_name" HeaderText="Item name" />
                                                <asp:BoundField DataField="item_desc" HeaderText="Description" />
                                                <asp:BoundField DataField="qty" HeaderText="Quantity" />
                                                <asp:BoundField DataField="price" HeaderText="Unit price" />
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" bgcolor="Silver" colspan="2">
                                        Shipping:
                                        <asp:Label ID="ShippingLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" bgcolor="Silver" colspan="2">
                                        Tax:
                                        <asp:Label ID="TaxLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" bgcolor="Silver" colspan="2">
                                        Total amount:
                                        <asp:Label ID="TotalAmountLabel" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" bgcolor="Silver" colspan="2">
                                        <span class="Apple-style-span" 
                                            style="border-collapse: separate; color: rgb(0, 0, 0); font-family: Arial; font-size: 13px; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: normal; orphans: 2; text-align: -webkit-right; text-indent: 0px; text-transform: none; white-space: nowrap; widows: 2; word-spacing: 0px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; -webkit-text-decorations-in-effect: none; -webkit-text-size-adjust: auto; -webkit-text-stroke-width: 0; ">
                                        Refunds: </span>
                                        <asp:Label ID="RefundLabel" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" bgcolor="Silver" colspan="2">
                                        Charged amount:
                                        <asp:Label ID="ChargedAmountLabel" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                </div>
    </LoggedInTemplate>
    <AnonymousTemplate>
        <asp:Login ID="Login1" runat="server" TitleText="" PasswordRecoveryText="Password Recovery" PasswordRecoveryUrl="~/password.aspx" DestinationPageUrl="~/shop_orders.aspx">
        </asp:Login>
    </AnonymousTemplate>
</asp:LoginView>
