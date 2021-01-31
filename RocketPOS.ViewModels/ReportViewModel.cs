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
        List<ProductWiseSalesReportModel> productWiseSalesReportModels = new List<ProductWiseSalesReportModel>();
        public List<DetailedDailyReportModel> GetDetailedDailyByDate(string Fromdate, string Todate)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                  var query = "Exec rptTodaySummary " + LoginDetail.OutletId + ",'"+ Fromdate +"','" + Todate + "'; ";

                detailedDailyReportModels = connection.Query<DetailedDailyReportModel>(query).ToList();
            }

            return detailedDailyReportModels;
        }
        public List<ProductWiseSalesReportModel> GetProductWiseSales(string Fromdate, string Todate, string ReportType)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Exec [rptProductWiseSumary] " + LoginDetail.OutletId + ",'" + Fromdate + "','" + Todate + "','" + ReportType + "'; ";

                productWiseSalesReportModels = connection.Query<ProductWiseSalesReportModel>(query).ToList();
            }

            return productWiseSalesReportModels;
        }
    }
}
