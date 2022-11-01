using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class LoadPDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            FileStream fs = new FileStream(Server.MapPath("~\\App_Data\\test.pdf"), FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] bytes = br.ReadBytes(Convert.ToInt32(fs.Length));
            br.Close();
            fs.Close();

            Response.Buffer = true;
    
            Response.Charset = "";

                Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.ContentType = "application/pdf";

            Response.AddHeader("content-disposition", "attachment;filename=" + Server.MapPath("~\\App_Data\\test.pdf"));

            Response.BinaryWrite(bytes);

            Response.Flush();

            Response.End();
        }
    }
}