using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class ContactPersonsController : Controller
    {
        //Repositories
        private I_OCPR_Repository i_OCPR_Repository;

        public ContactPersonsController(I_OCPR_Repository i_OCPR_Repository)
        {
            this.i_OCPR_Repository = i_OCPR_Repository;
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetContactPersons(string CardCode)
        {
            var ResultObject = i_OCPR_Repository.GetContactPersons(CardCode).Select(e => new SelectListItem()
            {
              Value = e.CntctCode.ToString(),
              Text = e.Name

            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetContactPersonsTelephoneNo(string ContactID)
        {
            if (ContactID != null) { 
                var ResultObject = i_OCPR_Repository.GetContactPersonDetails(Int32.Parse(ContactID));
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(string.Empty, JsonRequestBehavior.DenyGet);
            }
        }

    }
}