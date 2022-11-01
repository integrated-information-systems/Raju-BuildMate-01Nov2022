using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_CashSalesCreditDocLine_Repository : I_CashSalesCreditDocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_CashSalesCreditDocLine_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<CashSalesCreditDocLs> CashSalesCreditLineList
        {
            get
            {                
                    return dbcontext.CashSalesCreditDocLs.AsNoTracking().ToList();
                
            }
        }
        public IEnumerable<CashSalesCreditDocLs> GetCSCLinesByDocNum(string DocNum)
        {
            return dbcontext.CashSalesCreditDocLs.Include("CashSalesCreditDocH").AsNoTracking().Where(x => x.CashSalesCreditDocH.DocNum.Equals(DocNum)).OrderBy(x => x.LineNum).ToList();
        }
        public IEnumerable<CashSalesCreditDocPays> GetCSCPayLinesByDocNum(string DocNum)
        {
            return dbcontext.CashSalesCreditDocPays.Include("CashSalesCreditDocH").AsNoTracking().Where(x => x.CashSalesCreditDocH.DocNum.Equals(DocNum)).OrderBy(x => x.LineNum).ToList();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }

        public IEnumerable<CashSalesCreditDocLs> GetCashSalesCreditLinesByItemCode(string ItemCode, string CardCode)
        {

            
                return dbcontext.CashSalesCreditDocLs.Include("CashSalesCreditDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.CashSalesCreditDocH.CardCode.Equals(CardCode)).ToList();

        }
        public IEnumerable<CashSalesCreditDocLs> GetCashSalesCreditLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50)
        {


            return dbcontext.CashSalesCreditDocLs.Include("CashSalesCreditDocH").Where(x => x.ItemCode.Equals(ItemCode) && x.CashSalesCreditDocH.CardCode.Equals(CardCode)).OrderByDescending(x=> x.CashSalesCreditDocH.DocDate).Take(noOfRecords).ToList();

        }
    }
}
