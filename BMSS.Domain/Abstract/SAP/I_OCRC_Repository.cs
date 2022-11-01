using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OCRC_Repository
    {        
        IEnumerable<OCRC> CreditCards { get; }
    }
}
