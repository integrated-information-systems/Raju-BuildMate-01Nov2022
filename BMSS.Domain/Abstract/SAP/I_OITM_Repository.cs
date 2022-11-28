using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_OITM_Repository
    {
        IEnumerable<OITM> Items { get; }
        OITM GetItemDetails(string Itemcode);

        IEnumerable<ItemMonthlySales> GetItemMthlySales(string ItemCode, string WhsCode);
        IEnumerable<ListofStcoks> GetListOfStocks(string WhsCode);
        IEnumerable<WareHouseDetails> GetWareHouses();

        IEnumerable<TransactionTypesList> GetTransactionTypes();
    }
}
