using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OITW_Repository
    {        
        IEnumerable<OITW> GetLocationStockDetails(string Itemcode);
        IEnumerable<OITW> GetItemStockDetails();

        IEnumerable<IGrouping<OITM, OITW>> GetItemStockDetailsWithPagination(int page, int rowsCount, string search="", string orderBy="");

        int GetItemStockDetailsWithPaginationCount();

        decimal SAPAvailableQty(string itemCode,string WhsCode);
        decimal GetLocationStockAvailableQty(string ItemCode, string WhsCode);
    }
}
