using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_PaymentDocHeader_Repository : I_PaymentDocH_Repository
    {
        public IEnumerable<PaymentDocH> PaymentHeaderList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.PaymentDocH.AsNoTracking().ToList();
                }
            }
        }
        public DateTime GetLastPaidDate(string CardCode)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PaymentDocH.Where(x=> x.CardCode.Equals(CardCode)).OrderByDescending(x=> x.DocDate).Select(x=> x.DocDate).FirstOrDefault();
            }
        }
        public bool ResubmitPaymentToSAP(PaymentDocH PaymentObj, ref string ValidationMessage)
        {
            bool Result = true;

            try
            {
                using (var dbcontext = new DomainDb())
                {
                    PaymentDocH dbEntry = dbcontext.PaymentDocH.Find(PaymentObj.DocEntry);
                    if (dbEntry != null)
                    {
                        dbEntry.SyncStatus = 1;
                        dbEntry.SubmittedToSAP = true;

                        dbEntry.SubmittedOn = DateTime.Now;
                        dbEntry.SubmittedBy = PaymentObj.SubmittedBy;

                        dbcontext.SaveChanges();

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
                Result = false;
            }
            return Result;
        }
        public PaymentDocH GetByDocNumber(string DocNum)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PaymentDocH.Include("PaymentDocLs").Include("PaymentDocNotes").AsNoTracking().Where(x => x.DocNum.Equals(DocNum)).FirstOrDefault();
            }
        }
        public PaymentDocH GetByDocEntry(string DocEntry)
        {
            long DEntry = long.Parse(DocEntry);
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.PaymentDocH.Where(x => x.DocEntry.Equals(DEntry)).FirstOrDefault();
            }
        }

        public bool AddPayment(PaymentDocH PaymentObj, List<PaymentDocLs> Lines, List<PaymentDocNotes> NoteLines, ref string ValidationMessage, ref string PayDocNum)
        {
            bool Result = true;

            using (var dbcontext = new DomainDb())
            {
                using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {


                        if (PaymentObj.DocNum == "New")
                        {
                            NumberingPay numberingPayment = dbcontext.NumberingPay.Where(i => i.IsDefault.Equals(true) && (i.NextNo <= i.LastNo)).FirstOrDefault();
                            if (numberingPayment != null)
                            {
                                string DocNum = numberingPayment.Prefix + numberingPayment.NextNo;
                                numberingPayment.NextNo = numberingPayment.NextNo + 1;
                                numberingPayment.IsLocked = true;

                                long DocEntry = dbcontext.PaymentDocH.Count() + 1;
                                PaymentObj.DocEntry = DocEntry;
                                PaymentObj.DocNum = DocNum;
                                PayDocNum = DocNum;
                                PaymentObj.CreatedOn = DateTime.Now;
                                PaymentObj.SubmittedToSAP = true;


                                if (PaymentObj.SubmittedToSAP == true)
                                {
                                    PaymentObj.SubmittedBy = PaymentObj.CreatedBy;
                                    PaymentObj.SubmittedOn = DateTime.Now;
                                }

                                dbcontext.PaymentDocH.Add(PaymentObj);
                                

                                
                                ValidationMessage = PaymentObj.DocEntry.ToString();

                                List<PaymentDocLs> listLines = Lines;
                                for (int i = 0; i < listLines.Count(); i++)
                                {
                                    string DODocNo = listLines[i].ReferenceDocNum;
                                    decimal BalanceDue = listLines[i].BalanceDue;
                                    decimal DocTotal = listLines[i].DocTotal;
                                    decimal PaidAmount = listLines[i].PaymentAmount;
                                    DODocH foundDODoc = dbcontext.DODocH.Where(x => x.DocNum.Equals(DODocNo) && x.BalanceDue.Equals(BalanceDue) && x.GrandTotal.Equals(DocTotal)).FirstOrDefault();
                                    if (foundDODoc == null)
                                    {                                        
                                        ValidationMessage = "DO " + DODocNo + " Document Total/Balance Due changed, cannot add Payment";
                                        throw new Exception("Cannot add payment");
                                    }
                                    else
                                    {
                                        foundDODoc.SubmittedToSAP = true;
                                        foundDODoc.BalanceDue = foundDODoc.BalanceDue - PaidAmount;
                                    }
                                }


                                IEnumerable<PaymentDocLs> PaymentLines = GetPaymentLines(Lines, DocEntry);
                                dbcontext.PaymentDocLs.AddRange(PaymentLines);
                               

                                IEnumerable<PaymentDocNotes> PaymentNotes = GetPaymentNotes(NoteLines, DocEntry);
                                dbcontext.PaymentDocNotes.AddRange(PaymentNotes);


                                dbcontext.SaveChanges();

                                transaction.Commit();
                            }
                            else
                            {
                                               
                                ValidationMessage = "There is no Document Numbering Series definition found";
                                throw new Exception("Cannot add payment");
                            }
                        }
                        else
                        {
                            throw new Exception("Cannot add payment");
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

        public IEnumerable<PaymentDocLs> GetPaymentLines(List<PaymentDocLs> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }
        public IEnumerable<PaymentDocNotes> GetPaymentNotes(List<PaymentDocNotes> Lines, long DocEntry)
        {
            for (int i = 0; i < Lines.Count(); i++)
            {
                Lines[i].DocEntry = DocEntry;
            }
            return Lines;
        }
        public string UpdatePrintStatus(string Entry, string printedBy)
        {
            string DocNum = "";
            using (var dbcontext = new DomainDb())
            {

                long DocEntry = long.Parse(Entry);
                PaymentDocH dbEntry = dbcontext.PaymentDocH.Find(DocEntry);
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
