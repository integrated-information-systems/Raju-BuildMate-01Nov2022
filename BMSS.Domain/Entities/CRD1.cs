using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("CRD1")]
    public class CRD1
    {
        [Key]
        [Column(Order = 1)]
        public string CardCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Address { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Street { get; set; }
        [Key]
        [Column(Order = 4)]
        public string Block { get; set; }
        [Key]
        [Column(Order = 5)]
        public string City { get; set; }
        [Key]
        [Column(Order = 6)]
        public string County { get; set; }
        public string StreetNo { get; set; }

        public string AdresType { get; set; }
        [ForeignKey("CountryCodes")]
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public virtual OCRY CountryCodes { get; set; }
        public virtual OCRD Customer { get; set; }
    }
}
