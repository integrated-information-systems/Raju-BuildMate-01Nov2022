using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.POViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Purchase Order")]
    public class POController : Controller
    {
        private I_PODocH_Repository i_PODocH_Repository;
        private I_PODocLs_Repository i_PODocLs_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OACT_Repository i_OACT_Repository;
        private I_GRPODocH_Repository i_GRPODocH_Repository;
        IMapper _mapper;
        public POController(I_GRPODocH_Repository i_GRPODocH_Repository, I_OACT_Repository i_OACT_Repository, I_OCRD_Repository i_OCRD_Repository, I_PODocLs_Repository i_PODocLs_Repository, I_PODocH_Repository i_PODocH_Repository, I_OCTG_Repository i_OCTG_Repository, IMapper _mapper)
        {
            this.i_PODocH_Repository = i_PODocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OACT_Repository = i_OACT_Repository;
            this.i_PODocLs_Repository = i_PODocLs_Repository;
            this.i_GRPODocH_Repository = i_GRPODocH_Repository;

            this._mapper = _mapper;
        }
        public ActionResult Index()
        {
            POListFilterViewModel Model = new POListFilterViewModel();
            Model.POList = i_PODocH_Repository.POHeaderList;
            PopulateIndexViewModel(ref Model);
            return View(Model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(POListFilterViewModel Model)
        {

            if (ModelState.IsValid)
            {
                switch (Model.Status)
                {
                    case POFilter.All:
                        Model.POList = i_PODocH_Repository.POHeaderList;
                        break;
                    case POFilter.Open:
                        Model.POList = i_PODocH_Repository.GetPOHeaderListWithOpenQty();
                        break;
                    case POFilter.Closed:
                        Model.POList = i_PODocH_Repository.GetPOHeaderListNoOpenQty();
                        break;
                }
            }
            PopulateIndexViewModel(ref Model);
            return View(Model);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult CanPrint(string DocEntry)
        {
            if (CannotPrintAndSave(DocEntry))
            {
                var DOObj = new { canAccess = false };
                return Json(DOObj, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var DOObj = new { canAccess = true };
                return Json(DOObj, JsonRequestBehavior.DenyGet);
            }

        }
        private Boolean CannotPrintAndSave(string DocEntry)
        {
            PODocH POHeader = i_PODocH_Repository.GetByDocEntry(DocEntry);
            return POHeader.PrintedCount >= 1 && !User.IsInRole("Print After First Time");

        }
        [NonAction]
        void PopulateIndexViewModel(ref POListFilterViewModel Model)
        {
            Model.Statuses = new List<SelectListItem>();

            Model.Statuses.AddRange(
                Enum.GetValues(typeof(POFilter))
                    .Cast<POFilter>()
                    .Select(x => new SelectListItem
                    {
                        Text = x.ToString(),
                        Value = ((int)x).ToString()
                    }).ToList());
        }
        public ActionResult Edit(string DocNum)
        {
            POViewModel addPOViewModel = new POViewModel();

            PODocH POHObject = i_PODocH_Repository.GetByDocNumber(DocNum);

            if (POHObject != null)
            {
                addPOViewModel = _mapper.Map<PODocH, POViewModel>(POHObject);
                addPOViewModel.CardName = POHObject.CardCode;
                addPOViewModel.Lines = new List<POLineViewModel>();
                addPOViewModel.NoteLines = new List<PONoteViewModel>();
                
                addPOViewModel.Lines = _mapper.Map<List<PODocLs>, List<POLineViewModel>>(POHObject.PODocLs.OrderBy(x=> x.LineNum).ToList());

                if (POHObject.PODocNotes != null)
                {                     
                    addPOViewModel.NoteLines = _mapper.Map<List<PODocNotes>, List<PONoteViewModel>>(POHObject.PODocNotes.OrderBy(x => x.LineNum).ToList());
                }
                addPOViewModel.DocDate = POHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                if(POHObject.DeliveryDate.HasValue)
                addPOViewModel.DeliveryDate = POHObject.DeliveryDate.Value.ToString("dd'/'MM'/'yyyy");
                //addPOViewModel.DueDate = POHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addPOViewModel.DiscByPercent = addPOViewModel.DiscByPercent.ToLower();
                addPOViewModel.CurrentUserIsNotInRoleNotes = !(User.IsInRole("Notes") && POHObject.Status == (short)DocumentStatuses.Closed);

            }
            else
            {
                TempData["GlobalErrMsg"] = "Purchase order not found";
                return RedirectToAction("Index");
            }

            return View("Add", addPOViewModel);
        }
        public ActionResult Add()
        {
            POViewModel addPOViewModel = new POViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DeliveryDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DueDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",
                DiscByPercent = "true",
                DiscAmount = 0,
                DiscPercent = 0
            };
            return View(addPOViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Order-Edit")]
        public JsonResult Add(POViewModel POObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (POObj.Lines == null || POObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }

            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (POObj.DocNum.Equals("New"))
                {
                    PODocH PurchaseOrderHeader = new PODocH();
                    PurchaseOrderHeader = _mapper.Map<POViewModel, PODocH>(POObj);

                    var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(PurchaseOrderHeader.CardCode);

                    PurchaseOrderHeader.CardName = ResultCustomerObject.CardName;
                    PurchaseOrderHeader.SlpCode = ResultCustomerObject.SlpCode;
                    PurchaseOrderHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                    PurchaseOrderHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    PurchaseOrderHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    PurchaseOrderHeader.Status = (short)DocumentStatuses.Open;
                    

                    List<PODocLs> Lines = _mapper.Map<List<POLineViewModel>, List<PODocLs>>(POObj.Lines);
                    List<PODocNotes> NoteLines = _mapper.Map<List<PONoteViewModel>, List<PODocNotes>>(POObj.NoteLines);
                    PurchaseOrderHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string PODocNum = string.Empty;
                    if (!i_PODocH_Repository.AddPO(PurchaseOrderHeader, Lines, NoteLines, ref ValidationMessage, ref PODocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        POObj.DocNum = PODocNum;
                        POObj.DocEntry = long.Parse(ValidationMessage);
                    }
                }
                else
                {
                    PODocH PurchaseOrderHeader = i_PODocH_Repository.GetByDocNumber(POObj.DocNum);

                    if (PurchaseOrderHeader != null)
                    {
                        if (CannotPrintAndSave(PurchaseOrderHeader.DocEntry.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (PurchaseOrderHeader.PODocLs.Count() != POObj.Lines.Count() && PurchaseOrderHeader.CopiedGRPO!=null)
                        {
                            ErrList.Add("PO Copied to GRPO already, modification of lines not allowed,cannot proceed");
                        }
                        else {


                            long DocEntry = PurchaseOrderHeader.DocEntry;
                            PurchaseOrderHeader = _mapper.Map<POViewModel, PODocH>(POObj);
                            var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(PurchaseOrderHeader.CardCode);
                            PurchaseOrderHeader.CardName = ResultCustomerObject.CardName;
                            PurchaseOrderHeader.SlpCode = ResultCustomerObject.SlpCode;
                            PurchaseOrderHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                            PurchaseOrderHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                            PurchaseOrderHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;


                            PurchaseOrderHeader.DocEntry = DocEntry;
                            List<PODocLs> Lines = _mapper.Map<List<POLineViewModel>, List<PODocLs>>(POObj.Lines);
                            List<PODocNotes> NoteLines = _mapper.Map<List<PONoteViewModel>, List<PODocNotes>>(POObj.NoteLines);
                            PurchaseOrderHeader.UpdatedBy = User.Identity.Name;
                            string PODocNum = string.Empty;
                            string ValidationMessage = string.Empty;
                            if (!i_PODocH_Repository.AddPO(PurchaseOrderHeader, Lines, NoteLines, ref ValidationMessage, ref PODocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                POObj.DocNum = PODocNum;
                            }
                        }
                        
                    }
                    else
                    {
                        ErrList.Add("Document number not found");
                    }
                   
                }
            }
            if (ModelState.IsValid == false || ErrList.Count() > 0)
            {
                POObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                POObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        POObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    POObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        POObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(POObj, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Order-Edit")]
        public JsonResult UpdateNotes(POViewModel POObj)
        {
            POObj.ModelErrList = new List<string>();
            if (POObj.NoteLines != null)
            {
                PODocH DeliveryOrderHeader = i_PODocH_Repository.GetByDocNumber(POObj.DocNum);

                if (DeliveryOrderHeader != null)
                {
                    List<PODocNotes> NoteLines = _mapper.Map<List<PONoteViewModel>, List<PODocNotes>>(POObj.NoteLines);
                    if (POObj.NoteLines.Where(x => x.Note == null || x.Note.Trim() == string.Empty).Count() > 0)
                    {

                        POObj.IsModelValid = false;
                        POObj.ModelErrList.Add("Note Requied");

                    }
                    else
                    {
                        string validationMessage = string.Empty;
                        if (!i_PODocH_Repository.UpdateNotes(DeliveryOrderHeader, NoteLines, ref validationMessage))
                        {
                            POObj.IsModelValid = false;
                            POObj.ModelErrList.Add(validationMessage);
                        }

                    }
                }
                else
                {
                    POObj.IsModelValid = false;
                    POObj.ModelErrList.Add("Document not found");
                }
                return Json(POObj, JsonRequestBehavior.DenyGet);
            }
            else
            {

                POObj.IsModelValid = false;
                POObj.ModelErrList.Add("Notes Requied");
                return Json(POObj, JsonRequestBehavior.DenyGet);
            }

        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetPOLastPriceHistory(string ItemCode, string CardCode)
        {
            IEnumerable<PODocLs> LastPriceHistory = i_PODocLs_Repository.GetPOLinesByItemCodeWithLimit(ItemCode, CardCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.PODocH.DocDate).Select(e => new
            {
                DocDate = e.PODocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.PODocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        } 

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Order-Edit")]
        public JsonResult CopyToGRPO(string DocNum, int rowNo)
        {
            
            PODocH POHeader = i_PODocH_Repository.GetByDocNumber(DocNum);

            if (POHeader != null)
            {


                GRPODocH GRPOHeader = new GRPODocH();
                GRPOHeader = _mapper.Map<PODocH, GRPODocH>(POHeader);
                
                GRPOHeader.DocNum = "New";
                GRPOHeader.CopiedPO = POHeader.DocNum;
                GRPOHeader.PORNo = POHeader.Ref; // Supplier Reference
                GRPOHeader.Status = (short)DocumentStatuses.Open;
                var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(POHeader.CardCode);
                GRPOHeader.DueDate = GRPOHeader.DocDate.AddDays(ResultCustomerObject.PaymentTerm.ExtraDays);
                List<PODocLs> POLines = POHeader.PODocLs.Where(x=> x.GRPOQty != 0 && x.OpenQty != 0).OrderBy(x => x.LineNum).ToList();
                decimal TotalOpenQty = POLines.Sum(x => Math.Abs(x.OpenQty));
                decimal TotalGRPOQty = POLines.Sum(x => Math.Abs(x.GRPOQty));
                if (TotalOpenQty == 0)
                {
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = "Document dont have any lines with enought open Qty"
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
                }
                else if (TotalGRPOQty == 0)
                {
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = "Document dont have any lines with GRPO Qty specified"
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
                }  
                else if(Math.Abs(TotalOpenQty) < Math.Abs(TotalGRPOQty))
                {
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = "There is a mismatch between Open Qty available and GRPO Qty specified"
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
                }
                else
                {

                        bool errorFlag = false;
                        string errorMsg = string.Empty;
                        List<GRPODocLs> GRPOLines = _mapper.Map<List<PODocLs>, List<GRPODocLs>>(POLines);
                        int i = 0;
                        foreach(PODocLs line in POLines)
                        {
                            if(Math.Abs(line.OpenQty)>=Math.Abs(line.GRPOQty)) { 
                                GRPOLines[i].LineNum = i;
                                GRPOLines[i].Qty = line.GRPOQty;
                                GRPOLines[i].LineTotal = GRPOLines[i].Qty * GRPOLines[i].UnitPrice;
                                i++;
                            }
                            else
                            {
                                errorFlag = true;
                                errorMsg = "There is no enough open qty on line " + (i + 1) + ", Cannot proceed";
                                break;
                            }
                        }
                        if (errorFlag.Equals(false))
                        {                        
                        GRPOHeader.NetTotal = GRPOLines.Sum(x => x.LineTotal);
                        GRPOHeader.GstTotal = 0;
                        GRPOHeader.GstTotal += GRPOLines.Sum((x) => {
                            decimal PerLineGst = 0;
                            decimal PerLineTotalAfterDiscount = 0;
                            decimal DisAmount = 0;
                            if (GRPOHeader.DiscPercent > 0)
                                DisAmount = x.LineTotal * (GRPOHeader.DiscPercent / 100);

                            PerLineTotalAfterDiscount = x.LineTotal - DisAmount;
                            if(x.Gst>0)
                                PerLineGst = PerLineTotalAfterDiscount * (x.Gst / 100);
                            return PerLineGst;
                            //return Math.Round(PerLineGst, 2, MidpointRounding.AwayFromZero);
                        } );
                         
                            GRPOHeader.GrandTotal = Math.Round(GRPOHeader.NetTotal, 2, MidpointRounding.AwayFromZero) + Math.Round(GRPOHeader.GstTotal, 2, MidpointRounding.AwayFromZero);
                            List<PODocNotes> PONoteLines = POHeader.PODocNotes.OrderBy(x => x.LineNum).ToList();
                            List<GRPODocNotes> GRPONoteLines = _mapper.Map<List<PODocNotes>, List<GRPODocNotes>>(PONoteLines);
                            string GRPODocNum = "";
                            string ValidationMessage = string.Empty;
                            GRPOHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            if (!i_GRPODocH_Repository.AddGRPO(GRPOHeader, GRPOLines, GRPONoteLines, ref ValidationMessage, ref GRPODocNum))
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
                                List<string> GRPONumList = i_PODocH_Repository.GetPDNList(POHeader.DocEntry);
                                var DOResultObj = new
                                {
                                    Status = 200,
                                    ResultHtml = "",
                                    GRPONumList = GRPONumList,
                                    rowNo
                                };
                                return Json(DOResultObj, JsonRequestBehavior.DenyGet);
                            }
                        }
                        else
                        {
                            var ErrorObj = new
                            {
                                Status = 400,
                                ResultHtml = errorMsg
                            };
                            return Json(ErrorObj, JsonRequestBehavior.DenyGet);

                        }
                    }
                    
                }                            
            else
            {
                var ErrorObj = new
                {
                    Status = 400,
                    ResultHtml = "Document not found"
                };
                return Json(ErrorObj, JsonRequestBehavior.DenyGet);
            }




        }

    }
}