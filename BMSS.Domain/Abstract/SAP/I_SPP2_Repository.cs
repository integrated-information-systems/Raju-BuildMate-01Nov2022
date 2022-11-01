using BMSS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract.SAP
{
    public interface I_SPP2_Repository
    {
        IEnumerable<SPP2> GetItemSpecialPricesByPriceList(Int16 PriceListNum, string ItemCode);
        IEnumerable<SPP2> GetItemSpecialPrices(string Itemcode);
        IEnumerable<SPP2> GetCustomerItemSpecialPrices(string ItemCode, string CardCode);
        IEnumerable<OITM> GetCustomerSpecialPriceItemCodeList(string CardCode);
        decimal GetSpecialPriceByItemCustomerQuantity(string ItemCode, string CardCode, decimal Quantity);
    }
}
