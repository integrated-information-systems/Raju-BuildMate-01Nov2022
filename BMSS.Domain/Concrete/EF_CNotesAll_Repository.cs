using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_CNotesAll_Repository : I_Notes_Repository
    {
        

        public IEnumerable<object> NotesList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.CNotesAll.AsNoTracking().ToList();
                }
            }
        }
        public IEnumerable<object> GetNotesListByCardCode(string CardCode  )
        {
            IEnumerable<CNotesAll> CNotesList = null;
            using (var dbcontext = new DomainDb())
            {
                CNotesList = dbcontext.CNotesAll.AsNoTracking().Where(i => i.CardCode.Equals(CardCode)).ToList();
            }
            return CNotesList;
        }
        public object GetNote(int NoteID)
        {
            CNotesAll NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.CNotesAll.AsNoTracking().Where(i => i.NoteID.Equals(NoteID)).FirstOrDefault();
            }
            return NoteObject;
        }
        public void SaveNotes(object NoteObjParam)
        {
            if (NoteObjParam != null)
            {
                CNotesAll NoteObj = (CNotesAll)NoteObjParam;
                if (NoteObj.NoteID == 0)
                {
                   
                    using (var dbcontext = new DomainDb())
                    {
                        int DocEntry = dbcontext.CNotesAll.Count() + 1;
                        NoteObj.NoteID = DocEntry;
                        dbcontext.CNotesAll.Add(NoteObj);
                        dbcontext.SaveChanges();
                    }
                }
                else
                {
                    using (var dbcontext = new DomainDb())
                    {
                        CNotesAll dbEntry = dbcontext.CNotesAll.Find(NoteObj.NoteID);
                        if (dbEntry != null)
                        {
                            dbEntry.Note = NoteObj.Note;
                            dbEntry.CardCode = NoteObj.CardCode;                            
                            dbEntry.UpdatedBy = NoteObj.UpdatedBy;
                            dbEntry.UpdatedOn = DateTime.Now;

                            dbcontext.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
