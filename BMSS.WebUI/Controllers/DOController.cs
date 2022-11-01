using AutoMapper;
using BMSS.Domain;
using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.Planner;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.DOViewModels;
using BMSS.WebUI.Models.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BMSS.WebUI.Models.UserIdentity;
using BMSS.WebUI.Infrastructure.Identity;
using System.Security.Claims;
using BMSS.WebUI.Helpers.HtmlHelpers;
using BMSS.WebUI.Models.ItemViewModels;

namespace BMSS.WebUI.Controllers
{
    [Authorize(Roles = "Delivery Order/Invoice")]
    public class DOController : Controller
    {

        private I_DODocH_Repository i_DODocH_Repository;
        private I_DODocLs_Repository i_DODocLs_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_OACT_Repository i_OACT_Repository;
        private I_OITW_Repository i_OITW_Repository;
        private I_ITT1_Repository i_ITT1_Repository;
        private I_DeliveryPlanner_Repository i_DeliveryPlanner_Repository;
         private I_OWHS_Repository i_OWHS_Repository;
        private I_POR1_Repository i_POR1_Repository;
        IMapper _mapper;
        public DOController(I_OWHS_Repository i_OWHS_Repository, I_ITT1_Repository i_ITT1_Repository, I_POR1_Repository i_POR1_Repository, I_DeliveryPlanner_Repository i_DeliveryPlanner_Repository, I_OITW_Repository i_OITW_Repository, I_OACT_Repository i_OACT_Repository, I_OCRD_Repository i_OCRD_Repository, I_DODocLs_Repository i_DODocLs_Repository, I_DODocH_Repository i_DODocH_Repository, I_OCTG_Repository i_OCTG_Repository, IMapper _mapper)
        {
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_OACT_Repository = i_OACT_Repository;
            this.i_DODocLs_Repository =  i_DODocLs_Repository;
            this._mapper = _mapper;
            this.i_OITW_Repository = i_OITW_Repository;
            this.i_DeliveryPlanner_Repository = i_DeliveryPlanner_Repository;
            this.i_ITT1_Repository = i_ITT1_Repository;
            this.i_OWHS_Repository = i_OWHS_Repository;
            this.i_POR1_Repository = i_POR1_Repository;
        }

        //[HttpPost]
        //[AjaxOnly]
        //// GET: Item
        //public JsonResult IndexNewPagining(DTPagination pagination)
        //{
        //    string searchValue = pagination.search.value;
        //    string orderBy = "";
        //    string orderByDirection = pagination.order[0].dir;

        //    switch (int.Parse(pagination.order[0].column))
        //    {
        //        case 1:
        //            orderBy = "Key.ItemName";
        //            break;
        //        //case 2:
        //        //    orderBy = "InStock";
        //        //    break;
        //        //case 3:
        //        //    orderBy = "InOrder";
        //        //    break;
        //        //case 4:
        //        //    orderBy = "InvntItem";
        //        //    break;
        //        default:
        //            orderBy = "Key.ItemCode";
        //            break;
        //    }

        //    string orderByColumn = $"{orderBy} {orderByDirection}";

        //    IEnumerable<IGrouping<OITM, OITW>> ItemStockDetails = i_OITW_Repository.GetItemStockDetailsWithPagination(pagination.start, pagination.length, searchValue, orderByColumn);

        //    IEnumerable<ItemStockDetailViewModel> ItemStockDetailsList = ItemStockDetails.Select(e => new ItemStockDetailViewModel
        //    {
        //        ItemCode = e.Key.ItemCode,
        //        ItemName = e.Key.ItemName,
        //        InvntItem = e.Key.InvntItem,
        //        InStock = e.Key.InvntItem == "Y" ? e.Sum(c => c.OnHand - c.IsCommited) : 0,
        //        InOrder = e.Key.InvntItem == "Y" ? e.Sum(c => c.OnOrder) : 0,
        //        IsCommited = e.Sum(c => c.IsCommited),
        //    }).ToArray();
        //    int TotalRecords = i_OITW_Repository.GetItemStockDetailsWithPaginationCount();


        //    foreach (ItemStockDetailViewModel item in ItemStockDetailsList)
        //    {
        //        decimal LocalAvailableQty = 0;
        //        decimal SAPPOQty = 0;
        //        decimal WebPOQty = 0;
        //        LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCode(item.ItemCode);
        //        SAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode);
        //        WebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode);


