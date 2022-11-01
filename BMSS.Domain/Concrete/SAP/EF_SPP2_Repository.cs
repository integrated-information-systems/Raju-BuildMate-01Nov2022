using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_SPP2_Repository : I_SPP2_Repository
    {
        
        public IEnumerable<SPP2> GetItemSpecialPricesByPriceList(Int16 PriceListNum, string ItemCode)
        {
            IEnumerable<SPP2> ItemSpecialPrices = null;
            using (var dbcontext = new EFSapDbContext())
            {
                String CardCode = "*" + PriceListNum.ToString();
                ItemSpecialPrices = dbcontext.ItemSpecialPrices.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)  && i.CardCode.Equals(CardCode)).ToList();
            }
            return ItemSpecialPrices;
        }
        public decimal GetSpecialPriceByItemCustomerQuantity(string ItemCode, string CardCode, decimal Quantity)
        {

            decimal itemPrice = 0;

            using (var dbcontext = new EFSapDbContext())
            {
                var priceListNum = dbcontext.Customers.Where(x => x.CardCode == CardCode).Select(x => x.ListNum).Single();
                String priceCardCode = "*" + priceListNum.ToString();              
              
                var ItemSpecialPrices = dbcontext.ItemSpecialPrices.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.Amount<=Quantity && i.CardCode.Equals(priceCardCode)).OrderByDescending(x=> x.Amount).FirstOrDefault();

                if (ItemSpecialPrices != null)
                    itemPrice = ItemSpecialPrices.Price;
                else
                {
                    itemPrice = dbcontext.ItemPrices.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.PriceList.Equals(priceListNum)).Select(x=> x.Price).Single();                  
                }               

                 
            }
            return itemPrice;
        }
        public IEnumerable<SPP2> GetItemSpecialPrices(string ItemCode)
        {
            IEnumerable<SPP2> ItemSpecialPrices = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ItemSpecialPrices = dbcontext.ItemSpecialPrices.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).ToList();
            }
            return ItemSpecialPrices;
        }
        public IEnumerable<SPP2> GetCustomerItemSpecialPrices(string ItemCode, string CardCode)
        {
            IEnumerable<SPP2> ItemSpecialPrices = null;
            using (var dbcontext = new EFSapDbContext())
            {
                short priceListNum = dbcontext.Customers.Where(x => x.CardCode.Equals(CardCode)).Select(x => x.ListNum).FirstOrDefault();
                string cardCodeListNum = "*" + priceListNum.ToString();

                ItemSpecialPrices = dbcontext.ItemSpecialPrices.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.CardCode.Equals(cardCodeListNum)).ToList();
            }
            return ItemSpecialPrices;
        }
        
        public IEnumerable<OITM> GetCustomerSpecialPriceItemCodeList(string CardCode)
        {
            IEnumerable<OITM> ItemSpecialPriceItems = null;
            using (var dbcontext = new EFSapDbContext())
            {
                short priceListNum = dbcontext.Customers.Where(x => x.CardCode.Equals(CardCode)).Select(x => x.ListNum).First();
                string cardCodeListNum = "*" + priceListNum.ToString();
                IEnumerable<SPP2> ItemSpecialPrices = dbcontext.ItemSpecialPrices.AsNoTracking().Where(i => i.CardCode.Equals(cardCodeListNum));
                ItemSpecialPriceItems = dbcontext.Items.AsNoTracking().Where(i => ItemSpecialPrices.Any(j => j.ItemCode.Equals(i.ItemCode))).ToList();
            }
            return ItemSpecialPriceItems;
        }
    }
}
