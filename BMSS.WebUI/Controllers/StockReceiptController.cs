using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.StockReceiptViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Stock Receipt")]

    public class StockReceiptController : Controller
    {
        private I_StockReceiptDocH_Repository i_StockReceiptDocH_Repository;
        private I_StockReceiptDocLs_Repository i_StockReceiptDocLs_Repository;

        IMapper _mapper;
        public StockReceiptController(I_StockReceiptDocLs_Repository i_StockReceiptDocLs_Repository, I_StockReceiptDocH_Repository i_StockReceiptDocH_Repository, IMapper _mapper)
        {
            this.i_StockReceiptDocH_Repository = i_StockReceiptDocH_Repository;
            this.i_StockReceiptDocLs_Repository = i_StockReceiptDocLs_Repository;
            this._mapper = _mapper;
        }
        public ActionResult Index()
        {
            return View(i_StockReceiptDocH_Repository.StockReceiptHeaderList);
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
            StockReceiptDocH StockReceiptHeader = i_StockReceiptDocH_Repository.GetByDocEntry(DocEntry);
            return StockReceiptHeader.PrintedCount >= 1 && !User.IsInRole("Print After First Time");

        }
        public ActionResult Add()
        {
            StockReceiptViewModel addStockReceiptViewModel = new StockReceiptViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",
                SubmittedToSAP = false,

            };
            return View(addStockReceiptViewModel);
        }
        public ActionResult Edit(string DocNum)
        {
            StockReceiptViewModel addStockReceiptViewModel = new StockReceiptViewModel();

            StockReceiptDocH StockReceiptHObject = i_StockReceiptDocH_Repository.GetByDocNumber(DocNum);

            if (StockReceiptHObject != null)
            {
                addStockReceiptViewModel = _mapper.Map<StockReceiptDocH, StockReceiptViewModel>(StockReceiptHObject);

                addStockReceiptViewModel.Lines = new List<StockReceiptLineViewModel>();
                addStockReceiptViewModel.NoteLines = new List<StockReceiptNoteViewModel>();

                foreach (var entry in StockReceiptHObject.StockReceiptDocLs.OrderBy(x => x.LineNum))
                {
                    StockReceiptLineViewModel dOLine = new StockReceiptLineViewModel();
                    dOLine = _mapper.Map<StockReceiptDocLs, StockReceiptLineViewModel>(entry);
                    addStockReceiptViewModel.Lines.Add(dOLine);
                }
                if (StockReceiptHObject.StockReceiptDocNotes != null)
                {
                    foreach (var entry in StockReceiptHObject.StockReceiptDocNotes.OrderBy(x => x.LineNum))
                    {
                        StockReceiptNoteViewModel dONoteLine = new StockReceiptNoteViewModel();
                        dONoteLine = _mapper.Map<StockReceiptDocNotes, StockReceiptNoteViewModel>(entry);
                        addStockReceiptViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                addStockReceiptViewModel.DocDate = StockReceiptHObject.DocDate.ToString("dd'/'MM'/'yyyy");


            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add", addStockReceiptViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Stock Receipt-Edit")]
        public JsonResult Add(StockReceiptViewModel StockReceiptObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (StockReceiptObj.Lines == null || StockReceiptObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }

            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (StockReceiptObj.DocNum.Equals("New"))
                {
                    StockReceiptDocH StockReceiptHeader = new StockReceiptDocH();
                    StockReceiptHeader = _mapper.Map<StockReceiptViewModel, StockReceiptDocH>(StockReceiptObj);

                    List<StockReceiptDocLs> Lines = _mapper.Map<List<StockReceiptLineViewModel>, List<StockReceiptDocLs>>(StockReceiptObj.Lines);
                    List<StockReceiptDocNotes> NoteLines = _mapper.Map<List<StockReceiptNoteViewModel>, List<StockReceiptDocNotes>>(StockReceiptObj.NoteLines);
                    StockReceiptHeader.CreatedBy = User.Identity.Name;
                    StockReceiptHeader.Status = (short)DocumentStatuses.Open;
                    if (StockReceiptObj.SubmittedToSAP == true)
                    {
                        StockReceiptHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    string ValidationMessage = string.Empty;
                    string SRDocNum = string.Empty;
                    if (!i_StockReceiptDocH_Repository.AddStockReceipt(StockReceiptHeader, Lines, NoteLines, ref ValidationMessage, ref SRDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        StockReceiptObj.DocNum = SRDocNum;
                        StockReceiptObj.DocEntry = long.Parse(ValidationMessage);
                    }

                }
                else
                {
                    StockReceiptDocH StockReceiptHeader = i_StockReceiptDocH_Repository.GetByDocNumber(StockReceiptObj.DocNum);

                    if (StockReceiptHeader != null)
                    {

                        if (StockReceiptObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(StockReceiptHeader.DocEntry.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if(StockReceiptHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if (StockReceiptHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if (StockReceiptHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            StockReceiptHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            if (!i_StockReceiptDocH_Repository.ResubmitStockReceiptToSAP(StockReceiptHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = StockReceiptHeader.DocEntry;
                            StockReceiptHeader = _mapper.Map<StockReceiptViewModel, StockReceiptDocH>(StockReceiptObj);

                            StockReceiptHeader.DocEntry = DocEntry;

                            List<StockReceiptDocLs> Lines = _mapper.Map<List<StockReceiptLineViewModel>, List<StockReceiptDocLs>>(StockReceiptObj.Lines);
                            List<StockReceiptDocNotes> NoteLines = _mapper.Map<List<StockReceiptNoteViewModel>, List<StockReceiptDocNotes>>(StockReceiptObj.NoteLines);
                            StockReceiptHeader.UpdatedBy = User.Identity.Name;
                            StockReceiptHeader.Status = (short)DocumentStatuses.Open;
                            if (StockReceiptObj.SubmittedToSAP == true)
                            {
                                StockReceiptHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            string ValidationMessage = string.Empty;
                            string SRDocNum = string.Empty;
                            if (!i_StockReceiptDocH_Repository.AddStockReceipt(StockReceiptHeader, Lines, NoteLines, ref ValidationMessage, ref SRDocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                StockReceiptObj.DocNum = SRDocNum;
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

                StockReceiptObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                StockReceiptObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        StockReceiptObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    StockReceiptObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        StockReceiptObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(StockReceiptObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetStockReceiptLastPriceHistory(string ItemCode)
        {
            IEnumerable<StockReceiptDocLs> LastPriceHistory = i_StockReceiptDocLs_Repository.GetStockReceiptLinesByItemCodeWithLimit(ItemCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.StockReceiptDocH.DocDate).Select(e => new
            {
                DocDate = e.StockReceiptDocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.StockReceiptDocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }
    }
}