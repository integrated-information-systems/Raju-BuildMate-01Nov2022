using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OCPR_Repository
    {
        string GetSecondaryContactDetails(string CardCode, string ContactPersonName);
        OCPR GetContactPersonDetails(Int32 CntctCode);
        IEnumerable<OCPR> GetContactPersons(string CardCode);
        bool IsValidCode(Int32 Code);
    }
}
