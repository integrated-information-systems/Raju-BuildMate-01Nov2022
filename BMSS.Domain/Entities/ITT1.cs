using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{
    [Table("ITT1")]
    public class ITT1
    {
        [Key]
        [Column(Order = 1)]
        public string Father { get; set; }
        [Key]
        [Column(Order = 2)]       
        public string Code { get; set; }
        [Key]
        [Column(Order = 3)]
        public Int32 ChildNum { get; set; }

        public decimal Quantity { get; set; }

        public string Warehouse { get; set; }
    }
}
