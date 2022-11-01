using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_SNotesMngt_Repository : I_Notes_Repository
    {
      

        public IEnumerable<object> NotesList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.SNotesMngt.AsNoTracking().ToList();
                }
            }
        }
        public IEnumerable<object> GetNotesListByCardCode(string CardCode)
        {
            IEnumerable<SNotesMngt> CNotesList = null;
            using (var dbcontext = new DomainDb())
            {
                CNotesList = dbcontext.SNotesMngt.AsNoTracking().Where(i => i.CardCode.Equals(CardCode)).ToList();
            }
            return CNotesList;
        }
        public object GetNote(int NoteID)
        {
            SNotesMngt NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.SNotesMngt.AsNoTracking().Where(i => i.NoteID.Equals(NoteID)).FirstOrDefault();
            }
            return NoteObject;
        }
        public void SaveNotes(object NoteObjParam)
        {
            if (NoteObjParam != null)
            {
                SNotesMngt NoteObj = (SNotesMngt)NoteObjParam;
                if (NoteObj.NoteID == 0)
                {

                    using (var dbcontext = new DomainDb())
                    {
                        int DocEntry = dbcontext.SNotesMngt.Count() + 1;
                        NoteObj.NoteID = DocEntry;
                        dbcontext.SNotesMngt.Add(NoteObj);
                        dbcontext.SaveChanges();
                    }
                }
                else
                {
                    using (var dbcontext = new DomainDb())
                    {
                        SNotesMngt dbEntry = dbcontext.SNotesMngt.Find(NoteObj.NoteID);
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
