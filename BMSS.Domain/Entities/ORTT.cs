using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("ORTT")]
    public class ORTT
    {
        [Key]
        [Column(Order = 1)]
        public System.DateTime RateDate { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
