using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_StockIssueDocLs_Repository : IDisposable
    {
        IEnumerable<StockIssueDocLs> StockIssueLineList { get; }
        IEnumerable<StockIssueDocLs> GetStockIssueLinesByItemCode(string ItemCode);

        IEnumerable<StockIssueDocLs> GetStockIssueLinesByItemCodeWithLimit(string ItemCode, int noOfRecords = 50);

        IEnumerable<StockIssueDocLs> GetSILinesByDocNum(string DocNum);
    }
}
