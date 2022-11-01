using BMSS.Domain.Abstract;
using BMSS.Domain.Concrete.SAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete
{
    public class EF_OpeningBalance_Repository : I_OpeningBalance_Repository
    {
        private readonly DomainDb dbcontext;
        private readonly EFSapDbContext sapdbcontext;
        public EF_OpeningBalance_Repository()
        {
            dbcontext = new DomainDb();
            sapdbcontext = new EFSapDbContext();
        }

        public List<string> ItemList => sapdbcontext.Items.Select(x => x.ItemCode).ToList();

        public bool CheckOpeningBalanceStatus()
        {
            bool result = dbcontext.OpeningBalanceSetup.FirstOrDefault().SetOpeningBalance;
            return result;
        }

        public void Dispose()
        {
            dbcontext.Dispose();
            sapdbcontext.Dispose();
        }

        public void ResetOpeningBalanceStatus()
        {
            var op = dbcontext.OpeningBalanceSetup.Find(1);
            op.SetOpeningBalance = false;
            dbcontext.SaveChanges();
        }

        public void SetOpeningBalanceStatus()
        {
            var op = dbcontext.OpeningBalanceSetup.Find(1);
            op.SetOpeningBalance = true;
            dbcontext.SaveChanges();
        }
    }
}
