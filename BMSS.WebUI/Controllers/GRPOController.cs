using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.GRPOViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Purchase Delivery Note")]
    public class GRPOController : Controller
    {
        private I_GRPODocH_Repository i_GRPODocH_Repository;
        private I_GRPODocLs_Repository i_GRPODocLs_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OACT_Repository i_OACT_Repository;
        private I_ITT1_Repository i_ITT1_Repository;
        private I_PDN1_Repository i_PDN1_Repository;
        IMapper _mapper;
        public GRPOController(I_PDN1_Repository i_PDN1_Repository, I_ITT1_Repository i_ITT1_Repository, I_OACT_Repository i_OACT_Repository, I_OCRD_Repository i_OCRD_Repository, I_GRPODocLs_Repository i_GRPODocLs_Repository, I_GRPODocH_Repository i_GRPODocH_Repository, I_OCTG_Repository i_OCTG_Repository, IMapper _mapper)
        {
            this.i_GRPODocH_Repository = i_GRPODocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OACT_Repository = i_OACT_Repository;
            this.i_GRPODocLs_Repository = i_GRPODocLs_Repository;
            this._mapper = _mapper;
            this.i_ITT1_Repository = i_ITT1_Repository;
            this.i_PDN1_Repository = i_PDN1_Repository;
        }
        public ActionResult Index()
        {
            return View(i_GRPODocH_Repository.GRPOHeaderList);
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
            GRPODocH GRPOHeader = i_GRPODocH_Repository.GetByDocEntry(DocEntry);
            return GRPOHeader.PrintedCount >= 1 && !User.IsInRole("Print After First Time") ;

        }
        public ActionResult Add()
        {
            GRPOViewModel addGRPOViewModel = new GRPOViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DeliveryDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DueDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",
                DiscByPercent = "true",
                DiscAmount = 0,
                DiscPercent = 0,
                SubmittedToSAP = false
            };
            return View(addGRPOViewModel);
        }
        public ActionResult Edit(string DocNum)
        {
            GRPOViewModel addGRPOViewModel = new GRPOViewModel();

            GRPODocH GRPOHObject = i_GRPODocH_Repository.GetByDocNumber(DocNum);

            if (GRPOHObject != null)
            {
                addGRPOViewModel = _mapper.Map<GRPODocH, GRPOViewModel>(GRPOHObject);
                addGRPOViewModel.CardName = GRPOHObject.CardCode;
                addGRPOViewModel.Lines = new List<GRPOLineViewModel>();
                addGRPOViewModel.NoteLines = new List<GRPONoteViewModel>();

                foreach (var entry in GRPOHObject.GRPODocLs.OrderBy(x => x.LineNum))
                {
                    GRPOLineViewModel dOLine = new GRPOLineViewModel();
                    dOLine = _mapper.Map<GRPODocLs, GRPOLineViewModel>(entry);
                    addGRPOViewModel.Lines.Add(dOLine);
                }
                if (GRPOHObject.GRPODocNotes != null)
                {
                    foreach (var entry in GRPOHObject.GRPODocNotes.OrderBy(x => x.LineNum))
                    {
                        GRPONoteViewModel dONoteLine = new GRPONoteViewModel();
                        dONoteLine = _mapper.Map<GRPODocNotes, GRPONoteViewModel>(entry);
                        addGRPOViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                addGRPOViewModel.DocDate = GRPOHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                if(GRPOHObject.DeliveryDate.HasValue)
                addGRPOViewModel.DeliveryDate = GRPOHObject.DeliveryDate.Value.ToString("dd'/'MM'/'yyyy");
                addGRPOViewModel.DueDate = GRPOHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addGRPOViewModel.DiscByPercent = addGRPOViewModel.DiscByPercent.ToLower();
                addGRPOViewModel.CurrentUserIsNotInRoleNotes = !(User.IsInRole("Notes") && GRPOHObject.SyncedToSAP);

            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add", addGRPOViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Delivery Note-Edit")]
        public JsonResult Add(GRPOViewModel GRPOObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (GRPOObj.Lines == null || GRPOObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }

            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (GRPOObj.DocNum.Equals("New"))
                {
                    GRPODocH GRPOHeader = new GRPODocH();
                    GRPOHeader = _mapper.Map<GRPOViewModel, GRPODocH>(GRPOObj);
                    var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(GRPOHeader.CardCode);


                    GRPOHeader.CardName = ResultCustomerObject.CardName;
                    GRPOHeader.SlpCode = ResultCustomerObject.SlpCode;
                    GRPOHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                    GRPOHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    GRPOHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    GRPOHeader.Status = (short)DocumentStatuses.Open;
                    if (GRPOHeader.SubmittedToSAP.Equals(true))
                    {
                        GRPOHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    else
                    {

                    }
                    List<GRPODocLs> Lines = _mapper.Map<List<GRPOLineViewModel>, List<GRPODocLs>>(GRPOObj.Lines);
                    List<GRPODocNotes> NoteLines = _mapper.Map<List<GRPONoteViewModel>, List<GRPODocNotes>>(GRPOObj.NoteLines);
                    GRPOHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string GRPODocNum = "";
                    if (!i_GRPODocH_Repository.AddGRPO(GRPOHeader, Lines, NoteLines,ref ValidationMessage, ref GRPODocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        GRPOObj.DocNum = GRPODocNum;
                        GRPOObj.DocEntry = long.Parse(ValidationMessage);
                    }

                }
                else
                {
                    GRPODocH GRPOHeader = i_GRPODocH_Repository.GetByDocNumber(GRPOObj.DocNum);

                    if (GRPOHeader != null)
                    {

                        if (GRPOObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(GRPOHeader.DocEntry.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if(GRPOHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if (GRPOHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if (GRPOHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            GRPOHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            if (!i_GRPODocH_Repository.ResubmitGRPOToSAP(GRPOHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = GRPOHeader.DocEntry;
                            GRPOHeader = _mapper.Map<GRPOViewModel, GRPODocH>(GRPOObj);
                            var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(GRPOHeader.CardCode);
                            GRPOHeader.CardName = ResultCustomerObject.CardName;
                            GRPOHeader.SlpCode = ResultCustomerObject.SlpCode;
                            GRPOHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                            GRPOHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                            GRPOHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                            GRPOHeader.Status = (short)DocumentStatuses.Open;
                            GRPOHeader.DocEntry = DocEntry;
                            if (GRPOHeader.SubmittedToSAP.Equals(true))
                            {
                                GRPOHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            else
                            {

                            }
                            List<GRPODocLs> Lines = _mapper.Map<List<GRPOLineViewModel>, List<GRPODocLs>>(GRPOObj.Lines);
                            List<GRPODocNotes> NoteLines = _mapper.Map<List<GRPONoteViewModel>, List<GRPODocNotes>>(GRPOObj.NoteLines);
                            GRPOHeader.UpdatedBy = User.Identity.Name;
                            string ValidationMessage = string.Empty;
                            string GRPODocNum = "";
                            if (!i_GRPODocH_Repository.AddGRPO(GRPOHeader, Lines, NoteLines,ref ValidationMessage, ref GRPODocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                GRPOObj.DocNum = GRPODocNum;
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

                GRPOObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                GRPOObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        GRPOObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    GRPOObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        GRPOObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(GRPOObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Delivery Note-Edit")]
        public JsonResult UpdateNotes(GRPOViewModel GRPOObj)
        {
            GRPOObj.ModelErrList = new List<string>();
            if (GRPOObj.NoteLines != null)
            {
                GRPODocH DeliveryOrderHeader = i_GRPODocH_Repository.GetByDocNumber(GRPOObj.DocNum);

                if (DeliveryOrderHeader != null)
                {
                    List<GRPODocNotes> NoteLines = _mapper.Map<List<GRPONoteViewModel>, List<GRPODocNotes>>(GRPOObj.NoteLines);
                    if (GRPOObj.NoteLines.Where(x => x.Note == null || x.Note.Trim() == string.Empty).Count() > 0)
                    {

                        GRPOObj.IsModelValid = false;
                        GRPOObj.ModelErrList.Add("Note Requied");

                    }
                    else
                    {
                        string validationMessage = string.Empty;
                        if (!i_GRPODocH_Repository.UpdateNotes(DeliveryOrderHeader, NoteLines, ref validationMessage))
                        {
                            GRPOObj.IsModelValid = false;
                            GRPOObj.ModelErrList.Add(validationMessage);
                        }

                    }
                }
                else
                {
                    GRPOObj.IsModelValid = false;
                    GRPOObj.ModelErrList.Add("Document not found");
                }
                return Json(GRPOObj, JsonRequestBehavior.DenyGet);
            }
            else
            {

                GRPOObj.IsModelValid = false;
                GRPOObj.ModelErrList.Add("Notes Requied");
                return Json(GRPOObj, JsonRequestBehavior.DenyGet);
            }

        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetGRPOLastPriceHistory(string ItemCode)
        {
            IEnumerable<GRPODocLs> LastPriceHistory = i_GRPODocLs_Repository.GetGRPOLinesByItemCode(ItemCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.GRPODocH.DocDate).Select(e => new
            {
                DocDate = e.GRPODocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.GRPODocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }
        //[OverrideAuthorization]
        //[Authorize]
        //[HttpPost]
        //[AjaxOnly]
        //public JsonResult GetLastGRPOPrice(string ItemCode, string CardCode)
        //{
        //    decimal result = 0;

        //    IEnumerable<ITT1> ChildItemList = i_ITT1_Repository.GetChildItemList(ItemCode);
        //    if (ChildItemList != null && ChildItemList.Count() > 0)
        //    {
        //        result = ChildItemList.Select(x => i_GRPODocLs_Repository.GetLastPriceByItemCode(x.Code) * x.Quantity).Sum();
        //    }
        //    else
        //    {
        //        result = i_GRPODocLs_Repository.GetLastPriceByItemCode(ItemCode);
        //    }

        //    return Json(result, JsonRequestBehavior.DenyGet);
        //}
        [OverrideAuthorization]
        [Authorize]
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetLastGRPOPrice(string ItemCode, string CardCode)
        {
            decimal result = 0;

            IEnumerable<ITT1> ChildItemList = i_ITT1_Repository.GetChildItemList(ItemCode);
            if (ChildItemList != null && ChildItemList.Count() > 0)
            {
                result = ChildItemList.Select(x => i_PDN1_Repository.GetLastUnitPriceFromGRPO(x.Code, CardCode) * x.Quantity).Sum();
            }
            else
            {
                result = i_PDN1_Repository.GetLastUnitPriceFromGRPO(ItemCode, CardCode);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}