using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_PaymentDocH_Repository
    {
        IEnumerable<PaymentDocH> PaymentHeaderList { get; }
        bool ResubmitPaymentToSAP(PaymentDocH PaymentObj, ref string ValidationMessage);
        bool AddPayment(PaymentDocH PaymentObj, List<PaymentDocLs> Lines, List<PaymentDocNotes> NoteLines, ref string ValidationMessage, ref string PayDocNum);
        PaymentDocH GetByDocNumber(string DocNum);
      
        string UpdatePrintStatus(string DocEntry, string printedBy);
        DateTime GetLastPaidDate(string CardCode);
    }
}
