using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{
    [Table("OPOR")]
    public class OPOR
    {
        public OPOR()
        {
            this.POLines = new HashSet<POR1>();
        }
        [Key]
        public Int32 DocEntry { get; set; }
        public Int32 DocNum { get; set; }

        public Int32 Series { get; set; }
        public string CANCELED { get; set; }
        public string DocStatus { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public decimal DocTotal { get; set; }
        public string CardCode { get; set; }
        public virtual ICollection<POR1> POLines { get; set; }
        public virtual NNM1 SeriesPrefix { get; set; }
        public virtual OCRD Customer { get; set; }
    }
}
