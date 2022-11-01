using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.Planner;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.CashSalesViewModels;
using BMSS.WebUI.Models.DOViewModels;
using BMSS.WebUI.Models.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Cash Sales")]
    public class CashSalesController : Controller
    {
        private I_DODocH_Repository i_DODocH_Repository;
        private I_CashSalesDocH_Repository i_CashSalesDocH_Repository;
        private I_CashSalesDocLs_Repository i_CashSalesDocLs_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OACT_Repository i_OACT_Repository;
        private I_OSLP_Repository i_OSLP_Repository;
        private I_CashSalesCustomer_Repository i_CashSalesCustomer_Repository;
        private I_OITW_Repository i_OITW_Repository;
        private I_ITT1_Repository i_ITT1_Repository;
        private I_DeliveryPlanner_Repository i_DeliveryPlanner_Repository;
        private I_OWHS_Repository i_OWHS_Repository;
        IMapper _mapper;
        public CashSalesController(I_OWHS_Repository i_OWHS_Repository, I_DeliveryPlanner_Repository i_DeliveryPlanner_Repository, I_ITT1_Repository i_ITT1_Repository, I_OITW_Repository i_OITW_Repository, I_DODocH_Repository i_DODocH_Repository, I_CashSalesCustomer_Repository i_CashSalesCustomer_Repository, I_OSLP_Repository i_OSLP_Repository,I_OACT_Repository i_OACT_Repository, I_OCRD_Repository i_OCRD_Repository, I_CashSalesDocLs_Repository i_CashSalesDocLs_Repository, I_CashSalesDocH_Repository i_CashSalesDocH_Repository, I_OCTG_Repository i_OCTG_Repository, IMapper _mapper)
        {
            this.i_CashSalesDocH_Repository = i_CashSalesDocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OACT_Repository = i_OACT_Repository;
            this.i_CashSalesDocLs_Repository = i_CashSalesDocLs_Repository;
            this.i_OSLP_Repository = i_OSLP_Repository;
            this.i_CashSalesCustomer_Repository = i_CashSalesCustomer_Repository;
            this._mapper = _mapper;
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_OITW_Repository = i_OITW_Repository;
            this.i_ITT1_Repository = i_ITT1_Repository;
            this.i_DeliveryPlanner_Repository = i_DeliveryPlanner_Repository;
            this.i_OWHS_Repository = i_OWHS_Repository;
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
                    orderBy = "SyncStatus";
                    break;
                case 6:
                    orderBy = "SAPDocNum";
                    break;
                case 7:
                    orderBy = "DeliveryDate";
                    break;
                case 8:
                    orderBy = "DeliveryTime";
                    break;
                case 9:
                    orderBy = "CustomerRef";
                    break;
                default:
                    orderBy = "DocNum";
                    break;
            }
            string orderByColumn = $"{orderBy} {orderByDirection}";
            var listOfItems = i_CashSalesDocH_Repository.GetCSDetailsWithPagination(pagination.start, pagination.length, searchValue, orderByColumn);

            var items = listOfItems.Select((x) => new CashSalesListViewModel()
            {
                DocNum = x.DocNum,
                CardCode = x.CardCode,
                DocDate = x.DocDate.ToString("dd'/'MM'/'yyyy"),
                GrandTotal = x.GrandTotal,
                PrintStatus = ((PrintedStatuses)x.PrintedStatus).ToString().Replace("Status_", "").Replace("_", " "),
                SAPStatus = ((SyncStatuses)x.SyncStatus).ToString().Replace("Status_", "").Replace("_", " "),
                SAPDocNum = x.SAPDocNum,
                SyncRemarks = x.SubmittedBy != null ? $"{x.SyncRemarks}, Submitted by {x.SubmittedBy}, Submitted on {x.SubmittedOn?.ToString("dd'/'MM'/'yyyy hh:mm:ss")}" : x.SyncRemarks,
                DeliveryDate = x.DeliveryDate?.ToString("dd'/'MM'/'yyyy"),
                DeliveryTime = x.DeliveryTime,
                CustomerRef = x.CustomerRef,
                ShipToAddress1 = x.ShipToAddress1,             
                SentCount = x.SentToPlanner,
                DocEntry = x.DocEntry.ToString()

            });

            int TotalRecords = i_CashSalesDocH_Repository.GetCSDetailsWithPaginationCount();
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
        public ActionResult Indexnew()
        {
            var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            String PreferredLocation = string.Empty;
            PreferredLocation = claims?.FirstOrDefault(x => x.Type.Equals("PreferredLocation", StringComparison.OrdinalIgnoreCase))?.Value;
            ViewBag.PreferredLocation = PreferredLocation;


            List<SelectListItem> Warehouses = i_OWHS_Repository.Warehouses.Select(e => new SelectListItem
            {
                Text = e.WhsName,
                Value = e.WhsCode.ToString()
            }).ToList();
            ViewBag.WarehouseList = Warehouses;
            return View();
        }
        public ActionResult Index()
        {
            return View(i_CashSalesDocH_Repository.CashSalesHeaderList);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult CanPrintInvoice(string DocNumber)
        {
            if (CannotPrintAndSave(DocNumber))
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
        private Boolean CannotPrintAndSave(string DocNumber)
        {
            CashSalesDocH CashSalesHeader = i_CashSalesDocH_Repository.GetByDocNumber(DocNumber);
            return CashSalesHeader.INVPrintedCount >= 1 && !User.IsInRole("Print After First Time");

        }
        public ActionResult Edit(string DocNum)
        {

            var ResultObject = i_OACT_Repository.GLCodes.FirstOrDefault(x => x.AcctName.Equals("Cash at Bank - OCBC"));
            string defaultGLCode = string.Empty;
            if (ResultObject != null)
                defaultGLCode = ResultObject.AcctCode;

            ViewBag.defaultGLCode = defaultGLCode;

            CashSalesViewModel addCashSalesViewModel = new CashSalesViewModel();

            CashSalesDocH CashSalesHObject = i_CashSalesDocH_Repository.GetByDocNumber(DocNum);

            if (CashSalesHObject != null)
            {
                addCashSalesViewModel = _mapper.Map<CashSalesDocH, CashSalesViewModel>(CashSalesHObject);
                addCashSalesViewModel.CardName = addCashSalesViewModel.CardCode;
                addCashSalesViewModel.Lines = new List<CashSalesLineViewModel>();
                addCashSalesViewModel.NoteLines = new List<CashSalesNoteViewModel>();
                addCashSalesViewModel.PayLines = new List<CashSalesPayViewModel>();

                foreach (var entry in CashSalesHObject.CashSalesDocLs.OrderBy(x=>x.LineNum))
                {
                    CashSalesLineViewModel dOLine = new CashSalesLineViewModel();
                    dOLine = _mapper.Map<CashSalesDocLs, CashSalesLineViewModel>(entry);
                    addCashSalesViewModel.Lines.Add(dOLine);
                }
                if (CashSalesHObject.CashSalesDocNotes != null)
                {
                    foreach (var entry in CashSalesHObject.CashSalesDocNotes.OrderBy(x => x.LineNum))
                    {
                        CashSalesNoteViewModel dONoteLine = new CashSalesNoteViewModel();
                        dONoteLine = _mapper.Map<CashSalesDocNotes, CashSalesNoteViewModel>(entry);
                        addCashSalesViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                if (CashSalesHObject.CashSalesDocPays != null)
                {
                    foreach (var entry in CashSalesHObject.CashSalesDocPays.OrderBy(x => x.LineNum))
                    {
                        CashSalesPayViewModel dOPayLine = new CashSalesPayViewModel();
                        dOPayLine = _mapper.Map<CashSalesDocPays, CashSalesPayViewModel>(entry);
                        addCashSalesViewModel.PayLines.Add(dOPayLine);
                    }
                }
                addCashSalesViewModel.DocDate = CashSalesHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                if(CashSalesHObject.DeliveryDate.HasValue)
                addCashSalesViewModel.DeliveryDate = CashSalesHObject.DeliveryDate.Value.ToString("dd'/'MM'/'yyyy");
                //addCashSalesViewModel.DueDate = CashSalesHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addCashSalesViewModel.DiscByPercent = addCashSalesViewModel.DiscByPercent.ToLower();

            }
            else
            {
                TempData["GlobalErrMsg"] = "Cash Sales document not found";
                return RedirectToAction("Index");
            }

            return View("Add", addCashSalesViewModel);
        }

        public ActionResult Add()
        {
            var ResultObject = i_OACT_Repository.GLCodes.FirstOrDefault(x => x.AcctName.Equals("Cash at Bank - OCBC"));
            string defaultGLCode = string.Empty;
            if (ResultObject != null)
                defaultGLCode = ResultObject.AcctCode;

            ViewBag.defaultGLCode = defaultGLCode;



            var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            String PreferredCashSalesCustomer = string.Empty;
            PreferredCashSalesCustomer = claims?.FirstOrDefault(x => x.Type.Equals("PreferredCashSalesCustomer", StringComparison.OrdinalIgnoreCase))?.Value;
            String PreferredLocation = string.Empty;
            PreferredLocation = claims?.FirstOrDefault(x => x.Type.Equals("PreferredLocation", StringComparison.OrdinalIgnoreCase))?.Value;
            ViewBag.PreferredLocation = PreferredLocation;
            //var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(PreferredCashSalesCustomer);
            CashSalesViewModel addCashSalesViewModel = new CashSalesViewModel()
            {
                DocDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DeliveryDate = DateTime.Now.ToString("dd'/'MM'/'yyyy"),
                //DueDate= DateTime.Now.AddDays(ResultCustomerObject.PaymentTerm.ExtraDays).ToString("dd/MM/yyyy"),
                DocNum = "New",
                DiscByPercent = "true",
                DiscAmount = 0,
                DiscPercent = 0,
                SubmittedToSAP = false,
               // Currency = ResultCustomerObject.Currency,
                CardCode= PreferredCashSalesCustomer,
                CardName = PreferredCashSalesCustomer,                
               // SlpName = ResultCustomerObject.SalesPerson.SlpName,
            };
            return View(addCashSalesViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Cash Sales-Edit")]
        public JsonResult AddPlannerLine(DOPlannerRowViewModel DOPlannerLineObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            DateTime DDate = DateTime.Now;
            if (!DateTime.TryParseExact(DOPlannerLineObj.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DDate))
            {
                ErrList.Add("Delivery Date is not in valid Format");
            }
            switch (DOPlannerLineObj.CreatorLocation)
            {
                case "JYR":
                    DOPlannerLineObj.CreatorLocation = "1";
                    break;
                case "ENS":
                    DOPlannerLineObj.CreatorLocation = "2";
                    break;
                case "TRS":
                    DOPlannerLineObj.CreatorLocation = "3";
                    break;
                default:
                    DOPlannerLineObj.CreatorLocation = "1";
                    break;
            }
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                DeliveryPlanner DeliveryPlannerObject = new DeliveryPlanner();
                DeliveryPlannerObject = _mapper.Map<DOPlannerRowViewModel, DeliveryPlanner>(DOPlannerLineObj);
                DeliveryPlannerObject.AddedBy = User.Identity.Name.ToLower();
                DeliveryPlannerObject.CreatedBy = User.Identity.Name.ToLower();
                string ValidationMessage = string.Empty;
                if (!i_DeliveryPlanner_Repository.AddDOPlannerLine(DeliveryPlannerObject, ref ValidationMessage))
                {
                    //ErrList.Add("There is some issue in inserting");                   
                    ErrList.Add(ValidationMessage);
                }
                else
                {
                    int SentCount = 0;
                    if (!i_CashSalesDocH_Repository.NotePlannerSubmission(DeliveryPlannerObject.ReferenceNo, ref SentCount))
                    {
                        ErrList.Add("Sent to Planner, but cannot save delivey planner sent count");
                    }
                    DOPlannerLineObj.SentCount = SentCount;
                }
            }

            if (ModelState.IsValid == false || ErrList.Count() > 0)
            {

                DOPlannerLineObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                DOPlannerLineObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        DOPlannerLineObj.ModelErrList.Add(entry);
                    }
                }


                if (ErrList.Count() > 0)
                {
                    DOPlannerLineObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        DOPlannerLineObj.ModelErrList.Add(entry);
                    }
                }
            }
            return Json(DOPlannerLineObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Cash Sales-Edit")]
        public JsonResult Add(CashSalesViewModel CashSalesObj)
        {
            CashSalesObj.CashSalesCardName = "-";
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
                decimal LocalAvailableQty = 0;
                string CurrentItemCode = CashSalesObj.Lines[i].ItemCode;
                string CurrentLocation = CashSalesObj.Lines[i].Location;
                string CurrentLocationText = CashSalesObj.Lines[i].LocationText;
              

                LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(CurrentItemCode, CurrentLocation);
                IEnumerable<OITW> ItemStockDetails = i_OITW_Repository.GetLocationStockDetails(CurrentItemCode);
                string IsInventoryItem = ItemStockDetails.Select(x => x.Item.InvntItem).FirstOrDefault();
                string isUnitCostRequired = ItemStockDetails.Select(x => x.Item.U_ALLOW_UNIT_COST_ZERO).FirstOrDefault();
                if (IsInventoryItem.Equals("Y"))
                {
                    decimal SAPAvailableQty = i_OITW_Repository.GetLocationStockAvailableQty(CurrentItemCode, CurrentLocation);

                    decimal OrderItemQty = CashSalesObj.Lines.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => x.Qty);

                    decimal ExistingCSQty = 0;
                    if (!CashSalesObj.DocNum.Equals("New"))
                    {
                        ExistingCSQty = i_CashSalesDocH_Repository.GetByDocNumber(CashSalesObj.DocNum).CashSalesDocLs.Where(x => x.ItemCode.Equals(CurrentItemCode)).Sum(x => (decimal?)x.Qty) ?? 0;
                    }

                    decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty + ExistingCSQty);


                    if (TotalAvailableQty < OrderItemQty)
                    {
                        ErrList.Add("Item " + CurrentItemCode + " not have enought stock in " + CurrentLocationText + " warehouse in line no " + (i + 1));
                    }
                }
                else
                {
                    IEnumerable<ITT1> childItems = i_ITT1_Repository.GetChildItemList(CurrentItemCode);
                    if (childItems.Count() > 0)
                    {
                        decimal OrderItemQty = CashSalesObj.Lines.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => x.Qty);

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
                            if (!CashSalesObj.DocNum.Equals("New"))
                            {
                                ExistingDOQty = i_CashSalesDocH_Repository.GetByDocNumber(CashSalesObj.DocNum).CashSalesDocLs.Where(x => x.ItemCode.Equals(item.Code) && x.Location.Equals(CurrentLocation)).Sum(x => (decimal?)x.Qty) ?? 0;
                            }
                            //ExistingDOQty = (ExistingDOQty * item.Quantity);
                            TotalAvailableQty += (ChildLocalAvailableQty + SAPAvailableQty + ExistingDOQty) / item.Quantity;
                            
                            if (TotalAvailableQty < OrderedQty)
                            {
                                ErrList.Add("Child Item " + childItems.First().Code + "  not have enought stock in " + CurrentLocationText + " warehouse in line no " + (i + 1));
                            }
                        }

                    }
                }
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
                    CashSalesDocH CashSalesHeader = new CashSalesDocH();
                    CashSalesHeader = _mapper.Map<CashSalesViewModel, CashSalesDocH>(CashSalesObj);
                    var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CashSalesHeader.CardCode);
                    //var CashCustomerObject = i_CashSalesCustomer_Repository.GetByTelephoneNo(CashSalesHeader.OfficeTelNo);


                    CashSalesHeader.CardName = ResultCustomerObject.CardName;
                    //CashSalesHeader.CashSalesCardName = CashCustomerObject.CustomerName;

                    CashSalesHeader.SlpCode = ResultCustomerObject.SlpCode;
                    CashSalesHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;

                    CashSalesHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    CashSalesHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    CashSalesHeader.Status = (short)DocumentStatuses.Open;
                     
                    if (CashSalesHeader.SubmittedToSAP.Equals(true))
                    {
                        CashSalesHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    else
                    {

                    }
                    List<CashSalesDocLs> Lines = _mapper.Map<List<CashSalesLineViewModel>, List<CashSalesDocLs>>(CashSalesObj.Lines);
                    List<CashSalesDocNotes> NoteLines = _mapper.Map<List<CashSalesNoteViewModel>, List<CashSalesDocNotes>>(CashSalesObj.NoteLines);
                    List<CashSalesDocPays> PayLines = _mapper.Map<List<CashSalesPayViewModel>, List<CashSalesDocPays>>(CashSalesObj.PayLines);

                    CashSalesHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string CSDocNum = string.Empty;
                    if (!i_CashSalesDocH_Repository.AddCS(CashSalesHeader, Lines, NoteLines, PayLines,ref ValidationMessage, ref CSDocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        CashSalesObj.DocNum = CSDocNum;
                        CashSalesObj.DocEntry = long.Parse(ValidationMessage);
                    }

                }
                else
                {
                    CashSalesDocH CashSalesHeader = i_CashSalesDocH_Repository.GetByDocNumber(CashSalesObj.DocNum);

                    if (CashSalesHeader != null)
                    {
                       
                        if (CashSalesObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(CashSalesHeader.DocNum.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CashSalesHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if(CashSalesHeader.SyncedToSAP == true)
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
                        else if (CashSalesHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            CashSalesHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            CashSalesHeader.SubmittedBy = User.Identity.Name;
                            if (!i_CashSalesDocH_Repository.ResubmitCSToSAP(CashSalesHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = CashSalesHeader.DocEntry;
                            CashSalesHeader = _mapper.Map<CashSalesViewModel, CashSalesDocH>(CashSalesObj);
                            var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CashSalesHeader.CardCode);
                            //var CashCustomerObject = i_CashSalesCustomer_Repository.GetByTelephoneNo(CashSalesHeader.OfficeTelNo);

                            CashSalesHeader.CardName = ResultCustomerObject.CardName;
                            //CashSalesHeader.CashSalesCardName = CashCustomerObject.CustomerName;
                            CashSalesHeader.SlpCode = ResultCustomerObject.SlpCode;
                            CashSalesHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;                            

                            CashSalesHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                            CashSalesHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                            CashSalesHeader.Status = (short)DocumentStatuses.Open;
                            if (CashSalesHeader.SubmittedToSAP.Equals(true))
                            {
                                CashSalesHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            else
                            {

                            }
                            CashSalesHeader.DocEntry = DocEntry;

                            List<CashSalesDocLs> Lines = _mapper.Map<List<CashSalesLineViewModel>, List<CashSalesDocLs>>(CashSalesObj.Lines);
                            List<CashSalesDocNotes> NoteLines = _mapper.Map<List<CashSalesNoteViewModel>, List<CashSalesDocNotes>>(CashSalesObj.NoteLines);
                            List<CashSalesDocPays> PayLines = _mapper.Map<List<CashSalesPayViewModel>, List<CashSalesDocPays>>(CashSalesObj.PayLines);
                            CashSalesHeader.UpdatedBy = User.Identity.Name;
                            string ValidationMessage = string.Empty;
                            string CSDocNum = string.Empty;


                            if (!i_CashSalesDocH_Repository.AddCS(CashSalesHeader, Lines, NoteLines, PayLines,ref ValidationMessage, ref CSDocNum))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                            else
                            {
                                CashSalesObj.DocNum = CSDocNum;
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
        public JsonResult GetCashSalesLastPriceHistory(string ItemCode, string CardCode)
        {
            IEnumerable<CashSalesDocLs> LastPriceHistory = i_CashSalesDocLs_Repository.GetCashSalesLinesByItemCodeWithLimit(ItemCode, CardCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.CashSalesDocH.DocDate).Select(e => new
            {
                DocDate = e.CashSalesDocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.CashSalesDocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }


    }
}