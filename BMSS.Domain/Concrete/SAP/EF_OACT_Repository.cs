using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OACT_Repository : I_OACT_Repository
    {
        public IEnumerable<OACT> GLCodes
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.GLAccounts.AsNoTracking().Where(i => i.Finanse.Equals("Y") && i.U_Payment.Equals("Y")).ToList();                    
                }
            }
        }
        public string GetGLNameByCode(string GLCode)
        {
            string Result = string.Empty;
            OACT GLAccount = null;
            using (var dbcontext = new EFSapDbContext())
            {
                GLAccount = dbcontext.GLAccounts.AsNoTracking().Where(i => i.AcctCode.Equals(GLCode)).FirstOrDefault();
            }
            if (!GLAccount.Equals(null))
            {
                Result = GLAccount.AcctName;
            }
            return Result;
        }
    }
}
