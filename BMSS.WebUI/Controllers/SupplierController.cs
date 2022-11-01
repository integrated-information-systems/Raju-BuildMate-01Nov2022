using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    
    [Authorize]
    public class SupplierController : Controller
    { 
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OCPR_Repository i_OCPR_Repository;
        private I_GRPODocH_Repository i_GRPODocH_Repository;
        private I_PODocH_Repository i_PODocH_Repository;

        // GET: SNoteAll
        public SupplierController(I_GRPODocH_Repository i_GRPODocH_Repository, I_PODocH_Repository i_PODocH_Repository,I_OCRD_Repository i_OCRD_Repository, I_OCPR_Repository i_OCPR_Repository)
        {
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OCPR_Repository = i_OCPR_Repository;
            this.i_PODocH_Repository = i_PODocH_Repository;
            this.i_GRPODocH_Repository = i_GRPODocH_Repository;

        }
        [Authorize(Roles = "Suppliers")]
        // GET: Supplier
        public ActionResult Index()
        {
            return View(i_OCRD_Repository.Suppliers);
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetActiveSuppliers()
        {
            var ResultObject = i_OCRD_Repository.Suppliers.Select(e => new SelectListItem
            {
                Text = e.CardName,
                Value = e.CardCode
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Suppliers")]
        public ActionResult Detail(string CardCode)
        {
            OCRD custObject = i_OCRD_Repository.GetSupplierDetails(CardCode);
            
            if (custObject != null)
            {
                if (custObject.CntctPrsn != null)
                {
                    ViewBag.ContactSecondary = i_OCPR_Repository.GetSecondaryContactDetails(CardCode, custObject.CntctPrsn);                   
                }

                
                var APMemos = i_OCRD_Repository.GetSupplierARMemos(custObject.CardCode);
                var APInvoices = i_OCRD_Repository.GetSupplierAPInvoices(custObject.CardCode);

                decimal value1 = APInvoices.Where(e => e.DocDate.Month.Equals(DateTime.Now.Month) && e.DocDate.Year.Equals(DateTime.Now.Year) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal);
                decimal value2 = APMemos.Where(e => e.DocDate.Month.Equals(DateTime.Now.Month) && e.DocDate.Year.Equals(DateTime.Now.Year) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal);

                decimal ThisPeriod = APInvoices.Where(e => e.DocDate.Month.Equals(DateTime.Now.Month) && e.DocDate.Year.Equals(DateTime.Now.Year) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal) - APMemos.Where(e => e.DocDate.Month.Equals(DateTime.Now.Month) && e.DocDate.Year.Equals(DateTime.Now.Year) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal);
                decimal ThisYTD = APInvoices.Where(e => e.DocDate.Year.Equals(DateTime.Now.Year) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal) - APMemos.Where(e => e.DocDate.Year.Equals(DateTime.Now.Year) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal);
                decimal LastYTD = APInvoices.Where(e => e.DocDate.Year.Equals(DateTime.Now.Year - 1) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal) - APMemos.Where(e => e.DocDate.Year.Equals(DateTime.Now.Year - 1) && e.DocStatus.Equals("O") && e.CANCELED.Equals("N")).Sum(c => c.DocTotal);



                ViewBag.ThisPeriod = ThisPeriod;
                ViewBag.ThisYTD = ThisYTD;
                ViewBag.LastYTD = LastYTD;

                ViewBag.POTrans = i_PODocH_Repository.POHeaderListByCardCode(CardCode);
                ViewBag.GRPOTrans = i_GRPODocH_Repository.GRPOHeaderListByCardCode(CardCode);
            }

            else
            {
                TempData["GlobalErrMsg"] = "Supplier not found";
                return RedirectToAction("Index");
            }
            return View(custObject);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetSupplierDetails(string CardCode)
        {

            if (CardCode != null)
            {
                string DefaultTax = string.Empty;

                var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(CardCode);

                if (ResultCustomerObject.ECVatGroup != null)
                    DefaultTax = ResultCustomerObject.ECVatGroup;
                else
                    DefaultTax = ConfigurationManager.AppSettings["DefaultIncomingTax"];

                var ResultObject = new
                {
                    OfficeTelNo = ResultCustomerObject.Phone1,
                    ContactID = ResultCustomerObject.CntctPrsn,
                    ResultCustomerObject.Currency,
                    ResultCustomerObject.PaymentTerm.PymntGroup,
                    ResultCustomerObject.Fax,
                    ResultCustomerObject.SalesPerson.SlpName,
                    ResultCustomerObject.BillToDef,
                    ResultCustomerObject.ShipToDef,
                    ResultCustomerObject.ECVatGroup,
                    DefaultTaxGroup = DefaultTax,
                    PaymentTermDays = ResultCustomerObject.PaymentTerm.ExtraDays
                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }
            else
                {
                string DefaultTax = ConfigurationManager.AppSettings["DefaultIncomingTax"];
                var ResultObject = new
                {
                    OfficeTelNo = "",
                    ContactID = "",
                    Currency = "",
                    PymntGroup = "",
                    Fax = "",
                    SlpName = "",
                    BillToDef = "",
                    ShipToDef = "",
                    ECVatGroup = "",
                    DefaultTaxGroup = DefaultTax,
                    PaymentTermDays = "0"
                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }


        }
    }
}