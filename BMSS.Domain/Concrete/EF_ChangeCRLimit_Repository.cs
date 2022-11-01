using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_ChangeCRLimit_Repository: I_ChangeCRLimit_Repository
    {
        public IEnumerable<ChangeCRLimit> ChangeCRLimitList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.ChangeCRLimit.AsNoTracking().ToList();
                }
            }
        }
        public bool ChangeCreditLimit(ChangeCRLimit CRLObj)
        {
            bool Result = true;
            try
            {
                using (var dbcontext = new DomainDb())
                {
                    ChangeCRLimit dbEntry = dbcontext.ChangeCRLimit.Find(CRLObj.CardCode);
                    if (dbEntry != null)
                    {
                        dbEntry.SubmitToSAP = true;
                        dbEntry.NewLimit = CRLObj.NewLimit;
                        dbEntry.UpdatedBy = CRLObj.CreatedBy;
                        dbEntry.UpdatedOn = DateTime.Now;

                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        CRLObj.CreatedOn = DateTime.Now;
                        CRLObj.SubmitToSAP = true;
                        dbcontext.ChangeCRLimit.Add(CRLObj);
                        dbcontext.SaveChanges();
                    }
                                         
                }
            }
            catch
            {
                Result = false;
            }

            return Result;
        }
        public ChangeCRLimit GetChangeCRLimitDetails(string CardCode)
        {
            ChangeCRLimit CRLimit = null;
            using (var dbcontext = new DomainDb())
            {
                CRLimit = dbcontext.ChangeCRLimit.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.SubmitToSAP.Equals(true)).FirstOrDefault();
            }
            return CRLimit;
        }
    }
}
