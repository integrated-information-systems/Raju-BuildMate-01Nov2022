using BMSS.Domain.Abstract.SAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_SPP1_Repository : I_SPP1_Repository
    {
        public decimal GetItemNormalPrices(string itemcode, string cardcode)
        {
            decimal normalPrice = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                short priceListNum = dbcontext.Customers.Where(x => x.CardCode.Equals(cardcode)).Select(x => x.ListNum).FirstOrDefault();                 
                normalPrice = dbcontext.ItemSpecialPriceMasters.Where(x => x.ItemCode == itemcode && x.ListNum == priceListNum).Select(x=> x.Price).FirstOrDefault();
            }
            return normalPrice;
        }
    }
}
