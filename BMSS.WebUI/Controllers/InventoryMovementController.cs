using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Models.InventoryMovementViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize]
    public class InventoryMovementController : Controller
    {
        private I_InventoryMovement_Repository i_InventoryMovement_Repository;
        private I_OITM_Repository i_OITM_Repository;
        
        public InventoryMovementController(I_InventoryMovement_Repository i_InventoryMovement_Repository, I_OITM_Repository i_OITM_Repository)
        {
            this.i_InventoryMovement_Repository = i_InventoryMovement_Repository;
            this.i_OITM_Repository = i_OITM_Repository;
        }

        // GET: InventoryMovement
        public ActionResult Index()
        {
            List<InventoryMovementReportViewModels> ListofDocs = new List<InventoryMovementReportViewModels>();
            ViewBag.ItemList = i_OITM_Repository.Items.Select(e => new SelectListItem
            {
                Text = e.ItemName,
                Value = e.ItemCode
            }).ToList();
            decimal ReportTotalQty = 0;
            ViewBag.ReportTotalQty = ReportTotalQty;
            return View(ListofDocs);
        }
        [HttpPost]       
        public ActionResult GetInventoryMovementDetails(string ItemCode)
        {
            decimal ReportTotalQty = 0;

            

            List<InventoryMovementReportViewModels> ListofDocs = new List<InventoryMovementReportViewModels>();
            List<InventoryMovementReportViewModels> ListofDOs = i_InventoryMovement_Repository.GetDOLinesByItemCode(ItemCode).Select(e => new InventoryMovementReportViewModels
            {
                DocDate = e.DODocH.DocDate,
                DocType = "DO/INV",
                DocNum = e.DODocH.DocNum,
                ItemCode = e.ItemCode,
                Description = e.Description,
                Qty = e.Qty*-1,
                UnitPrice = e.UnitPrice,
                LocationText =e.LocationText
            }).ToList();


            ReportTotalQty = ReportTotalQty + ListofDOs.Sum(x => x.Qty);

            ListofDocs.AddRange(ListofDOs);
            List<InventoryMovementReportViewModels> ListofGRPOs = i_InventoryMovement_Repository.GetGRPOLinesByItemCode(ItemCode).Select(e => new InventoryMovementReportViewModels
            {
                DocDate = e.GRPODocH.DocDate,
                DocType = "GRPO",
                DocNum = e.GRPODocH.DocNum,
                ItemCode = e.ItemCode,
                Description = e.Description,
                Qty = e.Qty,
                UnitPrice = e.UnitPrice,
                LocationText = e.LocationText
            }).ToList();
            ListofDocs.AddRange(ListofGRPOs);
            ReportTotalQty = ReportTotalQty + ListofGRPOs.Sum(x => x.Qty);
            ViewBag.ReportTotalQty = ReportTotalQty;
            ViewBag.ItemList = i_OITM_Repository.Items.Select(e => new SelectListItem
            {
                Text = e.ItemName,
                Value = e.ItemCode
            }).ToList();
            return View("Index", ListofDocs);
        }
    }
}