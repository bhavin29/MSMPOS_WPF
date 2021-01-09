using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class CommonViewModel
    {
        AppSettings appSettings = new AppSettings();

        public List<WaiterModel> GetWaiters()
        {
            List<WaiterModel> waiters = new List<WaiterModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT Id,FirstName + ' '+ LastName AS FullName FROM Employee Where IsActive=1";
                waiters = connection.Query<WaiterModel>(query).ToList();
                return waiters;
            }
        }
    }
}
