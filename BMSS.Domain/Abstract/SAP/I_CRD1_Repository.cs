using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_CRD1_Repository
    {
        IEnumerable<CRD1> GetCustomerBillingAddresses(string CardCode);
        IEnumerable<CRD1> GetCustomerShippingAddresses(string CardCode);
    }
}
