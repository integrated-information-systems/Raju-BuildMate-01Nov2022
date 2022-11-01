using BMSS.Domain.Abstract.SAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.SAP
{
   public class EF_POR1_Repository : I_POR1_Repository
    {
        public decimal GetTotalPOStockBalanceByItemCode(string ItemCode, string WarhouseCode)
        {
            decimal TotalPOStock = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                TotalPOStock = dbcontext.POLines.Include("POHeader").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.WhsCode.Equals(WarhouseCode) && i.POHeader.DocStatus.Equals("O") && i.POHeader.CANCELED.Equals("N") && i.LineStatus.Equals("O")).Sum(x => (decimal?)x.OpenQty) ?? 0;
            }
            return TotalPOStock;
        }
        public decimal GetTotalPOStockBalanceByItemCode(string ItemCode)
        {
            decimal TotalPOStock = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                TotalPOStock = dbcontext.POLines.Include("POHeader").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.POHeader.DocStatus.Equals("O") &&  i.POHeader.CANCELED.Equals("N") && i.LineStatus.Equals("O")).Sum(x => (decimal?)x.Quantity) ?? 0;
            }
            return TotalPOStock;
        }
    }
}
