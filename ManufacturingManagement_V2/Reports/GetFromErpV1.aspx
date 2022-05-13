<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetFromErpV1.aspx.cs" Inherits="ManufacturingManagement_V2.Reports.GetFromErpV1" %>

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
<body>
    <form id="form1" runat="server">
    <div style="border:solid;border-width:1px;width:98%; font-family: verdana, Geneva, Tahoma, sans-serif;">
        Just from ERP V.1</div>
    </form>
</body>
</html>