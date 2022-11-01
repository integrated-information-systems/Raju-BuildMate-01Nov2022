using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class GeneralLedgerController : Controller
    {
        //Repositories
        private I_OACT_Repository i_OACT_Repository;
        private I_OCRC_Repository i_OCRC_Repository;
        public GeneralLedgerController(I_OACT_Repository i_OACT_Repository, I_OCRC_Repository i_OCRC_Repository)
        {
            this.i_OACT_Repository = i_OACT_Repository;
            this.i_OCRC_Repository = i_OCRC_Repository;
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetGLCodes()
        {
            var ResultObject = i_OACT_Repository.GLCodes.Select(e => new SelectListItem
            {
                Text = e.AcctName,
                Value = e.AcctCode
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetPaymentTypes()
        {
            //List<SelectListItem> ResultObject = new List<SelectListItem>
            //{
            //    new SelectListItem() { Text = "Cash", Value = "Cash" },
            //    new SelectListItem() { Text = "Nets", Value = "Nets" },
            //    new SelectListItem() { Text = "Visa / Master", Value = "Visa/Master" },
            //    new SelectListItem() { Text = "Cheque", Value = "Cheque" },
            //    new SelectListItem() { Text = "GIRO / Transfer", Value = "GIRO/Transfer" },
            //    new SelectListItem() { Text = "PayNow", Value = "PayNow" }
            //};
            var ResultObject = i_OCRC_Repository.CreditCards.Select(e => new SelectListItem
            {
                Text = e.CardName,
                Value = e.CreditCard.ToString()
            }).ToList();

            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
    }
}