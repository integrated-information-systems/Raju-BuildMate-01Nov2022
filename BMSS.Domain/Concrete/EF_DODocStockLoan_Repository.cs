using BMSS.Domain.Abstract;
using BMSS.Domain.Concrete.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_DODocStockLoan_Repository : I_DODocStockLoan_Repository
    {
        public IEnumerable<DODocStockLoan> StockLoanList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.DODocStockLoan.Include("DODocH").AsNoTracking().ToList();
                }
            }
        }
        public DODocStockLoan Get(long DocEntry)
        {
            DODocStockLoan dbEntry = null;
            using (var dbcontext = new DomainDb())
            {
                dbEntry = dbcontext.DODocStockLoan.Include(x=> x.DODocH).Include(x=> x.DODocH.DODocLs).Where(x => x.DocEntry.Equals(DocEntry)).FirstOrDefault();
            }
            return dbEntry;
        }
        public bool ReverseStockLoan(long DOEntry,string  ReversedBy, ref string ValidationMessage, ref DateTime ReversedOn)
        {
            bool Result = true;
            using (var dbcontext = new DomainDb())
            {
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        DODocStockLoan dbEntry = dbcontext.DODocStockLoan.Where(x=> x.DocEntry.Equals(DOEntry)).FirstOrDefault();
                        if (dbEntry != null)
                        {
                            
                            if (dbEntry.Reversed.Equals(false))
                            {
                                dbEntry.Reversed = true;
                                dbEntry.ReversedBy = ReversedBy;
                                dbEntry.ReversedOn = DateTime.Now;
                                ReversedOn = dbEntry.ReversedOn.Value;
                                
                                DODocLs dbDOLineEntry = dbcontext.DODocLs.Where(x => x.DocEntry.Equals(dbEntry.DODocEntry) && x.LineNum.Equals(dbEntry.LineNum)).FirstOrDefault();
                                dbDOLineEntry.LoanIssued = false;

                                dbcontext.SaveChanges();

                                int Count = dbcontext.DODocLs.Where(x => x.LoanIssued.Equals(true) && x.DocEntry.Equals(dbEntry.DODocEntry)).Count();
                                if(Count.Equals(0))
                                {
                                    DODocH dbDOHeaderEntry = dbcontext.DODocH.Find(dbEntry.DODocEntry);
                                    dbDOHeaderEntry.HaveStockLoan = false;                                   
                                }
                                InventoryLog inventoryLog = new InventoryLog()
                                {
                                    DocNum = dbEntry.DocNum,
                                    DocType = "Stock Loan - Reverse",
                                    ItemCode = dbEntry.ItemCode,
                                    Qty = -dbEntry.Qty,
                                    TStamp = DateTime.Now.Ticks.ToString(),
                                    CreatedOn = DateTime.Now,
                                    Location = dbDOLineEntry.Location,
                                    CreatedBy = ReversedBy,
                                    InsertOrUpdate = "Update"

                                };

                                dbcontext.InventoryLog.Add(inventoryLog);
                                dbcontext.SaveChanges();

                                var foundItem = dbcontext.StockBalance.Where(x => x.ItemCode == dbEntry.ItemCode && x.Location == dbDOLineEntry.Location).FirstOrDefault();
                                var StockLoanBalance = dbcontext.DODocLs.Include(y => y.DODocH.DODocStockLoan).Where(x => x.ItemCode.Equals(dbEntry.ItemCode) && x.Location.Equals(dbDOLineEntry.Location) && x.DODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.DODocH.DODocStockLoan.Sum(y => (y.Reversed == false && y.LineNum == x.LineNum) ? y.Qty : 0)) ?? 0;
                                if (foundItem == null)
                                {
                                    StockBalance stockBalance = new StockBalance()
                                    {
                                        ItemCode = dbEntry.ItemCode,                                        
                                        SLBalance = StockLoanBalance,
                                        Location = dbDOLineEntry.Location,
                                        UpdatedOn = DateTime.Now,
                                        UpdatedBy = ReversedBy
                                    };
                                    dbcontext.StockBalance.Add(stockBalance);

                                }
                                else
                                {                                   
                                    foundItem.SLBalance = StockLoanBalance;
                                }
                                StockBalanceLog stockBalanceLog = new StockBalanceLog()
                                {
                                    ItemCode = dbEntry.ItemCode,
                                    SLBalance = StockLoanBalance,
                                    Location = dbDOLineEntry.Location,
                                    UpdatedOn = DateTime.Now,
                                    UpdatedBy = ReversedBy
                                };
                                dbcontext.StockBalanceLog.Add(stockBalanceLog);


                                dbcontext.SaveChanges();

                                transaction.Commit();
                            }
                            else
                            {
                                Result = false;
                                ValidationMessage = "This document already reversed, cannot proceed";
                            }
                        }
                        else
                        {
                            Result = false;
                            ValidationMessage = "Document not found, cannot proceed";
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        transaction.Rollback();
                        Result = false;
                    }
                }
            }

            return Result;
        }


        public IEnumerable<InventoryLog> GetInventoryLogs(List<DODocLs> lines, string DocNum, string CreatedBy, string InsertOrUpdate = "Insert")
        {
            return lines.Select((x) => new InventoryLog()
            {
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
        }

        public void UpdateStockBalance(DomainDb dbContext, List<DODocLs> lines, string CreatedBy)
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
                    var foundItem = dbContext.StockBalance.Where(x => x.ItemCode == item.ItemCode && x.Location == whs.WhsCode).FirstOrDefault();

                    var DOBalance = dbContext.DODocLs.Include("DODocH").Where(x => x.ItemCode.Equals(item.ItemCode) && x.Location.Equals(whs.WhsCode) && x.DODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.Qty) ?? 0;

                    var StockLoanBalance = dbContext.DODocLs.Include(y => y.DODocH.DODocStockLoan).Where(x => x.ItemCode.Equals(item.ItemCode) && x.Location.Equals(whs.WhsCode) && x.DODocH.SAPDocNum.Equals(null)).Sum(x => (decimal?)x.DODocH.DODocStockLoan.Sum(y => (y.Reversed == false && y.LineNum == x.LineNum) ? y.Qty : 0)) ?? 0;

                    if (foundItem == null)
                    {
                        StockBalance stockBalance = new StockBalance()
                        {
                            ItemCode = item.ItemCode,
                            DOBalance = DOBalance,
                            SLBalance = StockLoanBalance,
                            Location = whs.WhsCode,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = CreatedBy
                        };
                        dbContext.StockBalance.Add(stockBalance);

                    }
                    else
                    {
                        foundItem.DOBalance = DOBalance;
                        foundItem.SLBalance = StockLoanBalance;
                    }
                    StockBalanceLog stockBalanceLog = new StockBalanceLog()
                    {
                        ItemCode = item.ItemCode,
                        DOBalance = DOBalance,
                        SLBalance = StockLoanBalance,
                        Location = whs.WhsCode,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = CreatedBy
                    };
                    dbContext.StockBalanceLog.Add(stockBalanceLog);
                }
            }

        }
    }
}
