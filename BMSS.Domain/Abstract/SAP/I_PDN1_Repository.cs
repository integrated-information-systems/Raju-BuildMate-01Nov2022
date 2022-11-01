using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_PDN1_Repository
    {
        decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode, string WarhouseCode);
        decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode);

        decimal GetLastUnitPriceFromGRPO(string ItemCode, string CardCode);
    }
}
