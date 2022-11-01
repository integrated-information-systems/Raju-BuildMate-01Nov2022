namespace BMSS.WebUI.Models.ItemViewModels
{
    public class ItemStockDetailViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string InvntItem { get; set; }
        public decimal? InStock { get; set; }
        public decimal? InOrder { get; set; }
        public decimal? IsCommited { get; set; }
        public decimal? TRSInStock { get; set; }
        public decimal? TRSInOrder { get; set; }
        public decimal? ENSInStock { get; set; }
        public decimal? ENSInOrder { get; set; }
        public string WhsCode { get; set; }
    }
}