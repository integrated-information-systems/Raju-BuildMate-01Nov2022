using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_SQDocLs_Repository
    {
        IEnumerable<SQDocLs> SalesQuotationLineList { get; }
        IEnumerable<SQDocLs> GetSQLinesByItemCode(string ItemCode, string CardCode);
    }
}
