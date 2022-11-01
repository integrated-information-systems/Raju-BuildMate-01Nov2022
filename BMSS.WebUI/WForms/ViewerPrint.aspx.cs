using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class ViewerPrint : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                string DocNum = Request.QueryString["DE"];
                string DocType = Request.QueryString["Ty"];
                string pdfFileName =  "_" + DocNum + "_" + DateTime.Now.ToString("ddMMyy") + ".pdf";
                FileStream fs = null;
                switch (DocType)
                {
                    case "Sales_Quotation":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Sales_Quotation + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Delivery_Order":
                        pdfFileName = "BM_DO" + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" +  pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "DO_Tax_Invoice":
                        pdfFileName = "BM_INV" + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Cash_Sales":
                        pdfFileName = "BM_DO" + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Cash_Sales_Tax_Invoice":
                        pdfFileName = "BM" + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Cash_Sales_Credit":
                        pdfFileName = "BM" + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Purchase_Order":
                        pdfFileName = "BM" + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Purchase_Delivery_Note":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Purchase_Delivery_Note + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Stock_Transfer":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Stock_Transfer + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Stock_In":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Stock_In + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Stock_Out":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Stock_Out + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "Payment":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.Payment + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                    case "SAP_PO":
                        pdfFileName = BMSS.WebUI.Models.General.PrintDocTypes.SAP_PO + pdfFileName;
                        fs = new FileStream(Server.MapPath("~\\App_Data\\ReportFiles\\" + pdfFileName), FileMode.Open, FileAccess.Read);
                        break;
                }
                
                BinaryReader br = new BinaryReader(fs);
                Byte[] bytes = br.ReadBytes(Convert.ToInt32(fs.Length));
                br.Close();
                fs.Close();

                Response.Buffer = true;

                Response.Charset = "";

                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Response.ContentType = "application/pdf";

                Response.AddHeader("content-disposition", "attachment;filename=" + pdfFileName);

                Response.BinaryWrite(bytes);

                Response.Flush();

                Response.End();

            }
            else
            {
                Response.Redirect("~");
            }
        }
    }
}