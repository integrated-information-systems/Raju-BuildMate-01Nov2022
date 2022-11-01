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
    public class EF_StockReceiptDocHeader_Repository : I_StockReceiptDocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_StockReceiptDocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<StockReceiptDocH> StockReceiptHeaderList
        {
            get
            {
                
                    return dbcontext.StockReceiptDocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();
                
            }
        }
        public bool ResubmitStockReceiptToSAP(StockReceiptDocH StockReceiptObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
                
                    StockReceiptDocH dbEntry = dbcontext.StockReceiptDocH.Find(StockReceiptObj.DocEntry);
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
        public StockReceiptDocH GetByDocNumber(string DocNum)
        {

            
                return dbcontext.StockReceiptDocH.Include("StockReceiptDocLs").Include("StockReceiptDocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
          
        }
        public StockReceiptDocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
             
                return dbcontext.StockReceiptDocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
            
        }

        public bool AddStockReceipt(StockReceiptDocH StockReceiptObj, List<StockReceiptDocLs> Lines, List<StockReceiptDocNotes> NoteLines, ref string ValidationMessage, ref string SRDocNum)
        {
            bool Result = true;

           
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (StockReceiptObj.DocNum == "New")
                        {
                            NumberingSR numberingStockReceipt = dbcontext.NumberingSR.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingStockReceipt != null)
                            {
                                string DocNum = numberingStockReceipt.Prefix + numberingStockReceipt.NextNo;
                                numberingStockReceipt.NextNo = numberingStockReceipt.NextNo + 1;
                                numberingStockReceipt.IsLocked = true;

                                long DocEntry = dbcontext.StockReceiptDocH.Count() + 1;
                                StockReceiptObj.DocEntry = DocEntry;
                                StockReceiptObj.DocNum = DocNum;
                                StockReceiptObj.CreatedOn = DateTime.Now;
                                StockReceiptObj.UpdatedOn = null;
                                StockReceiptObj.PrintedCount = 0;
                                SRDocNum = DocNum;
                                dbcontext.StockReceiptDocH.Add(StockReceiptObj);
                              
                               
                                ValidationMessage = StockReceiptObj.DocEntry.ToString();


                                IEnumerable<StockReceiptDocLs> StockReceiptLines = GetStockReceiptLines(Lines, DocEntry);
                                dbcontext.StockReceiptDocLs.AddRange(StockReceiptLines);
                               

                                IEnumerable<StockReceiptDocNotes> StockReceiptNotes = GetStockReceiptNotes(NoteLines, DocEntry);
                                dbcontext.StockReceiptDocNotes.AddRange(StockReceiptNotes);

                                WriteSRInventoryLogs(Lines, StockReceiptObj.DocNum, StockReceiptObj.CreatedBy, "Insert");
                                

                                dbcontext.SaveChanges();

                                UpdateStockBalance(Lines.Select(x => x.ItemCode).ToList(), StockReceiptObj.CreatedBy);

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
                            StockReceiptDocH dbEntry = dbcontext.StockReceiptDocH.Find(StockReceiptObj.DocEntry);
                            if (dbEntry != null)
                            {

                                dbEntry.DocDate = StockReceiptObj.DocDate;

                                dbEntry.Status = StockReceiptObj.Status;
                                dbEntry.SyncStatus = StockReceiptObj.SyncStatus;
                                dbEntry.SubmittedToSAP = StockReceiptObj.SubmittedToSAP;
                                dbEntry.UpdatedBy = StockReceiptObj.UpdatedBy;
                                dbEntry.UpdatedOn = DateTime.Now;
                                dbEntry.SubmittedToSAP = StockReceiptObj.SubmittedToSAP;
                                dbEntry.Ref = StockReceiptObj.Ref;
                                


                                long DocEntry = StockReceiptObj.DocEntry;
                                List<string> itemsToUpdateStock = new List<string>();
                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From StockReceiptDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            //dbcontext.SaveChanges();

                            var linesToDelete = dbcontext.StockReceiptDocLs.Where(x => x.DocEntry == DocEntry);
                                dbcontext.StockReceiptDocLs.RemoveRange(linesToDelete);

                                IEnumerable<StockReceiptDocLs> StockReceiptLines = GetStockReceiptLines(Lines, DocEntry);
                                dbcontext.StockReceiptDocLs.AddRange(StockReceiptLines);

                            itemsToUpdateStock = StockReceiptLines.Select(x => x.ItemCode).ToList().Union(linesToDelete.Select(x => x.ItemCode).ToList()).ToList();

                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From StockReceiptDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            //dbcontext.SaveChanges();
                            var notelinesToDelete = dbcontext.StockReceiptDocNotes.Where(x => x.DocEntry == DocEntry);
                                dbcontext.StockReceiptDocNotes.RemoveRange(notelinesToDelete);

                                IEnumerable<StockReceiptDocNotes> StockReceiptNotes = GetStockReceiptNotes(NoteLines, DocEntry);
                                dbcontext.StockReceiptDocNotes.AddRange(StockReceiptNotes);


                                WriteSRInventoryLogs(Lines, StockReceiptObj.DocNum, StockReceiptObj.UpdatedBy, "Update");
                                
                                dbcontext.SaveChanges();

                                UpdateStockBalance(itemsToUpdateStock, StockReceiptObj.UpdatedBy);

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
        public void WriteSRInventoryLogs(List<StockReceiptDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            var logs =  lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "Stock Receipt",
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

        public void UpdateStockBalance( List<string> lines, string CreatedBy)
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

                    var SRBalance = dbcontext.StockReceiptDocLs.Include("StockReceiptDocH").Where(x => x.ItemCode.Equals(item) && x.Location.Equals(whs.WhsCode) && x.StockReceiptDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            SRBalance = SRBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundItem.SRBalance = SRBalance;
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item,
                        SRBalance = SRBalance,
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLog);
                }
            }

        }
        public IEnumerable<StockReceiptDocLs> GetStockReceiptLines(List<StockReceiptDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }
        public IEnumerable<StockReceiptDocNotes> GetStockReceiptNotes(List<StockReceiptDocNotes> Lines, long DocEntry)
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
                StockReceiptDocH dbEntry = dbcontext.StockReceiptDocH.Find(DocEntry);
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
        public StockReceiptDocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.StockReceiptDocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is StockReceiptDocH stockReceiptDocH)
                    return stockReceiptDocH;
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
