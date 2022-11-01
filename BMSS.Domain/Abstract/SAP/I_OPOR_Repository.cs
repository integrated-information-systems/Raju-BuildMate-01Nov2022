using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OPOR_Repository : IDisposable
    {
        IEnumerable<OPOR> GetPODetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "");
        int GetPOTotalCount();

        int GetPODetailsWithPaginationCount(string search = "");
    }
}
