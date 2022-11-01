using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.ChangeCRLimitViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {                 
            //Repositories
            private I_OCRD_Repository i_OCRD_Repository;
            private I_ORCT_Repository i_ORCT_Repository;
            private I_OCPR_Repository i_OCPR_Repository;
            private I_DODocH_Repository i_DODocH_Repository;
            private I_SQDocH_Repository i_SQDocH_Repository;
            private I_CashSalesCreditDocH_Repository i_CashSalesCreditDocH_Repository;
            private I_CashSalesDocH_Repository i_CashSalesDocH_Repository;
            private I_PaymentDocH_Repository i_PaymentDocH_Repository;
        public CustomerController(I_ORCT_Repository i_ORCT_Repository, I_PaymentDocH_Repository i_PaymentDocH_Repository, I_CashSalesCreditDocH_Repository i_CashSalesCreditDocH_Repository, I_CashSalesDocH_Repository i_CashSalesDocH_Repository, I_OCRD_Repository i_OCRD_Repository, I_SQDocH_Repository i_SQDocH_Repository, I_OCPR_Repository i_OCPR_Repository, I_DODocH_Repository i_DODocH_Repository)
            {
                this.i_OCRD_Repository = i_OCRD_Repository;                
                this.i_OCPR_Repository = i_OCPR_Repository;
                this.i_DODocH_Repository = i_DODocH_Repository;
                this.i_SQDocH_Repository = i_SQDocH_Repository;
                this.i_CashSalesCreditDocH_Repository= i_CashSalesCreditDocH_Repository;
                this.i_CashSalesDocH_Repository = i_CashSalesDocH_Repository;
                this.i_PaymentDocH_Repository = i_PaymentDocH_Repository;
                this.i_ORCT_Repository = i_ORCT_Repository;
        }

        [HttpGet]
        [AjaxOnly]
        public JsonResult GetActiveCustomers()
        {
            var ResultObject = i_OCRD_Repository.Customers.Select(e => new SelectListItem
            {
                Text = e.CardName,
                Value = e.CardCode
            }).ToList();
        return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomersList()
        {
            //Stock qty by location
            var CustomerList = i_OCRD_Repository.Customers.Select(e => new ChangeCRLimitViewModel
            {
                 CardCode = e.CardCode,
                 CardName = e.CardName,
                 Balance = e.Balance,
                CreditLine = e.CreditLine,                
            }).ToList();
            return Json(CustomerList, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerDetails(string CardCode)
        {

            if (CardCode!=null) {
                string DefaultTax = string.Empty;

                var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CardCode);

                if (ResultCustomerObject.ECVatGroup != null)
                    DefaultTax = ResultCustomerObject.ECVatGroup;
                else
                    DefaultTax = ConfigurationManager.AppSettings["DefaultOutgoingTax"];

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
                string DefaultTax = ConfigurationManager.AppSettings["DefaultOutgoingTax"];
                var ResultObject = new
                {
                    OfficeTelNo = "",
                    ContactID ="",
                    Currency="",
                    PymntGroup="",
                    Fax="",
                    SlpName="",
                    BillToDef="",
                    ShipToDef="",
                    ECVatGroup="",
                    DefaultTaxGroup = DefaultTax,
                    PaymentTermDays = "0"
                };
                return Json(ResultObject, JsonRequestBehavior.DenyGet);
            }

           
        }
        [Authorize(Roles = "Customers")]
        // GET: Customer
        public ActionResult Index()
        {
            var Result = i_OCRD_Repository.GetListofCustomers();
            //foreach (OCRD ocrd in Result)
            //{
            //    OCRD custObject = i_OCRD_Repository.GetCustomerDetails(ocrd.CardCode);

            //    AgingBucket agingBucketSAP = i_OCRD_Repository.GetCustomerAgingBucketNew(ocrd.CardCode, custObject.SalesPerson.SlpName);

            //    ocrd.Balance = agingBucketSAP.OutStandingBalance.Value;
            //}


            return View(Result);
        }
            public ActionResult Detail(string CardCode)
            {

                //CardCode = Server.UrlDecode(CardCode);
                //CardCode = CardCode.Trim();
                OCRD custObject = i_OCRD_Repository.GetCustomerDetails(CardCode);
               
                
                if (custObject != null)
                {
                    if (custObject.CntctPrsn != null)
                    {
                        ViewBag.ContactSecondary = i_OCPR_Repository.GetSecondaryContactDetails(CardCode, custObject.CntctPrsn);
                    }
                    decimal DOThisPeriod = i_DODocH_Repository.DOTotalByCustomerCodeForYearMonth(CardCode, DateTime.Now.Year, DateTime.Now.Month);
                    decimal DOThisYTD = i_DODocH_Repository.DOTotalByCustomerCodeForYear(CardCode, DateTime.Now.Year);
                    decimal DOLastYTD = i_DODocH_Repository.DOTotalByCustomerCodeForYear(CardCode, DateTime.Now.Year-1);
                    ViewBag.SQTrans = i_SQDocH_Repository.SalesQuotationHeaderListByCardCode(CardCode);
                    ViewBag.DOTrans = i_DODocH_Repository.DOHeaderListByCardCode(CardCode);
                    ViewBag.CSTrans = i_CashSalesDocH_Repository.CSHeaderListByCardCode(CardCode);
                    ViewBag.CSCTrans = i_CashSalesCreditDocH_Repository.CSCHeaderListByCardCode(CardCode);

                    //decimal TotalDocBalance = i_DODocH_Repository.GetTotalSystemBalanceByCardCode(CardCode);
                    //custObject.Balance = TotalDocBalance;
                    ViewBag.ThisPeriod = DOThisPeriod;
                    ViewBag.ThisYTD = DOThisYTD;
                    ViewBag.LastYTD = DOLastYTD;
                    DateTime LastPaid = i_PaymentDocH_Repository.GetLastPaidDate(CardCode);
                    AgingBucket agingBucketSAP = i_OCRD_Repository.GetCustomerAgingBucketNew(CardCode, custObject.SalesPerson.SlpName);
                //    AgingBucket agingBucketWebsite = i_DODocH_Repository.GetCustomerAgingBucket(CardCode);
                    AgingBucket agingBucketWebsite = new AgingBucket();
                    AgingBucket agingBucket = new AgingBucket();
                    agingBucket.Current = agingBucketSAP.Current.GetValueOrDefault() + agingBucketWebsite.Current.GetValueOrDefault();
                    agingBucket.Months1 = agingBucketSAP.Months1.GetValueOrDefault() + agingBucketWebsite.Months1.GetValueOrDefault();
                    agingBucket.Months2 = agingBucketSAP.Months2.GetValueOrDefault() + agingBucketWebsite.Months2.GetValueOrDefault();
                    agingBucket.Months3 = agingBucketSAP.Months3.GetValueOrDefault() + agingBucketWebsite.Months3.GetValueOrDefault();
                    agingBucket.Months4 = agingBucketSAP.Months4.GetValueOrDefault() + agingBucketWebsite.Months4.GetValueOrDefault();
                    agingBucket.Months5 = agingBucketSAP.Months5.GetValueOrDefault() + agingBucketWebsite.Months5.GetValueOrDefault();
                    agingBucket.OutStandingBalance = agingBucketSAP.OutStandingBalance;
                    ViewBag.agingBucket = agingBucket;
                    ViewBag.LastPaid =  LastPaid == DateTime.MinValue ? "" : LastPaid.ToString("dd-MM-yyyy");
                    ViewBag.LastPaids = i_ORCT_Repository.GetLastPaidRecordsByCustomer(CardCode).Select(x=> new ORCT { DocDate= x.DocDate, DocTotal= x.DocTotal });
                
            }
                else
                {
                    TempData["GlobalErrMsg"] = "Customer not found";
                    return RedirectToAction("Index");
                }
                return View(custObject);
            }            

    }


}