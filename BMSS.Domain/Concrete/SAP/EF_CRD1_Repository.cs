using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public  class EF_CRD1_Repository : I_CRD1_Repository
    {
        public IEnumerable<CRD1> GetCustomerBillingAddresses(string CardCode)
        {
            IEnumerable<CRD1> InvoiceLines = null;
            using (var dbcontext = new EFSapDbContext())
            {
                InvoiceLines = dbcontext.CustomerAddresses.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.AdresType.Equals("B")).ToList();
            }
            return InvoiceLines;
        }
        public IEnumerable<CRD1> GetCustomerShippingAddresses(string CardCode)
        {
            IEnumerable<CRD1> InvoiceLines = null;
            using (var dbcontext = new EFSapDbContext())
            {
                InvoiceLines = dbcontext.CustomerAddresses.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.AdresType.Equals("S")).ToList();
            }
            return InvoiceLines;
        }
    }
}
