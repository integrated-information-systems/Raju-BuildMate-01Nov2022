using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSS.WebUI.WForms
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public static string companyFullName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            companyFullName = System.Configuration.ConfigurationManager.AppSettings["CompanyFullName"];
        }
    }
}