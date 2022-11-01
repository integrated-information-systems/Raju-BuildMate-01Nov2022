using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.PQViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Purchase Quotation")]
    public class PQController : Controller
    {
        private I_PQDocH_Repository i_PQDocH_Repository;
        private I_PQDocLs_Repository i_PQDocLs_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_PODocH_Repository i_PODocH_Repository;
        private I_OITW_Repository i_OITW_Repository;
        IMapper _mapper;
        public PQController(I_OITW_Repository i_OITW_Repository, I_PODocH_Repository i_PODocH_Repository, I_OCRD_Repository i_OCRD_Repository, I_PQDocLs_Repository i_PQDocLs_Repository, I_PQDocH_Repository i_PQDocH_Repository, IMapper _mapper)
        {
            this.i_PQDocH_Repository = i_PQDocH_Repository;
            this.i_PODocH_Repository = i_PODocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_PQDocLs_Repository = i_PQDocLs_Repository;
            this._mapper = _mapper;
            this.i_OITW_Repository = i_OITW_Repository;
        }
        public ActionResult Index()
        {
            return View(i_PQDocH_Repository.PurchaseQuotationHeaderList);
        }
        public ActionResult Add()
        {
            PQViewModel addPQViewModel = new PQViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DeliveryDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DueDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",
                DiscByPercent = "true",
                DiscAmount = 0,
                DiscPercent = 0
            };
            return View(addPQViewModel);
        }
        public ActionResult Edit(string DocNum)
        {
            PQViewModel addPQViewModel = new PQViewModel();

            PQDocH PQHObject = i_PQDocH_Repository.GetByDocNumber(DocNum);

            if (PQHObject != null)
            {
                addPQViewModel = _mapper.Map<PQDocH, PQViewModel>(PQHObject);
                addPQViewModel.CardName = PQHObject.CardCode;
                addPQViewModel.Lines = new List<PQLineViewModel>();
                addPQViewModel.NoteLines = new List<PQNoteViewModel>();

                addPQViewModel.Lines = _mapper.Map<List<PQDocLs>, List<PQLineViewModel>>(PQHObject.PQDocLs.OrderBy(x => x.LineNum).ToList());

                if (PQHObject.PQDocNotes != null)
                {
                    addPQViewModel.NoteLines = _mapper.Map<List<PQDocNotes>, List<PQNoteViewModel>>(PQHObject.PQDocNotes.OrderBy(x => x.LineNum).ToList());
                }
                addPQViewModel.DocDate = PQHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                if(PQHObject.DeliveryDate.HasValue)
                addPQViewModel.DeliveryDate = PQHObject.DeliveryDate.Value.ToString("dd'/'MM'/'yyyy");
                //addPQViewModel.DueDate = PQHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addPQViewModel.DiscByPercent = addPQViewModel.DiscByPercent.ToLower();
                addPQViewModel.CurrentUserIsNotInRoleNotes = !(User.IsInRole("Notes") && PQHObject.Status == (short)DocumentStatuses.Closed);

            }
            else
            {
                TempData["GlobalErrMsg"] = "Purchase order not found";
                return RedirectToAction("Index");
            }

            return View("Add", addPQViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Quotation-Edit")]
        public JsonResult Add(PQViewModel PQObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (PQObj.Lines == null || PQObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }

            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (PQObj.DocNum.Equals("New"))
                {
                    PQDocH PurchaseOrderHeader = new PQDocH();
                    PurchaseOrderHeader = _mapper.Map<PQViewModel, PQDocH>(PQObj);

                    var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(PurchaseOrderHeader.CardCode);

                    PurchaseOrderHeader.CardName = ResultCustomerObject.CardName;
                    PurchaseOrderHeader.SlpCode = ResultCustomerObject.SlpCode;
                    PurchaseOrderHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                    PurchaseOrderHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    PurchaseOrderHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    PurchaseOrderHeader.Status = (short)DocumentStatuses.Open;


                    List<PQDocLs> Lines = _mapper.Map<List<PQLineViewModel>, List<PQDocLs>>(PQObj.Lines);
                    List<PQDocNotes> NoteLines = _mapper.Map<List<PQNoteViewModel>, List<PQDocNotes>>(PQObj.NoteLines);
                    PurchaseOrderHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string PQDocNum = string.Empty;
                    if (!i_PQDocH_Repository.AddPQ(PurchaseOrderHeader, Lines, NoteLines, ref ValidationMessage, ref PQDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        PQObj.DocNum = PQDocNum;
                        PQObj.DocEntry = long.Parse(ValidationMessage);
                    }
                }
                else
                {
                    PQDocH PurchaseOrderHeader = i_PQDocH_Repository.GetByDocNumber(PQObj.DocNum);

                    if (PurchaseOrderHeader != null)
                    {
                        if (PurchaseOrderHeader.PQDocLs.Count() != PQObj.Lines.Count() && PurchaseOrderHeader.CopiedPO != null)
                        {
                            ErrList.Add("PQ Copied to PO already, modification of lines not allowed,cannot proceed");
                        }
                        else
                        {


                            long DocEntry = PurchaseOrderHeader.DocEntry;
                            PurchaseOrderHeader = _mapper.Map<PQViewModel, PQDocH>(PQObj);
                            var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(PurchaseOrderHeader.CardCode);
                            PurchaseOrderHeader.CardName = ResultCustomerObject.CardName;
                            PurchaseOrderHeader.SlpCode = ResultCustomerObject.SlpCode;
                            PurchaseOrderHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                            PurchaseOrderHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                            PurchaseOrderHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;


                            PurchaseOrderHeader.DocEntry = DocEntry;
                            List<PQDocLs> Lines = _mapper.Map<List<PQLineViewModel>, List<PQDocLs>>(PQObj.Lines);
                            List<PQDocNotes> NoteLines = _mapper.Map<List<PQNoteViewModel>, List<PQDocNotes>>(PQObj.NoteLines);
                            PurchaseOrderHeader.UpdatedBy = User.Identity.Name;
                            string PQDocNum = string.Empty;
                            string ValidationMessage = string.Empty;
                            if (!i_PQDocH_Repository.AddPQ(PurchaseOrderHeader, Lines, NoteLines, ref ValidationMessage, ref PQDocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                PQObj.DocNum = PQDocNum;
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
                PQObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                PQObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        PQObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    PQObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        PQObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(PQObj, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Purchase Quotation-Edit")]
        public JsonResult CopyToPO(string DocNum, int rowNo)
        {


            PQDocH PurchaseQuotationHeader = i_PQDocH_Repository.GetByDocNumber(DocNum);

            if (PurchaseQuotationHeader != null)
            {
                if (PurchaseQuotationHeader.CopiedPO != null)
                {
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = "Already copied to Purchase Order, cannot Proceed"
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
                }
                PODocH PurchaseOrderHeader = new PODocH();
                PurchaseOrderHeader = _mapper.Map<PQDocH, PODocH>(PurchaseQuotationHeader);
                PurchaseOrderHeader.DocNum = "New";
                PurchaseOrderHeader.CopiedPQ = PurchaseQuotationHeader.DocNum;
                PurchaseOrderHeader.Status = (short)DocumentStatuses.Open;
                var ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(PurchaseQuotationHeader.CardCode);
                PurchaseOrderHeader.DueDate = PurchaseOrderHeader.DocDate.AddDays(ResultCustomerObject.PaymentTerm.ExtraDays);
                List<PQDocLs> PQLines = PurchaseQuotationHeader.PQDocLs.ToList();
                List<PODocLs> POLines = _mapper.Map<List<PQDocLs>, List<PODocLs>>(PQLines);

                List<PQDocNotes> PQNoteLines = PurchaseQuotationHeader.PQDocNotes.ToList();
                List<PODocNotes> PONoteLines = _mapper.Map<List<PQDocNotes>, List<PODocNotes>>(PQNoteLines);
                string PODocNum = "";
                string ValidationMessage = string.Empty;
                 

                if (!i_PODocH_Repository.AddPO(PurchaseOrderHeader, POLines, PONoteLines, ref ValidationMessage, ref PODocNum))
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
                    var DOResultObj = new
                    {
                        Status = 200,
                        ResultHtml = "",
                        PODocNum,
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
                    ResultHtml = ""
                };
                return Json(ErrorObj, JsonRequestBehavior.DenyGet);
            }




        }
    }
}