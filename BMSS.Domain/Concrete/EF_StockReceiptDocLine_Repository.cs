using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_StockReceiptDocLine_Repository : I_StockReceiptDocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_StockReceiptDocLine_Repository()
        {
            dbcontext = new DomainDb();

        }
        public IEnumerable<StockReceiptDocLs> StockReceiptLineList
        {
            get
            {                
                    return dbcontext.StockReceiptDocLs.AsNoTracking().ToList();                
            }
        }
        public IEnumerable<StockReceiptDocLs> GetSRLinesByDocNum(string DocNum)
        {
            return dbcontext.StockReceiptDocLs.Include("StockReceiptDocH").AsNoTracking().Where(x => x.StockReceiptDocH.DocNum.Equals(DocNum)).OrderBy(x => x.LineNum).ToList();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }
        public IEnumerable<StockReceiptDocLs> GetStockReceiptLinesByItemCode(string ItemCode)
        {            
           return dbcontext.StockReceiptDocLs.Include("StockReceiptDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).ToList();            
        }
        public IEnumerable<StockReceiptDocLs> GetStockReceiptLinesByItemCodeWithLimit(string ItemCode, int noOfRecords=50)
        {
            return dbcontext.StockReceiptDocLs.Include("StockReceiptDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).OrderByDescending(x=> x.StockReceiptDocH.DocDate).Take(noOfRecords).ToList();
        }
    }
}
