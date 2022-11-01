using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_PDN1_Repository : I_PDN1_Repository
    {

        public decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode, string WarhouseCode)
        {
            decimal TotalGRPOStock = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                TotalGRPOStock = dbcontext.GRPOLines.Include("GRPOHeader").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.WhsCode.Equals(WarhouseCode)).Sum(x => (decimal?)x.Quantity) ?? 0;
            }
            return TotalGRPOStock;
        }
        public decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode)
        {
            decimal TotalGRPOStock = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                TotalGRPOStock = dbcontext.GRPOLines.Include("GRPOHeader").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).Sum(x => (decimal?)x.Quantity) ?? 0;
            }
            return TotalGRPOStock;
        }
        public decimal GetLastUnitPriceFromGRPO(string ItemCode, string CardCode)
        {
            decimal LastUnitPrice = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                LastUnitPrice = dbcontext.GRPOLines.Include("GRPOHeader").OrderByDescending(x=> x.GRPOHeader.DocDate).Where(i => i.ItemCode.Equals(ItemCode)).Select(x => x.Price).FirstOrDefault();
            }
            return LastUnitPrice;
        }
    }
}
