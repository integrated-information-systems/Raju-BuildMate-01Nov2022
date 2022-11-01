using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OVTG_Repository : I_OVTG_Repository
    {
        public IEnumerable<OVTG> TaxCodes
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.TaxCodes.AsNoTracking().ToList();
                }
            }
        }
    }
}
