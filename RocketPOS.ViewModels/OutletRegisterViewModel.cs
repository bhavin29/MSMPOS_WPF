using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using System.Data.SqlClient;
using RocketPOS.Core.Constants;

namespace RocketPOS.ViewModels
{
    public class OutletRegisterViewModel
    {
        AppSettings appSettings = new AppSettings();
        public int InsertOutletRegister(OutletRegisterModel outletRegisterModel)
        {
            int result = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();

                SqlTransaction sqltrans = connection.BeginTransaction();

                var query = "EXEC pxOutletRegister 1,@OutletId,@UserId,@OpeningBalance";

                result = connection.Execute(query, outletRegisterModel, sqltrans, 0, System.Data.CommandType.Text);

                if (result > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
            }
            return result;
        }

        public int UpdateOutletRegister(OutletRegisterModel outletRegisterModel)
        {
            int result = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction sqltrans = connection.BeginTransaction();

                var query = "EXEC pxOutletRegister 2, " + 
                                  LoginDetail.OutletId + ", " +
                                  LoginDetail.UserId + ", NULL ;";

                result = connection.Execute(query, outletRegisterModel, sqltrans, 0, System.Data.CommandType.Text);

                if (result > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
            }
            return result;
        }
    }
}
