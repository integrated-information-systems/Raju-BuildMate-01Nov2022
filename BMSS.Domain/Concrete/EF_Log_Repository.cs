using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete
{
    public class EF_Log_Repository : I_Log_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_Log_Repository()
        {
            dbcontext = new DomainDb();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }

        public void WriteLog(SyncLog syncLog)
        {
            dbcontext.SyncLog.Add(syncLog);
            dbcontext.SaveChanges();
        }

        public void WriteSyncLog(SyncErrLog syncErrLog)
        {
            dbcontext.SyncErrLog.Add(syncErrLog);
            dbcontext.SaveChanges();
        }
    }
}
