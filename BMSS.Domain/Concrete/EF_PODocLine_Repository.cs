using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_PODocLine_Repository : I_PODocLs_Repository
    {
        public IEnumerable<PODocLs> POLineList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.PODocLs.AsNoTracking().ToList();
                }
            }
        }

        public IEnumerable<PODocLs> GetPOLinesByItemCode(string ItemCode, string CardCode)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocLs.Include("PODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.PODocH.CardCode.Equals(CardCode)).ToList();
            }
        }
        public IEnumerable<PODocLs> GetPOLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocLs.Include("PODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.PODocH.CardCode.Equals(CardCode)).OrderByDescending(x=> x.PODocH.DocDate).Take(noOfRecords).ToList();
            }
        }

        public decimal GetTotalPOStockBalanceByItemCode(string ItemCode, string WarhouseCode)
        {
            decimal TotalPOStock = 0;
            //using (var dbcontext = new DomainDb())
            //{
            //    TotalPOStock = dbcontext.PODocLs.Include("PODocH").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode) && i.Location.Equals(WarhouseCode)).Sum(x => (decimal?)x.OpenQty) ?? 0;
            //}
            // PO used anymore
            return TotalPOStock;
        }
        public decimal GetTotalPOStockBalanceByItemCode(string ItemCode)
        {
            decimal TotalPOStock = 0;
            //using (var dbcontext = new DomainDb())
            //{
            //    TotalPOStock = dbcontext.PODocLs.Include("PODocH").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).Sum(x => (decimal?)x.OpenQty) ?? 0;
            //}
            // PO used anymore
            return TotalPOStock;
        }
    }
}
