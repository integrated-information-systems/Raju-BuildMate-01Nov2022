using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.General
{
    public enum PrintDocTypes
    {
        Sales_Quotation = 1,
        Delivery_Order = 2,
        DO_Tax_Invoice = 3,
        Cash_Sales = 4,
        Cash_Sales_Tax_Invoice=5,
        Cash_Sales_Credit = 6,        
        Purchase_Order = 7,
        Purchase_Delivery_Note = 8,
        Stock_Transfer =9,
        Stock_In = 10,
        Stock_Out = 11,
        Payment = 12,
        Purchase_Quotation = 13,
        SAP_PO = 14


    }
}