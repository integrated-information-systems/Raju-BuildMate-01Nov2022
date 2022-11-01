using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OPLN_Repository : I_OPLN_Repository
    {
        public IEnumerable<OPLN> PriceLists
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.PriceLists.AsNoTracking().ToList();
                }
            }
        }

        public string GetPriceListNameByCardCode(string cardCode)
        {
            string priceListName = "";
            using (var dbcontext = new EFSapDbContext())
            {
                var priceListNum = dbcontext.Customers.Where(x => x.CardCode == cardCode).Select(x => x.ListNum).FirstOrDefault();
                priceListName = dbcontext.PriceLists.Where(x => x.ListNum == priceListNum).Select(x => x.ListName).FirstOrDefault();
            }
            return priceListName;

        }
    }
}
