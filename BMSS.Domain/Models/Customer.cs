using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Models
{
    public class CustomerList
    {
        public string CardCode { get; set; }
        public string cardname { get; set; }
        public string GroupName { get; set; }
        public string SlpName { get; set; }
        public string frozenFor { get; set; }
        public decimal Balance { get; set; }
        public Nullable<System.DateTime> validTo { get; set; }
    }
}
