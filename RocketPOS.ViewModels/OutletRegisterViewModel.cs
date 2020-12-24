using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using RocketPOS.Model;
namespace RocketPOS.ViewModels
{
    public class OutletRegisterViewModel
    {
        public int InsertOutletRegister(OutletRegisterModel outletRegisterModel)
        {
            int result = 0;
            using (var connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]))
            {
                connection.Open();

                SqlTransaction sqltrans = connection.BeginTransaction();

                var query = "EXEC pxOutletRegister 1,@OutletId,@UserId,@OpeningBalance";

                //var query = "INSERT INTO OutletRegister " +
                //            "(OutletId,USerID,OpenDate,OpeningBalance) " +
                //            "VALUES " +
                //            "(@OutletId,@UserId,GETDATE(),@OpeningBalance) " +
                //            "); SELECT CAST(SCOPE_IDENTITY() as INT);";
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
            using (var connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]))
            {
                connection.Open();
                SqlTransaction sqltrans = connection.BeginTransaction();

                var query = "EXEC pxOutletRegister 2,@OutletId,@UserId,NULL";

                //var query = "Update OutletRegister SET CloseDateTime=GETDATE() " +
                //            " WHERE OutletId=@OutletId AND UserId=@UserId";
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
