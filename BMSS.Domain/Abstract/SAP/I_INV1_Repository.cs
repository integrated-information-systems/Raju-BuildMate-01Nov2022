using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_INV1_Repository
    {
        IEnumerable<INV1> GetInvoiceLines(string Itemcode);
    }
}
