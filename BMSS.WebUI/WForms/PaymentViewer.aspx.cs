using BMSS.Domain.Abstract;
using BMSS.Domain.Concrete;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class PaymentViewer : System.Web.UI.Page
    {
        public string URL = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                //if(Request.Url.IsDefaultPort)
                //{
                //    baseUrl += Request.Url.Port+20;
                //}
                //else
                //{
                //    baseUrl += Request.Url.Port;
                //}
                if (baseUrl.Equals("http://sapb1viewer.xyz"))
                {
                    baseUrl += ":81";
                }
                baseUrl += Request.ApplicationPath.TrimEnd('/') + "/";

                string fullURL = string.Empty;
                fullURL = Request.Url.ToString();
                String[] urlParts = fullURL.Split('/');

                string DocEntry = "-1";
                bool isNumeric = int.TryParse(urlParts[urlParts.Length - 1], out int n);
                if (isNumeric)
                {
                    DocEntry = urlParts[urlParts.Length - 1];
                }
                I_PaymentDocH_Repository i_PaymentDocH_Repository = new EF_PaymentDocHeader_Repository();
                string DocNum = i_PaymentDocH_Repository.UpdatePrintStatus(DocEntry, User.Identity.Name);
                string pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Payment + "_" + DocNum + "_" + DateTime.Now.ToString("ddMMyy") + ".pdf";
                URL = baseUrl + "WForms/ViewerPrint.aspx?DE=" + DocNum + "&Ty=" + BMSS.WebUI.Models.General.PrintDocTypes.Payment;

                ReportDocument cryRpt = new ReportDocument();
                cryRpt.Load(Server.MapPath("~\\App_Data\\Sales_Receipt.rpt"));
                cryRpt.SetDatabaseLogon("sa", "B1Admin");
                cryRpt.SetParameterValue("DocEntry", DocEntry);
                cryRpt.SetParameterValue("printuser", User.Identity.Name.ToString());

                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                CrDiskFileDestinationOptions.DiskFileName = Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName);
                CrExportOptions = cryRpt.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }

                cryRpt.Export();


                CrDiskFileDestinationOptions.DiskFileName = @"\\192.168.5.11\Hot Folder\" + pdfFileName;
                CrExportOptions = cryRpt.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }

                cryRpt.Export();

                this.CrystalReportViewer1.ReportSource = cryRpt;
                this.CrystalReportViewer1.ToolPanelView = ToolPanelViewType.None;
                this.CrystalReportViewer1.Zoom(125);
            }
            else
            {
                Response.Redirect("~");
            }

        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            this.CrystalReportViewer1.Dispose();
            GC.Collect();
        }
    }
}