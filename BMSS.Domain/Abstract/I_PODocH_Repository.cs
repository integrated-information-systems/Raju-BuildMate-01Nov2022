using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_PODocH_Repository
    {
        IEnumerable<PODocH> POHeaderList { get; }

        IEnumerable<PODocH> GetPOHeaderListNoOpenQty();
        IEnumerable<PODocH> GetPOHeaderListWithOpenQty();

        IEnumerable<PODocH> POHeaderListByCardCode(string CardCode);
        bool AddPO(PODocH POObj, List<PODocLs> Lines, List<PODocNotes> NoteLines, ref string ValidationMessage, ref string PODocNum);

        PODocH GetByDocNumber(string DocNum);
        PODocH GetByDocEntry(string DocEntry);
        List<string> GetPDNList(long DocEntry);

        bool UpdateNotes(PODocH POObj, List<PODocNotes> NoteLines, ref string ValidationMessage);
        string UpdatePrintStatus(string DocEntry, string printedBy);
    }
}
