using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_CashSalesCreditDocH_Repository : IDisposable
    {
        IEnumerable<CashSalesCreditDocH> CashSalesCreditHeaderList { get; }
        bool ResubmitCSCreditToSAP(CashSalesCreditDocH CSCObj, ref string ValidationMessage);
        bool AddCSC(CashSalesCreditDocH CSCObj, List<CashSalesCreditDocLs> Lines, List<CashSalesCreditDocNotes> NoteLines, List<CashSalesCreditDocPays> PayLines, ref string ValidationMessage, ref string CSCDocNum);
        CashSalesCreditDocH GetByDocNumber(string DocNum);
        CashSalesCreditDocH GetByDocEntry(string DocEntry);
        IEnumerable<CashSalesCreditDocH> CSCHeaderListByCardCode(string CardCode);
        string UpdatePrintStatus(string DocEntry, string printedBy);
        void WriteCSCInventoryLogs(List<CashSalesCreditDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void CommitChanges();
        void UpdateStockBalance(List<string> lines, string CreatedBy);
        CashSalesCreditDocH GetDocumentWaitForSyncing();
    }
}
