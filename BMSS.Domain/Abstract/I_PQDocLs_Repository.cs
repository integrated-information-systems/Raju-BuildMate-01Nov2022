using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract
{
    public interface I_PQDocLs_Repository
    {
        IEnumerable<PQDocLs> PurchaseQuotationLineList { get; }
        IEnumerable<PQDocLs> GetPQLinesByItemCode(string ItemCode, string CardCode);
        IEnumerable<PQDocLs> GetPQLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50);
    }
}
