using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_SNotesAll_Repository: I_Notes_Repository
    {
       
        public IEnumerable<object> NotesList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.SNotesAll.AsNoTracking().ToList();
                }
            }
        }
        public IEnumerable<object> GetNotesListByCardCode(string CardCode)
        {
            IEnumerable<SNotesAll> SNotesList = null;
            using (var dbcontext = new DomainDb())
            {
                SNotesList = dbcontext.SNotesAll.AsNoTracking().Where(i => i.CardCode.Equals(CardCode)).ToList();
            }
            return SNotesList;
        }
        public object GetNote(int NoteID)
        {
            SNotesAll NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.SNotesAll.AsNoTracking().Where(i => i.NoteID.Equals(NoteID)).FirstOrDefault();
            }
            return NoteObject;
        }
        public void SaveNotes(object NoteObjParam)
        {
            if (NoteObjParam != null)
            {
                SNotesAll NoteObj = (SNotesAll)NoteObjParam;
                if (NoteObj.NoteID == 0)
                {

                    using (var dbcontext = new DomainDb())
                    {
                        int DocEntry = dbcontext.SNotesAll.Count() + 1;
                        NoteObj.NoteID = DocEntry;
                        dbcontext.SNotesAll.Add(NoteObj);
                        dbcontext.SaveChanges();
                    }
                }
                else
                {
                    using (var dbcontext = new DomainDb())
                    {
                        SNotesAll dbEntry = dbcontext.SNotesAll.Find(NoteObj.NoteID);
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
