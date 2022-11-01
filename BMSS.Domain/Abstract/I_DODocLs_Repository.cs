using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_DODocLs_Repository: IDisposable
    {
        IEnumerable<DODocLs> DOLineList { get; }
        IEnumerable<DODocLs> GetDOLinesByDocNum(string DocNum);
        IEnumerable<DODocLs> GetDOLinesByItemCode(string ItemCode, string CardCode);

        IEnumerable<DODocLs> GetDOLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords=50);
    }
}
