using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OCRD_Repository
    {
       
        IEnumerable<OCRD> ActiveCustomers { get; }
        IEnumerable<OCRD> Customers { get; }
        OCRD GetCustomerDetails(string CardCode);
        IEnumerable<OCRD> ActiveSuppliers { get; }
        IEnumerable<OCRD> Suppliers { get; }
        OCRD GetSupplierDetails(string CardCode);
        bool IsValidCustomerCode(string CardCode);
        bool IsValidSupplierCode(string CardCode);
        AgingBucket GetCustomerAgingBucket(string CardCode);
        AgingBucket GetCustomerAgingBucketNew(string CardCode, string SlpName);
        List<ORPC> GetSupplierARMemos(string CardCode);
        List<OPCH> GetSupplierAPInvoices(string CardCode);
        List<CustomerList> GetListofCustomers();
    }
}
