using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.ItemViewModels
{
    public class ChileItemLocationStockViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public IEnumerable<LocationStockViewModel> StockDetails { get; set; }
    }
}