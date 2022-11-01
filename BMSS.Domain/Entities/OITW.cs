using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OITW")]
    public class OITW
    {
        [Key]
        [Column(Order= 1)]
        public string ItemCode { get; set; }
        [Key]
        [Column(Order= 2)]
        public string WhsCode { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? IsCommited { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? U_minstockperwhs { get; set; }

        public virtual OWHS Warehouse { get; set; }    
        public virtual OITM Item { get; set; }
    }
}
