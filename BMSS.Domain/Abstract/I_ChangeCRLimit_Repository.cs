using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_ChangeCRLimit_Repository
    {
        IEnumerable<ChangeCRLimit> ChangeCRLimitList { get; }
        bool ChangeCreditLimit(ChangeCRLimit CRLObj);
        ChangeCRLimit GetChangeCRLimitDetails(string CardCode);
    }
}
