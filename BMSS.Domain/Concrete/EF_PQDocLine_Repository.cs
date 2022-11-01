using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete
{
    public class EF_PQDocLine_Repository : I_PQDocLs_Repository
    {
        public IEnumerable<PQDocLs> PurchaseQuotationLineList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.PQDocLs.AsNoTracking().ToList();
                }
            }
        }

        public IEnumerable<PQDocLs> GetPQLinesByItemCode(string ItemCode, string CardCode)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PQDocLs.Include("PQDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.PQDocH.CardCode.Equals(CardCode)).ToList();
            }
        }

        public IEnumerable<PQDocLs> GetPQLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords=50)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PQDocLs.Include("PQDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.PQDocH.CardCode.Equals(CardCode)).OrderByDescending(x=> x.PQDocH.DocDate).Take(noOfRecords).ToList();
            }
        }

    }
}
