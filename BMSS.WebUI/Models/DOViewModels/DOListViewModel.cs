using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.DOViewModels
{
    public class DOListViewModel
    {
        public string DocEntry { get; set; }
        public string DocNum { get; set; }
        public string CardCode { get; set; }
        public string DocDate { get; set; }
        public decimal GrandTotal { get; set; }
        public string PrintStatus { get; set; }    
        public string SAPStatus { get; set; }
        public string SyncRemarks { get; set; }
        public string SAPDocNum { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string CustomerRef { get; set; }
        public string ToPlanner { get; set; }
        public int SentCount { get; set; }
        public string Print { get; set; }

        public string ShipToAddress1 { get; set; }
    }
}