        //        IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(item.ItemCode);
        //        IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
        //        {
        //            ItemCode = e.Father,
        //            Quantity = e.Quantity
        //        }).ToList();

        //        foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
        //        {
        //            LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCode(ParentItem.ItemCode) * ParentItem.Quantity);
        //            SAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode) * ParentItem.Quantity);
        //            WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode) * ParentItem.Quantity);
        //        }

        //        item.InStock = item.InvntItem == "Y" ? (item.InStock + LocalAvailableQty) : 0;
        //        item.InOrder = item.InvntItem == "Y" ? (SAPPOQty + WebPOQty) : 0;
        //    }
        //    //if(decimal.TryParse(searchValue, out decimal searchValueDemimal))
        //    //{
        //    //    ItemStockDetailsList = ItemStockDetailsList.Where(x => x.InOrder == searchValueDemimal || x.InStock == searchValueDemimal).ToList();
        //    //}
        //    int FilteredTotalRecords = 0;
        //    if (String.IsNullOrEmpty(searchValue))
        //        FilteredTotalRecords = TotalRecords;
        //    else
        //        FilteredTotalRecords = ItemStockDetailsList.Count();
        //    //return Json(ItemStockDetailsList, JsonRequestBehavior.AllowGet);
        //    return Json(new
        //    {
        //        draw = pagination.draw,
        //        recordsTotal = TotalRecords,
        //        recordsFiltered = FilteredTotalRecords,
        //        data = ItemStockDetailsList
        //    }, JsonRequestBehavior.AllowGet);
        //}


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
            var listOfItems = i_DODocH_Repository.GetDODetailsWithPagination(pagination.start, pagination.length, searchValue, orderByColumn);

