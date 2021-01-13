using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using RocketPOS.Core.Constants;
using System.Data;
using System.Linq;

namespace RocketPOS.ViewModels
{
    public class ReportViewModel
    {
        AppSettings appSettings = new AppSettings();

        List<DetailedDailyReportModel> detailedDailyReportModels = new List<DetailedDailyReportModel>();
        public List<DetailedDailyReportModel> GetDetailedDailyByDate(string Fromdate, string Todate)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                  var query = "Exec rptTodaySummary " + LoginDetail.OutletId + ",'"+ Fromdate +"','" + Todate + "'; ";

                detailedDailyReportModels = connection.Query<DetailedDailyReportModel>(query).ToList();
            }

            return detailedDailyReportModels;
        }
    }
}
