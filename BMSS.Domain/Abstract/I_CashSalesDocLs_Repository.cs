using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_CashSalesDocLs_Repository : IDisposable
    {
        IEnumerable<CashSalesDocLs> CashSalesLineList { get; }
        IEnumerable<CashSalesDocLs> GetCSLinesByDocNum(string DocNum);
        IEnumerable<CashSalesDocPays> GetCSPayLinesByDocNum(string DocNum);
        IEnumerable<CashSalesDocLs> GetCashSalesLinesByItemCode(string ItemCode, string CardCode);
        IEnumerable<CashSalesDocLs> GetCashSalesLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50);
    }
}
