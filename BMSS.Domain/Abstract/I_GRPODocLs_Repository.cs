using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_GRPODocLs_Repository : IDisposable
    {
        IEnumerable<GRPODocLs> GRPOLineList { get; }
        IEnumerable<GRPODocLs> GetGRPOLinesByItemCode(string ItemCode);
        decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode, string WarhouseCode);
        decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode);

        decimal GetLastPriceByItemCode(string ItemCode);
        IEnumerable<GRPODocLs> GetGRPOLinesByDocNum(string DocNum);
    }
}
