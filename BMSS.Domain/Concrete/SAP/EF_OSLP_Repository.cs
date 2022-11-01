using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OSLP_Repository : I_OSLP_Repository
    {
        public IEnumerable<OSLP> SalesPersons
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.SalesPersons.AsNoTracking().ToList();
                }
            }
        }
        public bool IsValidCode(Int32 Code)
        {
            bool Result = true;
            OSLP SalesPerson = null;
            using (var dbcontext = new EFSapDbContext())
            {
                SalesPerson = dbcontext.SalesPersons.AsNoTracking().Where(i => i.SlpCode.Equals(Code)).FirstOrDefault();
            }
            if (SalesPerson.Equals(null))
            {
                Result = false;
            }
            return Result;
        }
        public string GetSalesPersonName(Int32 Code)
        {
            string Result = string.Empty;
            OSLP SalesPerson = null;
            using (var dbcontext = new EFSapDbContext())
            {
                SalesPerson = dbcontext.SalesPersons.AsNoTracking().Where(i => i.SlpCode.Equals(Code)).FirstOrDefault();
            }
            if (!SalesPerson.Equals(null))
            {
                Result = SalesPerson.SlpName;
            }
            return Result;
        }
    }
}
