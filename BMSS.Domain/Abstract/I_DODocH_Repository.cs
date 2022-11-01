using BMSS.Domain.Models;
using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_DODocH_Repository : IDisposable

    {

        DODocH GetDocumentWaitForSyncing();
      
        decimal GetTotalSystemStockBalanceByItemCode(string ItemCode, string WarhouseCode);
        decimal GetTotalSystemStockBalanceByItemCode(string ItemCode);

        decimal GetTotalSystemStockBalanceByItemCodeNew(string ItemCode);
        decimal GetTotalSystemStockBalanceByItemCodeNew(string ItemCode, string WarhouseCode);
        decimal GetTotalSystemBalanceByCardCode(string CardCode);
        IEnumerable<DODocH> DOHeaderList { get; }
        IEnumerable<DODocH> DOHeaderListByCardCode(string CardCode);
        bool ResubmitDOToSAP(DODocH DOObj, ref string ValidationMessage);
        IEnumerable<DODocH> DOHeaderBalanceDueList(string CardCode);
        decimal DOTotalByCustomerCodeForYear(string CardCard, int Year);
        decimal DOTotalByCustomerCodeForYearMonth(string CardCard, int Year, int Month);
        bool NotePlannerSubmission(string DocNum, ref int SentCount);
        bool AddDO(DODocH DOObj, List<DODocLs> Lines, List<DODocNotes> NoteLines, ref string ValidationMessage, ref string DODocNum, bool UpdateInfo = false);
        DODocH GetByDocNumber(string DocNum);
        DODocH GetByDocEntry(string DocEntry);
        bool UpdateNotes(DODocH DOObj, List<DODocNotes> NoteLines, ref string ValidationMessage);
        string UpdatePrintStatus(string DocEntry, string printedBy, string DocType ="DO");
        AgingBucket GetCustomerAgingBucket(string CardCode);
        
        IEnumerable<DODocH> GetDODetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "");
        int GetDODetailsWithPaginationCount(string search= "");

        void WriteDOInventoryLogs(List<DODocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert");

        void CommitChanges();
        void UpdateStockBalance(List<string> lines, string CreatedBy);
        int GetDOTotalCount();
    }
}
