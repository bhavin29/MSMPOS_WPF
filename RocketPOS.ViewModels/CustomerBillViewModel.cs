using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class CustomerBillViewModel
    {
        AppSettings appSettings = new AppSettings();
        public int InsertBillDetail(CustomerBillModel customerBillModel)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@OutletId", customerBillModel.OutletId);
                dynamicParameters.Add("@CustomerOrderId", customerBillModel.CustomerOrderId);
                dynamicParameters.Add("@CustomerId", customerBillModel.CustomerId);
                dynamicParameters.Add("@GrossAmount", customerBillModel.GrossAmount);
                dynamicParameters.Add("@Discount", customerBillModel.Discount);
                dynamicParameters.Add("@ServiceCharge", customerBillModel.ServiceCharge);
                dynamicParameters.Add("@VatableAmount", customerBillModel.VatableAmount);
                dynamicParameters.Add("@TotalAmount", customerBillModel.TotalAmount);
                dynamicParameters.Add("@BillStatus", customerBillModel.BillStatus);
                dynamicParameters.Add("@OutletRegisterId", customerBillModel.OutletRegisterId);
                dynamicParameters.Add("@UserId", customerBillModel.UserId);
                dynamicParameters.Add("@PaymentMethodId", customerBillModel.PaymentMethodId);
                dynamicParameters.Add("@PaymentNumber", customerBillModel.PaymentNumber);
                dynamicParameters.Add("@ReceiptPrefix", LoginDetail.ReceiptPrefix);
                dynamicParameters.Add("@TaxAmount", customerBillModel.TaxAmount);

                insertedId = connection.Query<int>
                        (StoredProcedure.PX_INSERT_BILL_DETAILS, dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: 0).FirstOrDefault();
                return insertedId;
            }
        }

        public decimal GetCustomerTotalPaidAmount(string orderId)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Select isnull(sum(TotalAmount),0) from Bill where CustomerOrderId=" + orderId;
                return connection.Query<decimal>(query).FirstOrDefault();
            }
        }
    }
}
