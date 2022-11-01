using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Entities
{
    public class NNM1
    {
        [Key]
        public Int32 Series { get; set; }

        public string BeginStr { get; set; }
        public virtual ICollection<OPOR> PurchaseOrders { get; set; }
    }
}
