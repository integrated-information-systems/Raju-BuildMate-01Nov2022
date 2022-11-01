using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OCRY")]
    public class OCRY
    {
        public OCRY()
        {
            this.CustomerAddress = new HashSet<CRD1>();
        }
        [Key]
        public string Code { get; set; }        
        public string Name { get; set; }

        public virtual ICollection<CRD1> CustomerAddress { get; set; }
    }
}
