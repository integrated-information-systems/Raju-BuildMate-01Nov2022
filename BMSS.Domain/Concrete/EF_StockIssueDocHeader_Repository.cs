using BMSS.Domain.Abstract;
using BMSS.Domain.Concrete.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_StockIssueDocHeader_Repository : I_StockIssueDocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_StockIssueDocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<StockIssueDocH> StockIssueHeaderList
        {
            get
            {
                 
                    return dbcontext.StockIssueDocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();
                 
            }
        }
        public StockIssueDocH GetByDocNumber(string DocNum)
        {

            
                return dbcontext.StockIssueDocH.Include("StockIssueDocLs").Include("StockIssueDocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            
        }
        public StockIssueDocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
            
                return dbcontext.StockIssueDocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
          
        }
        public bool ResubmitStockIssueToSAP(StockIssueDocH StockIssueObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
              
                    StockIssueDocH dbEntry = dbcontext.StockIssueDocH.Find(StockIssueObj.DocEntry);
                    if (dbEntry != null)
                    {
                        dbEntry.SyncStatus = 1;
                        dbEntry.SubmittedToSAP = true;
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
        public bool AddStockIssue(StockIssueDocH StockIssueObj, List<StockIssueDocLs> Lines, List<StockIssueDocNotes> NoteLines, ref string ValidationMessage, ref string SIDocNum)
        {
            bool Result = true;

           
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (StockIssueObj.DocNum == "New")
                        {
                            NumberingSI numberingStockIssue = dbcontext.NumberingSI.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingStockIssue != null)
                            {
                                string DocNum = numberingStockIssue.Prefix + numberingStockIssue.NextNo;
                                numberingStockIssue.NextNo = numberingStockIssue.NextNo + 1;
                                numberingStockIssue.IsLocked = true;

                                long DocEntry = dbcontext.StockIssueDocH.Count() + 1;
                                StockIssueObj.DocEntry = DocEntry;
                                StockIssueObj.DocNum = DocNum;
                                StockIssueObj.CreatedOn = DateTime.Now;
                                StockIssueObj.UpdatedOn = null;
                                StockIssueObj.PrintedCount = 0;
                                SIDocNum = DocNum;
                                dbcontext.StockIssueDocH.Add(StockIssueObj);
                               
                               
                                ValidationMessage = StockIssueObj.DocEntry.ToString();

                                IEnumerable<StockIssueDocLs> StockIssueLines = GetStockIssueLines(Lines, DocEntry);
                                dbcontext.StockIssueDocLs.AddRange(StockIssueLines);
                                
                                IEnumerable<StockIssueDocNotes> StockIssueNotes = GetStockIssueNotes(NoteLines, DocEntry);
                                dbcontext.StockIssueDocNotes.AddRange(StockIssueNotes);

                                WriteSIInventoryLogs(Lines, StockIssueObj.DocNum, StockIssueObj.CreatedBy, "Insert");
                                   
                                dbcontext.SaveChanges();

                                UpdateStockBalance(Lines.Select(x => x.ItemCode).ToList(), StockIssueObj.CreatedBy);

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
                            StockIssueDocH dbEntry = dbcontext.StockIssueDocH.Find(StockIssueObj.DocEntry);
                            if (dbEntry != null)
                            {
                               
                                dbEntry.DocDate = StockIssueObj.DocDate;
                               
                                dbEntry.Status = StockIssueObj.Status;
                                dbEntry.SyncStatus = StockIssueObj.SyncStatus;
                                dbEntry.SubmittedToSAP = StockIssueObj.SubmittedToSAP;
                                dbEntry.UpdatedBy = StockIssueObj.UpdatedBy;
                                dbEntry.UpdatedOn = DateTime.Now;
                                dbEntry.SubmittedToSAP = StockIssueObj.SubmittedToSAP;
                                dbEntry.Ref = StockIssueObj.Ref;
                              

                                long DocEntry = StockIssueObj.DocEntry;
                                List<string> itemsToUpdateStock = new List<string>();
                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From StockIssueDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            //dbcontext.SaveChanges();
                            var linesToDelete = dbcontext.StockIssueDocLs.Where(x => x.DocEntry == DocEntry);
                                dbcontext.StockIssueDocLs.RemoveRange(linesToDelete);

                                IEnumerable<StockIssueDocLs> StockIssueLines = GetStockIssueLines(Lines, DocEntry);
                                dbcontext.StockIssueDocLs.AddRange(StockIssueLines);


                            itemsToUpdateStock = StockIssueLines.Select(x => x.ItemCode).ToList().Union(linesToDelete.Select(x => x.ItemCode).ToList()).ToList();

                            var notelinesToDelete = dbcontext.StockIssueDocNotes.Where(x => x.DocEntry == DocEntry);
                                dbcontext.StockIssueDocNotes.RemoveRange(notelinesToDelete);
                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From StockIssueDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                //dbcontext.SaveChanges();

                                IEnumerable<StockIssueDocNotes> StockIssueNotes = GetStockIssueNotes(NoteLines, DocEntry);
                                dbcontext.StockIssueDocNotes.AddRange(StockIssueNotes);


                            WriteSIInventoryLogs(Lines, StockIssueObj.DocNum, StockIssueObj.UpdatedBy, "Update");
                                 

                                dbcontext.SaveChanges();

                                UpdateStockBalance(itemsToUpdateStock, StockIssueObj.UpdatedBy);

                                dbcontext.SaveChanges();


                                transaction.Commit();
                            }
                            else
                            {
                                Result = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        transaction.Rollback();
                        Result = false;
                    }
                }
           


            return Result;
        }
        public void WriteSIInventoryLogs(List<StockIssueDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            var logs  = lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "Stock Issue",
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

                    var SIBalance = dbcontext.StockIssueDocLs.Include("StockIssueDocH").Where(x => x.ItemCode.Equals(item) && x.Location.Equals(whs.WhsCode) && x.StockIssueDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            SIBalance = SIBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundItem.SIBalance = SIBalance;
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item,
                        SIBalance = SIBalance,
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLog);
                }
            }

        }
        public IEnumerable<StockIssueDocLs> GetStockIssueLines(List<StockIssueDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }
        public IEnumerable<StockIssueDocNotes> GetStockIssueNotes(List<StockIssueDocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }

        public string UpdatePrintStatus(string Entry, string printedBy)
        {
            string DocNum = string.Empty;
            
                long DocEntry = long.Parse(Entry);
                StockIssueDocH dbEntry = dbcontext.StockIssueDocH.Find(DocEntry);
                if (dbEntry != null)
                {
                    DocNum = dbEntry.DocNum;
                    dbEntry.PrintedBy = printedBy;
                    dbEntry.PrintedOn = DateTime.Now;
                    dbEntry.PrintedCount = dbEntry.PrintedCount + 1;
                    dbEntry.PrintedStatus = 1;
                    dbcontext.SaveChanges();
                }

            
            return DocNum;

        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }
        public void CommitChanges()
        {
            dbcontext.SaveChanges();
        }
        public StockIssueDocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.StockIssueDocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is StockIssueDocH stockIssueDocH)
                    return stockIssueDocH;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }

}
