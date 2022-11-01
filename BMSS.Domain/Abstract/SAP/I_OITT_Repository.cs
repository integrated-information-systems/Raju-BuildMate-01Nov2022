using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OITT_Repository
    {
        IEnumerable<OITT> GetAllOITT { get; }
    }
}
