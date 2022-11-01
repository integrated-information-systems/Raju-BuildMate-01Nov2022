using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BMSS.Domain.Concrete
{
    public class EF_ItmNotesAll_Repository : I_ItmNotes_Repository
    {
        public IEnumerable<object> GetNotesListByItemCode(string ItemCode)
        {
            IEnumerable<INotesAll> ItmNotesList = null;
            using (var dbcontext = new DomainDb())
            {
                ItmNotesList = dbcontext.INotesAll.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).ToList();
            }
            return ItmNotesList;
        }
        public object GetNote(int NoteID)
        {
            INotesAll NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.INotesAll.AsNoTracking().Where(i => i.NoteID.Equals(NoteID)).FirstOrDefault();
            }
            return NoteObject;
        }
        public void SaveNotes(object NoteObjParam)
        {
            if (NoteObjParam != null)
            {
                INotesAll NoteObj = (INotesAll)NoteObjParam;
                if (NoteObj.NoteID == 0)
                {

                    using (var dbcontext = new DomainDb())
                    {
                        int DocEntry = dbcontext.INotesAll.Count() + 1;
                        NoteObj.NoteID = DocEntry;
                        dbcontext.INotesAll.Add(NoteObj);
                        dbcontext.SaveChanges();
                    }
                }
                else
                {
                    using (var dbcontext = new DomainDb())
                    {
                        INotesAll dbEntry = dbcontext.INotesAll.Find(NoteObj.NoteID);
                        if (dbEntry != null)
                        {
                            dbEntry.Note = NoteObj.Note;
                            dbEntry.ItemCode = NoteObj.ItemCode;
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
