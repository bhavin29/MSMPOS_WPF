using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class SettingsViewModel
    {
        AppSettings appSettings = new AppSettings();
        public List<SyncErrorModel> SyncData()
        {
            List<SyncErrorModel> syncErrorModel = new List<SyncErrorModel>();

            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                  var query = "EXEC [SyncData] ";
                syncErrorModel = connection.Query<SyncErrorModel>(query, commandTimeout:300).ToList();
           }
            return syncErrorModel;
        }
    }
}
