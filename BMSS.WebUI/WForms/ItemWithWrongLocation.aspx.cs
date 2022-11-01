using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Infrastructure.Identity;
using BMSS.WebUI.Models.UserIdentity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class ItemWithWrongLocation : System.Web.UI.Page
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!IsPostBack)
                {


                    DataTable ResultDataTable = new DataTable();
                    ResultDataTable.Columns.Add("Name", typeof(String));
                    ResultDataTable.Columns.Add("Code", typeof(String));
                    string defaultCashSalesCusutomer = string.Empty;
                    int defaultSalesPerson = 0;
                    using (var dbContext = new AppIdentityDbContext())
                    {

                        AppUser applicationUser = dbContext.Users.Where(x => x.UserName.ToLower().Equals(User.Identity.Name)).FirstOrDefault();
                        defaultCashSalesCusutomer = applicationUser.preferredCashSalesCustomer.ToUpper();
                        defaultSalesPerson = int.Parse(applicationUser.preferredSalesPerson);
                    }

                    I_OITT_Repository i_OITT = new EF_OITT_Repository();
IEnumerable<OITT> CardNames = i_OITT.GetAllOITT;                   
                    foreach (var item in CardNames)
                    {
                        DataRow row = ResultDataTable.NewRow();
                        row["Name"] = item.Name;
                        row["Code"] = item.Code;
                        ResultDataTable.Rows.Add(row);
                    }


                    CodeFrom.DataSource = ResultDataTable;
                    CodeFrom.DataTextField = "Name";
                    CodeFrom.DataValueField = "Code";
                    CodeFrom.DataBind();

                    CodeTo.DataSource = ResultDataTable;
                    CodeTo.DataTextField = "Name";
                    CodeTo.DataValueField = "Code";
                    CodeTo.DataBind();

                  
                }
                this.CrystalReportViewer1.PDFOneClickPrinting = false;
                string ReportFileName = Server.MapPath("~\\App_Data\\Items with Wrong Location.rpt");
                CrystalDecisions.Web.CrystalReportSource crReportDocument = new CrystalDecisions.Web.CrystalReportSource();


                crReportDocument.Report.FileName = ReportFileName;
                crReportDocument.ReportDocument.FileName = ReportFileName;

               

                if (Session["BOMCodeFr"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeFr@Select Code From OITT", Session["BOMCodeFr"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeFr@Select Code From OITT", "");
                    Session["BOMCodeFr"] = "";
                }
                if (Session["BOMCodeTo"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeTo@Select Code From OITT", Session["BOMCodeTo"]);
                                                                 
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeTo@Select Code From OITT", "");
                    Session["BOMCodeTo"] = "";
                }

                CrystalReportViewer1.ReportSource = crReportDocument;
                CrystalReportViewer1.EnableParameterPrompt = false;
                CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;



                CrystalReportViewer1.LogOnInfo[0].ConnectionInfo.ServerName = ConfigurationManager.AppSettings["DB_Server"];
                CrystalReportViewer1.LogOnInfo[0].ConnectionInfo.UserID = ConfigurationManager.AppSettings["DB_Username"];
                CrystalReportViewer1.LogOnInfo[0].ConnectionInfo.Password = ConfigurationManager.AppSettings["DB_Password"];
                CrystalReportViewer1.EnableDatabaseLogonPrompt = false;
            }
            else
            {
                Response.Redirect("~");
            }
        }         
        private void InitializeComponent()
        {            
            this.Load += new System.EventHandler(this.Page_Load);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {

                this.CrystalReportViewer1.PDFOneClickPrinting = false;
                string ReportFileName = Server.MapPath("~\\App_Data\\Items with Wrong Location.rpt");
                CrystalDecisions.Web.CrystalReportSource crReportDocument = new CrystalDecisions.Web.CrystalReportSource();


                crReportDocument.Report.FileName = ReportFileName;
                crReportDocument.ReportDocument.FileName = ReportFileName;

                if (CodeFrom.SelectedValue != null)
                {
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeFr@Select Code From OITT", CodeFrom.SelectedValue);
                    Session["BOMCodeFr"] = CodeFrom.SelectedValue;
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeFr@Select Code From OITT", Session["BOMCodeFr"]);

                if (CodeTo.SelectedValue != null)
                {
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeTo@Select Code From OITT", CodeTo.SelectedValue);
                    Session["BOMCodeTo"] = CodeTo.SelectedValue;
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("BOMCodeTo@Select Code From OITT", Session["BOMCodeTo"]);



                CrystalReportViewer1.ReportSource = crReportDocument;
                CrystalReportViewer1.EnableParameterPrompt = false;
                CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;



                CrystalReportViewer1.LogOnInfo[0].ConnectionInfo.ServerName = ConfigurationManager.AppSettings["DB_Server"];
                CrystalReportViewer1.LogOnInfo[0].ConnectionInfo.UserID = ConfigurationManager.AppSettings["DB_Username"];
                CrystalReportViewer1.LogOnInfo[0].ConnectionInfo.Password = ConfigurationManager.AppSettings["DB_Password"];
                CrystalReportViewer1.EnableDatabaseLogonPrompt = false;
            }
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            this.CrystalReportViewer1.Dispose();
            GC.Collect();
        }


    }
}