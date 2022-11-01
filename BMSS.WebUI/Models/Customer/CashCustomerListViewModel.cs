using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.Customer
{
    public class CashCustomerListViewModel
    {
        public long DocEntry { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CreatedOn { get; set; }
        public string SalesPerson { get; set; }
    }
}