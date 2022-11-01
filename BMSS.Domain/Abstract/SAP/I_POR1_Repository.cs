using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_POR1_Repository
    {
        decimal GetTotalPOStockBalanceByItemCode(string ItemCode, string WarhouseCode);
        decimal GetTotalPOStockBalanceByItemCode(string ItemCode);
    }
}
