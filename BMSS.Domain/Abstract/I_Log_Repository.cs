using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract
{
    public interface I_Log_Repository : IDisposable
    {
        void WriteLog(SyncLog syncLog);
        void WriteSyncLog(SyncErrLog syncErrLog);
    }
}
