using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OPCH")]
    public class OPCH
    {
        public OPCH()
        {
            this.APInvoiceLines = new HashSet<PCH1>();
        }
        [Key]
        public Int32 DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public string DocType { get; set; }
        public decimal DocTotal { get; set; }
        public string CardCode { get; set; }
        public string CANCELED { get; set; }
        public string DocStatus { get; set; }
        public virtual ICollection<PCH1> APInvoiceLines { get; set; }
        public virtual OCRD Customer { get; set; }
    }
}
