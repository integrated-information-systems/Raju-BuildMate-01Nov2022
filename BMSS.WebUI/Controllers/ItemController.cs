using BMSS.Domain.Abstract;
using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.ItemViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using BMSS.Domain;
using System.Globalization;

namespace BMSS.WebUI.Controllers
{
   
    [Authorize]
    public class ItemController : Controller
    {
        //Repositories
        private I_OITM_Repository i_OITM_Repository;
        private I_OITW_Repository i_OITW_Repository;
        private I_INV1_Repository i_INV1_Repository;
        private I_ITM1_Repository i_ITM1_Repository;
        private I_SPP2_Repository i_SPP2_Repository;
        private I_OADM_Repository i_OADM_Repository;
        private I_OINM_Repository i_OINM_Repository;
        private I_POR1_Repository i_POR1_Repository;
        private I_OCRD_Repository i_OCRD_Repository;
        private I_ITT1_Repository i_ITT1_Repository;
        private I_OPLN_Repository i_OPLN_Repository;

        private I_DODocH_Repository i_DODocH_Repository;
        private I_PODocLs_Repository i_PODocLs_Repository;
        private I_InventoryMovement_Repository i_InventoryMovement_Repository;
        public ItemController(I_InventoryMovement_Repository i_InventoryMovement_Repository, 
            I_ITT1_Repository i_ITT1_Repository, 
            I_OCRD_Repository i_OCRD_Repository, 
            I_PODocLs_Repository i_PODocLs_Repository, 
            I_POR1_Repository i_POR1_Repository, 
            I_DODocH_Repository i_DODocH_Repository, 
            I_OINM_Repository i_OINM_Repository, 
            I_OADM_Repository i_OADM_Repository, 
            I_OITM_Repository i_OITM_Repository, 
            I_OITW_Repository i_OITW_Repository, 
            I_INV1_Repository i_INV1_Repository, 
            I_ITM1_Repository i_ITM1_Repository, 
            I_SPP2_Repository i_SPP2_Repository, 
            I_OPLN_Repository i_OPLN_Repository)
        {
            this.i_OITM_Repository = i_OITM_Repository;
            this.i_OITW_Repository = i_OITW_Repository;
            this.i_INV1_Repository = i_INV1_Repository;
            this.i_ITM1_Repository = i_ITM1_Repository;
            this.i_SPP2_Repository = i_SPP2_Repository;
            this.i_OADM_Repository = i_OADM_Repository;
            this.i_OINM_Repository = i_OINM_Repository;
            this.i_POR1_Repository = i_POR1_Repository;
            this.i_DODocH_Repository = i_DODocH_Repository;
            this.i_PODocLs_Repository = i_PODocLs_Repository;
            this.i_OCRD_Repository = i_OCRD_Repository;
            this.i_ITT1_Repository = i_ITT1_Repository;
            this.i_OPLN_Repository = i_OPLN_Repository;
            this.i_InventoryMovement_Repository = i_InventoryMovement_Repository;
        }


        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemMthlySales(string ItemCode)
        {
            //ItemCode = "A01-0011G0";
            var ResultList = i_OITM_Repository.GetItemMthlySales(ItemCode);
            return Json(ResultList, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        [AjaxOnly]
        // GET: Item
        public JsonResult IndexNewPagining(DTPagination pagination, string WhsCode)
        {
            string searchValue = pagination.search.value;
            string orderBy = "";
            string orderByDirection = pagination.order[0].dir;
          
            switch (int.Parse(pagination.order[0].column))
            {
                case 1:
                    orderBy = "ItemCode";
                    break;
                case 2:
                    orderBy = "ItemName";
                    break;
                //case 2:
                //    orderBy = "InStock";
                //    break;
                //case 3:
                //    orderBy = "InOrder";
                //    break;
                //case 4:
                //    orderBy = "InvntItem";
                //    break;
                default:
                    orderBy = "ItemCode";
                    break;
            }

            string orderByColumn = $"{orderBy} {orderByDirection}";

            // WhsCode = "ALL";
            var ResultList = i_OITM_Repository.GetListOfStocks(WhsCode);
            //List<ItemStockDetailViewModelV1> Filterdata =  new List<ItemStockDetailViewModelV1>();
            var Filterdata = ResultList;

            if (!string.IsNullOrEmpty(searchValue))
            {
                Filterdata = ResultList.Where(x =>
                       x.ItemCode.ToString().ToUpper().Contains(searchValue.ToUpper())
                       ||
                       x.ItemName.ToUpper().Contains(searchValue.ToUpper()))
                        .Skip(pagination.start).Take(pagination.length).ToList();
            }
            else
            {
                Filterdata = ResultList
                    .Skip(pagination.start).Take(pagination.length).ToList();
            }

            if (orderByDirection == "desc")
            {
                var propertyname = typeof(Domain.Models.ListofStcoks).GetProperty(orderBy);
                Filterdata = Filterdata.OrderByDescending(x => propertyname.GetValue(x, null)).ToList();
            }
            else
            {
                var propertyname = typeof(Domain.Models.ListofStcoks).GetProperty(orderBy);
                Filterdata = Filterdata.OrderBy(x => propertyname.GetValue(x, null)).ToList();
            }

            IEnumerable<ItemStockDetailViewModelV1> ItemStockDetailsListV1 = Filterdata.Select(e => new ItemStockDetailViewModelV1
            {
                ItemCode = e.ItemCode,
                ItemName = e.ItemName,
                WhsCode = e.WhsCode,
                onhand = e.onhand,
                isCommited = e.isCommited,
                DraftGoodsReceipt = e.DraftGoodsReceipt,
                DraftGoodsIssue = e.DraftGoodsIssue,
                DraftCreditNote = e.DraftCreditNote,
                SOAvailable = e.SOAvailable,
                onOrder = e.onOrder              

            });
            int TotalRecords = ResultList.Count();




            //IEnumerable<IGrouping<OITM, OITW>> ItemStockDetails = i_OITW_Repository.GetItemStockDetailsWithPagination(pagination.start, pagination.length, searchValue, orderByColumn);

            //IEnumerable<ItemStockDetailViewModel> ItemStockDetailsList = ItemStockDetails.Select(e => new ItemStockDetailViewModel
            //{
            //    ItemCode = e.Key.ItemCode,
            //    ItemName = e.Key.ItemName,
            //    InvntItem = e.Key.InvntItem,
            //    InStock = e.Key.InvntItem == "Y" ? e.Sum(c => c.OnHand - c.IsCommited) : 0,
            //    InOrder = e.Key.InvntItem == "Y" ? e.Sum(c => c.OnOrder) : 0,
            //    TRSInStock = e.Key.InvntItem == "Y" ? e.Where(y => y.WhsCode == "TRS").Sum(c => c.OnHand - c.IsCommited) : 0,
            //    TRSInOrder = e.Key.InvntItem == "Y" ? e.Where(y => y.WhsCode == "TRS").Sum(c => c.OnOrder): 0,
            //    ENSInStock = e.Key.InvntItem == "Y" ? e.Where(y => y.WhsCode == "ENS").Sum(c => c.OnHand - c.IsCommited) : 0,
            //    ENSInOrder = e.Key.InvntItem == "Y" ? e.Where(y => y.WhsCode == "ENS").Sum(c => c.OnOrder) : 0,
            //    IsCommited = e.Sum(c => c.IsCommited),     
            //    WhsCode  = e.GroupBy(x=> x.WhsCode).Select(x=> x.Key.ToString()).First()


            //}).ToArray();
            //int TotalRecords = i_OITW_Repository.GetItemStockDetailsWithPaginationCount();





            /**** Old Code *****/

            //foreach (ItemStockDetailViewModelV1 item in ItemStockDetailsListV1)
            //{
            //    decimal LocalAvailableQty = 0;
            //    decimal SAPPOQty = 0;
            //    decimal WebPOQty = 0;
            //    decimal TRSLocalAvailableQty = 0;
            //    decimal TRSSAPPOQty = 0;
            //    decimal TRSWebPOQty = 0;
            //    decimal ENSLocalAvailableQty = 0;
            //    decimal ENSSAPPOQty = 0;
            //    decimal ENSWebPOQty = 0;
            //    LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(item.ItemCode);
            //    SAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode);
            //    WebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode);

            //    TRSLocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(item.ItemCode, "TRS");
            //    TRSSAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode, "TRS");
            //    TRSWebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode, "TRS");

            //    ENSLocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(item.ItemCode, "ENS");
            //    ENSSAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode, "ENS");
            //    ENSWebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode, "ENS");




            //    IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(item.ItemCode);
            //    IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
            //    {
            //        ItemCode = e.Father,
            //        Quantity = e.Quantity
            //    }).ToList();

            //    foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
            //    {
            //        LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode) * ParentItem.Quantity);
            //        SAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode) * ParentItem.Quantity);
            //        WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode) * ParentItem.Quantity);


            //        TRSLocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, "TRS") * ParentItem.Quantity);
            //        TRSSAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, "TRS") * ParentItem.Quantity);
            //        TRSWebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, "TRS") * ParentItem.Quantity);

            //        ENSLocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, "ENS") * ParentItem.Quantity);
            //        ENSSAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, "ENS") * ParentItem.Quantity);
            //        ENSWebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, "ENS") * ParentItem.Quantity);

            //    }

            //    //item.InStock = item.InvntItem == "Y" ? (item.InStock + LocalAvailableQty) : 0;
            //    //item.InOrder = item.InvntItem == "Y" ? (SAPPOQty + WebPOQty) : 0;

            //    //item.TRSInStock = item.InvntItem == "Y" ? (item.TRSInStock + TRSLocalAvailableQty) : 0;
            //    //item.TRSInOrder = item.InvntItem == "Y" ? (TRSSAPPOQty + TRSWebPOQty) : 0;

            //    //item.ENSInStock = item.InvntItem == "Y" ? (item.ENSInStock + ENSLocalAvailableQty) : 0;
            //    //item.ENSInOrder = item.InvntItem == "Y" ? (ENSSAPPOQty + ENSWebPOQty) : 0;

            //}
            
            
            /**** Old Code *****/
            int FilteredTotalRecords = 0;
            if (String.IsNullOrEmpty(searchValue))
                FilteredTotalRecords = TotalRecords;
            else
                FilteredTotalRecords = ItemStockDetailsListV1.Count();
          
            return Json(new
            {
                draw = pagination.draw,
                recordsTotal = TotalRecords,
                recordsFiltered = FilteredTotalRecords,
                data = ItemStockDetailsListV1
            }, JsonRequestBehavior.AllowGet);

        }



        [Authorize(Roles = "Stocks")]
        // GET: Item
        public ActionResult Indexnew()
        {
            IEnumerable<WareHouseDetails> wareHouseDetails = i_OITM_Repository.GetWareHouses().Select(e => new WareHouseDetails
            {
                WhsCode = e.WhsCode,
                WhsName = e.WhsName,               
            }).ToList();

            ViewBag.wareHouseDetails = wareHouseDetails;

            return View();
        }
        [Authorize(Roles = "Stocks")]
        // GET: Item
        public ActionResult Index()
        {
           IEnumerable<OITW> ItemStockDetails =   i_OITW_Repository.GetItemStockDetails();

           IEnumerable<ItemStockDetailViewModel> ItemStockDetailsList = ItemStockDetails.GroupBy(l => l.Item).Select(e => new ItemStockDetailViewModel
            {
                ItemCode = e.Key.ItemCode,
                ItemName = e.Key.ItemName,
                InvntItem = e.Key.InvntItem,
                InStock = e.Key.InvntItem == "Y" ? e.Sum(c => c.OnHand - c.IsCommited):0 ,
                InOrder =  e.Key.InvntItem == "Y" ? e.Sum(c => c.OnOrder):0,
                IsCommited = e.Sum(c => c.IsCommited),
            }).ToList();

             
            foreach (ItemStockDetailViewModel item in ItemStockDetailsList)
            {
                decimal LocalAvailableQty = 0;
                decimal SAPPOQty = 0;
                decimal WebPOQty = 0;
                LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(item.ItemCode);
                SAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode);
                WebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(item.ItemCode);


               


                item.InStock = item.InvntItem == "Y" ? (item.InStock + LocalAvailableQty):0;
                item.InOrder = item.InvntItem == "Y" ? (SAPPOQty + WebPOQty):0;
            }
                return View(ItemStockDetailsList);
        }
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetActiveItems()
        {
            var ResultObject = i_OITM_Repository.Items.Select(e => new SelectListItem
            {
                Text = e.ItemName,
                Value = e.ItemCode
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetLocationStockQty(string ItemCode)
        {
            //Stock qty by location
            IEnumerable<OITW> LocationStockQtyItems = i_OITW_Repository.GetLocationStockDetails(ItemCode);
            string IsInventoryItem = LocationStockQtyItems.Select(x => x.Item.InvntItem).FirstOrDefault();
            if (!string.IsNullOrEmpty(IsInventoryItem) && IsInventoryItem.Equals("Y"))
            {
                IEnumerable<LocationStockViewModel> LocationStockQty = LocationStockQtyItems.Select(e => new LocationStockViewModel
                {
                    WarhouseCode = e.Warehouse.WhsCode,
                    WarhouseName = e.Warehouse.WhsName,
                    AvailableQty = (e.OnHand - e.IsCommited),
                    OnOrder = 0
                }).ToList();
                foreach (LocationStockViewModel item in LocationStockQty)
                {
                    decimal LocalAvailableQty = 0;
                    decimal SAPPOQty = 0;
                    decimal WebPOQty = 0;
                    LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ItemCode, item.WarhouseCode);
                    SAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ItemCode, item.WarhouseCode);
                    WebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ItemCode, item.WarhouseCode);

                    IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(ItemCode);
                    IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
                    {
                        ItemCode = e.Father,
                        Quantity = e.Quantity
                    }).ToList();

                    foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
                    {
                        LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                        SAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                        WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                    }


                    item.AvailableQty = item.AvailableQty + LocalAvailableQty;
                    item.OnOrder = SAPPOQty + WebPOQty;




                }
                return Json(LocationStockQty, JsonRequestBehavior.DenyGet);
            }
            else
            {
                IEnumerable<LocationStockViewModel> LocationStockQty = LocationStockQtyItems.Select(e => new LocationStockViewModel
                {
                    WarhouseCode = e.Warehouse.WhsCode,
                    WarhouseName = e.Warehouse.WhsName,
                    AvailableQty = 0,
                    OnOrder = 0
                }).ToList();
                return Json(LocationStockQty, JsonRequestBehavior.DenyGet);
            }
            
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetChildItems(string ParentItemCode)
        {
            //Stock qty by location
            IEnumerable<ITT1> ChildItemList = i_ITT1_Repository.GetChildItemList(ParentItemCode);
            IEnumerable<ChileItemLocationStockViewModel> ChildItemStockList = ChildItemList.Select(e => new ChileItemLocationStockViewModel
            {
                ItemCode = e.Code,
                Quantity = e.Quantity
            }).ToList();
            foreach (ChileItemLocationStockViewModel ChildItem in ChildItemStockList)
            {
                IEnumerable<OITW> LocationStockQtyItems = i_OITW_Repository.GetLocationStockDetails(ChildItem.ItemCode);
                IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(ChildItem.ItemCode);
                IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
                {
                    ItemCode = e.Father,
                    Quantity = e.Quantity
                }).ToList();
                IEnumerable<LocationStockViewModel> LocationStockQty = LocationStockQtyItems.Select(e => new LocationStockViewModel
                {
                    WarhouseCode = e.Warehouse.WhsCode,
                    WarhouseName = e.Warehouse.WhsName,
                    AvailableQty = (e.OnHand - e.IsCommited),
                    OnOrder = 0
                }).ToList();
                ChildItem.ItemName = LocationStockQtyItems.Select(e => e.Item.ItemName).FirstOrDefault();
                foreach (LocationStockViewModel item in LocationStockQty)
                {
                    decimal LocalAvailableQty = 0;
                    decimal SAPPOQty = 0;
                    decimal WebPOQty = 0;
                  

                    LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ChildItem.ItemCode, item.WarhouseCode);
                    //parent Item calculation
                    //LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItemCode, item.WarhouseCode) * ChildItem.Quantity);
                   

                    SAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ChildItem.ItemCode, item.WarhouseCode);
                    //parent Item calculation
                   // SAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItemCode, item.WarhouseCode) * ChildItem.Quantity);
                    WebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ChildItem.ItemCode, item.WarhouseCode);
                    //parent Item calculation
                    // WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItemCode, item.WarhouseCode) * ChildItem.Quantity);


