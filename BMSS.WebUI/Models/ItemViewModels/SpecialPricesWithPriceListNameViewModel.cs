using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.ItemViewModels
{
    public class SpecialPricesWithPriceListNameViewModel
    {
        public string Key { get; set; }

        public IEnumerable<SPP2> List { get; set; }
    }
}