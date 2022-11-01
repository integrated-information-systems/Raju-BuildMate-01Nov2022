using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("ORPC")]
    public class ORPC
    {
        [Key]
        public Int32 DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public decimal DocTotal { get; set; }
        public string CardCode { get; set; }
        public string CANCELED { get; set; }
        public string DocStatus { get; set; }
        public virtual OCRD Customer { get; set; }
    }
}
