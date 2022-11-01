namespace BMSS.WebUI.Models.ItemViewModels
{
    public class LocationStockViewModel
    {
        public string WarhouseCode { get; set; }
        public string WarhouseName { get; set; }
        public decimal? AvailableQty { get; set; }        
        public decimal? MinStock { get; set; }
        public decimal? OnOrder { get; set; }
    }
}