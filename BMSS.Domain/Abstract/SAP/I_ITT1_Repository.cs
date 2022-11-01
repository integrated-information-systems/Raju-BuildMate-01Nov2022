using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_ITT1_Repository
    {
        IEnumerable<ITT1> GetChildItemList(string ParentItemCode);
        IEnumerable<ITT1> GetFatherItemList(string ChildItemCode);
    }
}
