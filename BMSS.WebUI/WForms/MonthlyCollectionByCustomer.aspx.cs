using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Infrastructure.Identity;
using BMSS.WebUI.Models.UserIdentity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class MonthlyCollectionByCustomer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!IsPostBack)
                {

                    DocDateFrom.Text = DateTime.Now.ToString("dd'/'MM'/'yyyy");
                    DocDateTo.Text = DateTime.Now.ToString("dd'/'MM'/'yyyy");

                    DataTable ResultDataTable = new DataTable();
                    ResultDataTable.Columns.Add("CardName", typeof(String));
                    ResultDataTable.Columns.Add("CardCode", typeof(String));


                    string defaultCashSalesCusutomer = string.Empty;
                    int defaultSalesPerson = 0;
                    using (var dbContext = new AppIdentityDbContext())
                    {

                        AppUser applicationUser = dbContext.Users.Where(x => x.UserName.ToLower().Equals(User.Identity.Name)).FirstOrDefault();
                        defaultCashSalesCusutomer = applicationUser.preferredCashSalesCustomer.ToUpper();
                        defaultSalesPerson = int.Parse(applicationUser.preferredSalesPerson);
                    }


                    I_OCRD_Repository i_OCRD = new EF_OCRD_Repository();
                    IEnumerable<OCRD> CardNames = i_OCRD.ActiveCustomers;                  
                    foreach (var item in CardNames)
                    {
                        DataRow row = ResultDataTable.NewRow();
                        row["CardName"] = item.CardName;
                        row["CardCode"] = item.CardCode;
                        ResultDataTable.Rows.Add(row);
                    }

                     
                    CardCodeFrom.DataSource = ResultDataTable;
                    CardCodeFrom.DataTextField = "CardName";
                    CardCodeFrom.DataValueField = "CardCode";
                    CardCodeFrom.DataBind();

                    CardCodeTo.DataSource = ResultDataTable;
                    CardCodeTo.DataTextField = "CardName";
                    CardCodeTo.DataValueField = "CardCode";
                    CardCodeTo.DataBind();




                    I_OSLP_Repository i_OSLP = new EF_OSLP_Repository();
                    IEnumerable<OSLP> SalesPersonNames = i_OSLP.SalesPersons.Where(x => !x.SlpName.Equals("-No Sales Employee-") && x.SlpCode.Equals(defaultSalesPerson));
                    DataTable SLPResultDataTable = new DataTable();
                    SLPResultDataTable.Columns.Add("SlpName", typeof(String));
                    SLPResultDataTable.Columns.Add("SlpCode", typeof(String));
                    foreach (var item in SalesPersonNames)
                    {
                        DataRow row = SLPResultDataTable.NewRow();
                        row["SlpName"] = item.SlpName;
                        row["SlpCode"] = item.SlpCode;
                        SLPResultDataTable.Rows.Add(row);
                    }
                    SalesPersonFrom.DataSource = SLPResultDataTable;
                    SalesPersonFrom.DataTextField = "SlpName";
                    SalesPersonFrom.DataValueField = "SlpName";
                    SalesPersonFrom.DataBind();

                    SalesPersonTo.DataSource = SLPResultDataTable;
                    SalesPersonTo.DataTextField = "SlpName";
                    SalesPersonTo.DataValueField = "SlpName";
                    SalesPersonTo.DataBind();
                }
                this.CrystalReportViewer1.PDFOneClickPrinting = false;
                string ReportFileName = Server.MapPath("~\\App_Data\\Mthly Collect by Cust.rpt");
                CrystalDecisions.Web.CrystalReportSource crReportDocument = new CrystalDecisions.Web.CrystalReportSource();


                crReportDocument.Report.FileName = ReportFileName;
                crReportDocument.ReportDocument.FileName = ReportFileName;

                if (Session["FromDate"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("@FromDate", Session["FromDate"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("@FromDate", DateTime.Now);
                    Session["FromDate"] = DateTime.Now;
                }

                if (Session["ToDate"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("@ToDate", Session["ToDate"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("@ToDate", DateTime.Now);
                    Session["ToDate"] = DateTime.Now;
                }
                if (Session["CardCodeFr"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeFr@select * from OCRD Where CardType ='C' order by CardCode", Session["CardCodeFr"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeFr@select * from OCRD Where CardType ='C' order by CardCode", "");
                    Session["CardCodeFr"] = "";
                }

                if (Session["CardCodeTo"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeTo@select * from OCRD Where CardType ='C' order by CardCode", Session["CardCodeTo"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeTo@select * from OCRD Where CardType ='C' order by CardCode", "");
                    Session["CardCodeTo"] = "";
                }


                if (Session["SalesPersonFr"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonFr@Select SlpName From OSLP order by SLpName", Session["SalesPersonFr"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonFr@Select SlpName From OSLP order by SLpName", "");
                    Session["SalesPersonFr"] = "";
                }
                if (Session["SalesPersonTo"] != null)
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonTo@Select SlpName From OSLP order by SLpName", Session["SalesPersonTo"]);
                else
                {
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonTo@Select SlpName From OSLP order by SLpName", "");
                    Session["SalesPersonTo"] = "";
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
                string ReportFileName = Server.MapPath("~\\App_Data\\Mthly Collect by Cust.rpt");
                CrystalDecisions.Web.CrystalReportSource crReportDocument = new CrystalDecisions.Web.CrystalReportSource();


                crReportDocument.Report.FileName = ReportFileName;
                crReportDocument.ReportDocument.FileName = ReportFileName;

                if (DocDateFrom.Text.Trim() != string.Empty)
                {
                    crReportDocument.ReportDocument.SetParameterValue("@FromDate", DocDateFrom.Text.Trim());
                    Session["FromDate"] = DocDateFrom.Text.Trim();
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("@FromDate", Session["FromDate"]);

                if (DocDateTo.Text.Trim() != string.Empty)
                {
                    crReportDocument.ReportDocument.SetParameterValue("@ToDate", DocDateTo.Text.Trim());
                    Session["ToDate"] = DocDateTo.Text.Trim();
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("@ToDate", Session["ToDate"]);

                if (CardCodeFrom.SelectedValue != null)
                {
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeFr@select * from OCRD Where CardType ='C' order by CardCode", CardCodeFrom.SelectedValue);
                    Session["CardCodeFr"] = CardCodeFrom.SelectedValue;
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeFr@select * from OCRD Where CardType ='C' order by CardCode", Session["CardCodeFr"]);


                if (CardCodeTo.SelectedValue != null)
                {
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeTo@select * from OCRD Where CardType ='C' order by CardCode", CardCodeTo.SelectedValue);
                    Session["CardCodeTo"] = CardCodeTo.SelectedValue;
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("CardCodeTo@select * from OCRD Where CardType ='C' order by CardCode", Session["CardCodeTo"]);

                if (SalesPersonFrom.SelectedValue != null)
                {
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonFr@Select SlpName From OSLP order by SLpName", SalesPersonFrom.SelectedValue);
                    Session["SalesPersonFr"] = SalesPersonFrom.SelectedValue;
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonFr@Select SlpName From OSLP order by SLpName", Session["SalesPersonFr"]);

                if (SalesPersonTo.SelectedValue != null)
                {
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonTo@Select SlpName From OSLP order by SLpName", SalesPersonTo.SelectedValue);
                    Session["SalesPersonTo"] = SalesPersonTo.SelectedValue;
                }
                else
                    crReportDocument.ReportDocument.SetParameterValue("SalesPersonTo@Select SlpName From OSLP order by SLpName", Session["SalesPersonTo"]);


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