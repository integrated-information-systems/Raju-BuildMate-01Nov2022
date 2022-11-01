using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_GRPODocH_Repository : IDisposable
    {
        IEnumerable<GRPODocH> GRPOHeaderList { get; }
        IEnumerable<GRPODocH> GRPOHeaderListByCardCode(string CardCode);
        bool ResubmitGRPOToSAP(GRPODocH GRPOObj, ref string ValidationMessage);
        bool AddGRPO(GRPODocH GRPOObj, List<GRPODocLs> Lines, List<GRPODocNotes> NoteLines, ref string ValidationMessage, ref string GRPODocNum);
        GRPODocH GetByDocNumber(string DocNum);
        GRPODocH GetByDocEntry(string DocEntry);
        bool UpdateNotes(GRPODocH GRPOObj, List<GRPODocNotes> NoteLines, ref string ValidationMessage);
        string UpdatePrintStatus(string DocEntry, string printedBy);
        void WriteGRPOInventoryLogs(List<GRPODocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");
        void CommitChanges();
        void UpdateStockBalance(List<string> lines, string CreatedBy);
        GRPODocH GetDocumentWaitForSyncing();

    }
}
