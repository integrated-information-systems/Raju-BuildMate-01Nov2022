using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_ItmNotes_Repository
    {
        void SaveNotes(Object NoteObj);
        Object GetNote(int NoteID);
        IEnumerable<Object> GetNotesListByItemCode(string CardCode);
    }
}
