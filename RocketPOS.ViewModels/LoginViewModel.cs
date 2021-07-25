using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using RocketPOS.Core.Constants;

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
                //var query = " SELECT U.Id as UserId,U.Username,O.Id as OutletId,O.OutletName,U.RoletypeId, E.LastName,E.FirstName,O.StoreId ," +
                //         " CASE WHEN(SELECT COUNT(*) from OutletRegister WHERE OutletId = ORG.OutletId and UserId = U.Id AND CloseDateTime IS Null) = 0 THEN 0 ELSE 1 END AS 'OutletRegisterStatus' " +
                //         " , (SELECT max(OpenDate) from OutletRegister WHERE OutletId = ORG.OutletId and UserId = U.Id AND CloseDateTime IS Null)  AS 'SystemDate'  , " +
                //         " C.ClientName,C.Address1,C.Address2,C.Email,C.Phone,C.Logo,C.WebSite,C.ReceiptPrefix,C.OrderPrefix,C.OpenTime,C.InvoiceTerms, " +
                //         "  C.CloseTime,C.CurrencyId,C.TimeZone,C.Header,C.Footer,C.Footer1,C.Footer2,C.Footer3,C.Footer4, ORG.Id as OutletRegisterId,C.MainWindowSettings,C.HeaderMarqueeText,C.DeliveryList,C.DiscountList,C.Powerby,C.TaxInclusive, C.IsItemOverright " +
                //          " from[User] U INNER JOIN OutletRegister ORG ON U.Id = ORG.UserId " +
                //         " INNER JOIN Outlet O On O.Id = ORG.OutletId " +
                //        " INNER JOIN Employee E On E.Id = U.EmployeeId " +
                //         " CROSS JOIN Client C " +
                //         " WHERE   ApprovalUserId IS NULL AND USERNAME = '" + userName + "' AND [PASSWORD] = '" + Password + "' AND U.IsActive=1 AND O.IsActive=1";


                var query = " SELECT U.Id as UserId,U.Username,O.Id as OutletId,O.OutletName,U.RoletypeId, E.LastName,E.FirstName,O.StoreId , " +
                     "  1 AS 'OutletRegisterStatus'" +
                     "  , Getdate() AS 'SystemDate' ," +
                     "  C.ClientName,C.Address1,C.Address2,C.Email,C.Phone,C.Logo,C.WebSite,C.ReceiptPrefix,C.OrderPrefix,C.OpenTime,C.InvoiceTerms,  " +
                     " C.CloseTime,C.CurrencyId,C.TimeZone,C.Header,C.Footer,C.Footer1,C.Footer2,C.Footer3,C.Footer4, " +
                     " 1 as OutletRegisterId,C.MainWindowSettings,C.HeaderMarqueeText,C.DeliveryList,C.DiscountList,C.Powerby,C.TaxInclusive, C.IsItemOverright" +
                     " from[User] U INNER JOIN" +
                     " Outlet O On O.Id = U.OutletId" +
                     " INNER JOIN Employee E On E.Id = U.EmployeeId" +
                     " CROSS JOIN Client C" +
                     " WHERE " +
                     " USERNAME = '" + userName + "' AND [PASSWORD] = '" + Password + "' AND U.IsActive = 1 AND O.IsActive = 1" +
                     " and U.Outletid = C.CurrentOutletId;";

                loginModel = connection.Query<LoginModel>(query).ToList();

            }

            return loginModel;
        }

        public int ValidateDiscountPassword(string password)
        {
            int validId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Select Id From [User] Where Password='" + password + "' And RoleTypeId In (1,2) And IsActive=1";
                validId = connection.Query<int>(query).FirstOrDefault();
            }
            return validId;
        }

        public void UpdateLoginLogout(string loginLogout)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = string.Empty;
                if (loginLogout.ToLower() == "login")
                {
                    query = @"Update [user] set lastlogin = Getdate() where id =" + LoginDetail.UserId.ToString() + ";";
                }
                else
                {
                    query = @"Update [user] set lastlogout  = Getdate() where id =" + LoginDetail.UserId.ToString() + ";";
                }
                connection.Query<bool>(query).FirstOrDefault();
            }
        }

        public int LoginHistory(int Loginlogout)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = string.Empty;
                query = @"INSERT LoginHistory( UserId, LoginLogout, Logindate,OutletId, SystemCounter ) " +
                        " Values( " +
                        LoginDetail.UserId + "," + Loginlogout + ",GetDate()," + LoginDetail.OutletId + ",'POS');" +
                          " SELECT CAST(SCOPE_IDENTITY() as int)";

                insertedId = connection.Query<int>(query).Single();
            }
            return insertedId;
        }


    }
}
