using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OACT")]
    public class OACT
    {
        [Key]
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string Finanse { get; set; }
        public string U_Payment { get; set; }
    }
}
