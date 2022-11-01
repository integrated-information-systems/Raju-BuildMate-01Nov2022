using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Models
{
    public class AgingBucket
    {
        public decimal? Current { get; set; }
        public decimal? Months1 { get; set; }
        public decimal? Months2 { get; set; }
        public decimal? Months3 { get; set; }
        public decimal? Months4 { get; set; }

        public decimal? Months5 { get; set; }


        public decimal? OutStandingBalance { get; set; }

    }
    public class SOA
    {

        public DateTime Refdate { get; set; }
        public decimal BalDueCred { get; set; }

    }

    public class LastPaid

    {
        public DateTime DocDate { get; set; }
        public decimal DocTotal { get; set; }
    }
}
