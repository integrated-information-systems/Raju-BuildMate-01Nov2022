using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OINM")]
    public class OINM
    {
        [Key]
        public Int32 TransNum { get; set; }
        public Int32 TransType { get; set; }
        public DateTime DocDate { get; set; }
        public string BASE_REF { get; set; }
        public string ref2 { get; set; }
        public string CardCode { get; set; }        
        public decimal? InQty { get; set; }
        public decimal? OutQty { get; set; }
        public decimal? Price { get; set; }
        public string ItemCode { get; set; }

        public string Warehouse { get; set; }
        public virtual OITM Item { get; set; }
        public virtual OCRD Customer { get; set; }
    }
}
