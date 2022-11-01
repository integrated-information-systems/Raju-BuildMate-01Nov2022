using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.SAPPOViewModels
{
    public class SAPPOListViewModel
    {

        public Int32 DocEntry { get; set; }
        public string DocNum { get; set; }
        public string CANCELED { get; set; }
        public string DocStatus { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public decimal DocTotal { get; set; }
        public string CardName { get; set; }
    }
}