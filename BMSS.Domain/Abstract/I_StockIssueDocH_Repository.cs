using System.Collections.Generic;
using System;
namespace BMSS.Domain.Abstract
{
    public interface I_StockIssueDocH_Repository : IDisposable
    {
        IEnumerable<StockIssueDocH> StockIssueHeaderList { get; }
        bool ResubmitStockIssueToSAP(StockIssueDocH StockIssueObj, ref string ValidationMessage);
        bool AddStockIssue(StockIssueDocH StockIssueObj, List<StockIssueDocLs> Lines, List<StockIssueDocNotes> NoteLines, ref string ValidationMessage, ref string SIDocNum);
        StockIssueDocH GetByDocNumber(string DocNum);
        StockIssueDocH GetByDocEntry(string DocEntry);
        string UpdatePrintStatus(string DocEntry, string printedBy);
        void WriteSIInventoryLogs(List<StockIssueDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void CommitChanges();
        void UpdateStockBalance(List<string> lines, string CreatedBy);
        StockIssueDocH GetDocumentWaitForSyncing();


    }
}
