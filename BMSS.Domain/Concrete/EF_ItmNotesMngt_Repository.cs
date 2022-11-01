using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_ItmNotesMngt_Repository : I_ItmNotes_Repository
    {
        public IEnumerable<object> GetNotesListByItemCode(string ItemCode)
        {
            IEnumerable<INotesMngt> ItmNotesList = null;
            using (var dbcontext = new DomainDb())
            {
                ItmNotesList = dbcontext.INotesMngt.AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).ToList();
            }
            return ItmNotesList;
        }
        public object GetNote(int NoteID)
        {
            INotesMngt NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.INotesMngt.AsNoTracking().Where(i => i.NoteID.Equals(NoteID)).FirstOrDefault();
            }
            return NoteObject;
        }
        public void SaveNotes(object NoteObjParam)
        {
            if (NoteObjParam != null)
            {
                INotesMngt NoteObj = (INotesMngt)NoteObjParam;
                if (NoteObj.NoteID == 0)
                {

                    using (var dbcontext = new DomainDb())
                    {
                        int DocEntry = dbcontext.INotesMngt.Count() + 1;
                        NoteObj.NoteID = DocEntry;
                        dbcontext.INotesMngt.Add(NoteObj);
                        dbcontext.SaveChanges();
                    }
                }
                else
                {
                    using (var dbcontext = new DomainDb())
                    {
                        INotesMngt dbEntry = dbcontext.INotesMngt.Find(NoteObj.NoteID);
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
