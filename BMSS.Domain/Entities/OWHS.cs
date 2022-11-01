using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OWHS")]
    public class OWHS
    {
        public OWHS()
        {
            this.WarehouseStocks = new HashSet<OITW>();
        }

        [Key]
        public string WhsCode { get; set; }
        public string WhsName { get; set; }

        public virtual ICollection<OITW> WarehouseStocks { get; set; }
    }
}
