using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OCRG")]
    public class OCRG
    {
        public OCRG()
        {
            this.Customers = new HashSet<OCRD>();
        }

        [Key]
        public Int16 GroupCode { get; set; }
        [Display(Name = "Customer Group")]
        public string GroupName { get; set; }

        public virtual ICollection<OCRD> Customers { get; set; }
    }
}
