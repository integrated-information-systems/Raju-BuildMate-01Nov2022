using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("INV1")]
    public class INV1
    {
        [Key]
        [Column(Order = 1)]
        public Int32 DocEntry { get; set; }       
        public string ItemCode { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        [Key]
        [Column(Order = 2)]
        public Int32 LineNum { get; set; }
        public virtual OINV InvoiceHeader { get; set; }
    }
}
