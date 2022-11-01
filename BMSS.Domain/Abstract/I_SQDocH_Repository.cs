using System.Collections.Generic;
using System;
namespace BMSS.Domain.Abstract
{
    public interface I_SQDocH_Repository : IDisposable
    {
        IEnumerable<SQDocH> SalesQuotationHeaderList { get; }
        IEnumerable<SQDocH> SalesQuotationHeaderListByCardCode(string CardCode);
        bool AddSQ(SQDocH SQObj, List<SQDocLs> Lines, List<SQDocNotes> NoteLines, ref string ValidationMessage, ref string SQDocNum);
        SQDocH GetByDocNumber(string DocNum);
        string UpdatePrintStatus(string DocEntry, string printedBy);
        int GetSQDetailsWithPaginationCount();
        IEnumerable<SQDocH> GetSQDetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "");
    }
}
