using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_StockReceiptDocH_Repository : IDisposable
    {
        IEnumerable<StockReceiptDocH> StockReceiptHeaderList { get; }
        bool ResubmitStockReceiptToSAP(StockReceiptDocH StockReceiptObj, ref string ValidationMessage);
        bool AddStockReceipt(StockReceiptDocH StockReceiptObj, List<StockReceiptDocLs> Lines, List<StockReceiptDocNotes> NoteLines, ref string ValidationMessage, ref string SRDocNum);
        StockReceiptDocH GetByDocNumber(string DocNum);
        StockReceiptDocH GetByDocEntry(string DocEntry);
        string UpdatePrintStatus(string DocEntry, string printedBy);

        void WriteSRInventoryLogs(List<StockReceiptDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void CommitChanges();
        void UpdateStockBalance(List<string> lines, string CreatedBy);
        StockReceiptDocH GetDocumentWaitForSyncing();
    }
}