            var items = listOfItems.Select((x) => new DOListViewModel()
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
            
            int TotalRecords = i_DODocH_Repository.GetDOTotalCount();
            int FilteredTotalRecords = i_DODocH_Repository.GetDODetailsWithPaginationCount(searchValue);
            if (String.IsNullOrEmpty(searchValue))
                FilteredTotalRecords = TotalRecords;
             
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

            return View(i_DODocH_Repository.DOHeaderList);
        }
        public ActionResult DOPreview()
        {
            return View();
        }
        public ActionResult Add()
        {
            DOViewModel addDOViewModel = new DOViewModel()
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
            ViewBag.disabledStockLoan = User.IsInRole("Stock Loan") ? "" : "disabled";
            return View(addDOViewModel);
        }
        public ActionResult Edit(string DocNum)
        {
            DOViewModel addDOViewModel = new DOViewModel();

            DODocH DOHObject = i_DODocH_Repository.GetByDocNumber(DocNum);

            if (DOHObject != null)
            {
                addDOViewModel = _mapper.Map<DODocH, DOViewModel>(DOHObject);
                addDOViewModel.CardName = DOHObject.CardCode;
                addDOViewModel.Lines = new List<DOLineViewModel>();
                addDOViewModel.NoteLines = new List<DONoteViewModel>();

                foreach (var entry in DOHObject.DODocLs.OrderBy(x => x.LineNum))
                {
                    DOLineViewModel dOLine = new DOLineViewModel();
                    dOLine = _mapper.Map<DODocLs, DOLineViewModel>(entry);
                    addDOViewModel.Lines.Add(dOLine);
                }
                if (DOHObject.DODocNotes != null)
                {
                    foreach (var entry in DOHObject.DODocNotes.OrderBy(x => x.LineNum))
                    {
                        DONoteViewModel dONoteLine = new DONoteViewModel();
                        dONoteLine = _mapper.Map<DODocNotes, DONoteViewModel>(entry);
                        addDOViewModel.NoteLines.Add(dONoteLine);
                    }
                }
                addDOViewModel.DocDate = DOHObject.DocDate.ToString("dd'/'MM'/'yyyy");
                if(DOHObject.DeliveryDate.HasValue)
                addDOViewModel.DeliveryDate = DOHObject.DeliveryDate.Value.ToString("dd'/'MM'/'yyyy");
                addDOViewModel.DueDate = DOHObject.DueDate.ToString("dd'/'MM'/'yyyy");
                addDOViewModel.DiscByPercent = addDOViewModel.DiscByPercent.ToLower();
                addDOViewModel.CurrentUserIsNotInRoleNotes = !(User.IsInRole("Notes") && DOHObject.SyncedToSAP);
            }
            else
            {
                TempData["GlobalErrMsg"] = "Sales quotation not found";
                return RedirectToAction("Index");
            }
            ViewBag.disabledStockLoan = User.IsInRole("Stock Loan") ? "" : "disabled";

            return View("Add", addDOViewModel);
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
            DODocH DOHeader = i_DODocH_Repository.GetByDocNumber(DocNumber);
            return (DOHeader.INVPrintedCount >= 1 || DOHeader.DOPrintedCount>=1 ) && !User.IsInRole("Print After First Time");

        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Delivery Order/Invoice-Edit")]
        public JsonResult AddPlannerLine(DOPlannerRowViewModel DOPlannerLineObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            DateTime DDate = DateTime.Now;
           if (!DateTime.TryParseExact(DOPlannerLineObj.DeliveryDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DDate)) {
                ErrList.Add("Delivery Date is not in valid Format");
            }
            switch(DOPlannerLineObj.CreatorLocation)
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
                    if (!i_DODocH_Repository.NotePlannerSubmission(DeliveryPlannerObject.ReferenceNo, ref SentCount))
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
        [EditAuthorize(Roles = "Delivery Order/Invoice-Edit")]
        public JsonResult Add(DOViewModel DOObj)
        {
            IEnumerable<string> ModelErrList = Enumerable.Empty<string>();
            List<string> ErrList = new List<string>();
            if (DOObj.Lines == null || DOObj.Lines.Count() <= 0)
            {
                ErrList.Add("No lines added");

            }
            if (DOObj.SelfCollect.Equals(true))
            {
                if (DOObj.SelfCollectRemarks1 == null)
                {
                    ErrList.Add("Self Collect Remarks 1 is required");
                }
                else if (DOObj.SelfCollectRemarks1.Trim().Equals(""))
                {
                    ErrList.Add("Self Collect Remarks 1 is required");
                }
            }
            else
            {
                if (DOObj.ShipToAddress1 == null)
                {
                    ErrList.Add("Shipping Address Line 1 is required");
                }
                else if (DOObj.ShipToAddress1.Trim().Equals(""))
                {
                    ErrList.Add("Shipping Address Line 1 is required");
                }
            }
            if (ModelState.IsValid) { 

                for (int i = 0; i < DOObj.Lines.Count(); i++)
                {
                    decimal LocalAvailableQty = 0;
                    string CurrentItemCode = DOObj.Lines[i].ItemCode;
                    string CurrentLocation = DOObj.Lines[i].Location;
                    string CurrentLocationText = DOObj.Lines[i].LocationText;
                    bool isLoanIssued = DOObj.Lines[i].LoanIssued;
                    LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(CurrentItemCode, CurrentLocation);
                    IEnumerable<OITW> ItemStockDetails = i_OITW_Repository.GetLocationStockDetails(CurrentItemCode);
                    string IsInventoryItem = ItemStockDetails.Select(x => x.Item.InvntItem).FirstOrDefault();
                    string isUnitCostRequired = ItemStockDetails.Select(x => x.Item.U_ALLOW_UNIT_COST_ZERO).FirstOrDefault();
                    if (IsInventoryItem.Equals("Y") && isLoanIssued.Equals(false))
                    {
                        decimal SAPAvailableQty = i_OITW_Repository.GetLocationStockAvailableQty(CurrentItemCode, CurrentLocation);

                        decimal OrderItemQty = DOObj.Lines.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => x.Qty);

                        decimal ExistingDOQty = 0;
                        if (!DOObj.DocNum.Equals("New"))
                        {
                            ExistingDOQty = i_DODocH_Repository.GetByDocNumber(DOObj.DocNum).DODocLs.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => (decimal?)x.Qty) ?? 0;
                        }

                        IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(CurrentItemCode);
                        IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
                        {
                            ItemCode = e.Father,
                            Quantity = e.Quantity
                        }).ToList();

                       // decimal ParentItemQuantity = 0;
                        foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
                        {
                            LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, CurrentLocation) * ParentItem.Quantity);
                            SAPAvailableQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, CurrentLocation) * ParentItem.Quantity);
                            //WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, CurrentLocation) * ParentItem.Quantity);
                           // ParentItemQuantity = ParentItem.Quantity;
                        }



