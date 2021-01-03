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
                var query = "SELECT Id,OutletId,TableName,PersonCapacity,TableIcon,Status,case when Status=1 then 'Open' When Status=2 then 'Occupied' When Status=3 then 'Clean' Else 'Unknown' End as StatusDescription FROM dbo.Tables Where OutletId=" + outletId + " And IsDeleted=0";
                tables = connection.Query<TableModel>(query).ToList();
                return tables;
            }
        }

        public void UpdateTableStatus(string tableId,int tableStatus)
        {
            if (!string.IsNullOrEmpty(tableId))
            {
                using (var connection = new SqlConnection(appSettings.GetConnectionString()))
                {
                    var query = "Update [Tables] set Status=" + tableStatus + " Where Id=" + tableId;
                    connection.Query<bool>(query).FirstOrDefault();
                } 
            }
        }
    }
}
