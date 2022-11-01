using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_ITM1_Repository : I_ITM1_Repository
    {
        public IEnumerable<ITM1> GetItemPrices(string ItemCode)
        {
            IEnumerable<ITM1> ItemPrices = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ItemPrices = dbcontext.ItemPrices.Include("PriceLists").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).ToList();
            }
            return ItemPrices;
        }
    }
}
