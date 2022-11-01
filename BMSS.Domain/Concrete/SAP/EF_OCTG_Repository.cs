using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OCTG_Repository : I_OCTG_Repository
    {
        public IEnumerable<OCTG> PaymentTerms
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.PaymentTerms.AsNoTracking().ToList();
                }
            }
        }
        public bool IsValidCode(short Code)
        {
            bool Result = true;
            OCTG PaymentTerm = null;
            using (var dbcontext = new EFSapDbContext())
            {
                PaymentTerm = dbcontext.PaymentTerms.AsNoTracking().Where(i => i.GroupNum.Equals(Code)).FirstOrDefault();
            }
            if (PaymentTerm.Equals(null))
            {
                Result = false;
            }
            return Result;
        }
        public string GetPaymentTermName(short Code)
        {
            string Result = string.Empty;
            OCTG PaymentTerm = null;
            using (var dbcontext = new EFSapDbContext())
            {
                PaymentTerm = dbcontext.PaymentTerms.AsNoTracking().Where(i => i.GroupNum.Equals(Code)).FirstOrDefault();
            }
            if (!PaymentTerm.Equals(null))
            {
                Result = PaymentTerm.PymntGroup;
            }
            return Result;
        }
        public Int16 GetExtraDays(short Code)
        {
            Int16 Result = 0;
            OCTG PaymentTerm = null;
            using (var dbcontext = new EFSapDbContext())
            {
                PaymentTerm = dbcontext.PaymentTerms.AsNoTracking().Where(i => i.GroupNum.Equals(Code)).FirstOrDefault();
            }
            if (!PaymentTerm.Equals(null))
            {
                Result = PaymentTerm.ExtraDays;
            }
            return Result;
        }
    }
}
