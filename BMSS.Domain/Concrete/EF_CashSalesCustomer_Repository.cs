using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_CashSalesCustomer_Repository: I_CashSalesCustomer_Repository
    {
        public IEnumerable<CashSalesCustomerMaster> CashSalesCustomerList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.CashSalesCustomerMaster.AsNoTracking().ToList();
                }
            }
        }
        public CashSalesCustomerMaster GetByDocEntry(long DocEntry)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.CashSalesCustomerMaster.AsNoTracking().Where(x => x.DocEntry.Equals(DocEntry)).FirstOrDefault();
            }
        }
        public CashSalesCustomerMaster GetByTelephoneNo(string CustomerID)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.CashSalesCustomerMaster.AsNoTracking().Where(x => x.CustomerID.Equals(CustomerID)).FirstOrDefault();
            }
        }
        public bool DeleteByTelephoneNo(string CustomerID)
        {
            bool Result = true;
            try
            {
                using (var dbcontext = new DomainDb())
                {
                    CashSalesCustomerMaster dbEntry = dbcontext.CashSalesCustomerMaster.Find(CustomerID);
                    if (dbEntry != null)
                    {
                        dbcontext.CashSalesCustomerMaster.Remove(dbEntry);
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        Result = false;
                    }
                }
            }
            catch
            {
                Result = false;
            }
            return Result;
        }
        public bool DeleteByDocEntry(long DocEntry)
        {
            bool Result = true;
            try
            {
                using (var dbcontext = new DomainDb())
                {
                    CashSalesCustomerMaster dbEntry = dbcontext.CashSalesCustomerMaster.Find(DocEntry);
                    if (dbEntry != null)
                    {
                        dbcontext.CashSalesCustomerMaster.Remove(dbEntry);
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        Result = false;
                    }
                }
            }
            catch
            {
                Result = false;
            }
            return Result;
        }
      
        public bool AddUpdateCashSalesCustomer(CashSalesCustomerMaster CSCObj)
        {
            bool Result = true;
            try
            {
                using (var dbcontext = new DomainDb())
                {
                    CashSalesCustomerMaster dbEntry = dbcontext.CashSalesCustomerMaster.Find(CSCObj.DocEntry);
                    if (dbEntry != null)
                    {
                        dbEntry.CustomerName = CSCObj.CustomerName;
                        dbEntry.AddressLine1 = CSCObj.AddressLine1;
                        dbEntry.AddressLine2 = CSCObj.AddressLine2;
                        dbEntry.AddressLine3 = CSCObj.AddressLine3;
                        dbEntry.AddressLine4 = CSCObj.AddressLine4;
                        dbEntry.SlpCode = CSCObj.SlpCode;
                        dbEntry.SlpName = CSCObj.SlpName;
                        dbEntry.Country = CSCObj.Country;
                        dbEntry.PostalCode = CSCObj.PostalCode;
                        dbEntry.UpdatedBy = CSCObj.UpdatedBy;
                        dbEntry.UpdatedOn = DateTime.Now;

                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        CSCObj.CreatedOn = DateTime.Now;
                        CSCObj.UpdatedOn = null;
                        dbcontext.CashSalesCustomerMaster.Add(CSCObj);
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
    }
}
