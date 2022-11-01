namespace BMSS.WebUI.Models.InventoryMovementViewModels
{
    public class InventoryMovementReportViewModels
    {
        public System.DateTime DocDate { get; set; }
        public string DocType { get; set; }
        public string DocNum { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }       
        public decimal UnitPrice { get; set; }
        public string LocationText { get; set; }
    }
}