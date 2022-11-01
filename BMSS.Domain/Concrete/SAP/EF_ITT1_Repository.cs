using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_ITT1_Repository : I_ITT1_Repository
    {
        public IEnumerable<ITT1> GetChildItemList(string ParentItemCode)
        {
            IEnumerable<ITT1> ChildItemList = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ChildItemList = dbcontext.ChildItems.AsNoTracking().Where(i => i.Father.Equals(ParentItemCode)).ToList();
            }
            return ChildItemList;
        }
        public IEnumerable<ITT1> GetFatherItemList(string ChildItemCode)
        {
            IEnumerable<ITT1> ChildItemList = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ChildItemList = dbcontext.ChildItems.AsNoTracking().Where(i => i.Code.Equals(ChildItemCode)).ToList();
            }
            return ChildItemList;
        }
    }
}
