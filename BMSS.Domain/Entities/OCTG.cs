using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OCTG")]
    public class OCTG
    {
        public OCTG()
        {
            this.Customers = new HashSet<OCRD>();
        }

        [Key]
        public Int16 GroupNum { get; set; }
        public string PymntGroup { get; set; }
        public Int16 ExtraDays { get; set; }
        

        public virtual ICollection<OCRD> Customers { get; set; }
    }
}
