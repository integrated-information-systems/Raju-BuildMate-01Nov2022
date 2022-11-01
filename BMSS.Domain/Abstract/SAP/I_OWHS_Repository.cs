using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OWHS_Repository
    {
        IEnumerable<OWHS> Warehouses { get; }

    }
}
