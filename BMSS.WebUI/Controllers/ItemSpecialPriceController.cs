using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.WebUI.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Controllers
{
    public class ItemSpecialPriceController : Controller
    {
        //Repositories
        
        private I_SPP2_Repository i_SPP2_Repository;
        private I_SPP1_Repository i_SPP1_Repository;
        private I_OPLN_Repository i_OPLN_Repository;
        private I_OITM_Repository i_OITM_Repository;
        public ItemSpecialPriceController(I_SPP1_Repository i_SPP1_Repository, I_OPLN_Repository i_OPLN_Repository, I_SPP2_Repository i_SPP2_Repository, I_OITM_Repository i_OITM_Repository)
        {
            this.i_SPP2_Repository = i_SPP2_Repository;
            this.i_OITM_Repository = i_OITM_Repository;
            this.i_OPLN_Repository = i_OPLN_Repository;
            this.i_SPP1_Repository = i_SPP1_Repository;
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerSpecialPrice(string ItemCode, string CardCode)
        {
            var ResultList = i_SPP2_Repository.GetCustomerItemSpecialPrices(ItemCode,CardCode);
            return Json(ResultList, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetCustomerSpecialPriceWithPriceListName(string ItemCode, string CardCode)
        {
            string priceListName = i_OPLN_Repository.GetPriceListNameByCardCode(CardCode);
            var resultList = i_SPP2_Repository.GetCustomerItemSpecialPrices(ItemCode, CardCode);
            var normalPrice = i_SPP1_Repository.GetItemNormalPrices(ItemCode, CardCode);
            return Json(new { Name = priceListName, Lines = resultList, NormalPrice = normalPrice  }, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetItemSpecialPrice(Int16 PriceListNum, string ItemCode)
        {
            var ResultList = i_SPP2_Repository.GetItemSpecialPricesByPriceList(PriceListNum, ItemCode);
            return Json(ResultList, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetPriceLists()
        {
            var ResultList = i_OPLN_Repository.PriceLists.Select(e => new SelectListItem
            {
                Text = e.ListName,
                Value = e.ListNum.ToString()
            }).ToList(); ;

            return Json(ResultList, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetCustomerSpecialPriceItems(string CardCode)
        {
            CardCode = HttpUtility.HtmlDecode(CardCode);

            IEnumerable<OITM> ResultList = i_SPP2_Repository.GetCustomerSpecialPriceItemCodeList(CardCode);

            var ResultObject = ResultList.Select(e => new SelectListItem
            {
                Text = e.ItemName,
                Value = e.ItemCode
            }).ToList();
            return Json(ResultObject, JsonRequestBehavior.AllowGet);

        }
    }
}