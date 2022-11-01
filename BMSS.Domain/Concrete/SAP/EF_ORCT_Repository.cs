using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_ORCT_Repository : I_ORCT_Repository
    {
        public IEnumerable<ORCT> GetLastPaidRecordsByCustomer(string cardCode)
        {
            using (var dbcontext = new EFSapDbContext())
            {
                return dbcontext.IncomingPayments.Where(x=> x.CardCode.Equals(cardCode) && x.Canceled.Equals("N")).OrderByDescending(x=> x.DocDate).Take(5).ToList();
            }
        }
    }
}
