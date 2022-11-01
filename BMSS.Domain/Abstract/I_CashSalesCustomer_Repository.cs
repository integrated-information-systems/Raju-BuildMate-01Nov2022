using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_CashSalesCustomer_Repository
    {
        IEnumerable<CashSalesCustomerMaster> CashSalesCustomerList { get; }
        bool AddUpdateCashSalesCustomer(CashSalesCustomerMaster CSCObj);
        CashSalesCustomerMaster GetByTelephoneNo(string CustomerID);
        CashSalesCustomerMaster GetByDocEntry(long DocEntry);
        bool DeleteByTelephoneNo(string CustomerID);
        bool DeleteByDocEntry(long DocEntry);


    }
}
