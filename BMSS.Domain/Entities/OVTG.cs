using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OVTG")]
    public class OVTG
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Rate { get; set; }
    }
}
