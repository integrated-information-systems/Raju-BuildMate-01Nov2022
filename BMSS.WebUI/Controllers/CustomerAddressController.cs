using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    public class CustomerAddressController : Controller
    {
        //Repositories
        private I_CRD1_Repository i_CRD1_Repository;

        public CustomerAddressController(I_CRD1_Repository i_CRD1_Repository)
        {
            this.i_CRD1_Repository = i_CRD1_Repository;
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerBillingAddresses(string CardCode)
        {
            var ResultObject = i_CRD1_Repository.GetCustomerBillingAddresses(CardCode).Select(e => new SelectListItem()
            {
                Value = e.Address,
                Text = e.Address

            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerBillingAddressDetails(string CardCode, string AddressCode)
        {
            var ResultObject = i_CRD1_Repository.GetCustomerBillingAddresses(CardCode).Where(x => x.Address.Equals(AddressCode)) .Select(e => new 
            {
                e.Street,
                e.Block,
                e.City,
                e.County,
                e.StreetNo
            }).FirstOrDefault();
            return Json(ResultObject, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerShippingAddresses(string CardCode)
        {
            var ResultObject = i_CRD1_Repository.GetCustomerShippingAddresses(CardCode).Select(e => new SelectListItem()
            {
                Value = e.Address,
                Text = e.Address

            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerShippingAddressDetails(string CardCode, string AddressCode)
        {
            var ResultObject = i_CRD1_Repository.GetCustomerShippingAddresses(CardCode).Where(x => x.Address.Equals(AddressCode)).Select(e => new
            {
                e.Street,
                e.Block,
                e.City,
                e.County,
                e.StreetNo
            }).FirstOrDefault();
            return Json(ResultObject, JsonRequestBehavior.DenyGet);
        }

    }
}