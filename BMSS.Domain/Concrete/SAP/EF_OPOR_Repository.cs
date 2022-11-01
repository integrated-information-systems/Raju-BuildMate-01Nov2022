using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OPOR_Repository : I_OPOR_Repository
    {
     
        private readonly EFSapDbContext sapdbcontext;

        
        public EF_OPOR_Repository()
        {
            sapdbcontext = new EFSapDbContext();
        }
        public IEnumerable<OPOR> GetPODetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "")
        {
            IEnumerable<OPOR> POs = null;

            if (string.IsNullOrEmpty(search))
                POs = sapdbcontext.POHeaders.OrderBy(orderBy).Skip(skip).Take(rowsCount).ToList();
            else
                POs = sapdbcontext.POHeaders.Where(x =>
                x.DocNum.ToString().Contains(search)
                ||
                x.Customer.CardName.Contains(search)
                ||
                x.CardCode.Contains(search)).OrderBy(orderBy)
                .Skip(skip).Take(rowsCount).ToList();

            return POs;

        }
        public void Dispose()
        {
            
            sapdbcontext.Dispose();
        }

        public int GetPOTotalCount()
        {
            int count = 0;

            count = sapdbcontext.POHeaders.Count();

            return count;
        }

        public int GetPODetailsWithPaginationCount(string search = "")
        {
            int count = 0;
            if (string.IsNullOrEmpty(search))
                count = sapdbcontext.POHeaders.Count();
            else
                count = sapdbcontext.POHeaders.Where(x =>
                    x.DocNum.ToString().Contains(search)
                    ||
                    x.Customer.CardName.Contains(search)
                    ||
                    x.CardCode.Contains(search)).Count();


            return count;
        }
    }
}
