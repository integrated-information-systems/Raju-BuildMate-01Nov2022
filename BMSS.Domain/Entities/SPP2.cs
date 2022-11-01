using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("SPP2")]
    public class SPP2
    {
        [Key]
        [Column(Order = 1)]        
        public string ItemCode { get; set; }
        [Key]
        [Column(Order = 2)]        
        public string CardCode { get; set; }
        [Key]
        [Column(Order = 3)]       
        public Int16 SPP1LNum { get; set; }
        [Key]
        [Column(Order = 4)]
        public Int16 SPP2LNum { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        
               
    }
}
