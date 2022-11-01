using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq.Dynamic;
using System.Linq;
using BMSS.Domain.Entities;
using BMSS.Domain.Concrete.SAP;

namespace BMSS.Domain.Concrete
{
    public class EF_CashSalesDocHeader_Repository : I_CashSalesDocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_CashSalesDocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<CashSalesDocH> CashSalesHeaderList
        {
            get
            {
                 
                    return dbcontext.CashSalesDocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();
                 
            }
        }
        public IEnumerable<CashSalesDocH> CSHeaderListByCardCode(string CardCode)
        {
             
                return dbcontext.CashSalesDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode)).ToList();
            
        }

        public bool ResubmitCSToSAP(CashSalesDocH CSObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
                
                    CashSalesDocH dbEntry = dbcontext.CashSalesDocH.Find(CSObj.DocEntry);
                    if (dbEntry != null)
                    {
                        dbEntry.SyncStatus = 1;
                        dbEntry.SubmittedToSAP = true;
                        dbEntry.SubmittedBy = CSObj.SubmittedBy;
                        dbEntry.SubmittedOn = DateTime.Now;
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

        public IEnumerable<CashSalesDocH> CashSalesHeaderBalanceDueList(string CardCode)
        {           
                return dbcontext.CashSalesDocH.AsNoTracking().ToList();           
        }
        public CashSalesDocH GetByDocNumber(string DocNum)
        {
           
                return dbcontext.CashSalesDocH.Include("CashSalesDocLs").Include("CashSalesDocNotes").Include("CashSalesDocPays").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();            
        }
        public bool NotePlannerSubmission(string DocNum, ref int SentCount)
        {
            bool Result = true;
            try
            { 
                    if (dbcontext.CashSalesDocH.Where(x => x.DocNum == DocNum).FirstOrDefault() is CashSalesDocH dODocH)
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
        public CashSalesDocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
            
                return dbcontext.CashSalesDocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();            
        }
        public bool AddCS(CashSalesDocH CashSalesObj, List<CashSalesDocLs> Lines, List<CashSalesDocNotes> NoteLines, List<CashSalesDocPays> PayLines, ref string ValidationMessage, ref string CSDocNum)
        {
            bool Result = true;

           
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (CashSalesObj.DocNum == "New")
                        {
                            NumberingCSale numberingCashSales = dbcontext.NumberingCSale.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingCashSales != null)
                            {
                                string DocNum = numberingCashSales.Prefix + numberingCashSales.NextNo;
                                numberingCashSales.NextNo = numberingCashSales.NextNo + 1;
                                numberingCashSales.IsLocked = true;

                                long DocEntry = dbcontext.CashSalesDocH.Count() + 1;
                                CashSalesObj.DocEntry = DocEntry;
                                CashSalesObj.Currency = "SGD";
                                CashSalesObj.DocNum = DocNum;
                                CashSalesObj.CreatedOn = DateTime.Now;
                                CashSalesObj.UpdatedOn = null;
                                CashSalesObj.INVPrintedCount = 0;                                
                                CSDocNum = DocNum;

                                if (CashSalesObj.SubmittedToSAP == true)
                                {
                                CashSalesObj.SubmittedBy = CashSalesObj.CreatedBy;
                                CashSalesObj.SubmittedOn = DateTime.Now;
                                }

                            dbcontext.CashSalesDocH.Add(CashSalesObj);
                             

                                
                                ValidationMessage = CashSalesObj.DocEntry.ToString();

                                IEnumerable<CashSalesDocLs> CashSalesLines = GetCashSalesLines(Lines, DocEntry);
                                dbcontext.CashSalesDocLs.AddRange(CashSalesLines);
                               

                                IEnumerable<CashSalesDocNotes> CashSalesNotes = GetCashSalesNotes(NoteLines, DocEntry);
                                dbcontext.CashSalesDocNotes.AddRange(CashSalesNotes);
                               

                                IEnumerable<CashSalesDocPays> CashSalesPays = GetCashSalesPays(PayLines, DocEntry);
                                dbcontext.CashSalesDocPays.AddRange(CashSalesPays);

                                WriteCSInventoryLogs(Lines, CashSalesObj.DocNum, CashSalesObj.CreatedBy, "Insert");
                                

                                dbcontext.SaveChanges();

                                UpdateStockBalance(Lines.Select(x => x.ItemCode).ToList(), CashSalesObj.CreatedBy);

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
                            CashSalesDocH dbEntry = dbcontext.CashSalesDocH.Find(CashSalesObj.DocEntry);
                            if (dbEntry != null)
                            {




                                dbEntry.CardCode = CashSalesObj.CardCode;
                                dbEntry.CardName = CashSalesObj.CardName;
                                dbEntry.DocDate = CashSalesObj.DocDate;
                                dbEntry.DueDate = CashSalesObj.DueDate;
                                dbEntry.DeliveryDate = CashSalesObj.DeliveryDate;
                                dbEntry.Status = CashSalesObj.Status;
                                dbEntry.SyncStatus = CashSalesObj.SyncStatus;
                                dbEntry.PaymentTerm = CashSalesObj.PaymentTerm;
                                dbEntry.PaymentTermName = CashSalesObj.PaymentTermName;
                                dbEntry.PaymentLocation = CashSalesObj.PaymentLocation;                              
                                dbEntry.Currency = "SGD";                               
                                dbEntry.OfficeTelNo = CashSalesObj.OfficeTelNo;
                                dbEntry.CashSalesCardName = CashSalesObj.CashSalesCardName;

                                dbEntry.SlpCode = CashSalesObj.SlpCode;
                                dbEntry.SlpName = CashSalesObj.SlpName;

                                //dbEntry.GLCode = CashSalesObj.GLCode;
                                //dbEntry.GLName = CashSalesObj.GLName;
                                //dbEntry.PaymentRemarks = CashSalesObj.PaymentRemarks;
                                //dbEntry.ChequeNoReference = CashSalesObj.ChequeNoReference;
                                //dbEntry.PaidAmount = CashSalesObj.PaidAmount;
                                //dbEntry.PaymentType = CashSalesObj.PaymentType;

                                dbEntry.HeaderRemarks1 = CashSalesObj.HeaderRemarks1;
                                dbEntry.HeaderRemarks2 = CashSalesObj.HeaderRemarks2;
                                dbEntry.HeaderRemarks3 = CashSalesObj.HeaderRemarks3;
                                dbEntry.HeaderRemarks4 = CashSalesObj.HeaderRemarks4;
                                dbEntry.FooterRemarks1 = CashSalesObj.FooterRemarks1;
                                dbEntry.FooterRemarks2 = CashSalesObj.FooterRemarks2;
                                dbEntry.FooterRemarks3 = CashSalesObj.FooterRemarks3;
                                dbEntry.FooterRemarks4 = CashSalesObj.FooterRemarks4;
                                dbEntry.CustomerRef = CashSalesObj.CustomerRef;
                                dbEntry.DeliveryTime = CashSalesObj.DeliveryTime;
                                 
                                dbEntry.ShipToAddress1 = CashSalesObj.ShipToAddress1;
                                dbEntry.ShipToAddress2 = CashSalesObj.ShipToAddress2;
                                dbEntry.ShipToAddress3 = CashSalesObj.ShipToAddress3;
                                dbEntry.ShipToAddress4 = CashSalesObj.ShipToAddress4;
                                dbEntry.ShipToAddress5 = CashSalesObj.ShipToAddress5;

                                dbEntry.BillToAddress1 = CashSalesObj.BillToAddress1;
                                dbEntry.BillToAddress2 = CashSalesObj.BillToAddress2;
                                dbEntry.BillToAddress3 = CashSalesObj.BillToAddress3;
                                dbEntry.BillToAddress4 = CashSalesObj.BillToAddress4;
                                dbEntry.BillToAddress5 = CashSalesObj.BillToAddress5;

                                //dbEntry.SelfCollect = CashSalesObj.SelfCollect;
                                //dbEntry.SelfCollectRemarks1 = CashSalesObj.SelfCollectRemarks1;
                                //dbEntry.SelfCollectRemarks2 = CashSalesObj.SelfCollectRemarks2;
                                //dbEntry.SelfCollectRemarks3 = CashSalesObj.SelfCollectRemarks3;
                                //dbEntry.SelfCollectRemarks4 = CashSalesObj.SelfCollectRemarks4;

                                dbEntry.DiscByPercent = CashSalesObj.DiscByPercent;
                                dbEntry.DiscAmount = CashSalesObj.DiscAmount;
                                dbEntry.DiscPercent = CashSalesObj.DiscPercent;
                                dbEntry.NetTotal = CashSalesObj.NetTotal;
                                dbEntry.GstTotal = CashSalesObj.GstTotal;
                                dbEntry.GrandTotal = CashSalesObj.GrandTotal;
                                dbEntry.Rounding = CashSalesObj.Rounding;
                                dbEntry.GrandTotalAftRounding = CashSalesObj.GrandTotalAftRounding;
                                dbEntry.UpdatedBy = CashSalesObj.UpdatedBy;
                                dbEntry.UpdatedOn = DateTime.Now;
                                dbEntry.SubmittedToSAP = CashSalesObj.SubmittedToSAP;

                                if (CashSalesObj.SubmittedToSAP == true)
                                {
                                    dbEntry.SubmittedBy = CashSalesObj.UpdatedBy;
                                    dbEntry.SubmittedOn = DateTime.Now;
                                }



                            long DocEntry = CashSalesObj.DocEntry;
                                List<string> itemsToUpdateStock = new List<string>();


                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From CashSalesDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            var linesToDelete = dbcontext.CashSalesDocLs.Where(x => x.DocEntry == DocEntry);
                                dbcontext.CashSalesDocLs.RemoveRange(linesToDelete);

                                IEnumerable<CashSalesDocLs> CashSalesLines = GetCashSalesLines(Lines, DocEntry);
                                dbcontext.CashSalesDocLs.AddRange(CashSalesLines);


                            itemsToUpdateStock = CashSalesLines.Select(x => x.ItemCode).ToList().Union(linesToDelete.Select(x => x.ItemCode).ToList()).ToList();


                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From CashSalesDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            //dbcontext.SaveChanges();
                            var notelinesToDelete = dbcontext.CashSalesDocNotes.Where(x => x.DocEntry == DocEntry);
                                dbcontext.CashSalesDocNotes.RemoveRange(notelinesToDelete);

                                IEnumerable<CashSalesDocNotes> CashSalesNotes = GetCashSalesNotes(NoteLines, DocEntry);
                                dbcontext.CashSalesDocNotes.AddRange(CashSalesNotes);


                                var paylinesToDelete = dbcontext.CashSalesDocPays.Where(x => x.DocEntry == DocEntry);
                                dbcontext.CashSalesDocPays.RemoveRange(paylinesToDelete);

                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From CashSalesDocPays Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                //dbcontext.SaveChanges();

                                IEnumerable<CashSalesDocPays> CashSalesPays = GetCashSalesPays(PayLines, DocEntry);
                                dbcontext.CashSalesDocPays.AddRange(CashSalesPays);


                                WriteCSInventoryLogs(Lines, CashSalesObj.DocNum, CashSalesObj.UpdatedBy, "Update");
                              

                                dbcontext.SaveChanges();

                                UpdateStockBalance(itemsToUpdateStock, CashSalesObj.UpdatedBy);

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

        public IEnumerable<CashSalesDocLs> GetCashSalesLines(List<CashSalesDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }
        public IEnumerable<CashSalesDocNotes> GetCashSalesNotes(List<CashSalesDocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }
        public IEnumerable<CashSalesDocPays> GetCashSalesPays(List<CashSalesDocPays> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }

        public void WriteCSInventoryLogs(List<CashSalesDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            var logs =  lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "CS",
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

        public void UpdateStockBalance(List<string> lines, string CreatedBy)
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

                    var CSBalance = dbcontext.CashSalesDocLs.Include("CashSalesDocH").Where(x => x.ItemCode.Equals(item) && x.Location.Equals(whs.WhsCode) && x.CashSalesDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;
                   
                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            CSBalance = CSBalance,                           
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundItem.CSBalance = CSBalance;                         
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item,
                        CSBalance = CSBalance,                       
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLog);
                }
            }

        }

        public string UpdatePrintStatus(string Entry, string printedBy, string DocType = "DO")
        {
            string DocNum = "";
             
                long DocEntry = long.Parse(Entry);
                CashSalesDocH dbEntry = dbcontext.CashSalesDocH.Find(DocEntry);
                if (dbEntry != null)
                {
                    DocNum = dbEntry.DocNum;
                    dbEntry.PrintedBy = printedBy;
                    dbEntry.PrintedOn = DateTime.Now;
                    if (DocType == "INV")
                    {
                        dbEntry.INVPrintedCount = dbEntry.INVPrintedCount + 1;
                    }
                    dbEntry.PrintedStatus = 1;
                    dbcontext.SaveChanges();
                }

            
            return DocNum;
        }
        public IEnumerable<CashSalesDocH> GetCSDetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "")
        {
            IEnumerable<CashSalesDocH> CSs = null;
            
                if (string.IsNullOrEmpty(search))
                    CSs = dbcontext.CashSalesDocH.OrderBy(orderBy).Skip(skip).Take(rowsCount).ToList();
                else
                    CSs = dbcontext.CashSalesDocH.Where(x =>
                    x.DocNum.Contains(search)
                    ||
                    x.CardName.Contains(search)
                    ||
                    x.CardCode.Contains(search)).OrderBy(orderBy)
                    .Skip(skip).Take(rowsCount).ToList();
            
            return CSs;
        }
        public int GetCSDetailsWithPaginationCount()
        {
            int count = 0;
            using (var dbcontext = new DomainDb())
            {
                count = dbcontext.CashSalesDocH.Count();
            }
            return count;
        }

        public void CommitChanges()
        {
            dbcontext.SaveChanges();
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }
        public CashSalesDocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.CashSalesDocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is CashSalesDocH cashSalesDocH)
                    return cashSalesDocH;
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
