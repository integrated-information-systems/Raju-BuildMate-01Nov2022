using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_ITM1_Repository
    {
        IEnumerable<ITM1> GetItemPrices(string Itemcode);
    }
}
