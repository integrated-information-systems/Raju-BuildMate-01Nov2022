using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.SQViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Sales Quotation")]
    public class SQController : Controller
    {
        private I_SQDocH_Repository i_SQDocH_Repository;
        private I_SQDocLs_Repository i_SQDocLs_Repository;         
        private I_OCRD_Repository i_OCRD_Repository;
        private I_DODocH_Repository i_DODocH_Repository;
        private I_OITW_Repository i_OITW_Repository;
        private I_ITT1_Repository i_ITT1_Repository;
        IMapper _mapper;
        public SQController(I_ITT1_Repository i_ITT1_Repository, I_OITW_Repository i_OITW_Repository, I_DODocH_Repository i_DODocH_Repository, I_OCRD_Repository i_OCRD_Repository, I_SQDocLs_Repository i_SQDocLs_Repository, I_SQDocH_Repository i_SQDocH_Repository, IMapper _mapper)
        {
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_SQDocH_Repository = i_SQDocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;             
            this.i_SQDocLs_Repository = i_SQDocLs_Repository;
            this._mapper = _mapper;
            this.i_OITW_Repository = i_OITW_Repository;
            this.i_ITT1_Repository = i_ITT1_Repository;
        }
        [HttpPost]
        [AjaxOnly]
        // GET: Item
        public JsonResult IndexNewPagining(DTPagination pagination)
        {
            string searchValue = pagination.search.value;
            string orderBy = "";
            string orderByDirection = pagination.order[0].dir;

            //coloumn index
            switch (int.Parse(pagination.order[0].column))
            {
                case 0:
                    orderBy = "DocNum";
                    break;
                case 1:
                    orderBy = "DocDate";
                    break;
                case 2:
                    orderBy = "CardCode";
                    break;
                case 3:
                    orderBy = "GrandTotal";
                    break;
                case 4:
                    orderBy = "PrintedStatus";
                    break;
                case 5:
                    orderBy = "DeliveryDate";
                    break;
                case 6:
                    orderBy = "DeliveryTime";
                    break;
                case 7:
                    orderBy = "CustomerRef";
                    break;
                case 8:
                   
                    break;
                case 9:
                    orderBy = "CopiedDO";
                    break;
                default:
                    orderBy = "DocNum";
                    break;
            }
            string orderByColumn = $"{orderBy} {orderByDirection}";
            var listOfItems = i_SQDocH_Repository.GetSQDetailsWithPagination(pagination.start, pagination.length, searchValue, orderByColumn);

            var items = listOfItems.Select((x) => new SQListViewModel()
            {
                DocNum = x.DocNum,
                CardCode = x.CardCode,
                DocDate = x.DocDate.ToString("dd'/'MM'/'yyyy"),
                GrandTotal = x.GrandTotal,
                PrintStatus = ((PrintedStatuses)x.PrintedStatus).ToString().Replace("Status_", "").Replace("_", " "),                 
                DeliveryDate = x.DeliveryDate?.ToString("dd'/'MM'/'yyyy"),
                DeliveryTime = x.DeliveryTime,
                CustomerRef = x.CustomerRef,
                DONum = x.CopiedDO,                
                DocEntry = x.DocEntry.ToString()

            });

            int TotalRecords = i_SQDocH_Repository.GetSQDetailsWithPaginationCount();
            int FilteredTotalRecords = 0;
            if (String.IsNullOrEmpty(searchValue))
                FilteredTotalRecords = TotalRecords;
            else
                FilteredTotalRecords = listOfItems.Count();
            return Json(new
            {
                draw = pagination.draw,
                recordsTotal = TotalRecords,
                recordsFiltered = FilteredTotalRecords,
                data = items
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexNew()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View(i_SQDocH_Repository.SalesQuotationHeaderList);
        }
        public ActionResult Edit(string DocNum)
        {
            SQViewModel addSQViewModel = new SQViewModel();
            
            SQDocH SQHObject = i_SQDocH_Repository.GetByDocNumber(DocNum);

            if (SQHObject != null)
            {
                addSQViewModel = _mapper.Map<SQDocH, SQViewModel>(SQHObject);
                addSQViewModel.CardName = SQHObject.CardCode;
                addSQViewModel.Lines = new List<SQLineViewModel>();
                addSQViewModel.NoteLines = new List<SQNoteViewModel>();
                
                addSQViewModel.Lines = _mapper.Map<List<SQDocLs>, List<SQLineViewModel>>(SQHObject.SQDocLs.OrderBy(x => x.LineNum).ToList());

                if (SQHObject.SQDocNotes != null)
                {
                    addSQViewModel.NoteLines = _mapper.Map<List<SQDocNotes>, List<SQNoteViewModel>>(SQHObject.SQDocNotes.OrderBy(x => x.LineNum).ToList());
                }
                addSQViewModel.DocDate = SQHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                if(SQHObject.DeliveryDate.HasValue)
                addSQViewModel.DeliveryDate = SQHObject.DeliveryDate.Value.ToString("dd'/'MM'/'yyyy");
                //addSQViewModel.DueDate = SQHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addSQViewModel.DiscByPercent = addSQViewModel.DiscByPercent.ToLower();
                
            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }

            return View("Add",addSQViewModel);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetSQLastPriceHistory(string ItemCode, string CardCode)
        {
            IEnumerable<SQDocLs> LastPriceHistory = i_SQDocLs_Repository.GetSQLinesByItemCode(ItemCode, CardCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.SQDocH.DocDate).Select(e => new 
            {
                DocDate =  e.SQDocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.SQDocH.DocNum,
                e.Qty,
                e.UnitPrice            
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }
        public ActionResult Add()
        {              


            SQViewModel addSQViewModel = new SQViewModel() {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DeliveryDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DueDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                DocNum = "New",               
                DiscByPercent="true",
                DiscAmount=0,
                DiscPercent=0
                };
            return View(addSQViewModel);
        }
        public ActionResult PrintPreview(string DocNum)
        {

            return View();
        }

        [HttpPost]       
        [AjaxOnly]
        [EditAuthorize(Roles = "Sales Quotation-Edit")]
        public JsonResult CopyToDO(string DocNum, int rowNo)
        {
         

            SQDocH SalesQuotationHeader = i_SQDocH_Repository.GetByDocNumber(DocNum);

            if (SalesQuotationHeader != null)
            {

                if (SalesQuotationHeader.CopiedDO != null)
                {
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = "Already copied to Delivery Order/Invoice, cannot Proceed"
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
                }
                DODocH DeliveryOrderHeader = new DODocH();
                DeliveryOrderHeader = _mapper.Map<SQDocH, DODocH>(SalesQuotationHeader);
                DeliveryOrderHeader.DocNum = "New";
                DeliveryOrderHeader.CopiedSQ = SalesQuotationHeader.DocNum;
                DeliveryOrderHeader.Status = (short)DocumentStatuses.Open;
                var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(SalesQuotationHeader.CardCode);
                DeliveryOrderHeader.DueDate = DeliveryOrderHeader.DocDate.AddDays(ResultCustomerObject.PaymentTerm.ExtraDays);
                List<SQDocLs> SQLines = SalesQuotationHeader.SQDocLs.OrderBy(x=> x.LineNum).ToList();
                List<DODocLs> DOLines = _mapper.Map<List<SQDocLs>, List<DODocLs>>(SQLines);   
                
                List<SQDocNotes> SQNoteLines = SalesQuotationHeader.SQDocNotes.ToList();
                List<DODocNotes> DONoteLines = _mapper.Map<List<SQDocNotes>, List<DODocNotes>>(SQNoteLines);
                string DODocNum = "";
                string ValidationMessage = string.Empty;
                bool HaveError = false;

                //for (int i = 0; i < DOLines.Count(); i++)
                //{
                //    decimal LocalAvailableQty = 0;
                //    LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(DOLines[i].ItemCode, DOLines[i].Location);
                //    IEnumerable<OITW> ItemStockDetails = i_OITW_Repository.GetLocationStockDetails(DOLines[i].ItemCode);
                //    string IsInventoryItem = ItemStockDetails.Select(x => x.Item.InvntItem).FirstOrDefault();
                //    if (IsInventoryItem.Equals("Y"))
                //    {
                //        decimal SAPAvailableQty = ItemStockDetails.Where(x => x.WhsCode.Equals(DOLines[i].Location)).Sum(x => (decimal?)(x.OnHand - x.IsCommited)) ?? 0;

                //        decimal OrderItemQty = DOLines.Where(x => x.ItemCode.Equals(DOLines[i].ItemCode) && x.Location.Equals(DOLines[i].Location)).Sum(x => x.Qty);
                //        decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty);
                //        if (TotalAvailableQty < OrderItemQty)
                //        {
                //            ValidationMessage = "Item " + DOLines[i].ItemCode + " not have enought stock in " + DOLines[i].LocationText + " warehouse in line no " + (i + 1);
                //            HaveError = true;
                //            break;
                //        }
                //    }
                //}
                

                    for (int i = 0; i < DOLines.Count(); i++)
                    {
                        decimal LocalAvailableQty = 0;
                        string CurrentItemCode = DOLines[i].ItemCode;
                        string CurrentLocation = DOLines[i].Location;
                        string CurrentLocationText = DOLines[i].LocationText;
                        bool isLoanIssued = DOLines[i].LoanIssued;
                        LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(CurrentItemCode, CurrentLocation);
                        IEnumerable<OITW> ItemStockDetails = i_OITW_Repository.GetLocationStockDetails(CurrentItemCode);
                        string IsInventoryItem = ItemStockDetails.Select(x => x.Item.InvntItem).FirstOrDefault();
                        string isUnitCostRequired = ItemStockDetails.Select(x => x.Item.U_ALLOW_UNIT_COST_ZERO).FirstOrDefault();
                        if (IsInventoryItem.Equals("Y") && isLoanIssued.Equals(false))
                        {
                            decimal SAPAvailableQty = i_OITW_Repository.GetLocationStockAvailableQty(CurrentItemCode, CurrentLocation);

                            decimal OrderItemQty = DOLines.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => x.Qty);

                            decimal ExistingDOQty = 0;
                            

                            decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty + ExistingDOQty);
                            if (TotalAvailableQty < OrderItemQty)
                            {
                                ValidationMessage = "Item " + CurrentItemCode + " not have enought stock in " + CurrentLocationText + " warehouse in line no " + (i + 1);
                                HaveError = true;
                                break;
                            }
                        }
                        else
                        {
                            IEnumerable<ITT1> childItems = i_ITT1_Repository.GetChildItemList(CurrentItemCode);
                            if (childItems.Count() > 0)
                            {
                                decimal OrderItemQty = DOLines.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => x.Qty);

                                foreach (var item in childItems)
                                {
                                    decimal ChildLocalAvailableQty = 0;
                                    decimal TotalAvailableQty = 0;
                                    decimal OrderedQty = OrderItemQty;
                                    //ChildLocalAvailableQty = (LocalAvailableQty * item.Quantity);
                                    ChildLocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(item.Code, CurrentLocation);
                                    decimal SAPAvailableQty = i_OITW_Repository.GetLocationStockAvailableQty(item.Code, CurrentLocation);
                                    //SAPAvailableQty = (SAPAvailableQty * item.Quantity);


                                    decimal ExistingDOQty = 0;
                                    if (!DeliveryOrderHeader.DocNum.Equals("New"))
                                    {
                                        ExistingDOQty = i_DODocH_Repository.GetByDocNumber(DeliveryOrderHeader.DocNum).DODocLs.Where(x => x.ItemCode.Equals(item.Code) && x.Location.Equals(CurrentLocation)).Sum(x => (decimal?)x.Qty) ?? 0;
                                    }
                                    //ExistingDOQty = (ExistingDOQty * item.Quantity);
                                    TotalAvailableQty += (ChildLocalAvailableQty + SAPAvailableQty + ExistingDOQty) / item.Quantity;

                                    if (TotalAvailableQty < OrderedQty)
                                    {
                                        ValidationMessage = "Child Item " + childItems.First().Code + "  not have enought stock in " + CurrentLocationText + " warehouse in line no " + (i + 1);
                                        HaveError = true;
                                        break;
                                    }
                                }

                            }
                        }
                        if (isUnitCostRequired != null && isUnitCostRequired == "N")
                        {
                            if (DOLines[i].UnitCost <= 0)
                            {
                                ValidationMessage = "For item " + CurrentItemCode + ", Unit Cost should be greater than zero in line no " + (i + 1);
                                HaveError = true;
                                break;
                            }
                        }

                    }
                
                if (HaveError == true)
                {
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = ValidationMessage
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);
                }

                decimal TotalDocBalance = i_DODocH_Repository.GetTotalSystemBalanceByCardCode(ResultCustomerObject.CardCode);
                ResultCustomerObject.Balance = TotalDocBalance + DeliveryOrderHeader.GrandTotal;
                if ((ResultCustomerObject.CreditLine - ResultCustomerObject.Balance) <= 0)
                {                    
                    var ErrorObj = new
                    {
                        Status = 400,
                        ResultHtml = "Exceeds Credit limit, cannot Proceed"
                    };
                    return Json(ErrorObj, JsonRequestBehavior.DenyGet);                    
                }
                else if (!i_DODocH_Repository.AddDO(DeliveryOrderHeader, DOLines, DONoteLines,ref ValidationMessage, ref DODocNum))
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
                        DODocNum,
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
         
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Sales Quotation-Edit")]
        public JsonResult Add(SQViewModel SQObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (SQObj.Lines == null || SQObj.Lines.Count()<=0)
            {
                ErrList.Add("No lines added");

            }
             
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (SQObj.DocNum.Equals("New"))
                {
                    SQDocH SalesQuotationHeader = new SQDocH();
                    SalesQuotationHeader = _mapper.Map<SQViewModel, SQDocH>(SQObj);

                    var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(SalesQuotationHeader.CardCode);

                    SalesQuotationHeader.CardName = ResultCustomerObject.CardName;
                    SalesQuotationHeader.SlpCode = ResultCustomerObject.SlpCode;
                    SalesQuotationHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                    SalesQuotationHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    SalesQuotationHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    SalesQuotationHeader.Status = (short)DocumentStatuses.Open;
                    List<SQDocLs> Lines = _mapper.Map<List<SQLineViewModel>, List<SQDocLs>>(SQObj.Lines);
                    List<SQDocNotes> NoteLines = _mapper.Map<List<SQNoteViewModel>, List<SQDocNotes>>(SQObj.NoteLines);
                    SalesQuotationHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string SQDocNum = string.Empty;
                    if (!i_SQDocH_Repository.AddSQ(SalesQuotationHeader, Lines, NoteLines,ref ValidationMessage, ref SQDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        SQObj.DocNum = SQDocNum;
                        SQObj.DocEntry = long.Parse(ValidationMessage);

                    }
                }
                else
                {
                    SQDocH SalesQuotationHeader = i_SQDocH_Repository.GetByDocNumber(SQObj.DocNum);
                   
                    if (SalesQuotationHeader != null)
                    {

                        long DocEntry = SalesQuotationHeader.DocEntry;
                        SalesQuotationHeader = _mapper.Map<SQViewModel, SQDocH>(SQObj);
                        var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(SalesQuotationHeader.CardCode);
                        SalesQuotationHeader.CardName = ResultCustomerObject.CardName;
                        SalesQuotationHeader.SlpCode = ResultCustomerObject.SlpCode;
                        SalesQuotationHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                        SalesQuotationHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                        SalesQuotationHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;


                        SalesQuotationHeader.DocEntry = DocEntry;
                        List<SQDocLs> Lines = _mapper.Map<List<SQLineViewModel>, List<SQDocLs>>(SQObj.Lines);
                        List<SQDocNotes> NoteLines = _mapper.Map<List<SQNoteViewModel>, List<SQDocNotes>>(SQObj.NoteLines);
                        SalesQuotationHeader.UpdatedBy = User.Identity.Name;
                        string SQDocNum = string.Empty;
                        string ValidationMessage = string.Empty;
                        if (!i_SQDocH_Repository.AddSQ(SalesQuotationHeader, Lines, NoteLines,ref ValidationMessage, ref SQDocNum))
                        {
                            ErrList.Add(ValidationMessage);
                        }
                        else
                        {
                            SQObj.DocNum = SQDocNum;
                          
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
                SQObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                SQObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0 )
                {
                    
                    foreach (var entry in ModelErrList)
                    {
                        SQObj.ModelErrList.Add(entry);
                    }                   
                }
                if (ErrList.Count() > 0)
                {
                    SQObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        SQObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(SQObj, JsonRequestBehavior.DenyGet);
        }

    }
}