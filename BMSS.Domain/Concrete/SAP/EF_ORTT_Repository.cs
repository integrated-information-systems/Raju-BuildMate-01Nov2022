using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_ORTT_Repository : I_ORTT_Repository
    {
        public decimal GetExchangeRate(string Currency, DateTime DocDate)
        {
            decimal ExchangeRate = 0;
            using (var dbcontext = new EFSapDbContext())
            {
                ORTT OADMObj = dbcontext.ExchangeRates.AsNoTracking().Where(i => i.Currency.Equals(Currency) && i.RateDate.Equals(DocDate)).FirstOrDefault();
                if(OADMObj!=null)
                {
                    ExchangeRate = OADMObj.Rate;
                }
            }
            return ExchangeRate;
        }
    }
}
