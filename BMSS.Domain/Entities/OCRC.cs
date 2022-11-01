using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OCRC")]
    public class OCRC
    {
        [Key]
        public Int16 CreditCard { get; set; }
        public string CardName { get; set; }
    }
}
