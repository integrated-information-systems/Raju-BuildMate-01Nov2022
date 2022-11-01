using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.PaymentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Payment")]
    public class PaymentController : Controller
    {
        private I_PaymentDocH_Repository i_PaymentDocH_Repository;
        private I_CashSalesDocH_Repository i_CashSalesDocH_Repository;
        private I_DODocH_Repository i_DODocH_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OACT_Repository i_OACT_Repository;
        IMapper _mapper;
        public PaymentController(I_OACT_Repository i_OACT_Repository, I_OCRD_Repository i_OCRD_Repository, I_CashSalesDocH_Repository i_CashSalesDocH_Repository, I_DODocH_Repository i_DODocH_Repository, I_PaymentDocH_Repository i_PaymentDocH_Repository, IMapper _mapper)
        {
            this.i_PaymentDocH_Repository = i_PaymentDocH_Repository;
            this.i_CashSalesDocH_Repository = i_CashSalesDocH_Repository;
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OACT_Repository = i_OACT_Repository;
            this._mapper = _mapper;
        }
         
        public ActionResult Index()
        {
            return View(i_PaymentDocH_Repository.PaymentHeaderList);
        }
        public ActionResult Edit(string DocNum)
        {
 


            PaymentViewModel addPaymentViewModel = new PaymentViewModel();

            PaymentDocH PaymentHObject = i_PaymentDocH_Repository.GetByDocNumber(DocNum);

            if (PaymentHObject != null)
            {
                addPaymentViewModel = _mapper.Map<PaymentDocH, PaymentViewModel>(PaymentHObject);
                addPaymentViewModel.CardName = PaymentHObject.CardCode;
                addPaymentViewModel.Lines = new List<PaymentLineViewModel>();
                addPaymentViewModel.NoteLines = new List<PaymentNoteViewModel>();

                foreach (var entry in PaymentHObject.PaymentDocLs)
                {
                    PaymentLineViewModel dOLine = new PaymentLineViewModel();
                    dOLine = _mapper.Map<PaymentDocLs, PaymentLineViewModel>(entry);
                    addPaymentViewModel.Lines.Add(dOLine);
                }
                if (PaymentHObject.PaymentDocNotes != null)
                {
                    foreach (var entry in PaymentHObject.PaymentDocNotes)
                    {
                        PaymentNoteViewModel dONoteLine = new PaymentNoteViewModel();
                        dONoteLine = _mapper.Map<PaymentDocNotes, PaymentNoteViewModel>(entry);
                        addPaymentViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                addPaymentViewModel.DocDate = PaymentHObject.DocDate.ToString("dd'/'MM'/'yyyy");


            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add", addPaymentViewModel);
        }
        public ActionResult Add()
        {
            var ResultObject = i_OACT_Repository.GLCodes.FirstOrDefault(x => x.AcctName.Equals("Cash at Bank - OCBC"));
            string defaultGLCode = string.Empty;
            if (ResultObject != null)
                defaultGLCode = ResultObject.AcctCode;

            PaymentViewModel addPaymentViewModel = new PaymentViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",
                SubmittedToSAP = false,
                GLCode = defaultGLCode

            };
            return View(addPaymentViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Payment-Edit")]
        public JsonResult Add(PaymentViewModel PaymentObj)
        {

            
 

            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();            
            if (PaymentObj.Lines == null || PaymentObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }            
            else if (PaymentObj.Lines.Where(x => x.Paid.Equals(true)).ToList().Count()<=0)
            {
                if (PaymentObj.DocNum.Equals("New")) { 
                    ErrList.Add("No DO lines selected for Payment");
                }
            }
            else if (PaymentObj.PaymentType.Equals("5") && PaymentObj.ChequeNoReference == null)
            {
                ErrList.Add("Cheque No Reference field is required");
            }
            else if(PaymentObj.PaymentType.Equals("5") && PaymentObj.ChequeNoReference.Trim().Equals(""))
            {
                ErrList.Add("Cheque No Reference field is required");
            }
            else if (PaymentObj.Lines.Where(x => x.Paid.Equals(true) && x.PaymentAmount<=0).ToList().Count() > 0)
            {
                ErrList.Add("DO lines cannot have zero allocation");
            }
            else if(PaymentObj.PaidAmount < (PaymentObj.Lines.Sum(x=> x.PaymentAmount) + (PaymentObj.Lines.Sum(x => x.DiscountAmount)))) {
                ErrList.Add("Paid Amount lesser than total Delivery Orders paid");
            }
            else if (PaymentObj.PaidAmount > (PaymentObj.Lines.Sum(x => x.PaymentAmount) + (PaymentObj.Lines.Sum(x => x.DiscountAmount))))
            {
                ErrList.Add("Paid Amount greater than total Delivery Orders paid");
            }
           
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (PaymentObj.DocNum.Equals("New"))
                {
                    PaymentDocH PaymentHeader = new PaymentDocH();
                    PaymentHeader = _mapper.Map<PaymentViewModel, PaymentDocH>(PaymentObj);
                    var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(PaymentHeader.CardCode);


                    PaymentHeader.CardName = ResultCustomerObject.CardName;                    
                    PaymentHeader.GLName = i_OACT_Repository.GetGLNameByCode(PaymentHeader.GLCode);
                    PaymentHeader.SubmittedToSAP = true;
                    PaymentHeader.Status = (short)DocumentStatuses.Open;
                    PaymentHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;

                    List<PaymentDocLs> Lines = _mapper.Map<List<PaymentLineViewModel>, List<PaymentDocLs>>(PaymentObj.Lines.Where(x => x.Paid.Equals(true)).ToList());
                    List<PaymentDocNotes> NoteLines = _mapper.Map<List<PaymentNoteViewModel>, List<PaymentDocNotes>>(PaymentObj.NoteLines);
                    PaymentHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string PayDocNum = string.Empty;
                    if (!i_PaymentDocH_Repository.AddPayment(PaymentHeader, Lines, NoteLines, ref ValidationMessage,ref PayDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        PaymentObj.DocNum = PayDocNum;
                    }

                }
                else
                {
                    PaymentDocH PaymentHeader = i_PaymentDocH_Repository.GetByDocNumber(PaymentObj.DocNum);
                    if (PaymentHeader != null)
                    {
                        if (PaymentHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if (PaymentHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if (PaymentHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            PaymentHeader.SubmittedBy = User.Identity.Name;
                            if (!i_PaymentDocH_Repository.ResubmitPaymentToSAP(PaymentHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
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

                PaymentObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                PaymentObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        PaymentObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    PaymentObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        PaymentObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(PaymentObj, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetDocList(string CardCode)
        {

            List<PaymentLineViewModel> ListOfDocuments = new List<PaymentLineViewModel>();
            if (CardCode != null)
            {                
                List<DODocH> ResultDOObjects = i_DODocH_Repository.DOHeaderBalanceDueList(CardCode).ToList() ;
                
                foreach (DODocH i in ResultDOObjects)
                {
                    ListOfDocuments.Add(new PaymentLineViewModel
                    {
                        LineNum = ListOfDocuments.Count() + 1,
                        PaymentAmount = 0,
                        ReferenceDocNum = i.DocNum,
                        CustomerRef = i.CustomerRef,
                        ReferenceDocType = "Delivery Order",
                        DocTotal = i.GrandTotal,
                        BalanceDue = i.BalanceDue,
                        DocDate = i.DocDate.ToString("dd'/'MM'/'yyyy")
                    });
                }
            }

            return Json(ListOfDocuments, JsonRequestBehavior.DenyGet);
        }
    }
    

}