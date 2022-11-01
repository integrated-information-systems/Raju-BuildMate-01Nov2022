<%@ Page Title="" Language="C#" MasterPageFile="~/WForms/Site.Master" AutoEventWireup="true" CodeBehind="SOA.aspx.cs" Inherits="BMSS.WebUI.WForms.SOA" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentHeader" runat="server">
    <section class="content-header">
        <h1 id="`">
           Statement of Account
            <small>Statement of Account</small>
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
                                  <asp:Label ID="lblForCardCodeFrom"  runat="server" Text="From Customer"></asp:Label>
                                  <asp:DropDownList ID="CardCodeFrom" runat="server" CssClass="form-control select2"  ></asp:DropDownList>                                     
                                </div> 
                                <div class="form-group col">
                                  <asp:Label ID="lblForCardCodeTo"  runat="server" Text="To"></asp:Label>
                                  <asp:DropDownList ID="CardCodeTo" runat="server" CssClass="form-control select2"  ></asp:DropDownList>                                     
                                </div> 
                            </div>
                        </div>                        
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-6 col-lg-4 col-xl-6">
                            <div class="row">
                                <div class="form-group col">
                                  <asp:Label ID="lblForSalesPersonFrom"  runat="server" Text="From Sales Person"></asp:Label>
                                  <asp:DropDownList ID="SalesPersonFrom" runat="server" CssClass="form-control select2"  ></asp:DropDownList>                                     
                                </div> 
                               <%-- <div class="form-group col">
                                  <asp:Label ID="lblForSalesPersonTo"  runat="server" Text="To"></asp:Label>
                                  <asp:DropDownList ID="SalesPersonTo" runat="server" CssClass="form-control select2"  ></asp:DropDownList>                                     
                                </div> --%>
                            </div>
                        </div>                        
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-6 col-lg-4 col-xl-6">
                            <div class="row">
                                <div class="form-group col">
                                  <asp:Label ID="lblForDocDateFrom"  runat="server" Text="From Date"></asp:Label>
                                  <asp:TextBox ID="DocDateFrom" runat="server" CssClass="form-control datepicker"  ></asp:TextBox>                                        
                                </div> 
                               <%-- <div class="form-group col">
                                  <asp:Label ID="lblForDocDateTo"  runat="server" Text="To"></asp:Label>
                                  <asp:TextBox ID="DocDateTo" runat="server" CssClass="form-control datepicker"  ></asp:TextBox>                                       
                                </div> --%>
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
