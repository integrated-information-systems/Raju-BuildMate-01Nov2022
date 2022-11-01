using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OCPR_Repository : I_OCPR_Repository
    {
        
        public OCPR GetContactPersonDetails(Int32 CntctCode)
        {
            OCPR ContactPerson = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ContactPerson = dbcontext.ContactPersons.AsNoTracking().Where(i => i.CntctCode.Equals(CntctCode)).FirstOrDefault();
            }
            return ContactPerson;
        }
        public bool IsValidCode(Int32 Code)
        {
            bool Result = true;
            OCPR ContactPerson = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ContactPerson = dbcontext.ContactPersons.AsNoTracking().Where(i => i.CntctCode.Equals(Code)).FirstOrDefault();
            }
            if (ContactPerson.Equals(null))
            {
                Result = false;
            }
            return Result;
        }
        public string GetSecondaryContactDetails(string CardCode, string ContactPersonName)
        {
            string Result = string.Empty;
            OCPR ContactPerson = null;
            if(!CardCode.Equals(null) && !ContactPersonName.Equals(null)) { 
                using (var dbcontext = new EFSapDbContext())
                {
                    ContactPerson = dbcontext.ContactPersons.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.Name.Equals(ContactPersonName)).FirstOrDefault();
                }
                if (!ContactPerson.Equals(null))
                {
                    Result = ContactPerson.Notes1;
                }
            }
            return Result;
        }
        public IEnumerable<OCPR> GetContactPersons(string CardCode)
        {
            IEnumerable<OCPR> ContactPersons = null;
            using (var dbcontext = new EFSapDbContext())
            {
                ContactPersons = dbcontext.ContactPersons.AsNoTracking().Where(i => i.CardCode.Equals(CardCode)).ToList();
            }
            return ContactPersons;
        }
    }
}
