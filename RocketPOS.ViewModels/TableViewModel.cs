using System.Collections.Generic;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
namespace RocketPOS.ViewModels
{
    public class TableViewModel
    {
        AppSettings appSettings = new AppSettings();

        public List<TableModel> GetTables(int outletId)
        {
            List<TableModel> tables = new List<TableModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT Id,OutletId,TableName,PersonCapacity,TableIcon,Status FROM dbo.Tables Where OutletId="+ outletId;
                tables = connection.Query<TableModel>(query).ToList();
                return tables;
            }
        }
    }
}
