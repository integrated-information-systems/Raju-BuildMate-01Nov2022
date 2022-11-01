using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_StockTransDocLine_Repository : I_StockTransDocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_StockTransDocLine_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<StockTransDocLs> StockTransLineList
        {
            get
            {
               
                    return dbcontext.StockTransDocLs.AsNoTracking().ToList();
                
            }
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }

        public IEnumerable<StockTransDocLs> GetStockTransLinesByItemCode(string ItemCode)
        {

             
                return dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).ToList();
           
        }
        public IEnumerable<StockTransDocLs> GetStockTransLinesByItemCodeWithLimit(string ItemCode, int noOfRecords = 50)
        {
            
                return dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).OrderByDescending(x=> x.StockTransDocH.DocDate).Take(noOfRecords).ToList();
            

        }
        public IEnumerable<StockTransDocLs> GetSTLinesByDocNum(string DocNum)
        {
            return dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.StockTransDocH.DocNum.Equals(DocNum)).OrderBy(x=> x.LineNum).ToList();
        }

        
    }
}
