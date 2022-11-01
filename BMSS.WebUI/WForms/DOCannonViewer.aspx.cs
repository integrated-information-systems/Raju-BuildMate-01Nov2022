using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class DOCannonViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                string fullURL = string.Empty;
                fullURL = Request.Url.ToString();
                String[] urlParts = fullURL.Split('/');

                string DocEntry = "-1";
                bool isNumeric = int.TryParse(urlParts[urlParts.Length - 1], out int n);
                if (isNumeric)
                {
                    DocEntry = urlParts[urlParts.Length - 1];
                }
                ReportDocument cryRpt = new ReportDocument();
                cryRpt.Load(Server.MapPath("~\\App_Data\\Delivery_Order_Canon_Solution.rpt"));
                cryRpt.SetDatabaseLogon("sa", "B1Admin");
                cryRpt.SetParameterValue("DocEntry", DocEntry);

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