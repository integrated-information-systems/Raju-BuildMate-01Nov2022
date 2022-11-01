using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.StockTransViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Stock Transfer")]
  
    public class StockTransController : Controller
    {
        private I_StockTransDocH_Repository i_StockTransDocH_Repository;
        private I_StockTransDocLs_Repository i_StockTransDocLs_Repository;
        private I_DODocH_Repository i_DODocH_Repository;
        private I_OITW_Repository i_OITW_Repository;
        IMapper _mapper;
        public StockTransController(I_OITW_Repository i_OITW_Repository, I_DODocH_Repository i_DODocH_Repository, I_StockTransDocLs_Repository i_StockTransDocLs_Repository, I_StockTransDocH_Repository i_StockTransDocH_Repository, IMapper _mapper)
        {
            this.i_StockTransDocH_Repository = i_StockTransDocH_Repository;
            this.i_StockTransDocLs_Repository = i_StockTransDocLs_Repository;
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_OITW_Repository = i_OITW_Repository;
            this._mapper = _mapper;
        }
        public ActionResult Index()
        {
            return View(i_StockTransDocH_Repository.StockTransHeaderList);
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
            StockTransDocH StockTransDocHeader = i_StockTransDocH_Repository.GetByDocEntry(DocEntry);
            return StockTransDocHeader.PrintedCount >= 1 && !User.IsInRole("Print After First Time");

        }
        public ActionResult Add()
        {
            StockTransViewModel addStockTransViewModel = new StockTransViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",
                SubmittedToSAP = false,

            };
            return View(addStockTransViewModel);
        }
        public ActionResult Edit(string DocNum)
        {
            StockTransViewModel addStockTransViewModel = new StockTransViewModel();

            StockTransDocH StockTransHObject = i_StockTransDocH_Repository.GetByDocNumber(DocNum);

            if (StockTransHObject != null)
            {
                addStockTransViewModel = _mapper.Map<StockTransDocH, StockTransViewModel>(StockTransHObject);

                addStockTransViewModel.Lines = new List<StockTransLineViewModel>();
                addStockTransViewModel.NoteLines = new List<StockTransNoteViewModel>();

                foreach (var entry in StockTransHObject.StockTransDocLs.OrderBy(x => x.LineNum))
                {
                    StockTransLineViewModel dOLine = new StockTransLineViewModel();
                    dOLine = _mapper.Map<StockTransDocLs, StockTransLineViewModel>(entry);
                    addStockTransViewModel.Lines.Add(dOLine);
                }
                if (StockTransHObject.StockTransDocNotes != null)
                {
                    foreach (var entry in StockTransHObject.StockTransDocNotes.OrderBy(x => x.LineNum))
                    {
                        StockTransNoteViewModel dONoteLine = new StockTransNoteViewModel();
                        dONoteLine = _mapper.Map<StockTransDocNotes, StockTransNoteViewModel>(entry);
                        addStockTransViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                addStockTransViewModel.DocDate = StockTransHObject.DocDate.ToString("dd'/'MM'/'yyyy");


            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add", addStockTransViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Stock Transfer-Edit")]
        public JsonResult Add(StockTransViewModel StockTransObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (StockTransObj.Lines == null || StockTransObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }
            for (int i = 0; i < StockTransObj.Lines.Count(); i++)
            {
                decimal LocalAvailableQty = 0;
                LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(StockTransObj.Lines[i].ItemCode, StockTransObj.Lines[i].FromLocation);
                IEnumerable<OITW> ItemStockDetails = i_OITW_Repository.GetLocationStockDetails(StockTransObj.Lines[i].ItemCode);
                string IsInventoryItem = ItemStockDetails.Select(x => x.Item.InvntItem).FirstOrDefault();
                if (IsInventoryItem.Equals("Y"))
                {
                    decimal SAPAvailableQty = ItemStockDetails.Where(x => x.WhsCode.Equals(StockTransObj.Lines[i].FromLocation)).Sum(x => (decimal?)(x.OnHand - x.IsCommited)) ?? 0;

                    decimal OrderItemQty = StockTransObj.Lines.Where(x => x.ItemCode.Equals(StockTransObj.Lines[i].ItemCode) && x.FromLocation.Equals(StockTransObj.Lines[i].FromLocation)).Sum(x => x.Qty);
                    decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty);
                    if (TotalAvailableQty < OrderItemQty)
                    {
                        ErrList.Add("Item " + StockTransObj.Lines[i].ItemCode + " not have enought stock in " + StockTransObj.Lines[i].FromLocationText + " warehouse in line no " + (i + 1));
                    }
                }
            }
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (StockTransObj.DocNum.Equals("New"))
                {
                    StockTransDocH StockTransHeader = new StockTransDocH();
                    StockTransHeader = _mapper.Map<StockTransViewModel, StockTransDocH>(StockTransObj);

                    List<StockTransDocLs> Lines = _mapper.Map<List<StockTransLineViewModel>, List<StockTransDocLs>>(StockTransObj.Lines);
                    List<StockTransDocNotes> NoteLines = _mapper.Map<List<StockTransNoteViewModel>, List<StockTransDocNotes>>(StockTransObj.NoteLines);
                    StockTransHeader.CreatedBy = User.Identity.Name;
                    StockTransHeader.Status = (short)DocumentStatuses.Open;
                    if (StockTransObj.SubmittedToSAP == true)
                    {
                        StockTransHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    string ValidationMessage = string.Empty;
                    string STDocNum = string.Empty;
                    if (!i_StockTransDocH_Repository.AddStockTrans(StockTransHeader, Lines, NoteLines,ref ValidationMessage, ref STDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        StockTransObj.DocNum = STDocNum;
                        StockTransObj.DocEntry = long.Parse(ValidationMessage);
                    }
                }
                else
                {
                    StockTransDocH StockTransHeader = i_StockTransDocH_Repository.GetByDocNumber(StockTransObj.DocNum);

                    if (StockTransHeader != null)
                    {

                        if (StockTransObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(StockTransHeader.DocEntry.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if(StockTransHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if (StockTransHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if (StockTransHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            StockTransHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            if (!i_StockTransDocH_Repository.ResubmitStockTransToSAP(StockTransHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = StockTransHeader.DocEntry;
                            StockTransHeader = _mapper.Map<StockTransViewModel, StockTransDocH>(StockTransObj);

                            StockTransHeader.DocEntry = DocEntry;

                            List<StockTransDocLs> Lines = _mapper.Map<List<StockTransLineViewModel>, List<StockTransDocLs>>(StockTransObj.Lines);
                            List<StockTransDocNotes> NoteLines = _mapper.Map<List<StockTransNoteViewModel>, List<StockTransDocNotes>>(StockTransObj.NoteLines);
                            StockTransHeader.UpdatedBy = User.Identity.Name;
                            StockTransHeader.Status = (short)DocumentStatuses.Open;
                            if (StockTransObj.SubmittedToSAP == true)
                            {
                                StockTransHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            string ValidationMessage = string.Empty;
                            string STDocNum = string.Empty;
                            if (!i_StockTransDocH_Repository.AddStockTrans(StockTransHeader, Lines, NoteLines, ref ValidationMessage,ref STDocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                StockTransObj.DocNum = STDocNum;
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

                StockTransObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                StockTransObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        StockTransObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    StockTransObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        StockTransObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(StockTransObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetStockTransLastPriceHistory(string ItemCode)
        {
            IEnumerable<StockTransDocLs> LastPriceHistory = i_StockTransDocLs_Repository.GetStockTransLinesByItemCodeWithLimit(ItemCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.StockTransDocH.DocDate).Select(e => new
            {
                DocDate = e.StockTransDocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.StockTransDocH.DocNum,
                e.Qty,               
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }
    }
}