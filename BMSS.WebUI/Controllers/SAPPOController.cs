using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.SAPPOViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
        [Authorize(Roles = "Purchase Order(SAP)")]
    public class SAPPOController : Controller
        {
        private I_OPOR_Repository i_OPOR_Repository;
        public SAPPOController(I_OPOR_Repository i_OPOR_Repository)
        {
            this.i_OPOR_Repository = i_OPOR_Repository;
        }
        [HttpPost]
        [AjaxOnly]
        // GET: Item
        public JsonResult IndexPagining(DTPagination pagination)
        {
            string searchValue = pagination.search.value;
            string orderBy = "";
            string orderByDirection = pagination.order[0].dir;
            switch (int.Parse(pagination.order[0].column))
            {
                case 0:
                    orderBy = "DocNum";
                    break;
                
                default:
                    orderBy = "DocNum";
                    break;
            }

            string orderByColumn = $"{orderBy} {orderByDirection}";
            var listOfItems = i_OPOR_Repository.GetPODetailsWithPagination(pagination.start, pagination.length, searchValue, orderByColumn);

            var items = listOfItems.Select((x) => new SAPPOListViewModel()
            {
                DocNum = x.SeriesPrefix.BeginStr + x.DocNum.ToString(),
                CardName = x.Customer.CardName,
                DocDate = x.DocDate.ToString("dd'/'MM'/'yyyy"),
                DocDueDate = x.DocDueDate.ToString("dd'/'MM'/'yyyy"),
                DocTotal = x.DocTotal,
                DocStatus = x.DocStatus,           
                DocEntry = x.DocEntry

            });

            int TotalRecords = i_OPOR_Repository.GetPOTotalCount();
            int FilteredTotalRecords = i_OPOR_Repository.GetPODetailsWithPaginationCount(searchValue);
            if (String.IsNullOrEmpty(searchValue))
                FilteredTotalRecords = TotalRecords;
            return Json(new
            {
                draw = pagination.draw,
                recordsTotal = TotalRecords,
                recordsFiltered = FilteredTotalRecords,
                data = items
            }, JsonRequestBehavior.AllowGet);

        }
            // GET: SAPPO
            public ActionResult Index()
        {
            return View();
        }
    }
}