using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OSLP")]
    public class OSLP
    {
        public OSLP()
        {
            this.Customers = new HashSet<OCRD>();
        }
        [Key]
        public Int32 SlpCode { get; set; }
        [Display(Name = "Sales Person")]
        public string SlpName { get; set; }

        public virtual ICollection<OCRD> Customers { get; set; }
    }
}
