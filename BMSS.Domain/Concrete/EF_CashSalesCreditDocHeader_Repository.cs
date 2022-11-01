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
    public class EF_CashSalesCreditDocHeader_Repository : I_CashSalesCreditDocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_CashSalesCreditDocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<CashSalesCreditDocH> CashSalesCreditHeaderList
        {
            get
           {                
                    return dbcontext.CashSalesCreditDocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();                
            }
        }
        public bool ResubmitCSCreditToSAP(CashSalesCreditDocH CSCObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
                
                    CashSalesCreditDocH dbEntry = dbcontext.CashSalesCreditDocH.Find(CSCObj.DocEntry);
                    if (dbEntry != null)
                    {
                        dbEntry.SyncStatus = 1;
                        dbEntry.SubmittedToSAP = true;
                        dbEntry.SubmittedBy = CSCObj.UpdatedBy;
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
        public CashSalesCreditDocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
             
                return dbcontext.CashSalesCreditDocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
            
        }
        public CashSalesCreditDocH GetByDocNumber(string DocNum)
        {

            
                return dbcontext.CashSalesCreditDocH.Include("CashSalesCreditDocLs").Include("CashSalesCreditDocNotes").Include("CashSalesCreditDocPays").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            
        }
        public IEnumerable<CashSalesCreditDocH> CSCHeaderListByCardCode(string CardCode)
        {
             
                return dbcontext.CashSalesCreditDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode)).ToList();
             
        }
        public bool AddCSC(CashSalesCreditDocH CashSalesObj, List<CashSalesCreditDocLs> Lines, List<CashSalesCreditDocNotes> NoteLines, List<CashSalesCreditDocPays> PayLines, ref string ValidationMessage, ref string CSCDocNum)
        {
            bool Result = true;

            
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (CashSalesObj.DocNum == "New")
                        {
                            NumberingCrSale numberingCashSalesCredit = dbcontext.NumberingCrSale.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingCashSalesCredit != null)
                            {
                                string DocNum = numberingCashSalesCredit.Prefix + numberingCashSalesCredit.NextNo;
                                numberingCashSalesCredit.NextNo = numberingCashSalesCredit.NextNo + 1;
                                numberingCashSalesCredit.IsLocked = true;

                                long DocEntry = dbcontext.CashSalesCreditDocH.Count() + 1;
                                CashSalesObj.DocEntry = DocEntry;
                                CashSalesObj.Currency = "SGD";
                                CashSalesObj.DocNum = DocNum;
                                CSCDocNum = DocNum;
                                CashSalesObj.CreatedOn = DateTime.Now;
                                CashSalesObj.PrintedCount = 0;
                                CashSalesObj.UpdatedOn = null;

                                if (CashSalesObj.SubmittedToSAP == true)
                                {
                                    CashSalesObj.SubmittedBy = CashSalesObj.CreatedBy;
                                    CashSalesObj.SubmittedOn = DateTime.Now;
                                }

                            dbcontext.CashSalesCreditDocH.Add(CashSalesObj);
                          

                                 
                                ValidationMessage = CashSalesObj.DocEntry.ToString();

                                IEnumerable<CashSalesCreditDocLs> CashSalesLines = GetCashSalesCreditLines(Lines, DocEntry);
                                dbcontext.CashSalesCreditDocLs.AddRange(CashSalesLines);
                               
                                IEnumerable<CashSalesCreditDocNotes> CashSalesNotes = GetCashSalesCreditNotes(NoteLines, DocEntry);
                                dbcontext.CashSalesCreditDocNotes.AddRange(CashSalesNotes);
                              

                                IEnumerable<CashSalesCreditDocPays> CashSalesPays = GetCashSalesCreditPays(PayLines, DocEntry);
                                dbcontext.CashSalesCreditDocPays.AddRange(CashSalesPays);
                              

                                WriteCSCInventoryLogs(Lines, CashSalesObj.DocNum, CashSalesObj.CreatedBy, "Insert");
                               

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
                            CashSalesCreditDocH dbEntry = dbcontext.CashSalesCreditDocH.Find(CashSalesObj.DocEntry);
                            if (dbEntry != null)
                            {




                                dbEntry.CardCode = CashSalesObj.CardCode;
                                dbEntry.CardName = CashSalesObj.CardName;
                                dbEntry.DocDate = CashSalesObj.DocDate;
                                //dbEntry.DueDate = CashSalesObj.DueDate;
                                //dbEntry.DeliveryDate = CashSalesObj.DeliveryDate;
                                dbEntry.Status = CashSalesObj.Status;
                                dbEntry.SyncStatus = CashSalesObj.SyncStatus;
                                //dbEntry.PaymentTerm = CashSalesObj.PaymentTerm;
                                //dbEntry.PaymentTermName = CashSalesObj.PaymentTermName;
                                dbEntry.PaymentLocation = CashSalesObj.PaymentLocation;
                                dbEntry.Currency = "SGD";
                                dbEntry.OfficeTelNo = CashSalesObj.OfficeTelNo;
                                dbEntry.CashSalesCardName = CashSalesObj.CashSalesCardName;
                                dbEntry.SRINo = CashSalesObj.SRINo;
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

                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From CashSalesCreditDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            var linesToDelete = dbcontext.CashSalesCreditDocLs.Where(x => x.DocEntry == DocEntry);
                                dbcontext.CashSalesCreditDocLs.RemoveRange(linesToDelete);



                           
                            IEnumerable<CashSalesCreditDocLs> CashSalesLines = GetCashSalesCreditLines(Lines, DocEntry);
                                dbcontext.CashSalesCreditDocLs.AddRange(CashSalesLines);


                            itemsToUpdateStock = CashSalesLines.Select(x => x.ItemCode).ToList().Union(linesToDelete.Select(x => x.ItemCode).ToList()).ToList();


                            //dbcontext.Database.ExecuteSqlCommand(@"Delete From CashSalesCreditDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                            var notelinesToDelete = dbcontext.CashSalesCreditDocNotes.Where(x => x.DocEntry == DocEntry);
                                dbcontext.CashSalesCreditDocNotes.RemoveRange(notelinesToDelete);

                                IEnumerable<CashSalesCreditDocNotes> CashSalesNotes = GetCashSalesCreditNotes(NoteLines, DocEntry);
                                dbcontext.CashSalesCreditDocNotes.AddRange(CashSalesNotes);

                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From CashSalesCreditDocPays Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));

                                var paylinesToDelete = dbcontext.CashSalesCreditDocPays.Where(x => x.DocEntry == DocEntry);
                                dbcontext.CashSalesCreditDocPays.RemoveRange(paylinesToDelete);

                                IEnumerable<CashSalesCreditDocPays> CashSalesPays = GetCashSalesCreditPays(PayLines, DocEntry);
                                dbcontext.CashSalesCreditDocPays.AddRange(CashSalesPays);

                                 WriteCSCInventoryLogs(Lines, CashSalesObj.DocNum, CashSalesObj.UpdatedBy, "Update");
                                 

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

        public IEnumerable<CashSalesCreditDocLs> GetCashSalesCreditLines(List<CashSalesCreditDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
                
            }
            return Lines;
        }

        public void WriteCSCInventoryLogs(List<CashSalesCreditDocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            var logs = lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "CSC",
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

                    var CSCBalance = dbcontext.CashSalesCreditDocLs.Include("CashSalesCreditDocH").Where(x => x.ItemCode.Equals(item) && x.Location.Equals(whs.WhsCode) && x.CashSalesCreditDocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            CSCBalance = CSCBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundItem.CSCBalance = CSCBalance;
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item,
                        CSCBalance = CSCBalance,    
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLog);
                }
            }

        }
        public IEnumerable<CashSalesCreditDocNotes> GetCashSalesCreditNotes(List<CashSalesCreditDocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
                
            }
            return Lines;
        }
        public IEnumerable<CashSalesCreditDocPays> GetCashSalesCreditPays(List<CashSalesCreditDocPays> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
                
            }
            return Lines;
        }
        public string UpdatePrintStatus(string Entry, string printedBy)
        {
            string DocNum = "";
           
                long DocEntry = long.Parse(Entry);
                CashSalesCreditDocH dbEntry = dbcontext.CashSalesCreditDocH.Find(DocEntry);
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
        public CashSalesCreditDocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.CashSalesCreditDocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is CashSalesCreditDocH cashSalesCreditDocH)
                    return cashSalesCreditDocH;
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
