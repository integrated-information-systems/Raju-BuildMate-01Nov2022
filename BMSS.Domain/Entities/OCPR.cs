using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OCPR")]
    public class OCPR
    {
        [Key]
        public Int32 CntctCode { get; set; }
        public string CardCode { get; set; }
        public string Name { get; set; }
        public string Tel1 { get; set; }
        public string Notes1 { get; set; }
    }
}
