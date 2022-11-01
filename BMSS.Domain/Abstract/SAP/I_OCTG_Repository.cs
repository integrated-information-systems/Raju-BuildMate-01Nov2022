using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OCTG_Repository
    {
        IEnumerable<OCTG> PaymentTerms { get; }

        bool IsValidCode(short Code);
        string GetPaymentTermName(short Code);
        Int16 GetExtraDays(short Code);
    }
}
