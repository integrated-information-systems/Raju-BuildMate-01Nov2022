using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OITM")]
    public class OITM
    {
        public OITM()
        {          
            this.Transactions = new HashSet<OINM>();
        }
        [Key]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
       
        [Display(Name = "Costing Method")]
        public string EvalSystem { get; set; }
        [Display(Name = "Minimum Stock (overall whs)")]
        public decimal MinLevel { get; set; }
        [Display(Name = "Sales Unit")]
       
        public string SalUnitMsr { get; set; }
        [Display(Name = "Items per sales unit")]
        
        public decimal NumInSale { get; set; }
        [Display(Name = "Purchase Unit")]
        public string BuyUnitMsr { get; set; }
        [Display(Name = "Items per purchase unit")]
        public decimal NumInBuy { get; set; }               
        [Display(Name = "Inventory Unit")]
        public string InvntryUom { get; set; }
        [Display(Name = "Unit Weight")]
        public string U_unitwt { get; set; }
        public string InvntItem { get; set; }
        public decimal AvgPrice { get; set; }

        public string DfltWH { get; set; }
        public string VatGourpSa { get; set; }

        public string U_ALLOW_UNIT_COST_ZERO { get; set; }
        public decimal? IWeight1 { get; set; }

        public Int16 ItmsGrpCod { get; set; }
        public virtual OITB ItemGroup { get; set; }

        public virtual ICollection<OINM> Transactions { get; set; }
    }
}
