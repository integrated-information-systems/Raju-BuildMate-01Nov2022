using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_CashSalesDocH_Repository : IDisposable
    {
        IEnumerable<CashSalesDocH> CashSalesHeaderList { get; }
        IEnumerable<CashSalesDocH> CashSalesHeaderBalanceDueList(string CardCode);
        bool ResubmitCSToSAP(CashSalesDocH CSObj, ref string ValidationMessage);
        bool AddCS(CashSalesDocH CSObj, List<CashSalesDocLs> Lines, List<CashSalesDocNotes> NoteLines, List<CashSalesDocPays> PayLines, ref string ValidationMessage, ref string CSDocNum);
        CashSalesDocH GetByDocNumber(string DocNum);
        CashSalesDocH GetByDocEntry(string DocEntry);
        IEnumerable<CashSalesDocH> CSHeaderListByCardCode(string CardCode);
        string UpdatePrintStatus(string DocEntry, string printedBy, string DocType = "DO");

        IEnumerable<CashSalesDocH> GetCSDetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "");
        int GetCSDetailsWithPaginationCount();
        bool NotePlannerSubmission(string DocNum, ref int SentCount);
        void WriteCSInventoryLogs(List<CashSalesDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void CommitChanges();
        void UpdateStockBalance(List<string> lines, string CreatedBy);

        CashSalesDocH GetDocumentWaitForSyncing();
    }
}
