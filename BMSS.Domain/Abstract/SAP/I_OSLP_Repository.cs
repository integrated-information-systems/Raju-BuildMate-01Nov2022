using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OSLP_Repository
    {
        IEnumerable<OSLP> SalesPersons { get; }

        bool IsValidCode(Int32 Code);

        string GetSalesPersonName(Int32 Code);
    }
}
