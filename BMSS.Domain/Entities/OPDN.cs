using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{
    [Table("OPDN")]
    public class OPDN
    {
        public OPDN()
        {
            this.GRPOLines = new HashSet<PDN1>();
        }
        [Key]
        public Int32 DocEntry { get; set; }               
        public string CANCELED { get; set; }
        public string DocStatus { get; set; }
        public string CardCode { get; set; }
        public DateTime DocDate { get; set; }
        public virtual ICollection<PDN1> GRPOLines { get; set; }
        
    }
}
