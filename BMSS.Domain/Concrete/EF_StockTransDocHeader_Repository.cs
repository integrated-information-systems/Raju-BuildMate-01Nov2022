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
    public class EF_StockTransDocHeader_Repository : I_StockTransDocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_StockTransDocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<StockTransDocH> StockTransHeaderList
        {
            get
            {                
                    return dbcontext.StockTransDocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();               
            }
        }
        public bool ResubmitStockTransToSAP(StockTransDocH StockReceiptObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
                
                    StockTransDocH dbEntry = dbcontext.StockTransDocH.Find(StockReceiptObj.DocEntry);
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
        public StockTransDocH GetByDocNumber(string DocNum)
        {
            
           return dbcontext.StockTransDocH.Include("StockTransDocLs").Include("StockTransDocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
          
        }
        public StockTransDocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
           
                return dbcontext.StockTransDocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
            
        }
        public bool AddStockTrans(StockTransDocH StockTransObj, List<StockTransDocLs> Lines, List<StockTransDocNotes> NoteLines, ref string ValidationMessage, ref string STDocNum)
        {
            bool Result = true;
 
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (StockTransObj.DocNum == "New")
                        {
                            NumberingST numberingStockTrans = dbcontext.NumberingST.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingStockTrans != null)
                            {
                                string DocNum = numberingStockTrans.Prefix + numberingStockTrans.NextNo;
                                numberingStockTrans.NextNo = numberingStockTrans.NextNo + 1;
                                numberingStockTrans.IsLocked = true;

                                long DocEntry = dbcontext.StockTransDocH.Count() + 1;
                                StockTransObj.DocEntry = DocEntry;
                                StockTransObj.DocNum = DocNum;
                                StockTransObj.CreatedOn = DateTime.Now;
                                StockTransObj.UpdatedOn = null;
                                StockTransObj.PrintedCount = 0;
                                STDocNum = DocNum;
                                dbcontext.StockTransDocH.Add(StockTransObj);
                               
                               
                                ValidationMessage = StockTransObj.DocEntry.ToString();

                                IEnumerable<StockTransDocLs> StockTransLines = GetStockTransLines(Lines, DocEntry);
                                dbcontext.StockTransDocLs.AddRange(StockTransLines);
                              

                                IEnumerable<StockTransDocNotes> StockTransNotes = GetStockTransNotes(NoteLines, DocEntry);
                                dbcontext.StockTransDocNotes.AddRange(StockTransNotes);


                                WriteSTSourceLocationInventoryLogs(Lines, StockTransObj.DocNum, StockTransObj.CreatedBy, "Insert");



                                WriteSTDestLocationInventoryLogs(Lines, StockTransObj.DocNum, StockTransObj.CreatedBy, "Insert");
                                

                                dbcontext.SaveChanges();

                                UpdateStockBalance(Lines, StockTransObj.CreatedBy);

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
                            StockTransDocH dbEntry = dbcontext.StockTransDocH.Find(StockTransObj.DocEntry);
                            if (dbEntry != null)
                            {

                                dbEntry.DocDate = StockTransObj.DocDate;

                                dbEntry.Status = StockTransObj.Status;
                                dbEntry.SyncStatus = StockTransObj.SyncStatus;
                                dbEntry.SubmittedToSAP = StockTransObj.SubmittedToSAP;
                                dbEntry.UpdatedBy = StockTransObj.UpdatedBy;
                                dbEntry.UpdatedOn = DateTime.Now;
                                dbEntry.SubmittedToSAP = StockTransObj.SubmittedToSAP;
                                dbEntry.Ref = StockTransObj.Ref;
                               


                                long DocEntry = StockTransObj.DocEntry;

                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From StockTransDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                //dbcontext.SaveChanges();

                                var linesToDelete = dbcontext.StockTransDocLs.Where(x => x.DocEntry == DocEntry);
                                dbcontext.StockTransDocLs.RemoveRange(linesToDelete);

                                IEnumerable<StockTransDocLs> StockTransLines = GetStockTransLines(Lines, DocEntry);
                                dbcontext.StockTransDocLs.AddRange(StockTransLines);

                                var notelinesToDelete = dbcontext.StockTransDocNotes.Where(x => x.DocEntry == DocEntry);
                                dbcontext.StockTransDocNotes.RemoveRange(notelinesToDelete);

                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From StockTransDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                //dbcontext.SaveChanges();

                                IEnumerable<StockTransDocNotes> StockTransNotes = GetStockTransNotes(NoteLines, DocEntry);
                                dbcontext.StockTransDocNotes.AddRange(StockTransNotes);

                                 WriteSTSourceLocationInventoryLogs(Lines, StockTransObj.DocNum, StockTransObj.UpdatedBy, "Update");

                                 WriteSTDestLocationInventoryLogs(Lines, StockTransObj.DocNum, StockTransObj.UpdatedBy, "Update");
                             
                                dbcontext.SaveChanges();

                                UpdateStockBalance(linesToDelete.ToList(), StockTransObj.UpdatedBy);
                                UpdateStockBalance(Lines, StockTransObj.UpdatedBy);

                                dbcontext.SaveChanges();


                                transaction.Commit();
                            }
                            else
                            {
                                Result = false;
                                ValidationMessage = "There is no Document found";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ValidationMessage = e.Message;
                        transaction.Rollback();
                        Result = false;
                    }
                }
             

            return Result;
        }

        public IEnumerable<StockTransDocLs> GetStockTransLines(List<StockTransDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }
        public IEnumerable<StockTransDocNotes> GetStockTransNotes(List<StockTransDocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }


        public void WriteSTSourceLocationInventoryLogs(List<StockTransDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            var logs = lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "Stock Transter - Source",
                ItemCode = x.ItemCode,
                Qty = -x.Qty,
                TStamp = DateTime.Now.Ticks.ToString(),
                CreatedOn = DateTime.Now,
                Location = x.FromLocation,
                CreatedBy = CreatedBy,
                InsertOrUpdate = InsertOrUpdate

            });

            dbcontext.InventoryLog.AddRange(logs);
        }
        public void WriteSTDestLocationInventoryLogs(List<StockTransDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            var logs = lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "Stock Transter - Dest",
                ItemCode = x.ItemCode,
                Qty = x.Qty,
                TStamp = DateTime.Now.Ticks.ToString(),
                CreatedOn = DateTime.Now,
                Location = x.ToLocation,
                CreatedBy = CreatedBy,
                InsertOrUpdate = InsertOrUpdate

            });

            dbcontext.InventoryLog.AddRange(logs);
        }
        public void UpdateStockBalance(List<StockTransDocLs> lines, string CreatedBy)
        {
            IEnumerable<OWHS> warehouses = null;
            using (var sapdbcontext = new EFSapDbContext())
            {
                warehouses = sapdbcontext.Warehouses.ToList();
            }


            foreach (var item in lines)
            {
                //foreach (var whs in warehouses)
                //{

                    var foundSourceItem = dbcontext.StockBalance.Where(x => x.ItemCode == item.ItemCode && x.Location == item.FromLocation ).FirstOrDefault();
                    var foundDestionItem = dbcontext.StockBalance.Where(x => x.ItemCode == item.ItemCode && x.Location == item.ToLocation  ).FirstOrDefault();

                    var STSourceBalance = dbcontext.StockTransDocLs.Include("StockTransDocH").Where(x => x.ItemCode.Equals(item.ItemCode) && x.FromLocation.Equals(foundSourceItem.Location) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                    var STDestBalance = dbcontext.StockTransDocLs.Include("StockTransDocH").Where(x => x.ItemCode.Equals(item.ItemCode) && x.ToLocation.Equals(foundDestionItem.Location) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    if (foundSourceItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item.ItemCode,
                            STBalance = -STSourceBalance ,                           
                            Location =  item.FromLocation,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                    dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundSourceItem.STBalance = -STSourceBalance;
                    }
                    if (foundDestionItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item.ItemCode,
                            STBalance = STDestBalance,
                            Location = item.ToLocation,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                    dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundDestionItem.STBalance = STDestBalance;
                    }
                    StockBalanceLog stockBalanceLogSrc = new StockBalanceLog()
                    {
                        ItemCode = item.ItemCode,
                        STBalance = -STSourceBalance,
                        Location = item.FromLocation,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                dbcontext.StockBalanceLog.Add(stockBalanceLogSrc);
                    StockBalanceLog stockBalanceLogDst = new StockBalanceLog()
                    {
                        ItemCode = item.ItemCode,
                        STBalance = STDestBalance,
                        Location = item.ToLocation,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                dbcontext.StockBalanceLog.Add(stockBalanceLogDst);
                //}
            }

        }
        public void UpdateStockBalanceItems(List<string> lines, string CreatedBy)
        {
            IEnumerable<OWHS> warehouses = null;
            using (var sapdbcontext = new EFSapDbContext())
            {
                warehouses = sapdbcontext.Warehouses.ToList();
            }

            foreach (var whs in warehouses)
            {
                foreach (var item in lines)
                {
                    //foreach (var whs in warehouses)
                    //{

                    var foundSourceItem = dbcontext.StockBalance.Where(x => x.ItemCode == item && x.Location == whs.WhsCode).FirstOrDefault();
                    //var foundDestionItem = dbcontext.StockBalance.Where(x => x.ItemCode == item && x.Location == item.ToLocation).FirstOrDefault();

                    var STSourceBalance = dbcontext.StockTransDocLs.Include("StockTransDocH").Where(x => x.ItemCode.Equals(item) && x.FromLocation.Equals(whs.WhsCode) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                    var STDestBalance = dbcontext.StockTransDocLs.Include("StockTransDocH").Where(x => x.ItemCode.Equals(item) && x.ToLocation.Equals(whs.WhsCode) && x.StockTransDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    if (foundSourceItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            STBalance = -STSourceBalance + STDestBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundSourceItem.STBalance = -STSourceBalance + STDestBalance;
                    }
                    //if (foundDestionItem == null)
                    //{
                    //    StockBalance stockBalance = new StockBalance()
                    //    {
                    //        ItemCode = item,
                    //        STBalance = STDestBalance,
                    //        Location = item.ToLocation,
                    //        UpdatedOn = DateTime.Now,
                    //        UpdatedBy = CreatedBy
                    //    };
                    //    dbcontext.StockBalance.Add(stockBalance);

                    //}
                    //else
                    //{
                    //    foundDestionItem.STBalance = STDestBalance;
                    //}
                    StockBalanceLog stockBalanceLogSrc = new StockBalanceLog()
                    {
                        ItemCode = item,
                        STBalance = -STSourceBalance + STDestBalance,
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLogSrc);
                    //StockBalanceLog stockBalanceLogDst = new StockBalanceLog()
                    //{
                    //    ItemCode = item,
                    //    STBalance = STDestBalance,
                    //    Location = item.ToLocation,
                    //    UpdatedOn = DateTime.Now,
                    //    UpdatedBy = CreatedBy
                    //};
                    //dbcontext.StockBalanceLog.Add(stockBalanceLogDst);
                    //}
                }
            }

        }
        public string UpdatePrintStatus(string Entry, string printedBy)
        {
            string DocNum = string.Empty;
            using (var dbcontext = new DomainDb())
            {
                long DocEntry = long.Parse(Entry);
                StockTransDocH dbEntry = dbcontext.StockTransDocH.Find(DocEntry);
                if (dbEntry != null)
                {
                    DocNum = dbEntry.DocNum;
                    dbEntry.PrintedBy = printedBy;
                    dbEntry.PrintedOn = DateTime.Now;
                    dbEntry.PrintedCount = dbEntry.PrintedCount + 1;
                    dbEntry.PrintedStatus = 1;
                    dbcontext.SaveChanges();
                }

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
        public StockTransDocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.StockTransDocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is StockTransDocH stockTransDocH)
                    return stockTransDocH;
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
