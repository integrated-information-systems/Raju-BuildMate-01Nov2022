using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_StockReceiptDocLs_Repository : IDisposable
    {
        IEnumerable<StockReceiptDocLs> StockReceiptLineList { get; }
        IEnumerable<StockReceiptDocLs> GetStockReceiptLinesByItemCode(string ItemCode);
        IEnumerable<StockReceiptDocLs> GetStockReceiptLinesByItemCodeWithLimit(string ItemCode, int noOfRecords=50);
        IEnumerable<StockReceiptDocLs> GetSRLinesByDocNum(string DocNum);
    }
}
