using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{

    [Table("POR1")]
    public class POR1
    {
        [Key]
        [Column(Order = 1)]
        public Int32 DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string WhsCode { get; set; }

        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        [Key]
        [Column(Order = 2)]
        public Int32 LineNum { get; set; }

        public virtual OPOR POHeader { get; set; }

        public string LineStatus { get; set; }

        public decimal OpenQty { get; set; }
    }
}
