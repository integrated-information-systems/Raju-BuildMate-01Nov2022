using BMSS.Domain.Abstract;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_InventoryMovement_Repository : I_InventoryMovement_Repository
    {
        public IEnumerable<DODocLs> GetDOLinesByItemCode(string ItemCode)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.DODocLs.Include("DODocH").Where(x => x.ItemCode.Equals(ItemCode) && x.DODocH.SAPDocNum.Equals(null)).ToList();
            }
        }
        public IEnumerable<GRPODocLs> GetGRPOLinesByItemCode(string ItemCode)
        {

            using (var dbcontext = new DomainDb())
            {
                return dbcontext.GRPODocLs.Include("GRPODocH").Where(x => x.ItemCode.Equals(ItemCode) && x.GRPODocH.SAPDocNum.Equals(null)).ToList();
            }
        }

        public IEnumerable<InvMovmentView> GetInvMovmentLinesByItemCode(string ItemCode)
        {
            using (var dbcontext = new DomainDb())
            {
                return dbcontext.InvMovmentView.Where(x => x.ItemCode.Equals(ItemCode) && x.SAPDocNum == null).ToList();
            }
        }

        public IEnumerable<InvMovmentView> GetInvMovmentLinesByItemCodeSP(string ItemCode)
        {
            //using (var dbcontext = new DomainDb())
            //{
            //    return dbcontext.InvMovmentView.Where(x => x.ItemCode.Equals(ItemCode) && x.SAPDocNum == null).ToList();
            //}

            IEnumerable<InvMovmentView> listInvMovmentView;
            using (var dbcontext = new DomainDb())
            {
                var SqlParamItemCode = new SqlParameter();
                SqlParamItemCode.ParameterName = "@ItemCode";
                SqlParamItemCode.SqlDbType = SqlDbType.VarChar;
                SqlParamItemCode.Direction = ParameterDirection.Input;
                SqlParamItemCode.Value = ItemCode;

                listInvMovmentView = dbcontext.Database.SqlQuery<InvMovmentView>(@"EXEC [dbo].[IISsp_GetInvMovmentLinesByItemCode] @ItemCode", SqlParamItemCode).ToList();

                listInvMovmentView = listInvMovmentView.Where(x => x.ItemCode.Equals(ItemCode) && x.SAPDocNum == null).ToList();

            }

            return listInvMovmentView;
        }

    }
}
