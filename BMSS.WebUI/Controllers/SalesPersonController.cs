using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class SalesPersonController : Controller
    {
        //Repositories
        private I_OSLP_Repository i_OSLP_Repository;

        public SalesPersonController(I_OSLP_Repository i_OSLP_Repository)
        {
            this.i_OSLP_Repository = i_OSLP_Repository;
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetSalesPersons()
        {
            var ResultObject = i_OSLP_Repository.SalesPersons.OrderBy(x => x.SlpCode).Select(e => new SelectListItem
            {
                Text = e.SlpName,
                Value = e.SlpCode.ToString()
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
    }
}