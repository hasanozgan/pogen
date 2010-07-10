<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%= _("Hello World") %>
    </div>
    
    <asp:HyperLink ID="Test" NavigateUrl='~/default.aspx?555' runat="server" ><%= _("Hello World") %></asp:HyperLink>
    
    <asp:DropDownList ID="DropDownList1" runat="server" 
        AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
    </asp:DropDownList>
<%--    
        <asp:GridView ID="GridView1" runat="server">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText='<%#  _("Hello World") %>' />
        </Columns>
        </asp:GridView>--%>
    </form>
</body>
</html>
