<%@ Page Title="" Language="C#" MasterPageFile="~/WForms/Site.Master" AutoEventWireup="true" CodeBehind="ItemWithWrongLocation.aspx.cs" Inherits="BMSS.WebUI.WForms.ItemWithWrongLocation" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentHeader" runat="server">
    <section class="content-header">
        <h1 id="`">
          Items With Wrong Location
            <small>Items With Wrong Location Report</small>
        </h1>        
    </section>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row" id="MainContent">
        <div class="col-12">
            <div class="box box-primary">     
                <div class="box-header">
                    <asp:ValidationSummary ID="HeaderValidationSummary" runat="server" ValidationGroup="HeaderValidation" />
                </div>
                <div class="box-body">                    
                    <div class="row">
                        <div class="col-sm-12 col-md-6 col-lg-4 col-xl-6">
                            <div class="row">
                                <div class="form-group col">
                                  <asp:Label ID="lblCodeFrom"  runat="server" Text="BOM Item Code From"></asp:Label>
                                  <asp:DropDownList ID="CodeFrom" runat="server" CssClass="form-control select2"  ></asp:DropDownList>                                     
                                </div> 
                                <div class="form-group col">
                                  <asp:Label ID="lblCardCodeTo"  runat="server" Text="BOM Item Code To"></asp:Label>
                                  <asp:DropDownList ID="CodeTo" runat="server" CssClass="form-control select2"  ></asp:DropDownList>                                     
                                </div> 
                            </div>
                        </div>                        
                    </div>
                        <div class="row">
                         <div class="col-sm-12 col-md-6 col-lg-4 col-xl-6">
                             <div class="row">
                                <div class="form-group col">
                                <asp:Button ID="btnSearch" runat="server" Text="Search"  CssClass="btn btn-primary"   ValidationGroup="HeaderValidation" OnClick="btnSearch_Click" />
                                    </div>
                                 </div>
                         </div>
                    </div>
                </div>      
               
            </div>
        </div>
        <div class="col-12">
            <div class="row">
               <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="True" BestFitPage="True" PrintMode="ActiveX" />
            </div>
        </div>
    </div>
</asp:Content>
