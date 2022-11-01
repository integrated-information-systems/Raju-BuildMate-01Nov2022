using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Models
{
    public abstract class DocNumbering
    {
        public long NumberingID { get; set; }

        public string SeriesName { get; set; }

        public long FirstNo { get; set; }
        public long NextNo { get; set; }
        public long LastNo { get; set; }

        public string Prefix { get; set; }
      
        public bool IsDefault { get; set; }

        public bool IsLocked { get; set; }
        public bool? IsUsed { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; set; }
    }
}
