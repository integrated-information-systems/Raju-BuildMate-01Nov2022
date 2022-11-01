using System.Web.Mvc;

namespace MVCTemplateV1.WebUI.Controllers
{
    public class ErrorHandlerController : Controller
    {
        // GET: ErrorHandler
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotFound404()
        {
            return View("404Error");
        }
        public ActionResult InternalError500()
        {
            return View("500Error");
        }
    }
}