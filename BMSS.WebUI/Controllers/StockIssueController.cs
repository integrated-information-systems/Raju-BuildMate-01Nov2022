using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.StockIssueViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Stock Issue")]    
    public class StockIssueController : Controller
    {
        private I_StockIssueDocH_Repository i_StockIssueDocH_Repository;
        private I_StockIssueDocLs_Repository i_StockIssueDocLs_Repository;
        private I_DODocH_Repository i_DODocH_Repository;
        private I_OITW_Repository i_OITW_Repository;
        IMapper _mapper;
        public StockIssueController(I_DODocH_Repository i_DODocH_Repository, I_OITW_Repository i_OITW_Repository, I_StockIssueDocLs_Repository i_StockIssueDocLs_Repository, I_StockIssueDocH_Repository i_StockIssueDocH_Repository, IMapper _mapper)
        {
            this.i_StockIssueDocH_Repository = i_StockIssueDocH_Repository;            
            this.i_StockIssueDocLs_Repository = i_StockIssueDocLs_Repository;
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_OITW_Repository = i_OITW_Repository;
            this._mapper = _mapper;
        }
        public ActionResult Index()
        {
            return View(i_StockIssueDocH_Repository.StockIssueHeaderList);
        }
        public ActionResult Add()
        {
            StockIssueViewModel addStockIssueViewModel = new StockIssueViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),                
                DocNum = "New",               
                SubmittedToSAP = false,
                
            };
            return View(addStockIssueViewModel);
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
            StockIssueDocH StockIssueHeader = i_StockIssueDocH_Repository.GetByDocEntry(DocEntry);
            return StockIssueHeader.PrintedCount >= 1 && !User.IsInRole("Print After First Time");

        }
        public ActionResult Edit(string DocNum)
        {
            StockIssueViewModel addStockIssueViewModel = new StockIssueViewModel();

            StockIssueDocH StockIssueHObject = i_StockIssueDocH_Repository.GetByDocNumber(DocNum);

            if (StockIssueHObject != null)
            {
                addStockIssueViewModel = _mapper.Map<StockIssueDocH, StockIssueViewModel>(StockIssueHObject);
                
                addStockIssueViewModel.Lines = new List<StockIssueLineViewModel>();
                addStockIssueViewModel.NoteLines = new List<StockIssueNoteViewModel>();

                foreach (var entry in StockIssueHObject.StockIssueDocLs.OrderBy(x => x.LineNum))
                {
                    StockIssueLineViewModel dOLine = new StockIssueLineViewModel();
                    dOLine = _mapper.Map<StockIssueDocLs, StockIssueLineViewModel>(entry);
                    addStockIssueViewModel.Lines.Add(dOLine);
                }
                if (StockIssueHObject.StockIssueDocNotes != null)
                {
                    foreach (var entry in StockIssueHObject.StockIssueDocNotes.OrderBy(x => x.LineNum))
                    {
                        StockIssueNoteViewModel dONoteLine = new StockIssueNoteViewModel();
                        dONoteLine = _mapper.Map<StockIssueDocNotes, StockIssueNoteViewModel>(entry);
                        addStockIssueViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                addStockIssueViewModel.DocDate = StockIssueHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                

            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add", addStockIssueViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Stock Issue-Edit")]
        public JsonResult Add(StockIssueViewModel StockIssueObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (StockIssueObj.Lines == null || StockIssueObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }
            for (int i = 0; i < StockIssueObj.Lines.Count(); i++)
            {
                decimal LocalAvailableQty = 0;
                LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(StockIssueObj.Lines[i].ItemCode, StockIssueObj.Lines[i].Location);
                IEnumerable<OITW> ItemStockDetails = i_OITW_Repository.GetLocationStockDetails(StockIssueObj.Lines[i].ItemCode);
                string IsInventoryItem = ItemStockDetails.Select(x => x.Item.InvntItem).FirstOrDefault();
                if (IsInventoryItem.Equals("Y"))
                {
                    decimal SAPAvailableQty = ItemStockDetails.Where(x => x.WhsCode.Equals(StockIssueObj.Lines[i].Location)).Sum(x => (decimal?)(x.OnHand - x.IsCommited)) ?? 0;

                    decimal OrderItemQty = StockIssueObj.Lines.Where(x => x.ItemCode.Equals(StockIssueObj.Lines[i].ItemCode) && x.Location.Equals(StockIssueObj.Lines[i].Location)).Sum(x => x.Qty);
                    decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty);
                    if (TotalAvailableQty < OrderItemQty)
                    {
                        ErrList.Add("Item " + StockIssueObj.Lines[i].ItemCode + " not have enought stock in " + StockIssueObj.Lines[i].LocationText + " warehouse in line no " + (i + 1));
                    }
                }
            }
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (StockIssueObj.DocNum.Equals("New"))
                {
                    StockIssueDocH StockIssueHeader = new StockIssueDocH();
                    StockIssueHeader = _mapper.Map<StockIssueViewModel, StockIssueDocH>(StockIssueObj);
 
                    List<StockIssueDocLs> Lines = _mapper.Map<List<StockIssueLineViewModel>, List<StockIssueDocLs>>(StockIssueObj.Lines);
                    List<StockIssueDocNotes> NoteLines = _mapper.Map<List<StockIssueNoteViewModel>, List<StockIssueDocNotes>>(StockIssueObj.NoteLines);
                    StockIssueHeader.CreatedBy = User.Identity.Name;
                    if (StockIssueObj.SubmittedToSAP == true)
                    {
                        StockIssueHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    StockIssueHeader.Status = (short)DocumentStatuses.Open;
                    string ValidationMessage = string.Empty;
                    string SIDocNum = string.Empty;
                    if (!i_StockIssueDocH_Repository.AddStockIssue(StockIssueHeader, Lines, NoteLines, ref ValidationMessage,ref SIDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        StockIssueObj.DocNum = SIDocNum;
                        StockIssueObj.DocEntry = long.Parse(ValidationMessage);
                    }

                }
                else
                {
                    StockIssueDocH StockIssueHeader = i_StockIssueDocH_Repository.GetByDocNumber(StockIssueObj.DocNum);

                    if (StockIssueHeader != null)
                    {

                        if (StockIssueObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(StockIssueHeader.DocEntry.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if(StockIssueHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if (StockIssueHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if (StockIssueHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            StockIssueHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            if (!i_StockIssueDocH_Repository.ResubmitStockIssueToSAP(StockIssueHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = StockIssueHeader.DocEntry;
                            StockIssueHeader = _mapper.Map<StockIssueViewModel, StockIssueDocH>(StockIssueObj);
                           
                            StockIssueHeader.DocEntry = DocEntry;

                            List<StockIssueDocLs> Lines = _mapper.Map<List<StockIssueLineViewModel>, List<StockIssueDocLs>>(StockIssueObj.Lines);
                            List<StockIssueDocNotes> NoteLines = _mapper.Map<List<StockIssueNoteViewModel>, List<StockIssueDocNotes>>(StockIssueObj.NoteLines);
                            StockIssueHeader.UpdatedBy = User.Identity.Name;
                            StockIssueHeader.Status = (short)DocumentStatuses.Open;
                            if (StockIssueObj.SubmittedToSAP == true)
                            {
                                StockIssueHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            string ValidationMessage = string.Empty;
                            string SIDocNum = string.Empty;
                            if (!i_StockIssueDocH_Repository.AddStockIssue(StockIssueHeader, Lines, NoteLines,ref ValidationMessage, ref SIDocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                StockIssueObj.DocNum = SIDocNum;
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

                StockIssueObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                StockIssueObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        StockIssueObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    StockIssueObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        StockIssueObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(StockIssueObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetStockIssueLastPriceHistory(string ItemCode)
        {
            IEnumerable<StockIssueDocLs> LastPriceHistory = i_StockIssueDocLs_Repository.GetStockIssueLinesByItemCodeWithLimit(ItemCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.StockIssueDocH.DocDate).Select(e => new
            {
                DocDate = e.StockIssueDocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.StockIssueDocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }
    }
}