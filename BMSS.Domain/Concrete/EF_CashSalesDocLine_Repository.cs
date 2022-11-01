using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_CashSalesDocLine_Repository : I_CashSalesDocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_CashSalesDocLine_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<CashSalesDocLs> CashSalesLineList
        {
            get
            {
             
                    return dbcontext.CashSalesDocLs.AsNoTracking().ToList();               
            }
        }
        public IEnumerable<CashSalesDocLs> GetCSLinesByDocNum(string DocNum)
        {
            return dbcontext.CashSalesDocLs.Include("CashSalesDocH").AsNoTracking().Where(x => x.CashSalesDocH.DocNum.Equals(DocNum)).OrderBy(x=> x.LineNum).ToList();
        }
        public IEnumerable<CashSalesDocPays> GetCSPayLinesByDocNum(string DocNum)
        {
            return dbcontext.CashSalesDocPays.Include("CashSalesDocH").AsNoTracking().Where(x => x.CashSalesDocH.DocNum.Equals(DocNum)).OrderBy(x => x.LineNum).ToList();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }

        public IEnumerable<CashSalesDocLs> GetCashSalesLinesByItemCode(string ItemCode, string CardCode)
        {

            
                return dbcontext.CashSalesDocLs.Include("CashSalesDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.CashSalesDocH.CardCode.Equals(CardCode)).ToList();
            
        }
        public IEnumerable<CashSalesDocLs> GetCashSalesLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50)
        {


            return dbcontext.CashSalesDocLs.Include("CashSalesDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.CashSalesDocH.CardCode.Equals(CardCode)).OrderByDescending(x=> x.CashSalesDocH.DocDate).Take(noOfRecords).ToList();

        }
    }
}
