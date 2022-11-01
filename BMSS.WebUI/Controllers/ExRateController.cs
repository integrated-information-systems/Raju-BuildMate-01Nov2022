using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class ExRateController : Controller
    {

        //Repositories
        private I_ORTT_Repository i_ORTT_Repository;

        public ExRateController(I_ORTT_Repository i_ORTT_Repository)
        {
            this.i_ORTT_Repository = i_ORTT_Repository;
        }


        [HttpPost]
        [AjaxOnly]
        public JsonResult GetExchangeRate(string Currency, string DocDate)
        {

            DateTime ValidDate = new DateTime();

            if (DateTime.TryParseExact(DocDate, "dd'/'MM'/'yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out ValidDate).Equals(false))
            {
                return Json("", JsonRequestBehavior.DenyGet);
            }
            else if(Currency.Trim().Equals(""))
            {
                return Json("", JsonRequestBehavior.DenyGet);
            }
            else if (Currency.Trim().Equals("SGD"))
            {
                return Json(1, JsonRequestBehavior.DenyGet);
            }
            else
            {
                decimal ResultObject = i_ORTT_Repository.GetExchangeRate(Currency, ValidDate);
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }
            
        }    

    }
}