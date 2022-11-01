using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Stock Loan")]
    public class StockLoanController : Controller
    {
        private I_DODocH_Repository i_DODocH_Repository;
        private I_DODocStockLoan_Repository i_DODocStockLoan_Repository;
        private I_OITW_Repository i_OITW_Repository;
        public StockLoanController(I_OITW_Repository i_OITW_Repository, I_DODocStockLoan_Repository i_DODocStockLoan_Repository, I_DODocH_Repository i_DODocH_Repository)
        {
            this.i_DODocStockLoan_Repository = i_DODocStockLoan_Repository;
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_OITW_Repository = i_OITW_Repository;
        }
        // GET: StockLoan
        public ActionResult Index()
        {
            return View(i_DODocStockLoan_Repository.StockLoanList);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Stock Loan-Edit")]
        public JsonResult ReverseStockLoan(long DOEntry, int rowNo)
        {


            string ValidationMessage = string.Empty;
            DateTime ReversedOn = DateTime.Now;
            string ReversedBy = User.Identity.Name;

            //**************** Insufficient Stock Check - Starts Here *******************
            //DODocStockLoan stockLoan = i_DODocStockLoan_Repository.Get(DOEntry);
            //string location = stockLoan.DODocH.DODocLs.Where(x=> x.LineNum.Equals(stockLoan.LineNum)).Select(x=> x.Location).FirstOrDefault();                        

            //decimal SAPAvailableQty = i_OITW_Repository.SAPAvailableQty(stockLoan.ItemCode, location);
            //decimal LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCode(stockLoan.ItemCode, location);
            //decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty - stockLoan.Qty);
            //if (TotalAvailableQty < 0)
            //{
            //    var ErrorObj = new
            //    {
            //        Status = 400,
            //        ResultHtml = $"Item {stockLoan.ItemCode} doesn't have enought stock"
            //    };
            //    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
            //}

            //else
            //**************** Insufficient Stock Check - Ends Here  *******************

            if (!i_DODocStockLoan_Repository.ReverseStockLoan(DOEntry, ReversedBy, ref ValidationMessage, ref ReversedOn))
            {

                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = ValidationMessage
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
            }
            else
            {
                    string rtime = ReversedOn.ToString("dd'/'MM'/'yyyy HH:mm:ss tt");
                    var DOResultObj = new
                    {
                        Status = 200,
                        ResultHtml = "",
                        ReversedOn=rtime,
                        rowNo
                    };
                    return Json(DOResultObj, JsonRequestBehavior.DenyGet);
            }


            
           




        }
    }
}