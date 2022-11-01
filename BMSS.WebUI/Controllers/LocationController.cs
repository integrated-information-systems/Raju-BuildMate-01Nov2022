using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    public class LocationController : Controller
    {
        //Repositories
        private I_OWHS_Repository i_OWHS_Repository;

        public LocationController(I_OWHS_Repository i_OWHS_Repository)
        {
            this.i_OWHS_Repository = i_OWHS_Repository;
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetWarehouses()
        {
            var ResultObject = i_OWHS_Repository.Warehouses.Select(e => new SelectListItem
            {
                Text = e.WhsName,
                Value = e.WhsCode.ToString()
            }).ToList();          
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
    }
}