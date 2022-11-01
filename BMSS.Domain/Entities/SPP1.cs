using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{
    [Table("SPP1")]
    class SPP1
    {
        [Key]
        [Column(Order = 1)]
        public string ItemCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CardCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public Int16 ListNum { get; set; }

        public string Currency { get; set; }
        public decimal Price { get; set; }

    }
}
