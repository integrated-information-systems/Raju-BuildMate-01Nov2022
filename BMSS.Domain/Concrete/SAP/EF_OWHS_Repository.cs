using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OWHS_Repository : I_OWHS_Repository
    {
        public IEnumerable<OWHS> Warehouses
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.Warehouses.AsNoTracking().ToList();
                }
            }
        }
    }
}
