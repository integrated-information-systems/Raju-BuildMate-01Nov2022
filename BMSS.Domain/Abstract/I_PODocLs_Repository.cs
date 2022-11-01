using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_PODocLs_Repository
    {
        IEnumerable<PODocLs> POLineList { get; }
        IEnumerable<PODocLs> GetPOLinesByItemCode(string ItemCode, string CardCode);
        IEnumerable<PODocLs> GetPOLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords=50);

        decimal GetTotalPOStockBalanceByItemCode(string ItemCode, string WarhouseCode);
        decimal GetTotalPOStockBalanceByItemCode(string ItemCode);
    }
}