                    //parent Item calculation
                    foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
                    {
                        LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                        SAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                        WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                    }

                    item.AvailableQty = item.AvailableQty + LocalAvailableQty;
                    item.OnOrder = SAPPOQty + WebPOQty;
                }
                ChildItem.StockDetails = LocationStockQty;

            }
            
            return Json(ChildItemStockList, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemDetails(string ItemCode)
        {
            string DefaultTax = ConfigurationManager.AppSettings["DefaultOutgoingTax"];

            if (ItemCode==null)
            {
                var OADMResultObject = new
                {
                    DefaultTax,
                    Weight = 0.0M
                };
                return Json(OADMResultObject, JsonRequestBehavior.AllowGet);
            }
            else if(!ItemCode.Equals(""))
            {            
                var ResultObject = i_OITM_Repository.Items.Where(x => x.ItemCode.Equals(ItemCode)).Select(e => new
                {
                    //DefaultTax = e.VatGourpSa,
                    DefaultTax,
                    Weight = string.IsNullOrEmpty(e.U_unitwt) ? 0M :  decimal.Parse(e.U_unitwt)

                }).FirstOrDefault();

                var ItemWeight = ResultObject.Weight;                
                if (ResultObject.DefaultTax == null)
                {
                    var DefaultTaxDetermination = i_OADM_Repository.CompanyDefaults.DfSVatItem;
                    var OADMResultObject = new
                    {
                        //DefaultTax = DefaultTaxDetermination,
                        DefaultTax,
                        Weight = ItemWeight
                    };

                    return Json(OADMResultObject, JsonRequestBehavior.DenyGet);
                }                 
                return Json(ResultObject, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var OADMResultObject = new
                {
                    DefaultTax,
                    Weight = 0
                };
                return Json(OADMResultObject, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemUnitPrice(string ItemCode, string CardCode)
        {
            decimal ItemAverageCost = 0;
            if (ItemCode != null)
            {
                if (!ItemCode.Equals("")) { 
                    OITM Itemobj = i_OITM_Repository.GetItemDetails(ItemCode);
                    if (Itemobj != null)
                    {
                        ItemAverageCost = Itemobj.AvgPrice;
                    }
                }
            }
                if (ItemCode == null || CardCode == null)
            {
                var OADMResultObject = new
                {
                    UnitPrice = 0,
                    UnitCost = ItemAverageCost
                };
                return Json(OADMResultObject, JsonRequestBehavior.AllowGet);
            }
            else if (!ItemCode.Equals("") && !CardCode.Equals(""))
            {
               
                var ResultCustomerObject = i_OCRD_Repository.GetCustomerDetails(CardCode);
                if (ResultCustomerObject == null)
                {
                    ResultCustomerObject = i_OCRD_Repository.GetSupplierDetails(CardCode);
                }
                if (ResultCustomerObject == null)
                {
                    var PriceResultObject = new
                    {
                        UnitPrice = 0,
                        UnitCost = ItemAverageCost
                    };
                    return Json(PriceResultObject, JsonRequestBehavior.DenyGet);
                }
                else
                {

                    var CustomerPriceList = ResultCustomerObject.ListNum;
                    IEnumerable<ITM1> ItemPrices = i_ITM1_Repository.GetItemPrices(ItemCode);
                    ITM1 ItemPrice = ItemPrices.Where(x => x.PriceList.Equals(CustomerPriceList)).FirstOrDefault();

                    if (ItemPrice == null)
                    {
                        var PriceResultObject = new
                        {
                            UnitPrice = 0,
                            UnitCost = ItemAverageCost
                        };
                        return Json(PriceResultObject, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        var PriceResultObject = new
                        {
                            UnitPrice = ItemPrice.Price,
                            UnitCost = ItemAverageCost
                        };
                        return Json(PriceResultObject, JsonRequestBehavior.DenyGet);
                    }
                }
            }
            else
            {
                var PriceResultObject = new
                {
                    UnitPrice = 0,
                    UnitCost = 0
                };
                return Json(PriceResultObject, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemSpecialPrice(string ItemCode, string CardCode, decimal Quantity)
        {
            decimal ItemSpecialPrice = 0;
            if (ItemCode != null 
                && 
                !ItemCode.Equals("")
                &&
                CardCode != null
                &&
                !CardCode.Equals("")
               )
            {
                ItemSpecialPrice =  i_SPP2_Repository.GetSpecialPriceByItemCustomerQuantity(ItemCode, CardCode, Quantity);

                return Json(ItemSpecialPrice, JsonRequestBehavior.DenyGet);
            }
                return Json(ItemSpecialPrice, JsonRequestBehavior.DenyGet);
        }
        //// New version don't use cardcode
        //[HttpPost]
        //[AjaxOnly]
        //public JsonResult GetItemUnitPrice(string ItemCode, string CardCode)
        //{
        //    if (ItemCode == null)
        //    {
        //        var PriceResultObject = new
        //        {
        //            UnitPrice = 0,
        //        };
        //        return Json(PriceResultObject, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (!ItemCode.Equals(""))
        //    {
        //        OITM Itemobj = i_OITM_Repository.GetItemDetails(ItemCode);
        //        var PriceResultObject = new
        //        {
        //            UnitPrice = Itemobj.AvgPrice,
        //        };
        //        return Json(PriceResultObject, JsonRequestBehavior.DenyGet);
        //    }
        //    else
        //    {
        //        var PriceResultObject = new
        //        {
        //            UnitPrice = 0,
        //        };
        //        return Json(PriceResultObject, JsonRequestBehavior.DenyGet);
        //    }
        //}
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemUOMValues(string ItemCode)
        {
            List<SelectListItem> UomValues = new List<SelectListItem>();

            if (ItemCode != null)
            {

                if (!ItemCode.Equals(""))
                {             
                    var SalesUomName = i_OITM_Repository.Items.Where(x => x.ItemCode.Equals(ItemCode)).Select(e => new SelectListItem
                    {
                        Text = e.SalUnitMsr,
                        Value = "InventryUOM"
                    }).FirstOrDefault();
                    var InvntryUomName = i_OITM_Repository.Items.Where(x => x.ItemCode.Equals(ItemCode)).Select(e => new SelectListItem
                    {
                        Text = e.InvntryUom,
                        Value = "SalesUOM"
                    }).FirstOrDefault();

                    if (SalesUomName.Text != null)
                    {
                        UomValues.Add(SalesUomName);
                    }
                    if (InvntryUomName.Text != null)
                    {
                        UomValues.Add(InvntryUomName);
                    }
                }
                else
                {
                    return Json(UomValues, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(UomValues, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemDefaultWhs(string ItemCode)
        {

            //if (ItemCode !=null)
            //{
            //    if (!ItemCode.Equals(""))
            //    {



            //        var ResultObject = i_OITM_Repository.Items.Where(x => x.ItemCode.Equals(ItemCode)).Select(e => new
            //        {
            //            DefaultWhs = e.DfltWH

            //        }).FirstOrDefault();


            //        if (ResultObject.DefaultWhs == null)
            //        {
            //            var DefaultWarehouse = i_OADM_Repository.CompanyDefaults.DfltWhs;
            //            var OADMResultObject = new
            //            {
            //                DefaultWhs = DefaultWarehouse
            //            };

            //            return Json(OADMResultObject, JsonRequestBehavior.DenyGet);
            //        }
            //        return Json(ResultObject, JsonRequestBehavior.DenyGet);
            //    }
            //    else
            //    {
            //        var ResultObject = new
            //        {
            //            DefaultWhs = string.Empty

            //        };

            //        return Json(ResultObject, JsonRequestBehavior.DenyGet);
            //    }
            //}
            //else
            //{
            //    var ResultObject = new
            //    {
            //        DefaultWhs = string.Empty

            //    };

            //    return Json(ResultObject, JsonRequestBehavior.DenyGet);
            //}
            var ResultObject = new
            {
                DefaultWhs = string.Empty

            };

            return Json(ResultObject, JsonRequestBehavior.DenyGet);
        }
        public ActionResult Detail(string ItemCode)
        {
            OITM itmObject = i_OITM_Repository.GetItemDetails(ItemCode);

            if (itmObject!=null)
            {
                if(itmObject.EvalSystem.Equals("A"))
                {
                    itmObject.EvalSystem = "Moving Average";
                }
                else if (itmObject.EvalSystem.Equals("S"))
                {
                    itmObject.EvalSystem = "Standard";
                }
                else if (itmObject.EvalSystem.Equals("F"))
                {
                    itmObject.EvalSystem = "FIFO";
                }
                //Stock qty by location
                IEnumerable<OITW> LocationStockQtyItems = i_OITW_Repository.GetLocationStockDetails(ItemCode);
                

                IEnumerable<LocationStockViewModel> LocationStockQty = LocationStockQtyItems.Select(e => new LocationStockViewModel
                {
                    WarhouseCode = e.Warehouse.WhsCode,
                    WarhouseName = e.Warehouse.WhsName,
                    AvailableQty = (e.OnHand - e.IsCommited),
                    OnOrder=0,
                    MinStock = e.U_minstockperwhs
                }).ToList();
                IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(ItemCode);
                IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
                {
                    ItemCode = e.Father,
                    Quantity = e.Quantity
                }).ToList();
                foreach (LocationStockViewModel item in LocationStockQty)
                {
                    decimal LocalAvailableQty = 0;
                    decimal SAPPOQty = 0;
                    decimal WebPOQty = 0;
                    LocalAvailableQty = i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ItemCode, item.WarhouseCode);
                    SAPPOQty = i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ItemCode, item.WarhouseCode);
                    WebPOQty = i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ItemCode, item.WarhouseCode);


                    foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
                    {
                        LocalAvailableQty += (i_DODocH_Repository.GetTotalSystemStockBalanceByItemCodeNew(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                        SAPPOQty += (i_POR1_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                        WebPOQty += (i_PODocLs_Repository.GetTotalPOStockBalanceByItemCode(ParentItem.ItemCode, item.WarhouseCode) * ParentItem.Quantity);
                    }

                    item.AvailableQty = item.AvailableQty + LocalAvailableQty;
                    item.OnOrder = SAPPOQty + WebPOQty;
                }
                ViewBag.LocationStockQty = LocationStockQty;
                ViewBag.TotalStock = LocationStockQty.Sum(a => a.AvailableQty);
                ViewBag.TotalMinumumStock = LocationStockQty.Sum(a => a.MinStock);
                ViewBag.TotalOnOrderStock = LocationStockQty.Sum(a => a.OnOrder);
                //Last price
                IEnumerable<INV1> InvoiceLines = i_INV1_Repository.GetInvoiceLines(ItemCode);
                IEnumerable<ItemLastPriceViewModel> LastPriceList = InvoiceLines.Select(e => new ItemLastPriceViewModel
                {
                    DocDate = e.InvoiceHeader.DocDate,
                    Price = e.Price,
                    Quantity = e.Quantity
                }).OrderByDescending(i => i.DocDate).ToList();

                ViewBag.LastPriceList = LastPriceList;

                //Price list band A to H
                IEnumerable<ITM1> ItemPrices = i_ITM1_Repository.GetItemPrices(ItemCode);
                IEnumerable<ItemPricesViewModel> ItemPriceList = ItemPrices.Select(e => new ItemPricesViewModel
                {
                    Currency = e.Currency,
                    PriceListName = e.PriceLists.ListName,
                    Price = e.Price
                }).OrderBy(i => i.PriceListName).ToList();

                ViewBag.ItemPriceList = ItemPriceList;

                // Ledger Information
                var myInClause = new string[] { "17", "14", "20", "21", "59", "60", "67" };
                List<OINM> LedgerInfo = i_OINM_Repository.GetTransactionDetailsByItemCode(ItemCode).Where(x => myInClause.Contains(x.TransType.ToString())).ToList();

                ViewBag.LedgerInfo = LedgerInfo;
                //List<InvMovmentView> LedgerInfoLocal = i_InventoryMovement_Repository.GetInvMovmentLinesByItemCodeSP(ItemCode).ToList();
                //foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
                //{
                //    var LedgerInfoLocalParent = i_InventoryMovement_Repository.GetInvMovmentLinesByItemCodeSP(ParentItem.ItemCode).ToList();
                //    LedgerInfoLocalParent.ForEach(x =>
                //    {
                //        x.InQty = x.InQty * ParentItem.Quantity;
                //        x.OutQty = x.OutQty * ParentItem.Quantity;
                //        x.UnitPrice = -1;
                //    });

                //    LedgerInfoLocal.AddRange(LedgerInfoLocalParent);
                //}
                
                List<InvMovmentView> LedgerInfoLocal = i_OINM_Repository.GetLedgerSalesTxn(ItemCode).ToList();

                foreach (OINM oinm in LedgerInfo)
                {
                    LedgerInfoLocal.Add(new InvMovmentView()
                    {
                        DocDate = oinm.DocDate,
                        DocNum = oinm.ref2 ,
                        TransType = oinm.TransType.ToString(),
                        CardCode = oinm.CardCode,
                        CardName = oinm.Customer != null ? oinm.Customer.CardName : "",
                        InQty = oinm.TransType == 67 ? oinm.InQty.Value: oinm.InQty.Value,
                        OutQty = oinm.TransType == 67 ? oinm.OutQty.Value: oinm.OutQty.Value,
                        UnitPrice = oinm.Price.Value,
                        Location = oinm.Warehouse,                        
                        BASE_REF = oinm.BASE_REF
                    });
                }
                LedgerInfoLocal = LedgerInfoLocal.Where(x => myInClause.Contains(x.TransType)).ToList();
                ViewBag.LedgerInfoLocal = LedgerInfoLocal.OrderByDescending(x=> x.DocDate);
                
            }
            else
            {
                TempData["GlobalErrMsg"] = "Item not found";
                return RedirectToAction("Index");
            }

            // Special Prices
            var SpecialPrices = i_SPP2_Repository.GetItemSpecialPrices(ItemCode);
            ViewBag.SpecialPrices = SpecialPrices;
            var SpecialPrices1 = i_OPLN_Repository.PriceLists.Join(SpecialPrices, t1 => "*" + t1.ListNum.ToString(), t2 => t2.CardCode, (t1, t2) => new { t1.ListName, t2 }).GroupBy(x=> x.ListName).Select(x=> new SpecialPricesWithPriceListNameViewModel { Key = x.Key, List = x.Where(j=> j.ListName==x.Key).Select(c=> c.t2).ToList()});
            ViewBag.SpecialPrices1 = SpecialPrices1;


            // Ware House details
            IEnumerable<WareHouseDetails> wareHouseDetails = i_OITM_Repository.GetWareHouses().Select(e => new WareHouseDetails
            {
                WhsCode = e.WhsCode,
                WhsName = e.WhsName,
            }).ToList();
            ViewBag.wareHouseDetails = wareHouseDetails;

            // Transaction Types
            IEnumerable<TransactionTypesList> transactionTypesList = i_OITM_Repository.GetTransactionTypes().Select(e => new TransactionTypesList
            {
                TxnCode = e.TxnCode,
                TxnTypeName = e.TxnTypeName,
            }).ToList();
            ViewBag.transactionTypesList = transactionTypesList;


            return View(itmObject);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetLedgerInfoTransDeatils(string whsCode, string transType, string ItemCode)
        {

            //IEnumerable<ITT1> ParentItemList = i_ITT1_Repository.GetFatherItemList(ItemCode);
            //IEnumerable<ChileItemLocationStockViewModel> ParentItemStockList = ParentItemList.Select(e => new ChileItemLocationStockViewModel
            //{
            //    ItemCode = e.Father,
            //    Quantity = e.Quantity
            //}).ToList();

            var myInClause = new string[] { "17", "14", "20", "21", "59", "60", "67" };
            List<OINM> LedgerInfo = i_OINM_Repository.GetTransactionDetailsByItemCode(ItemCode).Where(x => myInClause.Contains(x.TransType.ToString())).ToList();

            //List<OINM> LedgerInfo = i_OINM_Repository.GetTransactionDetailsByItemCode(ItemCode).ToList();

            ViewBag.LedgerInfo = LedgerInfo;
            //List<InvMovmentView> LedgerInfoLocal = i_InventoryMovement_Repository.GetInvMovmentLinesByItemCodeSP(ItemCode).ToList();
            //foreach (ChileItemLocationStockViewModel ParentItem in ParentItemStockList)
            //{
            //    var LedgerInfoLocalParent = i_InventoryMovement_Repository.GetInvMovmentLinesByItemCodeSP(ParentItem.ItemCode).ToList();
            //    LedgerInfoLocalParent.ForEach(x =>
            //    {
            //        x.InQty = x.InQty * ParentItem.Quantity;
            //        x.OutQty = x.OutQty * ParentItem.Quantity;
            //        x.UnitPrice = -1;
            //    });

            //    LedgerInfoLocal.AddRange(LedgerInfoLocalParent);
            //}

            List<InvMovmentView> LedgerInfoLocal = i_OINM_Repository.GetLedgerSalesTxn(ItemCode).ToList();

            foreach (OINM oinm in LedgerInfo)
            {
                LedgerInfoLocal.Add(new InvMovmentView()
                {
                    DocDate = oinm.DocDate,
                    DocNum = oinm.ref2,
                    TransType = oinm.TransType.ToString(),
                    CardCode = oinm.CardCode,
                    CardName = oinm.Customer != null ? oinm.Customer.CardName : "",
                    InQty = oinm.TransType == 67 ? oinm.InQty.Value : oinm.InQty.Value,
                    OutQty = oinm.TransType == 67 ? oinm.OutQty.Value : oinm.OutQty.Value,
                    UnitPrice = oinm.Price.Value,
                    Location = oinm.Warehouse,
                    BASE_REF = oinm.BASE_REF
                });
            }

            if(whsCode != "ALL")
                LedgerInfoLocal = LedgerInfoLocal.Where(x => x.Location.Equals(whsCode)).ToList();

            //var myInClause = new string[] { "17", "14", "20", "21", "59", "60", "67" };

            LedgerInfoLocal = LedgerInfoLocal.Where(x => myInClause.Contains(x.TransType)).ToList();

            switch (transType)
            {
                case "SalesTxn":
                    myInClause = new string[] { "17", "14" };
                    LedgerInfoLocal = LedgerInfoLocal.Where(x => myInClause.Contains(x.TransType)).ToList();
                    //LedgerInfoLocal = LedgerInfoLocal.Where(x => x.TransType == "17" || x.TransType == "14").ToList();
                    break;
                case "PurchaseTxn":
                    myInClause = new string[] { "20", "21" };
                    LedgerInfoLocal = LedgerInfoLocal.Where(x => myInClause.Contains(x.TransType)).ToList();
                    //LedgerInfoLocal = LedgerInfoLocal.Where(x => x.TransType == "20" || x.TransType == "21").ToList();
                    break;
                case "InvTxn":
                    myInClause = new string[] { "59","60","67" };
                    LedgerInfoLocal = LedgerInfoLocal.Where(x => myInClause.Contains(x.TransType)).ToList();

                    //LedgerInfoLocal = LedgerInfoLocal.Where(x => x.TransType == "60" || x.TransType == "59" || x.TransType == "67").ToList();
                    break;                
            }

            //if (transType == "SalesTxn")
            //{
            //    LedgerInfoLocal = LedgerInfoLocal.Where(x => x.Location.Equals(whsCode) && (x.TransType == "17" || x.TransType == "14")).ToList();
            //}
            //else if (transType == "PurchaseTxn")
            //{
            //    LedgerInfoLocal = LedgerInfoLocal.Where(x => x.Location.Equals(whsCode) && (x.TransType == "20" || x.TransType == "21")).ToList();
            //}
            //else if (transType == "InvTxn")
            //{
            //    LedgerInfoLocal = LedgerInfoLocal.Where(x => x.Location.Equals(whsCode) && (x.TransType == "60" || x.TransType == "59" || x.TransType == "67")).ToList();
            //}

            LedgerInfoLocal = LedgerInfoLocal.OrderByDescending(x => x.DocDate).ToList();
            ViewBag.LedgerInfoLocal = LedgerInfoLocal;

            // Ware House details
            IEnumerable<StringInvMovmentView> stringInvMovmentView = LedgerInfoLocal.Select(e => new StringInvMovmentView
            {
                DocDate = e.DocDate.ToString("dd-MM-yyy"),
                DocNum = e.DocNum,
                TransType = TransTypeValue(e.TransType),
                TransTypeSAPDOC = TransTypeSAPDOCValue(e.TransType),                                
                CardCode = e.CardCode,
                CardName = e.CardName,
                InQty = e.InQty,
                OutQty = e.OutQty,
                //UnitPrice = e.UnitPrice.ToString("N4", new CultureInfo("en-US")),
                UnitPrice = e.UnitPrice,
                Location = e.Location,
                BASE_REF = e.BASE_REF
            }).ToList();
            
            return Json(stringInvMovmentView, JsonRequestBehavior.DenyGet);
        }

        private string TransTypeValue(string strTransTypeValue)
        {
            string strTransType = strTransTypeValue;
            switch (strTransTypeValue)
            {

                case "14":
                    strTransType = "Sales";
                    break;
                case "17":
                    strTransType = "Sales";
                    break;
                case "20":
                    strTransType = "Purchase";
                    break;
                case "21":
                    strTransType = "Purchase";
                    break;
                case "59":
                    strTransType = "Purchase";
                    break;
                case "60":
                    strTransType = "Inventory";
                    break;
                case "67":
                    strTransType = "Inventory";
                    break;
            }
            return strTransType;
        }


        private string TransTypeSAPDOCValue(string strTransTypeValue)
        {
            string strTransType = strTransTypeValue;
            switch (strTransTypeValue)
            {
                
                case "14":
                    strTransType = "Credit Note";
                    break;
                case "17":
                    strTransType = "Sales Order";
                    break;                
                case "20":
                    strTransType = "Goods Receipt PO";
                    break;
                case "21":
                    strTransType = "Goods Return";
                    break;                
                case "59":
                    strTransType = "Goods Receipt";
                    break;               
                case "60":
                    strTransType = "Goods Issue";
                    break;
                case "67":
                    strTransType = "Transfer";
                    break;
            }
            return strTransType;
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemMthlySalesV1(string whsCode, string transType,string ItemCode)
        {
            //ItemCode = "A01-0011G0";
            var ResultList = i_OITM_Repository.GetItemMthlySales(whsCode);
            return Json(ResultList, JsonRequestBehavior.DenyGet);
        }



    }
}