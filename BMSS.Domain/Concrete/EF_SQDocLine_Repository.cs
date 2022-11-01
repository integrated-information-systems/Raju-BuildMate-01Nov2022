using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_SQDocLine_Repository : I_SQDocLs_Repository
    {
        public IEnumerable<SQDocLs> SalesQuotationLineList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.SQDocLs.AsNoTracking().ToList();
                }
            }
        }
        public IEnumerable<SQDocLs> GetSQLinesByItemCode(string ItemCode, string CardCode)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.SQDocLs.Include("SQDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.SQDocH.CardCode.Equals(CardCode)).ToList();
            }
        }
    }
}
