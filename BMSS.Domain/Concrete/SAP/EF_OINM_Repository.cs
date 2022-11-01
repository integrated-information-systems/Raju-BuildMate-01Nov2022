using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OINM_Repository : I_OINM_Repository
    {
        public IEnumerable<OINM> GetTransactionDetailsByItemCode(string ItemCode)
        {
            IEnumerable<OINM> Transactions = null;
            using (var dbcontext = new EFSapDbContext())
            {
                Transactions = dbcontext.Transactions.Include("Customer").Include("Item").AsNoTracking().Where(x => x.ItemCode.Equals(ItemCode)).OrderByDescending(e => e.DocDate).ToList();
            }
            return Transactions;
        }

        public IEnumerable<InvMovmentView> GetLedgerSalesTxn(string ItemCode)
        {
            
            IEnumerable<InvMovmentView> listInvMovmentView;
            using (var dbcontext = new EFSapDbContext())
            {
                var SqlParamItemCode = new SqlParameter();
                SqlParamItemCode.ParameterName = "@itemcode";
                SqlParamItemCode.SqlDbType = SqlDbType.VarChar;
                SqlParamItemCode.Direction = ParameterDirection.Input;
                SqlParamItemCode.Value = ItemCode;

                listInvMovmentView = dbcontext.Database.SqlQuery<InvMovmentView>(@"EXEC [dbo].[IISsp_LedgerSalesTxn] @itemcode", SqlParamItemCode).ToList();

                //listInvMovmentView = listInvMovmentView.Where(x => x.ItemCode.Equals(ItemCode) && x.SAPDocNum == null).ToList();

            }

            return listInvMovmentView;
        }



    }
}
