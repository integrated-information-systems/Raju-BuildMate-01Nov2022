using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OINM_Repository
    {
        IEnumerable<OINM> GetTransactionDetailsByItemCode(string ItemCode);
        IEnumerable<InvMovmentView> GetLedgerSalesTxn(string ItemCode);
    }
}
