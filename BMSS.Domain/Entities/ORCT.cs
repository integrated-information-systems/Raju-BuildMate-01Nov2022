using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{
    [Table("ORCT")]
    public class ORCT
    {
        [Key]
        public Int32 DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public decimal DocTotal { get; set; }
        public string CardCode { get; set; }

        public string Canceled { get; set; }
    }
}
