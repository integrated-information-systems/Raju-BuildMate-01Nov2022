using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OCRC_Repository : I_OCRC_Repository
    {
        public IEnumerable<OCRC> CreditCards
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.CreditCards.ToList();
                }
            }
        }
    }
}
