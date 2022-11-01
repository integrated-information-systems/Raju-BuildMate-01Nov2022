using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_StockIssueDocLine_Repository : I_StockIssueDocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_StockIssueDocLine_Repository()
        {
            dbcontext = new DomainDb();

        }
        public IEnumerable<StockIssueDocLs> StockIssueLineList
        {
            get
            {                
                    return dbcontext.StockIssueDocLs.AsNoTracking().ToList();                
            }
        }

        public IEnumerable<StockIssueDocLs> GetStockIssueLinesByItemCode(string ItemCode)
        {
             
                return dbcontext.StockIssueDocLs.Include("StockIssueDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).ToList();            
        }
        public IEnumerable<StockIssueDocLs> GetStockIssueLinesByItemCodeWithLimit(string ItemCode, int noOfRecords = 50)
        {
            return dbcontext.StockIssueDocLs.Include("StockIssueDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).OrderByDescending(x=> x.StockIssueDocH.DocDate).Take(noOfRecords).ToList();
        }
        public IEnumerable<StockIssueDocLs> GetSILinesByDocNum(string DocNum)
        {
            return dbcontext.StockIssueDocLs.Include("StockIssueDocH").AsNoTracking().Where(x => x.StockIssueDocH.DocNum.Equals(DocNum)).OrderBy(x=> x.LineNum).ToList();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}
