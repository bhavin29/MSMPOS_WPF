using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
namespace RocketPOS.ViewModels
{
    public class LoginViewModel
    {
        AppSettings appSettings = new AppSettings();
        List<LoginModel> loginModel = new List<LoginModel>();
 
        public List<LoginModel> GetUserLogin(string userName, string Password)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                var query = "SELECT U.Id as UserId,U.Username,U.OutletId,O.OutletName,U.RoletypeId, " +
                    " CASE WHEN(SELECT COUNT(*) from OutletRegister WHERE OutletId = U.OutletId and UserId = U.Id AND ApprovalUserId IS Null) = 0 THEN 0 ELSE 0 END AS 'OutletRegisterStatus' " +
                    " ,C.ClientName,C.Address1,C.Address2,C.Email,C.Phone,C.Logo,C.OpenTime,C.CloseTime,C.CurrencyId,C.TimeZone,C.Header,C.Footer,C.Footer1,C.Footer2,C.Footer3,C.Footer4 " +
                    " from " +
                    " [User] U " +
                    " INNER JOIN OUTLET O ON O.Id = U.OutletId " +
                    " CROSS JOIN Client C " +
                    " WHERE USERNAME = '" + userName + "' AND [PASSWORD] = '" + Password +"'";

                loginModel = connection.Query<LoginModel>(query).ToList();

            }

            return loginModel;
        }

    }
}
