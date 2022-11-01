using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
namespace BMSS.Domain.Concrete
{
    public class EF_SQDocHeader_Repository : I_SQDocH_Repository
    {
        private readonly DomainDb dbcontext;

        public EF_SQDocHeader_Repository()
        {
            dbcontext = new DomainDb();
        }
        public IEnumerable<SQDocH> SalesQuotationHeaderList
        {
            get
            {
                
                    return dbcontext.SQDocH.AsNoTracking().OrderByDescending(x => x.DocEntry).ToList();
                
            }
        }

        public SQDocH GetByDocNumber(string DocNum)
        {

           
                return dbcontext.SQDocH.Include("SQDocLs").Include("SQDocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            
        }
        public IEnumerable<SQDocH> SalesQuotationHeaderListByCardCode(string CardCode)
        {
            
                return dbcontext.SQDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode)).ToList();
            
        }
        public bool AddSQ(SQDocH SQObj, List<SQDocLs> Lines, List<SQDocNotes> NoteLines, ref string ValidationMessage, ref string SQDocNum)
        {
            bool Result = true;
            
                
                    using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        try
                        {

                    
                            if (SQObj.DocNum == "New")
                            {
                                NumberingSQ numberingSQ =   dbcontext.NumberingSQ.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                                if (numberingSQ !=null) { 
                                string DocNum = numberingSQ.Prefix + numberingSQ.NextNo;
                                numberingSQ.NextNo = numberingSQ.NextNo + 1;
                                numberingSQ.IsLocked = true;

                                long DocEntry = dbcontext.SQDocH.Count() + 1;
                                SQObj.DocEntry = DocEntry;
                                SQObj.DocNum = DocNum;
                                SQObj.CreatedOn = DateTime.Now;
                                SQObj.UpdatedOn = null;
                                SQDocNum = DocNum;
                               
                                dbcontext.SQDocH.Add(SQObj);
                                dbcontext.SaveChanges();

                               
                                ValidationMessage = SQObj.DocEntry.ToString();
                                IEnumerable<SQDocLs> SQLines = GetSQLines(Lines, DocEntry);
                                dbcontext.SQDocLs.AddRange(SQLines);
                                dbcontext.SaveChanges();

                                IEnumerable<SQDocNotes> SQNotes = GetSQNotes(NoteLines, DocEntry);
                                dbcontext.SQDocNotes.AddRange(SQNotes);
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
                            SQDocH dbEntry = dbcontext.SQDocH.Find(SQObj.DocEntry);
                                if (dbEntry != null)
                                {




                                dbEntry.CardCode = SQObj.CardCode;
                                dbEntry.CardName = SQObj.CardName;
                                dbEntry.DocDate = SQObj.DocDate;
                                dbEntry.DueDate = SQObj.DueDate;
                                dbEntry.DeliveryDate = SQObj.DeliveryDate;
                                dbEntry.Status = SQObj.Status;
                                dbEntry.PaymentTerm = SQObj.PaymentTerm;
                                dbEntry.PaymentTermName = SQObj.PaymentTermName;
                                dbEntry.Currency = SQObj.Currency;
                                dbEntry.ExRate = SQObj.ExRate;
                                dbEntry.OfficeTelNo = SQObj.OfficeTelNo;                               
                                dbEntry.Fax = SQObj.Fax;
                                dbEntry.SlpCode = SQObj.SlpCode;
                                dbEntry.SlpName = SQObj.SlpName;
                                dbEntry.CustomerRef = SQObj.CustomerRef;
                                dbEntry.DeliveryTime = SQObj.DeliveryTime;

                                dbEntry.HeaderRemarks1 = SQObj.HeaderRemarks1;
                                dbEntry.HeaderRemarks2 = SQObj.HeaderRemarks2;
                                dbEntry.HeaderRemarks3 = SQObj.HeaderRemarks3;
                                dbEntry.HeaderRemarks4 = SQObj.HeaderRemarks4;
                                dbEntry.FooterRemarks1 = SQObj.FooterRemarks1;
                                dbEntry.FooterRemarks2 = SQObj.FooterRemarks2;
                                dbEntry.FooterRemarks3 = SQObj.FooterRemarks3;
                                dbEntry.FooterRemarks4 = SQObj.FooterRemarks4;

                                dbEntry.ShipTo = SQObj.ShipTo;
                                dbEntry.ShipToAddress1 = SQObj.ShipToAddress1;
                                dbEntry.ShipToAddress2 = SQObj.ShipToAddress2;
                                dbEntry.ShipToAddress3 = SQObj.ShipToAddress3;
                                dbEntry.ShipToAddress4 = SQObj.ShipToAddress4;
                                dbEntry.ShipToAddress5 = SQObj.ShipToAddress5;


                                dbEntry.BillTo = SQObj.BillTo;
                                dbEntry.BillToAddress1 = SQObj.BillToAddress1;
                                dbEntry.BillToAddress2 = SQObj.BillToAddress2;
                                dbEntry.BillToAddress3 = SQObj.BillToAddress3;
                                dbEntry.BillToAddress4 = SQObj.BillToAddress4;
                                dbEntry.BillToAddress5 = SQObj.BillToAddress5;

                                dbEntry.DiscByPercent = SQObj.DiscByPercent;
                                dbEntry.DiscAmount = SQObj.DiscAmount;
                                dbEntry.DiscPercent = SQObj.DiscPercent;
                                dbEntry.NetTotal = SQObj.NetTotal;
                                dbEntry.GstTotal = SQObj.GstTotal;
                                dbEntry.GrandTotal = SQObj.GrandTotal;
                                //dbEntry.Rounding = SQObj.Rounding;
                                //dbEntry.GrandTotalAftRounding = SQObj.GrandTotalAftRounding;
                                dbEntry.UpdatedBy = SQObj.UpdatedBy;
                                dbEntry.UpdatedOn = DateTime.Now;

 

                                dbcontext.SaveChanges();


                                long DocEntry = SQObj.DocEntry;

                                dbcontext.Database.ExecuteSqlCommand(@"Delete From SQDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                dbcontext.SaveChanges();

                                IEnumerable<SQDocLs> SQLines = GetSQLines(Lines, DocEntry);
                                dbcontext.SQDocLs.AddRange(SQLines);
                                dbcontext.SaveChanges();

                                dbcontext.Database.ExecuteSqlCommand(@"Delete From SQDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                dbcontext.SaveChanges();

                                IEnumerable<SQDocNotes> SQNotes = GetSQNotes(NoteLines, DocEntry);
                                dbcontext.SQDocNotes.AddRange(SQNotes);
                                dbcontext.SaveChanges();

                                transaction.Commit();
                                }
                                else
                                {

                                }
                            }
                        }
                        catch(Exception e) 
                        {
                        Console.WriteLine(e.Message);
                            transaction.Rollback();
                            Result = false;
                        }
                    
                }
             
           
                return Result;
        }

        public IEnumerable<SQDocLs> GetSQLines(List<SQDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }

            return Lines;
        }
        public IEnumerable<SQDocNotes> GetSQNotes(List<SQDocNotes> Lines, long DocEntry)
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
            string DocNum = string.Empty;
           
                long DocEntry = long.Parse(Entry);
                SQDocH dbEntry = dbcontext.SQDocH.Find(DocEntry);
                if (dbEntry != null)
                {
                    DocNum = dbEntry.DocNum;
                    dbEntry.PrintedBy = printedBy;
                    dbEntry.PrintedOn = DateTime.Now;
                    dbEntry.PrintedStatus = 1;
                    dbcontext.SaveChanges();
                }

             
            return DocNum;
        }

        public IEnumerable<SQDocH> GetSQDetailsWithPagination(int skip, int rowsCount, string search = "", string orderBy = "")
        {
            IEnumerable<SQDocH> SQs = null;

            if (string.IsNullOrEmpty(search))
                SQs = dbcontext.SQDocH.OrderBy(orderBy).Skip(skip).Take(rowsCount).ToList();
            else
                SQs = dbcontext.SQDocH.Where(x =>
                x.DocNum.Contains(search)
                ||
                x.CardName.Contains(search)
                ||
                x.CardCode.Contains(search)).OrderBy(orderBy)
                .Skip(skip).Take(rowsCount).ToList();

            return SQs;
        }
        public int GetSQDetailsWithPaginationCount()
        {
            int count = 0;

            count = dbcontext.SQDocH.Count();

            return count;
        }
        public void Dispose()
        {
            dbcontext.Dispose();
        }
    }
}
