using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_StockTransDocLs_Repository : IDisposable
    {
        IEnumerable<StockTransDocLs> StockTransLineList { get; }
        IEnumerable<StockTransDocLs> GetStockTransLinesByItemCode(string ItemCode);
        IEnumerable<StockTransDocLs> GetStockTransLinesByItemCodeWithLimit(string ItemCode, int noOfRecords = 50);

        IEnumerable<StockTransDocLs> GetSTLinesByDocNum(string DocNum);
    }
}
