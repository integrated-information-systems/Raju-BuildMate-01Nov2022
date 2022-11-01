using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_Notes_Repository
    {         
        IEnumerable<Object> NotesList { get; }
        void SaveNotes(Object NoteObj);
        Object GetNote(int NoteID);
        IEnumerable<Object> GetNotesListByCardCode(string CardCode);
    }
}
