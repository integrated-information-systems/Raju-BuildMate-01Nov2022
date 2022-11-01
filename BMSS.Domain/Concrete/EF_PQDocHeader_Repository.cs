using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete
{
    public class EF_PQDocHeader_Repository : I_PQDocH_Repository
    {
        public IEnumerable<PQDocH> PurchaseQuotationHeaderList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.PQDocH.OrderByDescending(x => x.DocEntry).ToList();
                }
            }
        }

        public bool AddPQ(PQDocH PQObj, List<PQDocLs> Lines, List<PQDocNotes> NoteLines, ref string ValidationMessage, ref string PQDocNum)
        {
            bool Result = true;
            using (var dbcontext = new DomainDb())
            {

                try
                {
                    if (PQObj.DocNum == "New")
                    {
                        NumberingPQ numberingPQ = dbcontext.NumberingPQ.Where(i => i.IsDefault.Equals(true) && (i.NextNo < i.LastNo)).FirstOrDefault();
                        if (numberingPQ != null)
                        {
                            string DocNum = numberingPQ.Prefix + numberingPQ.NextNo;
                            numberingPQ.NextNo = numberingPQ.NextNo + 1;
                            numberingPQ.IsLocked = true;

                            long DocEntry = dbcontext.PQDocH.Count() + 1;
                            PQObj.DocEntry = DocEntry;
                            PQObj.DocNum = DocNum;
                            PQObj.CreatedOn = DateTime.Now;
                            PQObj.UpdatedOn = null;
                            PQDocNum = DocNum;

                            dbcontext.PQDocH.Add(PQObj);
                             

                            ValidationMessage = PQObj.DocEntry.ToString();
                            IEnumerable<PQDocLs> PQLines = GetPQLines(Lines, DocEntry);
                            dbcontext.PQDocLs.AddRange(PQLines);
                             

                            IEnumerable<PQDocNotes> PQNotes = GetPQNotes(NoteLines, DocEntry);
                            dbcontext.PQDocNotes.AddRange(PQNotes);


                            dbcontext.SaveChanges();
                        }
                        else
                        {
                            Result = false;
                            ValidationMessage = "There is no Document Numbering Series definition found";
                        }
                    }
                    else
                    {
                        PQDocH dbEntry = dbcontext.PQDocH.Find(PQObj.DocEntry);
                        if (dbEntry != null)
                        {
                            dbEntry.CardCode = PQObj.CardCode;
                            dbEntry.CardName = PQObj.CardName;
                            dbEntry.DocDate = PQObj.DocDate;
                            dbEntry.DueDate = PQObj.DueDate;
                            dbEntry.DeliveryDate = PQObj.DeliveryDate;
                            dbEntry.Status = PQObj.Status;
                            dbEntry.PaymentTerm = PQObj.PaymentTerm;
                            dbEntry.PaymentTermName = PQObj.PaymentTermName;
                            dbEntry.Currency = PQObj.Currency;
                            dbEntry.ExRate = PQObj.ExRate;
                            dbEntry.OfficeTelNo = PQObj.OfficeTelNo;
                            dbEntry.Fax = PQObj.Fax;
                            dbEntry.SlpCode = PQObj.SlpCode;
                            dbEntry.SlpName = PQObj.SlpName;
                            dbEntry.Ref = PQObj.Ref;
                            dbEntry.DeliveryTime = PQObj.DeliveryTime;
                            dbEntry.HeaderRemarks1 = PQObj.HeaderRemarks1;
                            dbEntry.HeaderRemarks2 = PQObj.HeaderRemarks2;
                            dbEntry.HeaderRemarks3 = PQObj.HeaderRemarks3;
                            dbEntry.HeaderRemarks4 = PQObj.HeaderRemarks4;
                            dbEntry.FooterRemarks1 = PQObj.FooterRemarks1;
                            dbEntry.FooterRemarks2 = PQObj.FooterRemarks2;
                            dbEntry.FooterRemarks3 = PQObj.FooterRemarks3;
                            dbEntry.FooterRemarks4 = PQObj.FooterRemarks4;

                            dbEntry.ShipTo = PQObj.ShipTo;
                            dbEntry.ShipToAddress1 = PQObj.ShipToAddress1;
                            dbEntry.ShipToAddress2 = PQObj.ShipToAddress2;
                            dbEntry.ShipToAddress3 = PQObj.ShipToAddress3;
                            dbEntry.ShipToAddress4 = PQObj.ShipToAddress4;
                            dbEntry.ShipToAddress5 = PQObj.ShipToAddress5;

                            dbEntry.BillTo = PQObj.BillTo;
                            dbEntry.BillToAddress1 = PQObj.BillToAddress1;
                            dbEntry.BillToAddress2 = PQObj.BillToAddress2;
                            dbEntry.BillToAddress3 = PQObj.BillToAddress3;
                            dbEntry.BillToAddress4 = PQObj.BillToAddress4;
                            dbEntry.BillToAddress5 = PQObj.BillToAddress5;

                            dbEntry.DiscByPercent = PQObj.DiscByPercent;
                            dbEntry.DiscAmount = PQObj.DiscAmount;
                            dbEntry.DiscPercent = PQObj.DiscPercent;
                            dbEntry.NetTotal = PQObj.NetTotal;
                            dbEntry.GstTotal = PQObj.GstTotal;
                            dbEntry.GrandTotal = PQObj.GrandTotal;
                            //dbEntry.Rounding = PQObj.Rounding;
                            //dbEntry.GrandTotalAftRounding = PQObj.GrandTotalAftRounding;
                            dbEntry.UpdatedBy = PQObj.UpdatedBy;
                            dbEntry.UpdatedOn = DateTime.Now;

                            long DocEntry = PQObj.DocEntry;

                            dbcontext.Database.ExecuteSqlCommand(@"Delete From PQDocLs Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));


                            IEnumerable<PQDocLs> PQLines = GetPQLines(Lines, DocEntry);
                            dbcontext.PQDocLs.AddRange(PQLines);


                            dbcontext.Database.ExecuteSqlCommand(@"Delete From PQDocNotes Where DocEntry=@DocEntry", new SqlParameter("@DocEntry", DocEntry));


                            IEnumerable<PQDocNotes> PQNotes = GetPQNotes(NoteLines, DocEntry);
                            dbcontext.PQDocNotes.AddRange(PQNotes);
                            dbcontext.SaveChanges();
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
                    Result = false;
                }
            }
                return Result;
        }

        public PQDocH GetByDocNumber(string DocNum)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PQDocH.Include("PQDocLs").Include("PQDocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            }
        }

        public IEnumerable<PQDocH> PurchaseQuotationHeaderListByCardCode(string CardCode)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PQDocH.AsNoTracking().Where(x => x.CardCode.Equals(CardCode)).ToList();
            }
        }

        public IEnumerable<PQDocLs> GetPQLines(List<PQDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
                Lines[i].TStamp = DateTime.Now.Ticks.ToString();
            }

            return Lines;
        }
        public IEnumerable<PQDocNotes> GetPQNotes(List<PQDocNotes> Lines, long DocEntry)
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
            using (var dbcontext = new DomainDb())
            {
                long DocEntry = long.Parse(Entry);
                PQDocH dbEntry = dbcontext.PQDocH.Find(DocEntry);
                if (dbEntry != null)
                {
                    DocNum = dbEntry.DocNum;
                    dbEntry.PrintedBy = printedBy;
                    dbEntry.PrintedOn = DateTime.Now;
                    dbEntry.PrintedStatus = 1;
                    dbcontext.SaveChanges();
                }

            }
            return DocNum;
        }
    }


}
