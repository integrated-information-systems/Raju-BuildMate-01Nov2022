using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.CashSalesCreditViewModels;
using BMSS.WebUI.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Cash Sales Credit Note")]
    public class CashSalesCreditController : Controller
    {
        private I_CashSalesCreditDocH_Repository i_CashSalesCreditDocH_Repository;
        private I_CashSalesCreditDocLs_Repository i_CashSalesCreditDocLs_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OSLP_Repository i_OSLP_Repository;
        private I_OACT_Repository i_OACT_Repository;
        private I_CashSalesCustomer_Repository i_CashSalesCustomer_Repository;
        private I_OITM_Repository i_OITM_Repository;
        IMapper _mapper;
        public CashSalesCreditController(I_OITM_Repository i_OITM_Repository, I_CashSalesCustomer_Repository i_CashSalesCustomer_Repository, I_OSLP_Repository i_OSLP_Repository, I_OACT_Repository i_OACT_Repository, I_OCRD_Repository i_OCRD_Repository, I_CashSalesCreditDocLs_Repository i_CashSalesCreditDocLs_Repository, I_CashSalesCreditDocH_Repository i_CashSalesCreditDocH_Repository, I_OCTG_Repository i_OCTG_Repository, IMapper _mapper)
        {
            this.i_CashSalesCreditDocH_Repository = i_CashSalesCreditDocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OACT_Repository = i_OACT_Repository;
            this.i_OSLP_Repository = i_OSLP_Repository;
            this.i_CashSalesCreditDocLs_Repository = i_CashSalesCreditDocLs_Repository;
            this.i_CashSalesCustomer_Repository = i_CashSalesCustomer_Repository;
            this.i_OITM_Repository = i_OITM_Repository;
            this._mapper = _mapper;
        }
        public ActionResult Index()
        {
            return View(i_CashSalesCreditDocH_Repository.CashSalesCreditHeaderList);
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
            CashSalesCreditDocH CashSalesCreditHeader = i_CashSalesCreditDocH_Repository.GetByDocEntry(DocEntry);
            return CashSalesCreditHeader.PrintedCount >= 1 && !User.IsInRole("Print After First Time");

        }
        public ActionResult Edit(string DocNum)
        {

            var ResultObject = i_OACT_Repository.GLCodes.FirstOrDefault(x => x.AcctName.Equals("Cash at Bank - OCBC"));
            string defaultGLCode = string.Empty;
            if (ResultObject != null)
                defaultGLCode = ResultObject.AcctCode;

            ViewBag.defaultGLCode = defaultGLCode;

            CashSalesCreditViewModel addCashSalesCreditViewModel = new CashSalesCreditViewModel();

            CashSalesCreditDocH CashSalesHObject = i_CashSalesCreditDocH_Repository.GetByDocNumber(DocNum);

            if (CashSalesHObject != null)
            {
                addCashSalesCreditViewModel = _mapper.Map<CashSalesCreditDocH, CashSalesCreditViewModel>(CashSalesHObject);
                addCashSalesCreditViewModel.CardName = addCashSalesCreditViewModel.CardCode;
                addCashSalesCreditViewModel.Lines = new List<CashSalesCreditLineViewModel>();
                addCashSalesCreditViewModel.NoteLines = new List<CashSalesCreditNoteViewModel>();
                addCashSalesCreditViewModel.PayLines = new List<CashSalesCreditPayViewModel>();

                foreach (var entry in CashSalesHObject.CashSalesCreditDocLs.OrderBy(x => x.LineNum))
                {
                    CashSalesCreditLineViewModel dOLine = new CashSalesCreditLineViewModel();
                    dOLine = _mapper.Map<CashSalesCreditDocLs, CashSalesCreditLineViewModel>(entry);
                    addCashSalesCreditViewModel.Lines.Add(dOLine);
                }
                if (CashSalesHObject.CashSalesCreditDocNotes != null)
                {
                    foreach (var entry in CashSalesHObject.CashSalesCreditDocNotes.OrderBy(x => x.LineNum))
                    {
                        CashSalesCreditNoteViewModel dONoteLine = new CashSalesCreditNoteViewModel();
                        dONoteLine = _mapper.Map<CashSalesCreditDocNotes, CashSalesCreditNoteViewModel>(entry);
                        addCashSalesCreditViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                if (CashSalesHObject.CashSalesCreditDocPays != null)
                {
                    foreach (var entry in CashSalesHObject.CashSalesCreditDocPays.OrderBy(x => x.LineNum))
                    {
                        CashSalesCreditPayViewModel dOPayLine = new CashSalesCreditPayViewModel();
                        dOPayLine = _mapper.Map<CashSalesCreditDocPays, CashSalesCreditPayViewModel>(entry);
                        addCashSalesCreditViewModel.PayLines.Add(dOPayLine);
                    }
                }
                addCashSalesCreditViewModel.DocDate = CashSalesHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                //addCashSalesCreditViewModel.DeliveryDate = CashSalesHObject.DeliveryDate.ToString("dd'/'MM'/'yyyy");
                //addCashSalesCreditViewModel.DueDate = CashSalesHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addCashSalesCreditViewModel.DiscByPercent = addCashSalesCreditViewModel.DiscByPercent.ToLower();

            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add", addCashSalesCreditViewModel);
        }

        public ActionResult Add()
        {
            //var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails("CASH-SALE");

            var ResultObject = i_OACT_Repository.GLCodes.FirstOrDefault(x => x.AcctName.Equals("Cash at Bank - OCBC"));
            string defaultGLCode = string.Empty;
            if (ResultObject != null)
                defaultGLCode = ResultObject.AcctCode;

            ViewBag.defaultGLCode = defaultGLCode;

            CashSalesCreditViewModel addCashSalesCreditViewModel = new CashSalesCreditViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DeliveryDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DueDate = DateTime.Now.AddDays(ResultCustomerObject.PaymentTerm.ExtraDays).ToString("dd/MM/yyyy"),
                DocNum = "New",
                DiscByPercent = "true",
                DiscAmount = 0,
                DiscPercent = 0,
                SubmittedToSAP = false,
                //Currency = ResultCustomerObject.Currency,
                //CardCode = ResultCustomerObject.CardCode,
                //CardName = ResultCustomerObject.CardName,
                //PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup,
                //SlpName = ResultCustomerObject.SalesPerson.SlpName,
            };
            return View(addCashSalesCreditViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Cash Sales Credit Note-Edit")]
        public JsonResult Add(CashSalesCreditViewModel CashSalesObj)
        {
            CashSalesObj.CashSalesCardName = "-";
            CashSalesObj.BillTo = "-";
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (CashSalesObj.Lines == null || CashSalesObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }
            else if (CashSalesObj.PayLines == null || CashSalesObj.PayLines.Count() <= 0)
            {
                ErrList.Add("No Pay lines added");
            }
            else if (CashSalesObj.GrandTotal > (CashSalesObj.PayLines.Sum(x => x.PaidAmount)))
            {
                ErrList.Add("Paid Amount lesser than Grand Total");
            }
            else if (CashSalesObj.GrandTotal < (CashSalesObj.PayLines.Sum(x => x.PaidAmount)))
            {
                ErrList.Add("Paid Amount greater than Grand Total");
            }
            for (int i = 0; i < CashSalesObj.Lines.Count(); i++)
            {
                OITM ItemDetails = i_OITM_Repository.GetItemDetails(CashSalesObj.Lines[i].ItemCode);
                string isUnitCostRequired = ItemDetails.U_ALLOW_UNIT_COST_ZERO;
                if (isUnitCostRequired != null && isUnitCostRequired == "N")
                {
                    if (CashSalesObj.Lines[i].UnitCost <= 0)
                        ErrList.Add("For item " + CashSalesObj.Lines[i].ItemCode + ", Unit Cost should be greater than zero in line no " + (i + 1));
                }

            }
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (CashSalesObj.DocNum.Equals("New"))
                {
                    CashSalesCreditDocH CashSalesCreditHeader = new CashSalesCreditDocH();
                    CashSalesCreditHeader = _mapper.Map<CashSalesCreditViewModel, CashSalesCreditDocH>(CashSalesObj);
                    var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CashSalesCreditHeader.CardCode);
                    //var CashCustomerObject = i_CashSalesCustomer_Repository.GetByTelephoneNo(CashSalesCreditHeader.OfficeTelNo);
                    

                    CashSalesCreditHeader.CardName = ResultCustomerObject.CardName;
                    //CashSalesCreditHeader.CashSalesCardName = CashCustomerObject.CustomerName;

                    CashSalesCreditHeader.SlpCode = ResultCustomerObject.SlpCode;
                    CashSalesCreditHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;

                    CashSalesCreditHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    CashSalesCreditHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    //CashSalesCreditHeader.GLName = i_OACT_Repository.GetGLNameByCode(CashSalesCreditHeader.GLCode);
                    CashSalesCreditHeader.Status = (short)DocumentStatuses.Open;
                    if (CashSalesCreditHeader.SubmittedToSAP.Equals(true))
                    {
                        CashSalesCreditHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    else
                    {

                    }

                    List<CashSalesCreditDocLs> Lines = _mapper.Map<List<CashSalesCreditLineViewModel>, List<CashSalesCreditDocLs>>(CashSalesObj.Lines);
                    List<CashSalesCreditDocNotes> NoteLines = _mapper.Map<List<CashSalesCreditNoteViewModel>, List<CashSalesCreditDocNotes>>(CashSalesObj.NoteLines);
                    List<CashSalesCreditDocPays> PayLines = _mapper.Map<List<CashSalesCreditPayViewModel>, List<CashSalesCreditDocPays>>(CashSalesObj.PayLines);

                    CashSalesCreditHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string CSCDocNum = string.Empty;
                    if (!i_CashSalesCreditDocH_Repository.AddCSC(CashSalesCreditHeader, Lines, NoteLines, PayLines,ref ValidationMessage, ref CSCDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        CashSalesObj.DocNum = CSCDocNum;
                        CashSalesObj.DocEntry = long.Parse(ValidationMessage);
                    }

                }
                else
                {
                    CashSalesCreditDocH CashSalesCreditHeader = i_CashSalesCreditDocH_Repository.GetByDocNumber(CashSalesObj.DocNum);

                    if (CashSalesCreditHeader != null)
                    {
                        if (CashSalesObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(CashSalesCreditHeader.DocEntry.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }

                        else if (CashSalesCreditHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if (CashSalesCreditHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if (CashSalesObj.PayLines == null || CashSalesObj.PayLines.Count() <= 0)
                        {
                            ErrList.Add("No Pay lines added");
                        }
                        else if (CashSalesObj.GrandTotal > (CashSalesObj.PayLines.Sum(x => x.PaidAmount)))
                        {
                            ErrList.Add("Paid Amount lesser than Grand Total");
                        }
                        else if (CashSalesObj.GrandTotal < (CashSalesObj.PayLines.Sum(x => x.PaidAmount)))
                        {
                            ErrList.Add("Paid Amount greater than Grand Total");
                        }
                        else if (CashSalesCreditHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            CashSalesCreditHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            CashSalesCreditHeader.SubmittedBy = User.Identity.Name;
                            if (!i_CashSalesCreditDocH_Repository.ResubmitCSCreditToSAP(CashSalesCreditHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = CashSalesCreditHeader.DocEntry;
                            CashSalesCreditHeader = _mapper.Map<CashSalesCreditViewModel, CashSalesCreditDocH>(CashSalesObj);
                            var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CashSalesCreditHeader.CardCode);
                            //var CashCustomerObject = i_CashSalesCustomer_Repository.GetByTelephoneNo(CashSalesCreditHeader.OfficeTelNo);

                            CashSalesCreditHeader.CardName = ResultCustomerObject.CardName;
                            //CashSalesCreditHeader.CashSalesCardName = CashCustomerObject.CustomerName;

                            CashSalesCreditHeader.SlpCode = ResultCustomerObject.SlpCode;
                            CashSalesCreditHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;

                            CashSalesCreditHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                            CashSalesCreditHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;

                            //CashSalesCreditHeader.GLName = i_OACT_Repository.GetGLNameByCode(CashSalesCreditHeader.GLCode);
                            CashSalesCreditHeader.Status = (short)DocumentStatuses.Open;
                            if (CashSalesCreditHeader.SubmittedToSAP.Equals(true))
                            {
                                CashSalesCreditHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            else
                            {

                            }

                            CashSalesCreditHeader.DocEntry = DocEntry;

                            List<CashSalesCreditDocLs> Lines = _mapper.Map<List<CashSalesCreditLineViewModel>, List<CashSalesCreditDocLs>>(CashSalesObj.Lines);
                            List<CashSalesCreditDocNotes> NoteLines = _mapper.Map<List<CashSalesCreditNoteViewModel>, List<CashSalesCreditDocNotes>>(CashSalesObj.NoteLines);
                            List<CashSalesCreditDocPays> PayLines = _mapper.Map<List<CashSalesCreditPayViewModel>, List<CashSalesCreditDocPays>>(CashSalesObj.PayLines);

                            CashSalesCreditHeader.UpdatedBy = User.Identity.Name;
                            string ValidationMessage = string.Empty;
                            string CSCDocNum = string.Empty;
                            if (!i_CashSalesCreditDocH_Repository.AddCSC(CashSalesCreditHeader, Lines, NoteLines, PayLines, ref ValidationMessage, ref CSCDocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                CashSalesObj.DocNum = CSCDocNum;
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

                CashSalesObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                CashSalesObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        CashSalesObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    CashSalesObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        CashSalesObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(CashSalesObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCashSalesCreditLastPriceHistory(string ItemCode, string CardCode)
        {
            IEnumerable<CashSalesCreditDocLs> LastPriceHistory = i_CashSalesCreditDocLs_Repository.GetCashSalesCreditLinesByItemCodeWithLimit(ItemCode, CardCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.CashSalesCreditDocH.DocDate).Select(e => new
            {
                DocDate = e.CashSalesCreditDocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.CashSalesCreditDocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }

    }
}