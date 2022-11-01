using System;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_ORTT_Repository 
    {
        decimal GetExchangeRate(string Currency, DateTime DocDate);
    }
}
