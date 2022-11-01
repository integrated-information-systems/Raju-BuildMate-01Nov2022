using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract
{
    public interface I_PQDocH_Repository
    {
        IEnumerable<PQDocH> PurchaseQuotationHeaderList { get; }
        IEnumerable<PQDocH> PurchaseQuotationHeaderListByCardCode(string CardCode);
        bool AddPQ(PQDocH PQObj, List<PQDocLs> Lines, List<PQDocNotes> NoteLines, ref string ValidationMessage, ref string PQDocNum);
        PQDocH GetByDocNumber(string DocNum);
        string UpdatePrintStatus(string DocEntry, string printedBy);
    }
}
