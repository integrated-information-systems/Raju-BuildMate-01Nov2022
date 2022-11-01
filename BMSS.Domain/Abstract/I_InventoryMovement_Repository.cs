using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_InventoryMovement_Repository
    {
        IEnumerable<DODocLs> GetDOLinesByItemCode(string ItemCode);
        IEnumerable<GRPODocLs> GetGRPOLinesByItemCode(string ItemCode);
        IEnumerable<InvMovmentView> GetInvMovmentLinesByItemCode(string ItemCode);

        IEnumerable<InvMovmentView> GetInvMovmentLinesByItemCodeSP(string ItemCode);
    }
}
