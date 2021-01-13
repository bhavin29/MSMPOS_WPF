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
                //var query = "SELECT Id,OutletId,TableName,PersonCapacity,TableIcon,Status,case when Status=1 then 'Open' When Status=2 then 'Occupied' When Status=3 then 'Clean' Else 'Unknown' End as StatusDescription FROM dbo.Tables Where OutletId=" + outletId + " And IsDeleted=0";
                var query = "SELECT CO.Id As OrderId,TA.Id,TA.OutletId,TA.TableName,TA.PersonCapacity,sum(CO.AllocatedPerson) As AllocatedPerson,TA.TableIcon,TA.Status,case when TA.Status=1 then 'Open' When TA.Status=2 then 'Occupied' When TA.Status=3 then 'Clean' Else 'Unknown' End as StatusDescription " +
                            "FROM dbo.Tables TA LEFT JOIN CustomerOrder CO ON CO.TableId = TA.Id AND TA.Status = 2 AND CO.OrderStatus = 1 " +
                            "Where TA.OutletId ="+ outletId + " And TA.IsDeleted = 0 Group By CO.Id,TA.Id,TA.OutletId,TA.TableName,TA.PersonCapacity,TA.TableIcon,TA.Status ";
                    tables = connection.Query<TableModel>(query).ToList();
                return tables;
            }
        }

        public List<TableModel> GetPendingTables(int outletId)
        {
            List<TableModel> tables = new List<TableModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Select Id,OutletId,TableName,PersonCapacity,Status from dbo.Tables Where Status=1 And OutletId = "+ outletId + " And IsDeleted = 0 ";
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
