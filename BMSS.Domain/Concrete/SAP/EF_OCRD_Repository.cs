using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OCRD_Repository : I_OCRD_Repository
    {
         
        public IEnumerable<OCRD> Customers
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.Customers.Include("CustomerGroup").Include("SalesPerson").AsNoTracking().Where(i => i.CardType.Equals("C")).ToList();
                }
            }
        }
        public bool IsValidCustomerCode(string CardCode)
        {
            bool Result = true;
            OCRD Customer = null;
            using (var dbcontext = new EFSapDbContext())
            {
                Customer = dbcontext.Customers.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.CardType.Equals("C")).FirstOrDefault();
            }
            if (Customer.Equals(null))
            {
                Result = false;
            }
            return Result;
        }
        public bool IsValidSupplierCode(string CardCode)
        {
            bool Result = true;
            OCRD Customer = null;
            using (var dbcontext = new EFSapDbContext())
            {
                Customer = dbcontext.Customers.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.CardType.Equals("S")).FirstOrDefault();
            }
            if (Customer.Equals(null))
            {
                Result = false;
            }
            return Result;
        }
        public IEnumerable<OCRD> ActiveCustomers
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.Customers.Include("CustomerGroup").Include("SalesPerson").AsNoTracking().Where(i => i.CardType.Equals("C") && i.frozenFor.Equals("N") && i.validTo == null).ToList();

                }
            }
        }
        public IEnumerable<OCRD> Suppliers
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.Customers.Include("CustomerGroup").Include("SalesPerson").AsNoTracking().Where(i => i.CardType.Equals("S")).ToList();
                }
            }
        }
        public IEnumerable<OCRD> ActiveSuppliers
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.Customers.Include("CustomerGroup").Include("SalesPerson").AsNoTracking().Where(i => i.CardType.Equals("S") && i.frozenFor.Equals("N") && i.validTo == null).ToList();                   
                }
            }
        }
        public OCRD GetCustomerDetails(string CardCode)
        {
            OCRD Customer = null;
            using (var dbcontext = new EFSapDbContext())
            {
                Customer = dbcontext.Customers.Include("CustomerAddress").Include("CustomerAddress.CountryCodes").Include("CustomerGroup").Include("SalesPerson").Include("ARMemos").Include("ARInvoices").Include("PaymentTerm").Include("Pricelist").AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.CardType.Equals("C")).FirstOrDefault();
                
            }
            return Customer;
        }
        public OCRD GetSupplierDetails(string CardCode)
        {
            OCRD Customer = null;
            using (var dbcontext = new EFSapDbContext())
            {
                //Customer = dbcontext.Customers.Include("CustomerAddress").Include("CustomerAddress.CountryCodes").Include("CustomerGroup").Include("SalesPerson").Include("APMemos").Include("APInvoices").Include("PaymentTerm").Include("Pricelist").AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.CardType.Equals("S")).FirstOrDefault();
                Customer = dbcontext.Customers.Include("CustomerAddress").Include("CustomerAddress.CountryCodes").Include("CustomerGroup").Include("SalesPerson").Include("PaymentTerm").AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.CardType.Equals("S")).FirstOrDefault();
            }
            return Customer;
        }
        public List<ORPC> GetSupplierARMemos(string CardCode)
        {
            List<ORPC> collection = null;
            using (var dbcontext = new EFSapDbContext())
            {
                collection = dbcontext.APCreditMemos.Where(x => x.CardCode.Equals(CardCode)).ToList();
            }

            return collection;
        }
        public List<OPCH> GetSupplierAPInvoices(string CardCode)
        {
            List<OPCH> collection = null;
            using (var dbcontext = new EFSapDbContext())
            {
                collection = dbcontext.APInvoiceHeaders.Where(x => x.CardCode.Equals(CardCode)).ToList();
            }

            return collection;
        }
        public AgingBucket GetCustomerAgingBucketNew(string cardCode, string SlpName)
        {
            AgingBucket agingBucket = new AgingBucket();
            using (var dbcontext = new EFSapDbContext())
            {
                var CardCodeFr = new SqlParameter();
                CardCodeFr.ParameterName = "@CardCodeFr";
                CardCodeFr.SqlDbType = SqlDbType.VarChar;
                CardCodeFr.Direction = ParameterDirection.Input;
                CardCodeFr.Value = cardCode;
                var CardCodeTo = new SqlParameter();
                CardCodeTo.ParameterName = "@CardCodeTo";
                CardCodeTo.SqlDbType = SqlDbType.VarChar;
                CardCodeTo.Direction = ParameterDirection.Input;
                CardCodeTo.Value = cardCode;
                var SalesPerson = new SqlParameter();
                SalesPerson.ParameterName = "@SalesPerson";
                SalesPerson.SqlDbType = SqlDbType.VarChar;
                SalesPerson.Direction = ParameterDirection.Input;
                //SalesPerson.Value = "All";
                SalesPerson.Value = SlpName;
                var AgeDateTo = new SqlParameter();
                AgeDateTo.ParameterName = "@AgeDateTo"; 
                AgeDateTo.SqlDbType = SqlDbType.DateTime;
                AgeDateTo.Direction = ParameterDirection.Input;
                AgeDateTo.Value = DateTime.Now;

                IEnumerable<SOA> items = dbcontext.Database.SqlQuery<SOA>(@"EXEC [dbo].[IISsp_SOA_20210427] @CardCodeFr, @CardCodeTo,  @SalesPerson, @AgeDateTo ", CardCodeFr, CardCodeTo, SalesPerson, AgeDateTo ).ToList();

                agingBucket.Current = items.Where(x => (((DateTime.Now.Year - x.Refdate.Year) * 12) + (DateTime.Now.Month - x.Refdate.Month)) == 0).Sum(x => x.BalDueCred);
                agingBucket.Months1 = items.Where(x => (((DateTime.Now.Year - x.Refdate.Year) * 12) + (DateTime.Now.Month - x.Refdate.Month)) == 1).Sum(x => x.BalDueCred);
                agingBucket.Months2 = items.Where(x => (((DateTime.Now.Year - x.Refdate.Year) * 12) + (DateTime.Now.Month - x.Refdate.Month)) == 2).Sum(x => x.BalDueCred);
                agingBucket.Months3 = items.Where(x => (((DateTime.Now.Year - x.Refdate.Year) * 12) + (DateTime.Now.Month - x.Refdate.Month)) == 3).Sum(x => x.BalDueCred);
                agingBucket.Months4 = items.Where(x => (((DateTime.Now.Year - x.Refdate.Year) * 12) + (DateTime.Now.Month - x.Refdate.Month)) == 4).Sum(x => x.BalDueCred);
                agingBucket.Months5 = items.Where(x => (((DateTime.Now.Year - x.Refdate.Year) * 12) + (DateTime.Now.Month - x.Refdate.Month)) > 4).Sum(x => x.BalDueCred);
                agingBucket.OutStandingBalance = items.Sum(x => x.BalDueCred);

            }
            return agingBucket;
        }
        public AgingBucket GetCustomerAgingBucket(string CardCode)
        {
        
            AgingBucket agingBucket = new AgingBucket();
            using (var dbcontext = new EFSapDbContext())
            {
                agingBucket = dbcontext.Database.SqlQuery<AgingBucket>(
                    @"SELECT
Sum(CASE

WHEN(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 < 31 THEN

CASE

WHEN balduecred < > 0 THEN balduecred * -1

ELSE balduedeb

END

END) AS 'Current',

Sum(CASE

WHEN(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 > 30

AND(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 < 61 THEN

CASE

WHEN balduecred < > 0 THEN balduecred * -1

ELSE balduedeb

END

END) AS 'Months1',

SUM(CASE

WHEN(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 > 60

AND(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 < 91 THEN

CASE

WHEN balduecred < > 0 THEN balduecred * -1

ELSE balduedeb

END

END) AS 'Months2',

SUM(CASE

WHEN(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 > 90

AND(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 < 121 THEN

CASE

WHEN balduecred < > 0 THEN balduecred * -1

ELSE balduedeb

END

END) AS 'Months3',

SUM(CASE

WHEN(DATEDIFF(DD, RefDate, Current_Timestamp)) + 1 > 120

THEN

CASE

WHEN balduecred <> 0 THEN balduecred * -1

ELSE balduedeb

END

END) AS 'Months4'

FROM JDT1 T0

INNER JOIN OCRD T1

ON T0.ShortName = T1.CardCode

AND T1.CardType = 'C'

WHERE

T0.IntrnMatch = '0'

AND T0.BalDueDeb != T0.BalDueCred

AND t1.CardCode = @CardCode

 ", new SqlParameter("@CardCode", CardCode)).FirstOrDefault();
            }

            return agingBucket;
        }

        public List<CustomerList> GetListofCustomers()
        {
            List<CustomerList> lstCust = new List<CustomerList>();
            using (var dbcontext = new EFSapDbContext())
            {
                lstCust = dbcontext.Database.SqlQuery<CustomerList>(@"EXEC [dbo].[IISsp_SOA_GetListOfCustomers] ").ToList();
            }
            return lstCust;
        }
    }
}
