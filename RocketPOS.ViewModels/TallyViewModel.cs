using RocketPOS.Model;
using RocketPOS.Core;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;

namespace RocketPOS.ViewModels
{
    public class TallyViewModel
    {
        AppSettings appSettings = new AppSettings();

        public List<TallySetupModel> GetTallySetup()
        {
            List<TallySetupModel> tallySetupModel = new List<TallySetupModel>();

            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                tallySetupModel = db.Query<TallySetupModel>("select KeyName,LedgerName from tallysetup").ToList();

            }

            return tallySetupModel;
        }

        public List<TallySalesVoucherModel> GetSalesVoucherData(string fromDate, string toDate)
        {
            List<TallySalesVoucherModel> tallySalesVouchers = new List<TallySalesVoucherModel>();

            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = "select convert(varchar(10), B.BillDateTime,103) as BillDate,round(sum(TotalAmount),0) as Cash,round(sum(VatableAmount),0) as CashSales, round((sum(TotalAmount)-(sum(VatableAmount)+sum(TaxAmount))),0) AS ExemptedSales,round(sum(TaxAmount),0) AS OutputVAT " +
                               " from bill b " +
                               " Where convert(varchar(10), B.BillDateTime, 103) between '" + fromDate + "' And '" + toDate + "' And B.BillStatus = 4 AND B.OutletId = " + LoginDetail.OutletId +
                               " group by convert(varchar(10), B.BillDateTime,103) ";


                tallySalesVouchers = db.Query<TallySalesVoucherModel>(query).ToList();
             }

            return tallySalesVouchers;
        }
    }
}
