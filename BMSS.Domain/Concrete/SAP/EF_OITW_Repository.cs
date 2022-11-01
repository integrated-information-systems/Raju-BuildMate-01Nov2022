using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Dynamic;
namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OITW_Repository : I_OITW_Repository
    {

        public decimal GetLocationStockAvailableQty(string ItemCode, string WhsCode)
        {
            decimal AvailableQty = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                AvailableQty = dbcontext.WarehouseStocks.Where(i => i.ItemCode.Equals(ItemCode) &&  i.WhsCode.Equals(WhsCode)).Sum(x => (decimal?)(x.OnHand - x.IsCommited)) ?? 0; ;
            }
            return AvailableQty;
        }
        public IEnumerable<OITW> GetLocationStockDetails(string ItemCode)
        {
            IEnumerable<OITW> LocationStocks = null;
            using (var dbcontext = new EFSapDbContext())
            {
                LocationStocks = dbcontext.WarehouseStocks.Include("Warehouse").Include("Item").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).ToList();
            }
            return LocationStocks;
        }
        public IEnumerable<OITW> GetItemStockDetails()
        {
            IEnumerable<OITW> LocationStocks = null;
            using (var dbcontext = new EFSapDbContext())
            {
                LocationStocks = dbcontext.WarehouseStocks.Include("Item").AsNoTracking().ToList();
            }
            return LocationStocks;
        }     

        public IEnumerable<IGrouping<OITM, OITW>> GetItemStockDetailsWithPagination(int skip, int rowsCount, string search="", string orderBy="")
        {
            IEnumerable<IGrouping<OITM, OITW>> LocationStocks = null;
            using (var dbcontext = new EFSapDbContext())
            {
                if(string.IsNullOrEmpty(search))
                    LocationStocks = dbcontext.WarehouseStocks.Include(x => x.Item).GroupBy(x=> x.Item).OrderBy(orderBy).Skip(skip).Take(rowsCount).ToList();
                    else
                    LocationStocks = dbcontext.WarehouseStocks.Where(x=> 
                    x.ItemCode.Contains(search) 
                    || 
                    x.Item.ItemName.Contains(search) ).Include(x => x.Item).GroupBy(x => x.Item).OrderBy(orderBy).Skip(skip).Take(rowsCount).ToList();
            }
            return LocationStocks;
        }
        
        public int GetItemStockDetailsWithPaginationCount()
        {
            int count = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                count = dbcontext.WarehouseStocks.Include(x => x.Item).GroupBy(x => x.Item).Count();
            }
            return count;
        }
        public decimal SAPAvailableQty(string itemCode, string whsCode)
        {
           return GetLocationStockDetails(itemCode).Where(x => x.WhsCode.Equals(whsCode)).Sum(x => (decimal?)(x.OnHand - x.IsCommited)) ?? 0;
        }
    }
}
