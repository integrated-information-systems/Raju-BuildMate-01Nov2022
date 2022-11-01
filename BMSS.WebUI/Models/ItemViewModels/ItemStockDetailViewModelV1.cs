namespace BMSS.WebUI.Models.ItemViewModels
{
    public class ItemStockDetailViewModelV1
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

    public partial class StringInvMovmentView
    {
        public string TransType { get; set; }
        public string TransTypeSAPDOC { get; set; }
        public string DocDate { get; set; }
        public string DocNum { get; set; }
        public string SAPDocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal InQty { get; set; }
        public decimal OutQty { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemCode { get; set; }
        public string CreatedOn { get; set; }
        public string Location { get; set; }
        public string BASE_REF { get; set; }
    }



}