using BMSS.Domain.Abstract;
using BMSS.Domain.Concrete.SAP;
using BMSS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq.Dynamic;
using System.Linq;
using BMSS.Domain.Entities;
 
namespace BMSS.Domain.Concrete
{
    public class EF_DODocHeader_Repository : I_DODocH_Repository
    {
        private readonly DomainDb dbcontext;
        private readonly EFSapDbContext sapdbcontext;

        public EF_DODocHeader_Repository()
        {
            dbcontext = new DomainDb();
            sapdbcontext = new EFSapDbContext();
        }
        public IEnumerable<DODocH> DOHeaderList
        {
            get
            {
               
                    return dbcontext.DODocH.OrderByDescending(x => x.DocEntry).ToList();
                
            }
        }
        public IEnumerable<DODocH> DOHeaderListByCardCode(string CardCode)
        {
             
                return dbcontext.DODocH.Where(x => x.CardCode.Equals(CardCode)).ToList();
           
        }
        public Decimal DOTotalByCustomerCodeForYear(string CardCode, int Year)
        {
            Decimal DOTotal = 0;
            Decimal CSTotal = 0;
            Decimal CSCTotal = 0;
           
                DOTotal= dbcontext.DODocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.DocDate.Year.Equals(Year) && x.SAPDocNum.Equals(null)).Sum(e=> (decimal?)e.GrandTotal) ?? 0;
                CSTotal = dbcontext.CashSalesDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.DocDate.Year.Equals(Year) && x.SAPDocNum.Equals(null)).Sum(e => (decimal?)e.GrandTotal) ?? 0;
                CSCTotal = dbcontext.CashSalesCreditDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.DocDate.Year.Equals(Year) && x.SAPDocNum.Equals(null)).Sum(e => (decimal?)e.GrandTotal) ?? 0;
           
            return DOTotal + CSTotal +CSCTotal;
        }
        public Decimal DOTotalByCustomerCodeForYearMonth(string CardCode, int Year, int Month)
        {
            Decimal DOTotal = 0;
            Decimal CSTotal = 0;
            Decimal CSCTotal = 0;
           
                DOTotal = dbcontext.DODocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.DocDate.Year.Equals(Year) && x.DocDate.Month.Equals(Month) && x.SAPDocNum.Equals(null)).Sum(e => (decimal?)e.GrandTotal) ?? 0;
                CSTotal = dbcontext.CashSalesDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.DocDate.Year.Equals(Year) && x.SAPDocNum.Equals(null)).Sum(e => (decimal?)e.GrandTotal) ?? 0;
                CSCTotal = dbcontext.CashSalesCreditDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.DocDate.Year.Equals(Year) && x.SAPDocNum.Equals(null)).Sum(e => (decimal?)e.GrandTotal) ?? 0;
            
            return DOTotal + CSTotal - CSCTotal;
        }
        public IEnumerable<DODocH> DOHeaderBalanceDueList(string CardCode)
        { 
                
                    return dbcontext.DODocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.BalanceDue>0 ).ToList();
                
        }
        public DODocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
            
                return dbcontext.DODocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
             
        }
        public void CommitChanges()
        {
            dbcontext.SaveChanges();
        }
        public DODocH GetByDocNumber(string DocNum)
        {

            
                return dbcontext.DODocH.Include("DODocLs").Include("DODocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            
        }
        public bool NotePlannerSubmission(string DocNum,ref int SentCount)
        {
            bool Result = true;
            try
            {
                
                    if (dbcontext.DODocH.Where(x => x.DocNum == DocNum).FirstOrDefault() is DODocH dODocH)
                    {
                        dODocH.SentToPlanner += 1;
                        SentCount = dODocH.SentToPlanner;
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        Result = false;
                    }
               
                return Result;
            }
            catch (Exception ex)
            {
                Result = false;
            }
            return Result;
        }
        public DODocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.DODocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is DODocH dODocH)
                    return dODocH;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public decimal GetTotalSystemStockBalanceByItemCode(string ItemCode, string WarhouseCode)
        {
            decimal FinalTotal = 0;
            decimal DOTotal = 0;
            decimal StockLoanAllottedTotal = 0;
            decimal CashSalesTotal = 0;
            decimal CashSalesCreditTotal = 0;
            decimal GRPOTotal = 0;
            decimal StockReceiptTotal = 0;
            decimal StockIssueTotal = 0;
            decimal StockTransOriginTotal = 0;
            decimal StockTransDestinationTotal = 0;
           
                DOTotal = dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.DODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                //Stock Loan Calculation
                IEnumerable<DODocLs> OpenDOLinesList = dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.DODocH.SAPDocNum.Equals(null)).ToList();                
                foreach (DODocLs item in OpenDOLinesList)
                {
                    decimal OpenStockLoanLineTotal= dbcontext.DODocStockLoan.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.LineNum.Equals(x.LineNum) && x.DODocH.DocNum.Equals(item.DODocH.DocNum) && x.Reversed.Equals(false)).Sum(x => (decimal?)x.Qty) ?? 0;
                    StockLoanAllottedTotal = StockLoanAllottedTotal + (OpenStockLoanLineTotal);
                }

                CashSalesTotal = dbcontext.CashSalesDocLs.Include("CashSalesDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.CashSalesDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                CashSalesCreditTotal = dbcontext.CashSalesCreditDocLs.Include("CashSalesCreditDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.CashSalesCreditDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                GRPOTotal = dbcontext.GRPODocLs.Include("GRPODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.GRPODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockReceiptTotal = dbcontext.StockReceiptDocLs.Include("StockReceiptDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.StockReceiptDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockIssueTotal = dbcontext.StockIssueDocLs.Include("StockIssueDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode) && x.StockIssueDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockTransOriginTotal = dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.FromLocation.Equals(WarhouseCode) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockTransDestinationTotal = dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.ToLocation.Equals(WarhouseCode) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
            
            FinalTotal = FinalTotal - DOTotal;
            FinalTotal = FinalTotal + StockLoanAllottedTotal;
            FinalTotal = FinalTotal - CashSalesTotal;
            FinalTotal = FinalTotal + CashSalesCreditTotal;
            FinalTotal = FinalTotal + GRPOTotal;
            FinalTotal = FinalTotal + StockReceiptTotal;
            FinalTotal = FinalTotal - StockIssueTotal;
            FinalTotal = FinalTotal + StockTransDestinationTotal;
            FinalTotal = FinalTotal - StockTransOriginTotal;

            return FinalTotal;
        }
        public decimal GetTotalSystemStockBalanceByItemCodeNew(string ItemCode, string WarhouseCode)
        {
            decimal FinalTotal = 0;
            decimal DOTotal = 0;
            decimal StockLoanAllottedTotal = 0;
            decimal CashSalesTotal = 0; 
            decimal CashSalesCreditTotal = 0;
            decimal GRPOTotal = 0;
            decimal StockReceiptTotal = 0;
            decimal StockIssueTotal = 0;
            decimal StockTransOriginTotal = 0;
            decimal StockTransDestinationTotal = 0;
            
                var stockBalance = dbcontext.StockBalance.Where(x => x.ItemCode.Equals(ItemCode) && x.Location.Equals(WarhouseCode)).FirstOrDefault();
            if (stockBalance != null)
            {
                DOTotal = stockBalance.DOBalance;
                StockLoanAllottedTotal = stockBalance.SLBalance;
                CashSalesTotal = stockBalance.CSBalance;

                CashSalesCreditTotal = stockBalance.CSCBalance;
                GRPOTotal = stockBalance.GRPOBalance;
                StockReceiptTotal = stockBalance.SRBalance;
                StockIssueTotal = stockBalance.SIBalance;

                StockTransDestinationTotal = stockBalance.STBalance;

            }
            
            FinalTotal = FinalTotal - DOTotal;
            FinalTotal = FinalTotal + StockLoanAllottedTotal;
            FinalTotal = FinalTotal - CashSalesTotal;
            FinalTotal = FinalTotal + CashSalesCreditTotal;
            FinalTotal = FinalTotal + GRPOTotal;
            FinalTotal = FinalTotal + StockReceiptTotal;
            FinalTotal = FinalTotal - StockIssueTotal;
            FinalTotal = FinalTotal + StockTransDestinationTotal;
            FinalTotal = FinalTotal - StockTransOriginTotal;

            return FinalTotal;
        }
        public decimal GetTotalSystemStockBalanceByItemCodeNew(string ItemCode)
        { 
            decimal FinalTotal = 0;
            decimal DOTotal = 0;
            decimal StockLoanAllottedTotal = 0;
            decimal CashSalesTotal = 0;
            decimal CashSalesCreditTotal = 0;
            decimal GRPOTotal = 0;
            decimal StockReceiptTotal = 0;
            decimal StockIssueTotal = 0;
            decimal StockTransOriginTotal = 0;
            decimal StockTransDestinationTotal = 0;
            

                var stockBalances = dbcontext.StockBalance.Where(x => x.ItemCode.Equals(ItemCode)).ToList();
            if (stockBalances.Count()>0)
            {
                DOTotal  = stockBalances.Sum(x=> x.DOBalance);
                StockLoanAllottedTotal = stockBalances.Sum(x=> x.SLBalance);
                CashSalesTotal = stockBalances.Sum(x=> x.CSBalance);

                CashSalesCreditTotal = stockBalances.Sum(x=> x.CSCBalance);
                GRPOTotal = stockBalances.Sum(x=> x.GRPOBalance);
                StockReceiptTotal = stockBalances.Sum(x=> x.SRBalance);
                StockIssueTotal = stockBalances.Sum(x=>x.SIBalance);
            }      
             
            FinalTotal = FinalTotal - DOTotal;
            FinalTotal = FinalTotal + StockLoanAllottedTotal;
            FinalTotal = FinalTotal - CashSalesTotal;
            FinalTotal = FinalTotal + CashSalesCreditTotal;
            FinalTotal = FinalTotal + GRPOTotal;
            FinalTotal = FinalTotal + StockReceiptTotal;
            FinalTotal = FinalTotal - StockIssueTotal;
            FinalTotal = FinalTotal + StockTransDestinationTotal;
            FinalTotal = FinalTotal - StockTransOriginTotal;

            return FinalTotal;
        }
        public decimal GetTotalSystemStockBalanceByItemCode(string ItemCode)
        {
            decimal FinalTotal = 0;
            decimal DOTotal = 0;
            decimal StockLoanAllottedTotal = 0;
            decimal CashSalesTotal = 0;
            decimal CashSalesCreditTotal = 0;
            decimal GRPOTotal = 0;
            decimal StockReceiptTotal = 0;
            decimal StockIssueTotal = 0;
            decimal StockTransOriginTotal = 0;
            decimal StockTransDestinationTotal = 0;
             
                DOTotal = dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.DODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                //Stock Loan Calculation
                IEnumerable<DODocLs> OpenDOLinesList = dbcontext.DODocLs.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.DODocH.SAPDocNum.Equals(null)).ToList();
                foreach (DODocLs item in OpenDOLinesList)
                {
                    decimal OpenStockLoanLineTotal = dbcontext.DODocStockLoan.Include("DODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.LineNum.Equals(x.LineNum) && x.DODocH.DocNum.Equals(item.DODocH.DocNum) && x.Reversed.Equals(false)).Sum(x => (decimal?)x.Qty) ?? 0;
                    StockLoanAllottedTotal = StockLoanAllottedTotal + (OpenStockLoanLineTotal);
                }

                CashSalesTotal = dbcontext.CashSalesDocLs.Include("CashSalesDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.CashSalesDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                CashSalesCreditTotal = dbcontext.CashSalesCreditDocLs.Include("CashSalesCreditDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.CashSalesCreditDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                GRPOTotal = dbcontext.GRPODocLs.Include("GRPODocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.GRPODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockReceiptTotal = dbcontext.StockReceiptDocLs.Include("StockReceiptDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.StockReceiptDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockIssueTotal = dbcontext.StockIssueDocLs.Include("StockIssueDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.StockIssueDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockTransOriginTotal = dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)  && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                StockTransDestinationTotal = dbcontext.StockTransDocLs.Include("StockTransDocH").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
             
            FinalTotal = FinalTotal - DOTotal;
            FinalTotal = FinalTotal + StockLoanAllottedTotal;
            FinalTotal = FinalTotal - CashSalesTotal;
            FinalTotal = FinalTotal + CashSalesCreditTotal;
            FinalTotal = FinalTotal + GRPOTotal;
            FinalTotal = FinalTotal + StockReceiptTotal;
            FinalTotal = FinalTotal - StockIssueTotal;
            FinalTotal = FinalTotal + StockTransDestinationTotal;
            FinalTotal = FinalTotal - StockTransOriginTotal;

            return FinalTotal;
        }
        public decimal GetTotalSystemBalanceByCardCode(string CardCode)
        {
            decimal FinalTotal = 0;
            decimal DOTotal = 0;
            //decimal PaymentTotal = 0;
            //decimal CashSalesTotal = 0;
            //decimal CashSalesCreditTotal = 0;
            decimal SAPCustomerBalance = 0;
           
                DOTotal = dbcontext.DODocH.AsNoTracking().Where(x => x.SAPDocNum.Equals(null) && x.CardCode.Equals(CardCode)).Sum(x => (decimal?)x.GrandTotal) ?? 0;
                //CashSalesTotal = dbcontext.CashSalesDocH.AsNoTracking().Where(x => x.SAPDocNum.Equals(null) && x.CardCode.Equals(CardCode)).Sum(x => (decimal?)x.GrandTotal) ?? 0;
                //CashSalesCreditTotal = dbcontext.CashSalesCreditDocH.AsNoTracking().Where(x => x.SAPDocNum.Equals(null) && x.CardCode.Equals(CardCode)).Sum(x => (decimal?)x.GrandTotal) ?? 0;
                //PaymentTotal = dbcontext.PaymentDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode) && x.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.PaidAmount) ?? 0;               
            
              
                SAPCustomerBalance = sapdbcontext.Customers.AsNoTracking().Where(i => i.CardCode.Equals(CardCode) && i.CardType.Equals("C")).Sum(x => x.Balance);
            
            FinalTotal = FinalTotal + DOTotal;
            //FinalTotal = FinalTotal + CashSalesTotal;
           // FinalTotal = FinalTotal - CashSalesCreditTotal;            
            //FinalTotal = FinalTotal - PaymentTotal;
            FinalTotal = FinalTotal + SAPCustomerBalance;
            
            return FinalTotal;
        }
        public bool ResubmitDOToSAP(DODocH DOObj, ref string ValidationMessage)
        {
            bool Result = true;
            
            try
            {
                
                    DODocH dbEntry = dbcontext.DODocH.Find(DOObj.DocEntry);
                    if (dbEntry != null)
                    {
                        dbEntry.SyncStatus = 1;
                        dbEntry.SubmittedToSAP = true;
                        dbEntry.SubmittedOn = DateTime.Now;
                        dbEntry.SubmittedBy = DOObj.SubmittedBy;
                        dbcontext.SaveChanges();

                    }
                    else
                    {
                        Result = false;
                        ValidationMessage = "Document not found";
                    }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Result = false;
            }
            return Result;
        }
        public bool UpdateNotes(DODocH DOObj, List<DODocNotes> NoteLines, ref string ValidationMessage)
        {
            bool Result = true;
            
                try
                {
                    DODocH dbEntry = dbcontext.DODocH.Find(DOObj.DocEntry);
                    dbcontext.Database.ExecuteSqlCommand(@"Delete From DODocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DOObj.DocEntry));
                    dbcontext.SaveChanges();

                    IEnumerable<DODocNotes> DONotes = GetDONotes(NoteLines, DOObj.DocEntry);
                    dbcontext.DODocNotes.AddRange(DONotes);
                    dbcontext.SaveChanges();
                }
                catch
                {
                    Result = false;
                }
            
            return Result;
        }
        
        public bool AddDO(DODocH DOObj, List<DODocLs> Lines, List<DODocNotes> NoteLines, ref string ValidationMessage, ref string DODocNum, bool UpdateInfo = false)
        {
            bool Result = true;

            
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (DOObj.DocNum == "New")
                        {
                            NumberingDO numberingDO = dbcontext.NumberingDO.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                           
                            if (numberingDO != null)
                            {
                                string DocNum = numberingDO.Prefix + numberingDO.NextNo;
                                numberingDO.NextNo = numberingDO.NextNo + 1;
                                numberingDO.IsLocked = true;

                                long DocEntry = dbcontext.DODocH.Count() + 1;
                                DOObj.DocEntry = DocEntry;
                                DOObj.Currency = "SGD";
                                DOObj.DocNum = DocNum;
                                DOObj.CreatedOn = DateTime.Now;
                                DOObj.UpdatedOn = null;
                                DOObj.SyncedToSAP = false;
                                DOObj.BalanceDue = DOObj.GrandTotal;
                                DOObj.INVPrintedCount = 0;
                                DOObj.DOPrintedCount = 0;
                                if (Lines.Where(x => x.LoanIssued.Equals(true)).Count()>0)
                                {
                                    DOObj.HaveStockLoan = true;
                                }
                                else
                                {
                                    DOObj.HaveStockLoan = false;
                                }

                                if(DOObj.SubmittedToSAP== true)
                                {
                                    DOObj.SubmittedBy = DOObj.CreatedBy;
                                    DOObj.SubmittedOn = DateTime.Now;
                                }

                                dbcontext.DODocH.Add(DOObj);
                                

                                 
                                ValidationMessage = DOObj.DocEntry.ToString();
                                DODocNum = DOObj.DocNum;

                                IEnumerable<DODocLs> DOLines = GetDOLines(Lines, DocEntry);
                                dbcontext.DODocLs.AddRange(DOLines);

                            WriteDOInventoryLogs(Lines, DOObj.DocNum, DOObj.CreatedBy, "Insert");
                               

                                if (DOObj.HaveStockLoan.Equals(true))
                                {
                                    IEnumerable<DODocStockLoan> DOStockLoanLines = GetStockLoanLines(Lines, DocEntry, DOObj.CreatedBy);
                                    foreach (DODocStockLoan item in DOStockLoanLines)
                                    {
                                        NumberingSL numberingSL = dbcontext.NumberingSL.Where(i => i.IsDefault.Equals(true) && (i.NextNo <= i.LastNo)).FirstOrDefault();
                                        if (numberingSL != null)
                                        {
                                            string DocNumSL = numberingSL.Prefix + numberingSL.NextNo;
                                            numberingSL.NextNo = numberingSL.NextNo + 1;
                                            numberingSL.IsLocked = true;
                                            
                                            item.DocNum = DocNumSL;

                                            long StockLoanDocEntry = dbcontext.DODocStockLoan.Count() + 1;
                                            item.DocEntry = StockLoanDocEntry;

                                            dbcontext.DODocStockLoan.Add(item);

                                            string Location = DOLines.Where(x => x.LineNum == item.LineNum).Select(x => x.Location).First();
                                            InventoryLog inventoryLog = new InventoryLog()
                                            {
                                                DocNum = DocNumSL,
                                                DocType = "Stock Loan",
                                                ItemCode = item.ItemCode,
                                                Qty = item.Qty,
                                                TStamp = DateTime.Now.Ticks.ToString(),
                                                CreatedOn = DateTime.Now,
                                                Location = Location,
                                                CreatedBy = DOObj.CreatedBy,
                                                InsertOrUpdate = "Insert"

                                            };
                                            dbcontext.InventoryLog.Add(inventoryLog);


                                        }
                                        else
                                        {
                                            Result = false;
                                            ValidationMessage = "There is no Document Numbering Series definition found for Stock Loan";
                                            throw new Exception("There is no Document Numbering Series definition found for Stock Loan");
                                        }
                                    }
                                    //dbcontext.DODocStockLoan.AddRange(DOStockLoanLines);
                                    //dbcontext.SaveChanges();
                                }

                                IEnumerable<DODocNotes> DONotes = GetDONotes(NoteLines, DocEntry);                                
                                dbcontext.DODocNotes.AddRange(DONotes);
                                
                                

                                if (DOObj.CopiedSQ !=null)
                                {
                                    if(DOObj.CopiedSQ!=string.Empty) { 
                                        string SQDocNum = DOObj.CopiedSQ;
                                        SQDocH SQHeader = dbcontext.SQDocH.Where(i => i.DocNum.Equals(SQDocNum)).FirstOrDefault();
                                        if (SQHeader != null)
                                        {                                        
                                            SQHeader.CopiedDO = DODocNum;
                                            
                                        }
                                    }
                                }
                                dbcontext.SaveChanges();
                                UpdateStockBalance(Lines.Select(x => x.ItemCode).ToList(), DOObj.CreatedBy);

                                dbcontext.SaveChanges();
                                transaction.Commit();
                            }
                            else
                            {
                                Result = false;
                                ValidationMessage = "There is no Document Numbering Series definition found";
                               
                                
                            }
                        }
                        else
                        {
                            DODocH dbEntry = dbcontext.DODocH.Find(DOObj.DocEntry);
                            if (dbEntry != null)
                            {
                                if (dbEntry.HaveStockLoan.Equals(true) && UpdateInfo.Equals(false))
                                {
                                    Result = false;
                                    ValidationMessage = "This document have pending stock loan, cannot proceed";
                                }
                                else if (dbEntry.HaveStockLoan.Equals(false) && UpdateInfo.Equals(true))
                                {
                                    Result = false;
                                    ValidationMessage = "This document don't have pending stock loan, cannot proceed";
                                }
                                else if (dbEntry.SubmittedToSAP.Equals(true))
                                {
                                    Result = false;
                                    ValidationMessage = "This document have been already submitted to SAP, cannot proceed";
                                }
                                else if (dbEntry.SyncedToSAP.Equals(true))
                                {
                                    Result = false;
                                    ValidationMessage = "This document have been already synced to SAP, cannot proceed";
                                }
                                else { 

                                    if ((Lines.Where(x => x.LoanIssued.Equals(true)).Count() > 0) || dbEntry.HaveStockLoan.Equals(true)  )
                                    {
                                        dbEntry.HaveStockLoan = true;
                                    }
                                    else
                                    {
                                        dbEntry.HaveStockLoan = false;
                                    }


                                    dbEntry.CardCode = DOObj.CardCode;
                                    dbEntry.CardName = DOObj.CardName;
                                    dbEntry.DocDate = DOObj.DocDate;
                                    dbEntry.DueDate = DOObj.DueDate;
                                    dbEntry.DeliveryDate = DOObj.DeliveryDate;
                                    dbEntry.Status = DOObj.Status;
                                    dbEntry.SyncStatus = DOObj.SyncStatus;
                                    dbEntry.PaymentTerm = DOObj.PaymentTerm;
                                    dbEntry.PaymentTermName = DOObj.PaymentTermName;
                                    dbEntry.Currency = "SGD";

                                    dbEntry.CustomerRef = DOObj.CustomerRef;
                                    dbEntry.DeliveryTime = DOObj.DeliveryTime;
                                    dbEntry.CustomerTelNo = DOObj.CustomerTelNo;

                                    dbEntry.OfficeTelNo = DOObj.OfficeTelNo;
                                    dbEntry.Fax = DOObj.Fax;

                                    dbEntry.SlpCode = DOObj.SlpCode;
                                    dbEntry.SlpName = DOObj.SlpName;

                                    dbEntry.HeaderRemarks1 = DOObj.HeaderRemarks1;
                                    dbEntry.HeaderRemarks2 = DOObj.HeaderRemarks2;
                                    dbEntry.HeaderRemarks3 = DOObj.HeaderRemarks3;
                                    dbEntry.HeaderRemarks4 = DOObj.HeaderRemarks4;
                                    dbEntry.FooterRemarks1 = DOObj.FooterRemarks1;
                                    dbEntry.FooterRemarks2 = DOObj.FooterRemarks2;
                                    dbEntry.FooterRemarks3 = DOObj.FooterRemarks3;
                                    dbEntry.FooterRemarks4 = DOObj.FooterRemarks4;

                                    dbEntry.ShipTo = DOObj.ShipTo;
                                    dbEntry.ShipToAddress1 = DOObj.ShipToAddress1;
                                    dbEntry.ShipToAddress2 = DOObj.ShipToAddress2;
                                    dbEntry.ShipToAddress3 = DOObj.ShipToAddress3;
                                    dbEntry.ShipToAddress4 = DOObj.ShipToAddress4;
                                    dbEntry.ShipToAddress5 = DOObj.ShipToAddress5;
                                    dbEntry.BillTo = DOObj.BillTo;
                                    dbEntry.BillToAddress1 = DOObj.BillToAddress1;
                                    dbEntry.BillToAddress2 = DOObj.BillToAddress2;
                                    dbEntry.BillToAddress3 = DOObj.BillToAddress3;
                                    dbEntry.BillToAddress4 = DOObj.BillToAddress4;
                                    dbEntry.BillToAddress5 = DOObj.BillToAddress5;
                                    dbEntry.SelfCollect = DOObj.SelfCollect;
                                    dbEntry.SelfCollectRemarks1 = DOObj.SelfCollectRemarks1;
                                    dbEntry.SelfCollectRemarks2 = DOObj.SelfCollectRemarks2;
                                    dbEntry.SelfCollectRemarks3 = DOObj.SelfCollectRemarks3;
                                    dbEntry.SelfCollectRemarks4 = DOObj.SelfCollectRemarks4;
                                    dbEntry.SelfCollectRemarks5 = DOObj.SelfCollectRemarks5;
                                    if (UpdateInfo.Equals(false))
                                    {
                                        dbEntry.DiscByPercent = DOObj.DiscByPercent;
                                        dbEntry.DiscAmount = DOObj.DiscAmount;
                                        dbEntry.DiscPercent = DOObj.DiscPercent;
                                        dbEntry.NetTotal = DOObj.NetTotal;
                                        dbEntry.GstTotal = DOObj.GstTotal;
                                        dbEntry.GrandTotal = DOObj.GrandTotal;
                                        dbEntry.Rounding = DOObj.Rounding;
                                        dbEntry.GrandTotalAftRounding = DOObj.GrandTotalAftRounding;
                                        dbEntry.BalanceDue = DOObj.GrandTotal;                                      
                                        dbEntry.SubmittedToSAP = DOObj.SubmittedToSAP;
                                    }

                                    dbEntry.UpdatedBy = DOObj.UpdatedBy;
                                    dbEntry.UpdatedOn = DateTime.Now;

                                    if (DOObj.SubmittedToSAP == true)
                                    {
                                        dbEntry.SubmittedBy = DOObj.UpdatedBy;
                                        dbEntry.SubmittedOn = DateTime.Now;
                                    }




                                long DocEntry = DOObj.DocEntry;
                                    List<string> itemsToUpdateStock = new List<string>();

                                    if (UpdateInfo.Equals(false)) { 
                                       // dbcontext.Database.ExecuteSqlCommand(@"Delete From DODocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                        
                                        IEnumerable<DODocLs> DOLines = GetDOLines(Lines, DocEntry);
                                        var linesToDelete = dbcontext.DODocLs.Where(x => x.DocEntry == DocEntry);
                                        dbcontext.DODocLs.RemoveRange(linesToDelete);
                                        dbcontext.DODocLs.AddRange(DOLines);

                                    itemsToUpdateStock = DOLines.Select(x => x.ItemCode).ToList().Union(linesToDelete.Select(x => x.ItemCode).ToList()).ToList();

                                    WriteDOInventoryLogs(Lines , DOObj.DocNum, DOObj.UpdatedBy, "Update");
                                       

                                        if (dbEntry.HaveStockLoan.Equals(true))
                                        {
                                            IEnumerable<DODocStockLoan> DOStockLoanLines = GetStockLoanLines(Lines, DocEntry,DOObj.UpdatedBy);
                                            foreach (DODocStockLoan item in DOStockLoanLines)
                                            {
                                                NumberingSL numberingSL = dbcontext.NumberingSL.Where(i => i.IsDefault.Equals(true) && (i.NextNo <= i.LastNo)).FirstOrDefault();
                                                if (numberingSL != null)
                                                {
                                                    string DocNumSL = numberingSL.Prefix + numberingSL.NextNo;
                                                    numberingSL.NextNo = numberingSL.NextNo + 1;
                                                    numberingSL.IsLocked = true;
                                                     
                                                    item.DocNum = DocNumSL;

                                                    long StockLoanDocEntry = dbcontext.DODocStockLoan.Count() + 1;
                                                    item.DocEntry = StockLoanDocEntry;

                                                   

                                                    dbcontext.DODocStockLoan.Add(item);
                                                    string Location = DOLines.Where(x => x.LineNum == item.LineNum).Select(x => x.Location).First();
                                                    InventoryLog inventoryLog = new InventoryLog()
                                                    {
                                                        DocNum = DocNumSL,
                                                        DocType = "Stock Loan",
                                                        ItemCode = item.ItemCode,
                                                        Qty = item.Qty,
                                                        TStamp = DateTime.Now.Ticks.ToString(),
                                                        CreatedOn = DateTime.Now,
                                                        Location = Location,
                                                        CreatedBy = DOObj.UpdatedBy,
                                                        InsertOrUpdate = "Insert"

                                                    };
                                                    dbcontext.InventoryLog.Add(inventoryLog);

                                                }
                                                else
                                                {
                                                    Result = false;
                                                    ValidationMessage = "There is no Document Numbering Series definition found for Stock Loan";
                                                    throw new Exception("There is no Document Numbering Series definition found for Stock Loan");
                                                }
                                            }
                                            
                                            //dbcontext.DODocStockLoan.AddRange(DOStockLoanLines);
                                            //dbcontext.SaveChanges();
                                        }
                                    }
                                    //  dbcontext.Database.ExecuteSqlCommand(@"Delete From DODocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                    var notesToDelete = dbcontext.DODocNotes.Where(x => x.DocEntry == DocEntry);
                                    dbcontext.DODocNotes.RemoveRange(notesToDelete);

                                    IEnumerable<DODocNotes> DONotes = GetDONotes(NoteLines, DocEntry);
                                    dbcontext.DODocNotes.AddRange(DONotes);
                                    dbcontext.SaveChanges();
                                    UpdateStockBalance(itemsToUpdateStock, DOObj.UpdatedBy);

                                    dbcontext.SaveChanges();
                                    transaction.Commit();
 
                                }
                                
                            }
                            else
                            {
                                Result = false;
                                ValidationMessage = "Document not found";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ValidationMessage = "Ëxception:" + e.Message.ToString() + e.InnerException.InnerException.Message.ToString();
                        transaction.Rollback();
                        Result = false;
                    }
               
            }


            return Result;
        }
        public IEnumerable<DODocStockLoan> GetStockLoanLines(List<DODocLs> Lines, long DocEntry, string CreatedBy)
        {
            Lines = Lines.Where(x => x.LoanIssued.Equals(true)).ToList();
            List<DODocStockLoan> StockLoanLines = new List<DODocStockLoan>();
            for (int i = 0; i < Lines.Count(); i++)
            {
                DODocStockLoan obj = new DODocStockLoan() {
                    DODocEntry = DocEntry,
                    LineNum = Lines[i].LineNum,
                    ItemCode = Lines[i].ItemCode,
                    Description = Lines[i].Description,
                    Qty = Lines[i].Qty,
                    Reversed = false,
                    TStamp = DateTime.Now.Ticks.ToString(),
                    CreatedBy= CreatedBy,
                    CreatedOn = DateTime.Now,
                    
                };
                StockLoanLines.Add(obj);
            }
            return StockLoanLines;
        }
        
        public IEnumerable<DODocLs> GetDOLines(List<DODocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].LineNum = i;
                Lines[i].DocEntry = DocEntry;               
                Lines[i].TStamp =DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }
        public void WriteDOInventoryLogs(List<DODocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
             var logs =  lines.Select((x) => new InventoryLog() {
                DocNum = DocNum,
                DocType = "DO",
                ItemCode = x.ItemCode,
                Qty = x.Qty,
                TStamp = DateTime.Now.Ticks.ToString(),
                CreatedOn = DateTime.Now,
                Location = x.Location,
                CreatedBy = CreatedBy,
                InsertOrUpdate = InsertOrUpdate

            });

            
            dbcontext.InventoryLog.AddRange(logs);
        }
      
        public void UpdateStockBalance(  List<string> lines, string CreatedBy)
        {
            IEnumerable<OWHS> warehouses = null;
            using (var sapdbcontext = new EFSapDbContext())
            {
                warehouses = sapdbcontext.Warehouses.ToList();
            }
             

            foreach (var item in lines)
            {
                foreach (var whs in warehouses)
                {

             
                    var foundItem = dbcontext.StockBalance.Where(x => x.ItemCode == item && x.Location == whs.WhsCode).FirstOrDefault();
              
                    var DOBalance = dbcontext.DODocLs.Include("DODocH").Where(x => x.ItemCode.Equals(item) && x.Location.Equals(whs.WhsCode) && x.DODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    var StockLoanBalance = dbcontext.DODocLs.Include(y => y.DODocH.DODocStockLoan).Where(x => x.ItemCode.Equals(item)  && x.Location.Equals(whs.WhsCode) && x.DODocH.SAPDocNum.Equals(null)).Sum(x =>(decimal?)x.DODocH.DODocStockLoan.Sum(y=> (y.Reversed==false && y.LineNum==x.LineNum) ? y.Qty : 0)) ?? 0;
 
                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            DOBalance = DOBalance,
                            SLBalance = StockLoanBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundItem.DOBalance = DOBalance;
                        foundItem.SLBalance = StockLoanBalance;
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item,
                        DOBalance = DOBalance,
                        SLBalance = StockLoanBalance,
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLog);
                }
            }
            
        }
        public IEnumerable<DODocNotes> GetDONotes(List<DODocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                //byte[] b = new byte[] { 0, 0, 0, 0 };
                //DateTime t = DateTime.Now;
                //Array.Copy(BitConverter.GetBytes(t.Ticks), 0, b, 0, 4);
                //Lines[i].TStamp = b;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }
        public IEnumerable<DODocH> GetDODetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "")
        {   
            IEnumerable<DODocH> DOs = null;
           
                if (string.IsNullOrEmpty(search))
                    DOs = dbcontext.DODocH.OrderBy(orderBy).Skip(skip).Take(rowsCount).ToList();
                else
                    DOs = dbcontext.DODocH.Where(x =>
                    x.DocNum.Contains(search)
                    ||
                    x.CardName.Contains(search)
                    ||
                    x.CardCode.Contains(search)).OrderBy(orderBy)
                    .Skip(skip).Take(rowsCount).ToList();
            
            return DOs;
        }
        public int GetDODetailsWithPaginationCount(string search = "")
        {
            int count = 0;
            if (string.IsNullOrEmpty(search))
                count = dbcontext.DODocH.Count();
            else
                count = dbcontext.DODocH.Where(x =>
                    x.DocNum.Contains(search)
                    ||
                    x.CardName.Contains(search)
                    ||
                    x.CardCode.Contains(search)).Count();


            return count;
        }
        public int GetDOTotalCount()
        {
            int count = 0;

            count = dbcontext.DODocH.Count();

            return count;
        }
        public string UpdatePrintStatus(string Entry, string printedBy, string DocType = "DO")
        {
            string DocNum = "";
            
                long DocEntry = long.Parse(Entry);
                DODocH dbEntry = dbcontext.DODocH.Find(DocEntry);
                if (dbEntry != null)
                {
                    DocNum = dbEntry.DocNum;
                    dbEntry.PrintedBy = printedBy;
                    dbEntry.PrintedOn = DateTime.Now;
                    if(DocType == "INV")
                    {
                        dbEntry.INVPrintedCount = dbEntry.INVPrintedCount + 1;
                    }
                    else
                    {
                        dbEntry.DOPrintedCount = dbEntry.DOPrintedCount + 1;
                    }
                    dbEntry.PrintedStatus = 1;
                    dbcontext.SaveChanges();
                     
                }            
             
            return DocNum;

             
        }
        public AgingBucket GetCustomerAgingBucket(string CardCode)
        {
            AgingBucket agingBucket = new AgingBucket();
             
                agingBucket = dbcontext.Database.SqlQuery<AgingBucket>(
                    @"SELECT

Sum(CASE

WHEN (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 < 31 THEN

T0.GrandTotal

ElSE 0

END) AS 'Current',

Sum(CASE

WHEN (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 > 30

AND (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 < 61 THEN

T0.GrandTotal

ElSE 0

END) AS 'Months1',

SUM(CASE

WHEN (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 > 60

AND (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 < 91 THEN

T0.GrandTotal

ElSE 0

END) AS 'Months2',

SUM(CASE

WHEN (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 > 90

AND (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 < 121 THEN

T0.GrandTotal

ElSE 0

END) AS 'Months3',

SUM(CASE

WHEN (DATEDIFF(DD, T0.DocDate,Current_Timestamp)) +1 > 120

THEN

T0.GrandTotal

ElSE 0

END) AS 'Months4'

FROM DODocH T0

 
WHERE
T0.CardCode = @CardCode AND SAPDocNum IS NULL

 ", new SqlParameter("@CardCode", CardCode)).FirstOrDefault();
           

            return agingBucket;
        }

        public void Dispose()
        {
            dbcontext.Dispose();
            sapdbcontext.Dispose();
        }
    }
}
