using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OPLN")]
    public class OPLN
    {
        public OPLN()
        {
            this.ItemPrices = new HashSet<ITM1>();
            this.Customers = new HashSet<OCRD>();
        }

        [Key]
        public Int16 ListNum { get; set; }
        public string ListName { get; set; }

        public ICollection<ITM1> ItemPrices { get; set; }

        public virtual ICollection<OCRD> Customers { get; set; }
    }
}
