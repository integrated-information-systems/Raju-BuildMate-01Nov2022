using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OVTG_Repository
    {
        IEnumerable<OVTG> TaxCodes { get; }
    }
}
