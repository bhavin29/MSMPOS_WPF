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
                string query = " select convert(varchar(10), BD.BillDate,103) as BillDate,round(sum(BillAmount), 0) as Sales,TallyLedgerName,TallyLedgerNamePark,TallyBillPostfix, " +
                                " sum(round(CO.VatableAmount, 0)) as CashSales, Sum(round(CO.NonVatableAmount, 0)) as ExemptedSales,sum(round(CO.TaxAmount, 0)) as OutputVAT " +
                                " from Billdetail BD " +
                                " INNER join PaymentMethod PM on PM.Id = BD.PaymentMethodId Inner join Bill B on B.ID = BD.BillId inner join CustomerOrder CO on CO.ID = B.CustomerOrderId " +
                                " Where Convert(Date, BD.BillDate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103)  And B.BillStatus = 4 AND B.OutletId = " + LoginDetail.OutletId +
                                " group by convert(varchar(10), BD.BillDate, 103),TallyLedgerName,TallyLedgerNamePark,TallyBillPostfix ";

                tallySalesVouchers = db.Query<TallySalesVoucherModel>(query).ToList();
             }

            return tallySalesVouchers;
        }
    }
}
