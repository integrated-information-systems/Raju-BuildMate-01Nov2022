using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class TaxController : Controller
    {
        //Repositories
        private I_OVTG_Repository i_OVTG_Repository;

        public TaxController(I_OVTG_Repository i_OVTG_Repository)
        {
            this.i_OVTG_Repository = i_OVTG_Repository;
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetTaxCodes(string TaxType)
        {
            
            var ResultObject = i_OVTG_Repository.TaxCodes.Where(x =>  x.Category.Equals(TaxType)).OrderBy(x => x.Name).Select(e => new SelectListItem
            {
                Text = e.Code,
                Value = e.Rate.ToString()
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
    }
}