using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_ORCT_Repository
    {
        IEnumerable<ORCT> GetLastPaidRecordsByCustomer(string CardCode);
    }
}
