<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="BMSS.WebUI.WForms.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
      <link href="print.min.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src='print.min.js'></script>
</head>
<body>
     
     
     <p>
        <input type="button" id="bt" onclick="printJS({printable:'http://localhost/BMSSNew/Wforms/LoadPDF.aspx', type:'pdf', showModal:true})" value="Print PDF" />
    </p>
</body>
</html>
