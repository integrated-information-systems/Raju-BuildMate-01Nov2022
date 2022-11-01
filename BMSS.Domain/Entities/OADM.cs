using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OADM")]
    public class OADM
    {
        [Key]
        public string CompnyName { get; set; }
        public string DfltWhs { get; set; }
        public string DfSVatItem { get; set; }
        
    }
}
