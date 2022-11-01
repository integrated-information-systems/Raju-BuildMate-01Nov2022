<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DOCannonViewer.aspx.cs" Inherits="BMSS.WebUI.WForms.DOCannonViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">                
        <center>
            <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="True" BestFitPage="True" PrintMode="ActiveX" />
        </center>
         
    </form>
     
</body>
</html>

