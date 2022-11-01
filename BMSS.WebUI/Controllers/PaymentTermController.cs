using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    public class PaymentTermController : Controller
    {
        //Repositories
        private I_OCTG_Repository i_OCTG_Repository;

        public PaymentTermController( I_OCTG_Repository i_OCTG_Repository)
        {           
            this.i_OCTG_Repository = i_OCTG_Repository;            
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetPaymentTerms()
        {
            var ResultObject = i_OCTG_Repository.PaymentTerms.Select(e => new SelectListItem
            {
                Text = e.PymntGroup,
                Value = e.GroupNum.ToString()
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
    }
}