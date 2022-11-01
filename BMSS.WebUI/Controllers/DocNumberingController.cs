using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Document Numbering")]
    public class DocNumberingController : Controller
    {
        // GET: DocNumbering
        public ActionResult Index()
        {
            return View();
        }
    }
}