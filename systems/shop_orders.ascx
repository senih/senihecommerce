<%@ Control Language="C#" AutoEventWireup="true" CodeFile="shop_orders.ascx.cs" Inherits="systems_shop_orders" %>

<asp:LoginView ID="LoginView" runat="server">
    <RoleGroups>
        <asp:RoleGroup Roles="Administrators">
            <ContentTemplate>
            <div>
                <asp:GridView ID="OrdersGridView" runat="server" AutoGenerateColumns="False" AllowSorting="true" AllowPaging="true">
                    <Columns>
                        <asp:BoundField DataField="order_id" HeaderText="ID" />
                        <asp:BoundField DataField="google_order_number" HeaderText="Google #" />
                        <asp:BoundField DataField="total" HeaderText="Total" />
                        <asp:BoundField DataField="status" HeaderText="Payment" />
                        <asp:BoundField DataField="shipping_status" HeaderText="Shipment" />
                        <asp:BoundField DataField="order_by" HeaderText="User" />
                        <asp:BoundField DataField="order_date" HeaderText="Order date" />
                        <asp:CommandField ButtonType="Button" ShowSelectButton="true" />
                    </Columns>
                </asp:GridView>
            </div>
            </ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
    <AnonymousTemplate>
        <asp:Login ID="Login1" runat="server" TitleText="" PasswordRecoveryText="Password Recovery" PasswordRecoveryUrl="~/password.aspx" DestinationPageUrl="~/shop_orders.aspx">
        </asp:Login>
    </AnonymousTemplate>
</asp:LoginView>