                        decimal TotalAvailableQty = (LocalAvailableQty + SAPAvailableQty + ExistingDOQty);

                       // OrderItemQty = OrderItemQty * ParentItemQuantity;

                        if (TotalAvailableQty < OrderItemQty)
                        {
                            ErrList.Add("Item " + CurrentItemCode + " not have enought stock in " + CurrentLocationText + " warehouse in line no " + (i + 1));
                        }
                    }
                    else
                    {
                       IEnumerable<ITT1> childItems =  i_ITT1_Repository.GetChildItemList(CurrentItemCode);
                        if (childItems.Count() > 0)
                        {
                            decimal OrderItemQty = DOObj.Lines.Where(x => x.ItemCode.Equals(CurrentItemCode) && x.Location.Equals(CurrentLocation)).Sum(x => x.Qty);
                            
                            foreach (var item in childItems)
                            {
                                IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(item.Code);
                                IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
                                {
                                    ItemCode = e.Father,
                                    Quantity = e.Quantity
                                }).ToList();

                                decimal ChildLocalAvailableQty = 0;
                                decimal TotalAvailableQty = 0;
                                decimal OrderedQty = OrderItemQty;
                                //ChildLocalAvailableQty = (LocalAvailableQty * item.Quantity);
                                ChildLocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(item.Code, CurrentLocation);
                                decimal SAPAvailableQty = i_OITW_Repository.GetLocationStockAvailableQty(item.Code, CurrentLocation);
                                //SAPAvailableQty = (SAPAvailableQty * item.Quantity);
                               
                               
                                decimal ExistingDOQty = 0;
                                if (!DOObj.DocNum.Equals("New"))
                                {
                                    ExistingDOQty = i_DODocH_Repository.GetByDocNumber(DOObj.DocNum).DODocLs.Where(x => x.ItemCode.Equals(item.Code) && x.Location.Equals(CurrentLocation)).Sum(x => (decimal?)x.Qty) ?? 0;
                                }

                                //parent Item calculation
                                foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
                                {
                                    ChildLocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, CurrentLocation) * ParentItem.Quantity);
                                    SAPAvailableQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, CurrentLocation) * ParentItem.Quantity);
                                   // WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                                }


                                //ExistingDOQty = (ExistingDOQty * item.Quantity);
                                //TotalAvailableQty += (ChildLocalAvailableQty + SAPAvailableQty +  ExistingDOQty) /item.Quantity;                                
                                // TotalAvailableQty += (ChildLocalAvailableQty + SAPAvailableQty + ExistingDOQty) * item.Quantity;
                                TotalAvailableQty += (ChildLocalAvailableQty + SAPAvailableQty + ExistingDOQty);

                                OrderedQty = OrderedQty * item.Quantity;

                                if (TotalAvailableQty < OrderedQty)
                                {
                                    ErrList.Add("Child Item " + childItems.First().Code + "  not have enought stock in " + CurrentLocationText + " warehouse in line no " + (i + 1));
                                }                                
                            }
                           
                        }
                    }
                    if(isUnitCostRequired != null && isUnitCostRequired == "N")
                    {
                        if(DOObj.Lines[i].UnitCost <=0)
                            ErrList.Add("For item " + CurrentItemCode + ", Unit Cost should be greater than zero in line no " + (i + 1));
                    }
                    
                }
            }
            if (ModelState.IsValid && ErrList.Count() <= 0)
            {
                if (DOObj.DocNum.Equals("New") && DOObj.UpdateInfo.Equals(false))
                {
                    DODocH DeliveryOrderHeader = new DODocH();
                    DeliveryOrderHeader = _mapper.Map<DOViewModel, DODocH>(DOObj);
                    var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(DeliveryOrderHeader.CardCode);

                   
                    DeliveryOrderHeader.CardName = ResultCustomerObject.CardName;
                    DeliveryOrderHeader.SlpCode = ResultCustomerObject.SlpCode;
                    DeliveryOrderHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                    DeliveryOrderHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                    DeliveryOrderHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                    DeliveryOrderHeader.Status = (short)DocumentStatuses.Open;

                    if(DeliveryOrderHeader.SubmittedToSAP.Equals(true))
                    {
                        DeliveryOrderHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                    }
                    else
                    {

                    }

                    List<DODocLs> Lines = _mapper.Map<List<DOLineViewModel>, List<DODocLs>>(DOObj.Lines);
                    List<DODocNotes> NoteLines = _mapper.Map<List<DONoteViewModel>, List<DODocNotes>>(DOObj.NoteLines);
                    DeliveryOrderHeader.CreatedBy = User.Identity.Name;
                    string ValidationMessage = string.Empty;
                    string DODocNum = "";

                    decimal TotalDocBalance = i_DODocH_Repository.GetTotalSystemBalanceByCardCode(ResultCustomerObject.CardCode);
                    ResultCustomerObject.Balance = TotalDocBalance + DeliveryOrderHeader.GrandTotal;
                    if((ResultCustomerObject.CreditLine - ResultCustomerObject.Balance)<=0)
                    {
                        ErrList.Add("Exceeds Credit limit, cannot Proceed");
                    }
                    else if (!i_DODocH_Repository.AddDO(DeliveryOrderHeader, Lines, NoteLines,ref ValidationMessage, ref DODocNum))
                    {
                        ErrList.Add(ValidationMessage);
                    }
                    else
                    {
                        DOObj.DocNum = DODocNum;
                        DOObj.DocEntry = long.Parse(ValidationMessage);
                    }

                }
                else
                {
                    DODocH DeliveryOrderHeader = i_DODocH_Repository.GetByDocNumber(DOObj.DocNum);

                    if (DeliveryOrderHeader != null)
                    {
                       
                        if(DOObj.SubmittedToSAP == true && !User.IsInRole("Submit To SAP"))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (CannotPrintAndSave(DeliveryOrderHeader.DocNum.ToString()))
                        {
                            ErrList.Add("Access denied");
                        }
                        else if (DeliveryOrderHeader.SubmittedToSAP == true)
                        {
                            ErrList.Add("This document already submitted to SAP");
                        }
                        else if(DeliveryOrderHeader.HaveStockLoan == true && DOObj.UpdateInfo.Equals(false))
                        {
                            ErrList.Add("This document have pending stock loan, cannot proceed");
                        }
                        else if(DeliveryOrderHeader.HaveStockLoan == false && DOObj.UpdateInfo.Equals(true)) {

                            ErrList.Add("This document don't have pending stock loan, cannot proceed");

                        }
                        else if(DeliveryOrderHeader.SyncedToSAP == true)
                        {
                            ErrList.Add("This document already synced to SAP");
                        }
                        else if(DeliveryOrderHeader.SyncStatus.Equals((short)SyncStatuses.Sync_Failed))
                        {
                            string ValidationMessage = string.Empty;
                            DeliveryOrderHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            DeliveryOrderHeader.SubmittedBy = User.Identity.Name;
                            if (!i_DODocH_Repository.ResubmitDOToSAP(DeliveryOrderHeader, ref ValidationMessage))
                            {
                                ErrList.Add(ValidationMessage);
                            }
                        }
                        else
                        {
                            long DocEntry = DeliveryOrderHeader.DocEntry;
                            decimal PreviousBalance = DeliveryOrderHeader.GrandTotal;
                            DeliveryOrderHeader = _mapper.Map<DOViewModel, DODocH>(DOObj);
                            var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(DeliveryOrderHeader.CardCode);
                            DeliveryOrderHeader.CardName = ResultCustomerObject.CardName;
                            DeliveryOrderHeader.SlpCode = ResultCustomerObject.SlpCode;
                            DeliveryOrderHeader.SlpName = ResultCustomerObject.SalesPerson.SlpName;
                            DeliveryOrderHeader.PaymentTerm = ResultCustomerObject.PaymentTerm.GroupNum;
                            DeliveryOrderHeader.PaymentTermName = ResultCustomerObject.PaymentTerm.PymntGroup;
                            DeliveryOrderHeader.Status = (short)DocumentStatuses.Open;
                            DeliveryOrderHeader.DocEntry = DocEntry;
                            if (DeliveryOrderHeader.SubmittedToSAP.Equals(true))
                            {
                                DeliveryOrderHeader.SyncStatus = (short)SyncStatuses.Waiting_For_Syncing;
                            }
                            else
                            {

                            }
                            List<DODocLs> Lines = _mapper.Map<List<DOLineViewModel>, List<DODocLs>>(DOObj.Lines);
                            List<DODocNotes> NoteLines = _mapper.Map<List<DONoteViewModel>, List<DODocNotes>>(DOObj.NoteLines);
                            DeliveryOrderHeader.UpdatedBy = User.Identity.Name;
                            string ValidationMessage = string.Empty;
                            string DODocNum = "";
                            decimal TotalDocBalance = i_DODocH_Repository.GetTotalSystemBalanceByCardCode(ResultCustomerObject.CardCode);
                            ResultCustomerObject.Balance = TotalDocBalance + DeliveryOrderHeader.GrandTotal - PreviousBalance;
                            if ((ResultCustomerObject.CreditLine - ResultCustomerObject.Balance) <= 0)
                            {
                                ErrList.Add("Exceeds Credit limit, cannot Proceed");
                            }
                            else if (!i_DODocH_Repository.AddDO(DeliveryOrderHeader, Lines, NoteLines, ref ValidationMessage, ref DODocNum, DOObj.UpdateInfo))
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
                
                DOObj.IsModelValid = ModelState.IsValid;
                ModelErrList = Helpers.Functions.HelperFunctions.GetErrorListFromModelState(ViewData.ModelState);
                DOObj.ModelErrList = new List<string>();
                if (ModelErrList.Count() > 0)
                {

                    foreach (var entry in ModelErrList)
                    {
                        DOObj.ModelErrList.Add(entry);
                    }
                }
                if (ErrList.Count() > 0)
                {
                    DOObj.IsModelValid = false;
                    foreach (var entry in ErrList)
                    {
                        DOObj.ModelErrList.Add(entry);
                    }
                }
            }            
            return Json(DOObj, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [AjaxOnly]
        [EditAuthorize(Roles = "Delivery Order/Invoice-Edit")]
        public JsonResult UpdateNotes(DOViewModel DOObj)
        {
            DOObj.ModelErrList = new List<string>();
            if (DOObj.NoteLines != null)
            {
                DODocH DeliveryOrderHeader = i_DODocH_Repository.GetByDocNumber(DOObj.DocNum);

                if (DeliveryOrderHeader != null)
                {
                    List<DODocNotes> NoteLines = _mapper.Map<List<DONoteViewModel>, List<DODocNotes>>(DOObj.NoteLines);
                    if (DOObj.NoteLines.Where(x => x.Note == null || x.Note.Trim() == string.Empty).Count() > 0)
                    {

                        DOObj.IsModelValid = false;
                        DOObj.ModelErrList.Add("Note Requied");

                    }
                    else
                    {                                           
                        string validationMessage = string.Empty;
                        if (!i_DODocH_Repository.UpdateNotes(DeliveryOrderHeader, NoteLines, ref validationMessage))
                        {
                            DOObj.IsModelValid = false;
                            DOObj.ModelErrList.Add(validationMessage);
                        }

                    }
                }
                else
                {
                    DOObj.IsModelValid = false;
                    DOObj.ModelErrList.Add("Document not found");
                }
                return Json(DOObj, JsonRequestBehavior.DenyGet);
            }
            else{

                DOObj.IsModelValid = false;
                DOObj.ModelErrList.Add("Notes Requied");
                return Json(DOObj, JsonRequestBehavior.DenyGet);
            }

        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetDOLastPriceHistory(string ItemCode,string CardCode)
        {
            IEnumerable<DODocLs> LastPriceHistory = i_DODocLs_Repository.GetDOLinesByItemCodeWithLimit(ItemCode, CardCode);
            var Result = LastPriceHistory.OrderByDescending(x => x.DODocH.DocDate).Select(e => new
            {
                DocDate = e.DODocH.DocDate.ToString("dd'/'MM'/'yyyy"),
                e.DODocH.DocNum,
                e.Qty,
                e.UnitPrice
            }).ToList();
            return Json(Result, JsonRequestBehavior.DenyGet);

        }

    }
}