using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_INV1_Repository : I_INV1_Repository
    {
        public IEnumerable<INV1> GetInvoiceLines(string ItemCode)
        {
            IEnumerable<INV1> InvoiceLines = null;
            using (var dbcontext = new EFSapDbContext())
            {
                InvoiceLines = dbcontext.InvoiceLines.Include("InvoiceHeader").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).ToList();
            }
            return InvoiceLines;
        }
    }
}
