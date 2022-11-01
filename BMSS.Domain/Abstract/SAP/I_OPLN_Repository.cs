using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OPLN_Repository
    {
        IEnumerable<OPLN> PriceLists { get; }
        string GetPriceListNameByCardCode(string cardCode);
    }
}
