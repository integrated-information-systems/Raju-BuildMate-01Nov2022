using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Models
{
    public class ItemMonthlySales
    {
        public string MonthYr { get; set; }
        public string Quantity { get; set; }
        
    }

    public class ListofStcoks
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public decimal? onhand { get; set; }
        public decimal? isCommited { get; set; }
        public decimal? DraftGoodsReceipt { get; set; }
        public decimal? DraftGoodsIssue { get; set; }
        public decimal? DraftCreditNote { get; set; }
        public decimal? SOAvailable { get; set; }
        public decimal? onOrder { get; set; }

    }

    public class WareHouseDetails
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }

    }

    public class TransactionTypesList
    {
        public string TxnCode { get; set; }
        public string TxnTypeName { get; set; }

    }


}
