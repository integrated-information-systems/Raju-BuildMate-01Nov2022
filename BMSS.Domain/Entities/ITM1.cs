using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("ITM1")]
    public class ITM1
    {
        [Key]
        [Column(Order = 1)]
        public string ItemCode { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("PriceLists")]
        public Int16 PriceList { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        //because this table already have field named 'PriceList', i declared this relationship as PriceList's'
        public virtual OPLN PriceLists { get; set; }
    }
}
