<%@ Control Language="C#" AutoEventWireup="true" CodeFile="shop_orders.ascx.cs" Inherits="systems_shop_orders" %>

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
                                    <asp:TextBox ID="FromDateTextBox" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="ToDateTextBox" runat="server"></asp:TextBox>
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
                        <asp:GridView ID="OrdersGridView" runat="server" SkinID="gridOrders" AutoGenerateColumns="False" AllowSorting="true" AllowPaging="true">
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
                        <asp:Label ID="StatusLabel" runat="server"></asp:Label>
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
