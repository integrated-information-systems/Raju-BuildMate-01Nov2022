using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OITM_Repository : I_OITM_Repository
    {
        public IEnumerable<OITM> Items
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.Items.Include("ItemGroup").AsNoTracking().ToList();
                }
            }
        }
        public OITM GetItemDetails(string ItemCode)
        {
            OITM Item = null;
            using (var dbcontext = new EFSapDbContext())
            {
                Item = dbcontext.Items.Include("ItemGroup").AsNoTracking().Where(i => i.ItemCode.Equals(ItemCode)).FirstOrDefault();              
            }
            return Item;
        }
        
        public IEnumerable<ItemMonthlySales> GetItemMthlySales(string ItemCode,string WhsCode)
        {
            IEnumerable<ItemMonthlySales> itemMonthlySales;            
            using (var dbcontext = new EFSapDbContext())
            {
                var SqlParamItemCode = new SqlParameter();
                SqlParamItemCode.ParameterName = "@itemcode";
                SqlParamItemCode.SqlDbType = SqlDbType.VarChar;
                SqlParamItemCode.Direction = ParameterDirection.Input;
                SqlParamItemCode.Value = ItemCode;

                var SqlParamWhsCode = new SqlParameter();
                SqlParamWhsCode.ParameterName = "@whscode";
                SqlParamWhsCode.SqlDbType = SqlDbType.VarChar;
                SqlParamWhsCode.Direction = ParameterDirection.Input;
                SqlParamWhsCode.Value = WhsCode;

                itemMonthlySales = dbcontext.Database.SqlQuery<ItemMonthlySales>(@"EXEC [dbo].[IISsp_StocksMthlySales] @itemcode, @whscode", SqlParamItemCode, SqlParamWhsCode).ToList();
                
            }


            return itemMonthlySales;
        }

        public IEnumerable<ListofStcoks> GetListOfStocks(string WhsCode)
        {
            IEnumerable<ListofStcoks> listofStcoks;
            using (var dbcontext = new EFSapDbContext())
            {
                var SqlParamItemCode = new SqlParameter();
                SqlParamItemCode.ParameterName = "@WhsCode";
                SqlParamItemCode.SqlDbType = SqlDbType.VarChar;
                SqlParamItemCode.Direction = ParameterDirection.Input;
                SqlParamItemCode.Value = WhsCode;

                listofStcoks = dbcontext.Database.SqlQuery<ListofStcoks>(@"EXEC [dbo].[IISsp_ListOfStocks] @WhsCode", SqlParamItemCode).ToList();
                
            }
            
            return listofStcoks;
        }

        public IEnumerable<WareHouseDetails> GetWareHouses()
        {
            IEnumerable<WareHouseDetails> wareHouseDetails;
            using (var dbcontext = new EFSapDbContext())
            {

                wareHouseDetails = dbcontext.Database.SqlQuery<WareHouseDetails>(
                   @"
                        Select WhsCode,WhsName from OWHS Order by WhsCode DESC
                    ").ToList();                               

            }

            wareHouseDetails = wareHouseDetails.Concat(new[]{new WareHouseDetails {
                                                 WhsCode = "ALL",
                                                 WhsName = "ALL"
                                                 }});
            wareHouseDetails = wareHouseDetails.OrderBy(x => x.WhsCode); 
            return wareHouseDetails;
        }


        public IEnumerable<TransactionTypesList> GetTransactionTypes()
        {                       
            IEnumerable<TransactionTypesList> transactionTypesList = Enumerable.Empty<TransactionTypesList>();

            transactionTypesList = transactionTypesList.Concat(new[]{new TransactionTypesList {
                                                     TxnCode = "ALL",
                                                     TxnTypeName = "ALL"
                                                 }});
            transactionTypesList = transactionTypesList.Concat(new[]{new TransactionTypesList {
                                                     TxnCode = "SalesTxn",
                                                     TxnTypeName = "Sales Transaction"
                                                 }});
            transactionTypesList = transactionTypesList.Concat(new[]{new TransactionTypesList {
                                                     TxnCode = "PurchaseTxn",
                                                     TxnTypeName = "Purchase Transaction"
                                                 }});
            transactionTypesList = transactionTypesList.Concat(new[]{new TransactionTypesList {
                                                     TxnCode = "InvTxn",
                                                     TxnTypeName = "Inventory Transaction"
                                                 }});
            transactionTypesList = transactionTypesList.OrderBy(x => x.TxnCode);
            return transactionTypesList;
        }





    }
}
