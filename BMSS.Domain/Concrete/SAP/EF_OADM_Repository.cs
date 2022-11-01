using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OADM_Repository : I_OADM_Repository
    {
        public OADM CompanyDefaults
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.CompanyDefaults.AsNoTracking().FirstOrDefault();
                }
            }
        }
    }
}
