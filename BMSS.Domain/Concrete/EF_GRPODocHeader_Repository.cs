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
    public class EF_GRPODocHeader_Repository : I_GRPODocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_GRPODocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<GRPODocH> GRPOHeaderList
        {
            get
            { 
               return dbcontext.GRPODocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();               
            }
        }
        public bool ResubmitGRPOToSAP(GRPODocH GRPOObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
                
                    GRPODocH dbEntry = dbcontext.GRPODocH.Find(GRPOObj.DocEntry);
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
        public IEnumerable<GRPODocH> GRPOHeaderListByCardCode(string CardCode)
        {
           
                return dbcontext.GRPODocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode)).ToList();
       
        }
        public GRPODocH GetByDocNumber(string DocNum)
        {

            
                return dbcontext.GRPODocH.Include("GRPODocLs").Include("GRPODocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
           
        }
        public GRPODocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
            
                return dbcontext.GRPODocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
            
        }
        public bool UpdateNotes(GRPODocH GRPOObj, List<GRPODocNotes> NoteLines, ref string ValidationMessage)
        {
            bool Result = true;
            
                try
                {
                    GRPODocH dbEntry = dbcontext.GRPODocH.Find(GRPOObj.DocEntry);
                    dbcontext.Database.ExecuteSqlCommand(@"Delete From GRPODocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", GRPOObj.DocEntry));
                    dbcontext.SaveChanges();

                    IEnumerable<GRPODocNotes> GRPONotes = GetGRPONotes(NoteLines, GRPOObj.DocEntry);
                    dbcontext.GRPODocNotes.AddRange(GRPONotes);
                    dbcontext.SaveChanges();
                }
                catch
                {
                    Result = false;
                }
            
            return Result;
        }
        public bool AddGRPO(GRPODocH GRPOObj, List<GRPODocLs> Lines, List<GRPODocNotes> NoteLines, ref string ValidationMessage, ref string GRPODocNum)
        {
            bool Result = true;

             
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (GRPOObj.DocNum == "New")
                        {
                            NumberingGRN numberingGRN = dbcontext.NumberingGRN.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingGRN != null)
                            {
                                string DocNum = numberingGRN.Prefix + numberingGRN.NextNo;
                                numberingGRN.NextNo = numberingGRN.NextNo + 1;
                                numberingGRN.IsLocked = true;
                                //GRPOObj.Currency = "SGD";

                                long DocEntry = dbcontext.GRPODocH.Count() + 1;
                                GRPOObj.DocEntry = DocEntry;
                                GRPOObj.DocNum = DocNum;
                                GRPOObj.CreatedOn = DateTime.Now;
                                GRPOObj.UpdatedOn = null;
                                GRPOObj.PrintedCount = 0;
                                if (GRPOObj.CopiedPO != null)
                                {
                                    if (GRPOObj.CopiedPO != string.Empty)
                                    {
                                        GRPOObj.SubmittedToSAP = true;
                                    }
                                }
                                dbcontext.GRPODocH.Add(GRPOObj);
                                

                              
                                GRPODocNum = GRPOObj.DocNum;
                                ValidationMessage = GRPOObj.DocEntry.ToString();

                                IEnumerable<GRPODocLs> GRPOLines = GetGRPOLines(Lines, DocEntry);
                                dbcontext.GRPODocLs.AddRange(GRPOLines);
                               

                                IEnumerable<GRPODocNotes> GRPONotes = GetGRPONotes(NoteLines, DocEntry);
                                dbcontext.GRPODocNotes.AddRange(GRPONotes);
                               


                                if (GRPOObj.CopiedPO != null)
                                {
                                    if (GRPOObj.CopiedPO != string.Empty)
                                    {
                                        string PODocNum = GRPOObj.CopiedPO;
                                        PODocH POHeader = dbcontext.PODocH.Where(i => i.DocNum.Equals(PODocNum)).FirstOrDefault();
                                        if (POHeader != null)
                                        {
                                            POHeader.CopiedGRPO = GRPODocNum;
                                            POHeader.Status = 2; // update to Status Closed (short)DocumentStatuses.Open
                                            POHeader.POPDNs.Add(new POPDNs() { PDNDocNum = GRPODocNum, TStamp = DateTime.Now.Ticks.ToString() });
                                           
                                        }
                                        var POLines = dbcontext.PODocLs.Where(i => i.DocEntry.Equals(POHeader.DocEntry) && i.GRPOQty>0 ).ToList();
                                        POLines.ForEach(x => x.OpenQty = x.OpenQty - Math.Abs(x.GRPOQty));
                                        POLines.ForEach(x => x.GRPOQty = 0);

                                        var NegativePOLines = dbcontext.PODocLs.Where(i => i.DocEntry.Equals(POHeader.DocEntry) && i.GRPOQty < 0).ToList();
                                        NegativePOLines.ForEach(x => x.OpenQty = (Math.Abs(x.OpenQty) - Math.Abs(x.GRPOQty))*-1);
                                        NegativePOLines.ForEach(x => x.GRPOQty = 0);


                                      
                                    }
                                }


                            WriteGRPOInventoryLogs(Lines, GRPOObj.DocNum, GRPOObj.CreatedBy, "Insert");
                                 

                                dbcontext.SaveChanges();

                                UpdateStockBalance(Lines.Select(x => x.ItemCode).ToList(), GRPOObj.CreatedBy);

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
                            GRPODocH dbEntry = dbcontext.GRPODocH.Find(GRPOObj.DocEntry);
                            if (dbEntry != null)
                            {

                                if (dbEntry.SubmittedToSAP.Equals(true))
                                {
                                    Result = false;
                                    ValidationMessage = "This document have been already submitted to SAP, cannot proceed";
                                }
                                else if (dbEntry.SyncedToSAP.Equals(true))
                                {
                                    Result = false;
                                    ValidationMessage = "This document have been already synced to SAP, cannot proceed";
                                }
                                else
                                {


                                    dbEntry.CardCode = GRPOObj.CardCode;
                                    dbEntry.CardName = GRPOObj.CardName;
                                    dbEntry.DocDate = GRPOObj.DocDate;
                                    dbEntry.DueDate = GRPOObj.DueDate;
                                    dbEntry.DeliveryDate = GRPOObj.DeliveryDate;
                                    dbEntry.Status = GRPOObj.Status;
                                    dbEntry.SyncStatus = GRPOObj.SyncStatus;
                                    dbEntry.PaymentTerm = GRPOObj.PaymentTerm;
                                    dbEntry.PaymentTermName = GRPOObj.PaymentTermName;
                                    dbEntry.Currency = GRPOObj.Currency;
                                    dbEntry.ExRate = GRPOObj.ExRate;
                                    dbEntry.OfficeTelNo = GRPOObj.OfficeTelNo;
                                    dbEntry.Fax = GRPOObj.Fax;

                                    dbEntry.SlpCode = GRPOObj.SlpCode;
                                    dbEntry.SlpName = GRPOObj.SlpName;
                                    dbEntry.PORNo = GRPOObj.PORNo;
                                    dbEntry.HeaderRemarks1 = GRPOObj.HeaderRemarks1;
                                    dbEntry.HeaderRemarks2 = GRPOObj.HeaderRemarks2;
                                    dbEntry.HeaderRemarks3 = GRPOObj.HeaderRemarks3;
                                    dbEntry.HeaderRemarks4 = GRPOObj.HeaderRemarks4;
                                    dbEntry.FooterRemarks1 = GRPOObj.FooterRemarks1;
                                    dbEntry.FooterRemarks2 = GRPOObj.FooterRemarks2;
                                    dbEntry.FooterRemarks3 = GRPOObj.FooterRemarks3;
                                    dbEntry.FooterRemarks4 = GRPOObj.FooterRemarks4;

                                    dbEntry.ShipTo = GRPOObj.ShipTo;
                                    dbEntry.ShipToAddress1 = GRPOObj.ShipToAddress1;
                                    dbEntry.ShipToAddress2 = GRPOObj.ShipToAddress2;
                                    dbEntry.ShipToAddress3 = GRPOObj.ShipToAddress3;
                                    dbEntry.ShipToAddress4 = GRPOObj.ShipToAddress4;
                                    dbEntry.ShipToAddress5 = GRPOObj.ShipToAddress5;

                                    dbEntry.BillTo = GRPOObj.BillTo;
                                    dbEntry.BillToAddress1 = GRPOObj.BillToAddress1;
                                    dbEntry.BillToAddress2 = GRPOObj.BillToAddress2;
                                    dbEntry.BillToAddress3 = GRPOObj.BillToAddress3;
                                    dbEntry.BillToAddress4 = GRPOObj.BillToAddress4;
                                    dbEntry.BillToAddress5 = GRPOObj.BillToAddress5;

                                    dbEntry.SelfCollect = GRPOObj.SelfCollect;
                                    dbEntry.SelfCollectRemarks1 = GRPOObj.SelfCollectRemarks1;
                                    dbEntry.SelfCollectRemarks2 = GRPOObj.SelfCollectRemarks2;
                                    dbEntry.SelfCollectRemarks3 = GRPOObj.SelfCollectRemarks3;
                                    dbEntry.SelfCollectRemarks4 = GRPOObj.SelfCollectRemarks4;
                                    dbEntry.SelfCollectRemarks5 = GRPOObj.SelfCollectRemarks5;

                                    dbEntry.DiscByPercent = GRPOObj.DiscByPercent;
                                    dbEntry.DiscAmount = GRPOObj.DiscAmount;
                                    dbEntry.DiscPercent = GRPOObj.DiscPercent;
                                    dbEntry.NetTotal = GRPOObj.NetTotal;
                                    dbEntry.GstTotal = GRPOObj.GstTotal;
                                    dbEntry.GrandTotal = GRPOObj.GrandTotal;
                                    dbEntry.Rounding = GRPOObj.Rounding;
                                    dbEntry.GrandTotalAftRounding = GRPOObj.GrandTotalAftRounding;
                                    dbEntry.UpdatedBy = GRPOObj.UpdatedBy;
                                    dbEntry.UpdatedOn = DateTime.Now;
                                    dbEntry.SubmittedToSAP = GRPOObj.SubmittedToSAP;


                                    dbcontext.SaveChanges();


                                    long DocEntry = GRPOObj.DocEntry;
                                    List<string> itemsToUpdateStock = new List<string>();
                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From GRPODocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                //dbcontext.SaveChanges();

                                var linesToDelete = dbcontext.GRPODocLs.Where(x => x.DocEntry == DocEntry);
                                    dbcontext.GRPODocLs.RemoveRange(linesToDelete);

                                    IEnumerable<GRPODocLs> GRPOLines = GetGRPOLines(Lines, DocEntry);
                                    dbcontext.GRPODocLs.AddRange(GRPOLines);
                                //dbcontext.SaveChanges();


                                itemsToUpdateStock = GRPOLines.Select(x => x.ItemCode).ToList().Union(linesToDelete.Select(x => x.ItemCode).ToList()).ToList();

                                //dbcontext.Database.ExecuteSqlCommand(@"Delete From GRPODocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                //dbcontext.SaveChanges();

                                var notelinesToDelete = dbcontext.GRPODocNotes.Where(x => x.DocEntry == DocEntry);
                                    dbcontext.GRPODocNotes.RemoveRange(notelinesToDelete);

                                    IEnumerable<GRPODocNotes> GRPONotes = GetGRPONotes(NoteLines, DocEntry);
                                    dbcontext.GRPODocNotes.AddRange(GRPONotes);


                                    WriteGRPOInventoryLogs(Lines, GRPOObj.DocNum, GRPOObj.UpdatedBy, "Update");
                                     

                                    dbcontext.SaveChanges();

                                    UpdateStockBalance(itemsToUpdateStock, GRPOObj.UpdatedBy);

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
                        transaction.Rollback();
                        Result = false;
                    }
                
            }


            return Result;
        }
        public void WriteGRPOInventoryLogs(List<GRPODocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
          var  logs = lines.Select((x) => new InventoryLog()
            {
                DocNum = DocNum,
                DocType = "GRPO",
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

                    var GRPOBalance = dbcontext.GRPODocLs.Include("GRPODocH").Where(x => x.ItemCode.Equals(item) && x.Location.Equals(whs.WhsCode) && x.GRPODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                   
                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item,
                            GRPOBalance = GRPOBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbcontext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                       
                        foundItem.GRPOBalance = GRPOBalance;
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item,                       
                        GRPOBalance = GRPOBalance,
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbcontext.StockBalanceLog.Add(stockBalanceLog);
                }
            }

        }
        public IEnumerable<GRPODocLs> GetGRPOLines(List<GRPODocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }
        public IEnumerable<GRPODocNotes> GetGRPONotes(List<GRPODocNotes> Lines, long DocEntry)
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
                GRPODocH dbEntry = dbcontext.GRPODocH.Find(DocEntry);
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
        public GRPODocH GetDocumentWaitForSyncing()
        {
            try
            {

                if (dbcontext.GRPODocH.Where(x => x.SubmittedToSAP == true).FirstOrDefault() is GRPODocH gRPODocH)
                    return gRPODocH;
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
