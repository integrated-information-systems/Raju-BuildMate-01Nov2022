using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_PODocHeader_Repository : I_PODocH_Repository
    {
        public IEnumerable<PODocH> POHeaderList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.PODocH.Include("POPDNs").OrderByDescending(x => x.DocEntry).ToList();
                }
            }
        }
      
        public IEnumerable<PODocH> POHeaderListByCardCode(string CardCode)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode)).ToList();
            }
        }
        public IEnumerable<PODocH> GetPOHeaderListNoOpenQty()
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocH.Include("POPDNs").AsNoTracking().Where(x => x.PODocLs.Sum(line=> Math.Abs(line.OpenQty))==0).ToList();
            }
        }
        public IEnumerable<PODocH> GetPOHeaderListWithOpenQty()
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocH.Include("POPDNs").AsNoTracking().Where(x => x.PODocLs.Sum(line => Math.Abs(line.OpenQty)) > 0).ToList();
            }
        }
        public PODocH GetByDocNumber(string DocNum)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocH.Include("PODocLs").Include("PODocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            }
        }
        public PODocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PODocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
            }
        }
        public bool UpdateNotes(PODocH POObj, List<PODocNotes> NoteLines, ref string ValidationMessage)
        {
            bool Result = true;
            using (var dbcontext = new DomainDb())
            {
                try
                {
                    PODocH dbEntry = dbcontext.PODocH.Find(POObj.DocEntry);
                    dbcontext.Database.ExecuteSqlCommand(@"Delete From PODocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", POObj.DocEntry));
                    dbcontext.SaveChanges();

                    IEnumerable<PODocNotes> PODocNotes = GetPONotes(NoteLines, POObj.DocEntry);
                    dbcontext.PODocNotes.AddRange(PODocNotes);
                    dbcontext.SaveChanges();
                }
                catch
                {
                    Result = false;
                }
            }
            return Result;
        }


        public bool AddPO(PODocH POObj, List<PODocLs> Lines, List<PODocNotes> NoteLines, ref string ValidationMessage, ref string PODocNum)
        {
            bool Result = true;

            using (var dbcontext = new DomainDb())
            {
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (POObj.DocNum == "New")
                        {
                            NumberingPO numberingPO = dbcontext.NumberingPO.Where(i => i.IsDefault.Equals(true) && (i.NextNo<i.LastNo)).FirstOrDefault();
                            if (numberingPO != null)
                            {
                                string DocNum = numberingPO.Prefix + numberingPO.NextNo;
                                numberingPO.NextNo = numberingPO.NextNo + 1;
                                numberingPO.IsLocked = true;

                                long DocEntry = dbcontext.PODocH.Count() + 1;
                                POObj.DocEntry = DocEntry;
                                POObj.DocNum = DocNum;
                                POObj.CreatedOn = DateTime.Now;
                                POObj.UpdatedOn = null;
                                POObj.PrintedCount = 0;
                                PODocNum = DocNum;
                                dbcontext.PODocH.Add(POObj);
                                dbcontext.SaveChanges();

                                
                                ValidationMessage = POObj.DocEntry.ToString();

                                IEnumerable<PODocLs> POLines = GetPOLines(Lines, DocEntry);
                                dbcontext.PODocLs.AddRange(POLines);
                                dbcontext.SaveChanges();

                                IEnumerable<PODocNotes> PONotes = GetPONotes(NoteLines, DocEntry);
                                dbcontext.PODocNotes.AddRange(PONotes);
                                dbcontext.SaveChanges();

                                if (POObj.CopiedPQ != null)
                                {
                                    if (POObj.CopiedPQ != string.Empty)
                                    {
                                        string PQDocNum = POObj.CopiedPQ;
                                        PQDocH PQHeader = dbcontext.PQDocH.Where(i => i.DocNum.Equals(PQDocNum)).FirstOrDefault();
                                        if (PQHeader != null)
                                        {
                                            PQHeader.CopiedPO = PODocNum;
                                            PQHeader.Status = 2; // update to Status Closed (short)DocumentStatuses.Open                                           
                                            dbcontext.SaveChanges();
                                        }
                                    }
                                }


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
                            PODocH dbEntry = dbcontext.PODocH.Find(POObj.DocEntry);
                            if (dbEntry != null)
                            {


                                if (dbEntry.CopiedGRPO == null)
                                {

                                    dbEntry.CardCode = POObj.CardCode;
                                    dbEntry.CardName = POObj.CardName;
                                    dbEntry.DocDate = POObj.DocDate;
                                    dbEntry.DueDate = POObj.DueDate;
                                    dbEntry.DeliveryDate = POObj.DeliveryDate;
                                    dbEntry.Status = POObj.Status;
                                    dbEntry.PaymentTerm = POObj.PaymentTerm;
                                    dbEntry.PaymentTermName = POObj.PaymentTermName;
                                    dbEntry.Currency = POObj.Currency;
                                    dbEntry.ExRate = POObj.ExRate;
                                    dbEntry.OfficeTelNo = POObj.OfficeTelNo;
                                    dbEntry.Fax = POObj.Fax;
                                    dbEntry.SlpCode = POObj.SlpCode;
                                    dbEntry.SlpName = POObj.SlpName;
                                    dbEntry.Ref = POObj.Ref;
                                    dbEntry.DeliveryTime = POObj.DeliveryTime;
                                    dbEntry.HeaderRemarks1 = POObj.HeaderRemarks1;
                                    dbEntry.HeaderRemarks2 = POObj.HeaderRemarks2;
                                    dbEntry.HeaderRemarks3 = POObj.HeaderRemarks3;
                                    dbEntry.HeaderRemarks4 = POObj.HeaderRemarks4;
                                    dbEntry.FooterRemarks1 = POObj.FooterRemarks1;
                                    dbEntry.FooterRemarks2 = POObj.FooterRemarks2;
                                    dbEntry.FooterRemarks3 = POObj.FooterRemarks3;
                                    dbEntry.FooterRemarks4 = POObj.FooterRemarks4;

                                    dbEntry.ShipTo = POObj.ShipTo;
                                    dbEntry.ShipToAddress1 = POObj.ShipToAddress1;
                                    dbEntry.ShipToAddress2 = POObj.ShipToAddress2;
                                    dbEntry.ShipToAddress3 = POObj.ShipToAddress3;
                                    dbEntry.ShipToAddress4 = POObj.ShipToAddress4;
                                    dbEntry.ShipToAddress5 = POObj.ShipToAddress5;

                                    dbEntry.BillTo = POObj.BillTo;
                                    dbEntry.BillToAddress1 = POObj.BillToAddress1;
                                    dbEntry.BillToAddress2 = POObj.BillToAddress2;
                                    dbEntry.BillToAddress3 = POObj.BillToAddress3;
                                    dbEntry.BillToAddress4 = POObj.BillToAddress4;
                                    dbEntry.BillToAddress5 = POObj.BillToAddress5;

                                    dbEntry.SelfCollect = POObj.SelfCollect;
                                    dbEntry.SelfCollectRemarks1 = POObj.SelfCollectRemarks1;
                                    dbEntry.SelfCollectRemarks2 = POObj.SelfCollectRemarks2;
                                    dbEntry.SelfCollectRemarks3 = POObj.SelfCollectRemarks3;
                                    dbEntry.SelfCollectRemarks4 = POObj.SelfCollectRemarks4;
                                    dbEntry.SelfCollectRemarks5 = POObj.SelfCollectRemarks5;

                                    dbEntry.DiscByPercent = POObj.DiscByPercent;
                                    dbEntry.DiscAmount = POObj.DiscAmount;
                                    dbEntry.DiscPercent = POObj.DiscPercent;
                                    dbEntry.NetTotal = POObj.NetTotal;
                                    dbEntry.GstTotal = POObj.GstTotal;
                                    dbEntry.GrandTotal = POObj.GrandTotal;
                                    //dbEntry.Rounding = POObj.Rounding;
                                    //dbEntry.GrandTotalAftRounding = POObj.GrandTotalAftRounding;
                                    dbEntry.UpdatedBy = POObj.UpdatedBy;
                                    dbEntry.UpdatedOn = DateTime.Now;



                                    dbcontext.SaveChanges();


                                    long DocEntry = POObj.DocEntry;

                                    dbcontext.Database.ExecuteSqlCommand(@"Delete From PODocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                    dbcontext.SaveChanges();

                                    IEnumerable<PODocLs> POLines = GetPOLines(Lines, DocEntry);
                                    dbcontext.PODocLs.AddRange(POLines);
                                    dbcontext.SaveChanges();

                                    dbcontext.Database.ExecuteSqlCommand(@"Delete From PODocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));
                                    dbcontext.SaveChanges();

                                    IEnumerable<PODocNotes> PONotes = GetPONotes(NoteLines, DocEntry);
                                    dbcontext.PODocNotes.AddRange(PONotes);
                                    dbcontext.SaveChanges();

                                    transaction.Commit();
                                }
                                else
                                {
                                    long DocEntry = POObj.DocEntry;
                                    for (int i = 0; i < Lines.Count(); i++)
                                    {
                                        PODocLs POL = dbcontext.PODocLs.Where(x => x.DocEntry.Equals(DocEntry) && x.LineNum.Equals(i) && x.OpenQty!=0).FirstOrDefault();
                                        if(POL !=null) { 
                                            POL.GRPOQty = Lines[i].GRPOQty;
                                            dbcontext.SaveChanges();
                                        }
                                    }                                                                       
                                    transaction.Commit();
                                }
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
                        Console.WriteLine(e.Message);
                        transaction.Rollback();
                        Result = false;
                    }
                }
            }


            return Result;
        }

        public IEnumerable<PODocLs> GetPOLines(List<PODocLs> Lines, long DocEntry)
        {
            int HaveGRPOCount = Lines.Where(x => x.GRPOQty > 0).Count();
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                if (HaveGRPOCount > 0)
                {
                    Lines[i].OpenQty = Lines[i].Qty;
                }
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }
        public IEnumerable<PODocNotes> GetPONotes(List<PODocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }
            return Lines;
        }

        public List<string> GetPDNList(long DocEntry)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.POPDNs.Where(x => x.DocEntry== DocEntry).OrderByDescending(x=> x.PDNDocNum).Select(x=> x.PDNDocNum).ToList();
            }
        }
        public string UpdatePrintStatus(string Entry, string printedBy)
        {
            string DocNum = "";
            using (var dbcontext = new DomainDb())
            {
                long DocEntry = long.Parse(Entry);
                PODocH dbEntry = dbcontext.PODocH.Find(DocEntry);
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
    }
}
