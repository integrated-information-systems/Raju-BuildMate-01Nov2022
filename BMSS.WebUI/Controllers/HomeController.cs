using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            
            return View();
        }
       
    }
}