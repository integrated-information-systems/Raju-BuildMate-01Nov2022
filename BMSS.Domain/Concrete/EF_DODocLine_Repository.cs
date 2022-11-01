using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_DODocLine_Repository : I_DODocLs_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_DODocLine_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<DODocLs> DOLineList
        {
            get
            {               
              return dbcontext.DODocLs.AsNoTracking().ToList();                 
            }
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
        public IEnumerable<DODocLs> GetDOLinesByDocNum(string DocNum)
        {
            return dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.DODocH.DocNum.Equals(DocNum)).OrderBy(x=> x.LineNum).ToList();
        }
        public IEnumerable<DODocLs> GetDOLinesByItemCode(string ItemCode, string CardCode)
        {

            return dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.DODocH.CardCode.Equals(CardCode)).ToList();            
        }
        public IEnumerable<DODocLs> GetDOLinesByItemCodeWithLimit(string ItemCode, string CardCode, int noOfRecords = 50)
        {

            return dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.DODocH.CardCode.Equals(CardCode)).OrderByDescending(x=> x.DODocH.DocDate).Take(noOfRecords).ToList();
        }

    }
}
