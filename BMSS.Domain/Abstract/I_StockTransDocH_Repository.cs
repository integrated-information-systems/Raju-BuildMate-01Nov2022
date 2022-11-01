using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_StockTransDocH_Repository : IDisposable
    {
        IEnumerable<StockTransDocH> StockTransHeaderList { get; }
        bool ResubmitStockTransToSAP(StockTransDocH StockTransObj, ref string ValidationMessage);
        bool AddStockTrans(StockTransDocH StockTransObj, List<StockTransDocLs> Lines, List<StockTransDocNotes> NoteLines, ref string ValidationMessage, ref string STDocNum);
        StockTransDocH GetByDocNumber(string DocNum);
        StockTransDocH GetByDocEntry(string DocEntry);
        string UpdatePrintStatus(string DocEntry, string printedBy);

        
        void WriteSTSourceLocationInventoryLogs(List<StockTransDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void WriteSTDestLocationInventoryLogs(List<StockTransDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void CommitChanges();
        void UpdateStockBalance(List<StockTransDocLs> lines, string CreatedBy);

        void UpdateStockBalanceItems(List<string> lines, string CreatedBy);
        StockTransDocH GetDocumentWaitForSyncing();
    }
}
