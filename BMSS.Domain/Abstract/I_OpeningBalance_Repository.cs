using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract
{
    public interface I_OpeningBalance_Repository : IDisposable
    {
        bool CheckOpeningBalanceStatus();
        void SetOpeningBalanceStatus();
        void ResetOpeningBalanceStatus();
        List<string> ItemList { get; }
    }
}
