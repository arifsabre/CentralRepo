<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayIFrameRpt.aspx.cs" Inherits="ManufacturingManagement_V2.Reports.DisplayIFrameRpt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
     <%--<script type="text/javascript">
         function zoom() {
             document.body.style.zoom = "100%";
         //or
             //document.body.style.zoom = screen.logicalXDPI / screen.deviceXDPI;//for IE
         }
</script>--%>
    <script type="text/javascript">
        function setSize(elem) {
            var the_height;
            the_height = elem.contentWindow.document.body.scrollHeight;
            elem.height = the_height; // Its works fine in IE, Chrome, Safari but not work in FF and opera
        }
       </script> 
</head>
<%--<body onload="zoom()">--%>
<body>
    <form id="form1" runat="server">
    <div style="border:solid;border-width:1px;width:98%;">
    <table style="width:100%;">
            <tr>
                <td>
                    <span style="color: rgb(255, 102, 0); font-family: Arial, Helvetica, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: bold; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(247, 247, 247); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">PRAG ERP </span>
                    <asp:Label ID="lblRptName" runat="server" Font-Bold="True" Font-Size="12pt" ForeColor="Blue" Font-Names="Verdana"></asp:Label>
                                &nbsp;
                    <asp:Label ID="lblUser" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="7pt"></asp:Label>
                                <asp:HiddenField ID="hfOptionURL" runat="server" />
                                <asp:HiddenField ID="hfReturnURL" runat="server" />
                </td>
                <td align="right" width="15%">
                    <asp:LinkButton ID="lbOption" runat="server" Font-Size="10pt" OnClick="lbOption_Click" Font-Names="Verdana" Font-Bold="True">Options</asp:LinkButton>
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="lblRetuen" runat="server" Font-Size="10pt" OnClick="lblRetuen_Click" Font-Names="Verdana" Font-Bold="True">Back</asp:LinkButton>
                &nbsp;&nbsp;|&nbsp;
                    <asp:LinkButton ID="lblLogout" runat="server" Font-Size="10pt" OnClick="lblLogout_Click" Font-Names="Verdana" Font-Bold="True">Logout</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="border:dotted;border-width:2px;">
                        <iframe id="frmRpt" runat="server" style="width: 100%;height: 100px; 
                            border: none;background-color:lightblue;border:medium;border-color:blue;
                            vertical-align:central;">
                        </iframe>
                    </div>
                </td>
            </tr>
      </table>
    </div>
    </form>
</body>
</html>