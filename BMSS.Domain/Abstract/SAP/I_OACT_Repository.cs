using BMSS.Domain.Entities;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OACT_Repository
    {
        IEnumerable<OACT> GLCodes { get; }
        string GetGLNameByCode(string GLCode);
    }
}
