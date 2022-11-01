using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_CashSalesCreditDocLs_Repository : IDisposable
    {
        IEnumerable<CashSalesCreditDocLs> CashSalesCreditLineList { get; }
        IEnumerable<CashSalesCreditDocLs> GetCSCLinesByDocNum(string DocNum);

        IEnumerable<CashSalesCreditDocPays> GetCSCPayLinesByDocNum(string DocNum);
        IEnumerable<CashSalesCreditDocLs> GetCashSalesCreditLinesByItemCode(string ItemCode, string CardCode);
        IEnumerable<CashSalesCreditDocLs> GetCashSalesCreditLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50);
    }
}
