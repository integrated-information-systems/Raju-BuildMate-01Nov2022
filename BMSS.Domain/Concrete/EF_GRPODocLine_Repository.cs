using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace BMSS.Domain.Concrete
{
    public class EF_GRPODocLine_Repository : I_GRPODocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_GRPODocLine_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<GRPODocLs> GRPOLineList
        {
            get
            {
                 
                    return dbcontext.GRPODocLs.AsNoTracking().ToList();
                
            }
        }

        public IEnumerable<GRPODocLs> GetGRPOLinesByItemCode(string ItemCode)
        {

           
                return dbcontext.GRPODocLs.Include(x => x.GRPODocH).AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).ToList();
            
        }

        public decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode, string WarhouseCode)
        {
            decimal TotalGRPOStock = 0;
            
                TotalGRPOStock = dbcontext.GRPODocLs.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.Location.Equals(WarhouseCode)).Sum(x => (decimal?)x.Qty) ?? 0;
            
            return TotalGRPOStock;
        }
        public decimal GetTotalGRPOStockBalanceByItemCode(string ItemCode)
        {
            decimal TotalGRPOStock = 0;
            
                TotalGRPOStock = dbcontext.GRPODocLs.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).Sum(x => (decimal?)x.Qty) ?? 0;
            
            return TotalGRPOStock;
        }
        public decimal GetLastPriceByItemCode(string ItemCode)
        {
            decimal LastPrice = 0;
             
                LastPrice = dbcontext.GRPODocLs.Include(x=> x.GRPODocH).AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).OrderByDescending(x=>x.GRPODocH.DocDate).Select(x => x.UnitPrice).FirstOrDefault();
            
            return LastPrice;
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }
        public  IEnumerable<GRPODocLs> GetGRPOLinesByDocNum(string DocNum)
        {
            return dbcontext.GRPODocLs.Include("GRPODocH").AsNoTracking().Where(x => x.GRPODocH.DocNum.Equals(DocNum)).OrderBy(x=> x.LineNum).ToList();
        }
    }
}
