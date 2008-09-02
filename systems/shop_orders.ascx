<%@ Control Language="C#" AutoEventWireup="true" CodeFile="shop_orders.ascx.cs" Inherits="systems_shop_orders" %>

<%@ Register assembly="EclipseWebSolutions.DatePicker" namespace="EclipseWebSolutions.DatePicker" tagprefix="cc1" %>


<asp:LoginView ID="LoginView" runat="server">
    <RoleGroups>
        <asp:RoleGroup Roles="Administrators">
            <ContentTemplate>
                <div>
                    <div>
                        <table style="width:100%">
                            <tr>
                                <td>
                                    <asp:Label ID="PaymentLabel" runat="server" Text="Payment status"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="ShipmentLabel" runat="server" Text="Shipment status"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="FromLabel" runat="server" Text="From:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="ToLabel" runat="server" Text="To:"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="PaymentDropDownList" runat="server">
                                        <asp:ListItem Value="ALL_PAY">All</asp:ListItem>
                                        <asp:ListItem Value="REVIEWING">Reviewing</asp:ListItem>
                                        <asp:ListItem Value="CHARGEABLE">Chargeable</asp:ListItem>
                                        <asp:ListItem Value="CHARGING">Charging</asp:ListItem>
                                        <asp:ListItem Value="CHARGED">Charged</asp:ListItem>
                                        <asp:ListItem Value="PAYMENT_DECLINED">Declined</asp:ListItem>
                                        <asp:ListItem Value="CANCELLED">Canceled</asp:ListItem>
                                        <asp:ListItem Value="CANCELLED_BY_GOOGLE">Google canceled</asp:ListItem>
                                        <asp:ListItem>Refunded</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ShipmentDropDownList" runat="server">
                                        <asp:ListItem Value="ALL_SHIP">All</asp:ListItem>
                                        <asp:ListItem Value="NEW">New</asp:ListItem>
                                        <asp:ListItem Value="PROCESSING">Processing</asp:ListItem>
                                        <asp:ListItem Value="DELIVERED">Delivered</asp:ListItem>
                                        <asp:ListItem Value="WILL_NOT_DELIVER">Canceled</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <cc1:DatePicker ID="FromDatePicker" runat="server" />
                                </td>
                                <td>
                                    <cc1:DatePicker ID="ToDatePicker" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="OrderNumberLabel" runat="server" Text="OrderNumber"></asp:Label>
                                </td>
                                <td>
                                    &nbsp;</td>
                                <td>
                                    (mm/dd/yyyy)
                                </td>
                                <td>
                                    (mm/dd/yyyy)</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="OrderNumberTextBox" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    &nbsp;</td>
                                <td>
                                    &nbsp;</td>
                                <td>                                    
                                    <asp:Button ID="SearchButton" runat="server" Text="Search" onclick="SearchButton_Click" />
                                </td>
                            </tr>
                        </table>                        
                    </div>
                    <br /><hr /><br />
                    <div>
                        <asp:GridView ID="OrdersGridView" runat="server" SkinID="gridOrders" 
                            AutoGenerateColumns="False" AllowSorting="true" AllowPaging="true" 
                            onselectedindexchanged="OrdersGridView_SelectedIndexChanged" 
                            DataKeyNames="google_order_number">
                            <Columns>
                                <asp:BoundField DataField="order_id" HeaderText="ID" />
                                <asp:BoundField DataField="google_order_number" HeaderText="Google #" />
                                <asp:BoundField DataField="total" HeaderText="Total" />
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
                                    <td bgcolor="Silver">
                                        Order number:
                                        <asp:Label ID="GoogleOrderNumberLabel" runat="server"></asp:Label>
                                    </td>
                                    <td align="right" bgcolor="Silver">
                                        <asp:Button ID="CancelButton" runat="server" onclick="CancelButton_Click" 
                                            Text="Cancel order" />
                                        &nbsp;<asp:Button ID="ArchiveButton" runat="server" onclick="ArchiveButton_Click" 
                                            Text="Archive order" />
                                    </td>
                                </tr>
                                <tr>
                                    <td bgcolor="Silver">
                                        Financial state:
                                        <asp:Label ID="FinancialLabel" runat="server"></asp:Label>
                                    </td>
                                    <td align="right" bgcolor="Silver">
                                        <asp:Panel ID="ChargingPanel" runat="server">
                                            <asp:Button ID="ChargeButton" runat="server" onclick="ChargeButton_Click" Text="Charge order" />
                                            <asp:LinkButton ID="PartialChargingButton" runat="server" onclick="PartialChargingButton_Click">Partial charging</asp:LinkButton>
                                        </asp:Panel>
                                        <asp:Panel ID="PartialChargingPanel" runat="server">
                                            <asp:TextBox ID="PartialAmountTextBox" runat="server"></asp:TextBox>
                                            <asp:Button ID="ChargePartialButton" runat="server" Text="Charge" 
                                                onclick="ChargePartialButton_Click" />
                                            <asp:LinkButton ID="CancelPartialChargingButton" runat="server" onclick="CancelPartialChargingButton_Click">Cancel</asp:LinkButton>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td bgcolor="Silver">
                                        Fulfillment state:
                                        <asp:Label ID="FulfillmentLabel" runat="server"></asp:Label>
                                    </td>
                                    <td align="right" bgcolor="Silver">
                                        <asp:LinkButton ID="RefundPanelButton" runat="server" 
                                            onclick="RefundPanelButton_Click">Refund</asp:LinkButton>
                                        &nbsp;
                                        <asp:LinkButton ID="ShipPanelButton" runat="server" 
                                            onclick="ShipPanelButton_Click">Ship</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">                                        
                                        <asp:Panel ID="RefundPanel" runat="server">
                                            <table>
                                            <tr>
                                                <td>Comments:</td>
                                                <td>
                                                    <asp:TextBox ID="RefundCommentTextBox" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Refund amount:</td>
                                                <td>
                                                    <asp:TextBox ID="RefundTextBox" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                                        ControlToValidate="RefundTextBox" ErrorMessage="*"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="RangeValidator" runat="server" 
                                                        ControlToValidate="RefundTextBox" ErrorMessage="*" Type="Double"></asp:RangeValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="RefundStatusLabel" runat="server" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Button ID="RefundButton" runat="server" Text="Process refund" 
                                                        onclick="RefundButton_Click" />
                                                    &nbsp;<asp:Button ID="CancelRefundButton" runat="server" 
                                                        onclick="CancelRefundButton_Click" Text="Cancel" 
                                                        CausesValidation="False" />
                                                </td>
                                            </tr>
                                        </table>
                                        </asp:Panel>
                                        <asp:Panel ID="ShippingPanel" runat="server">
                                            <table>
                                                <tr>
                                                    <td align="right">Carrier:</td>
                                                    <td>
                                                        <asp:TextBox ID="CarrierTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">Tracking #:</td>
                                                    <td>
                                                        <asp:TextBox ID="TrackingNumberTextBox" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Button ID="SendShippingInfoButton" runat="server" 
                                                            Text="Send shipping info" onclick="SendShippingInfoButton_Click" />
                                                        &nbsp;<asp:Button ID="CancelSendingShippingInfoButton" runat="server" 
                                                            Text="Cancel" CausesValidation="False" 
                                                            onclick="CancelSendingShippingInfoButton_Click" />
                                                    </td>
                                                </tr>
                                            </table>                                               
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:GridView ID="OrderItemsGridView" runat="server" 
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
                                        Charged amount:
                                        <asp:Label ID="ChargedAmountLabel" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
    <AnonymousTemplate>
        <asp:Login ID="Login1" runat="server" TitleText="" PasswordRecoveryText="Password Recovery" PasswordRecoveryUrl="~/password.aspx" DestinationPageUrl="~/shop_orders.aspx">
        </asp:Login>
    </AnonymousTemplate>
</asp:LoginView>
