using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_SPP1_Repository
    {
        decimal GetItemNormalPrices(string itemcode, string cardCode);
    }
}
