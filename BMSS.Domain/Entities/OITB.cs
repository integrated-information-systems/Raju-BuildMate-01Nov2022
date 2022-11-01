using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OITB")]
    public class OITB
    {
        public OITB()
        {
            this.Items = new HashSet<OITM>();
        }
        [Key]
        public Int16 ItmsGrpCod { get; set; }
        [Display(Name = "Item Group")]
        public string ItmsGrpNam { get; set; }

        public virtual ICollection<OITM> Items { get; set; }
    }
}
