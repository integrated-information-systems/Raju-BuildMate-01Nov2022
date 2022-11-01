using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_DODocStockLoan_Repository
    {
        IEnumerable<DODocStockLoan> StockLoanList { get; }

        DODocStockLoan Get(long DocEntry);

        bool ReverseStockLoan(long DOEntry, string ReversedBy, ref string ValidationMessage, ref DateTime ReversedOn);
    }
}
