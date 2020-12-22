using Dapper;
using Microsoft.SqlServer.Server;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class CustomerOrderViewModel
    {
        AppSettings AppSettings = new AppSettings();
        public int AddCustomerOrder(CustomerOrderModel customerOrderModel, DataTable customerOrderItem)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(AppSettings.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@CustomerOrderItemData", customerOrderItem.AsTableValuedParameter(StoredProcedure.TABLE_TYPE_CUST_ORDER_ITEMDATA));
                    dynamicParameters.Add("@OutletId", customerOrderModel.OutletId);
                    dynamicParameters.Add("@SalesInvoiceNumber", customerOrderModel.SalesInvoiceNumber);
                    dynamicParameters.Add("@CustomerId", customerOrderModel.CustomerId);
                    dynamicParameters.Add("@WaiterEmployeeId", customerOrderModel.WaiterEmployeeId);
                    dynamicParameters.Add("@OrderType", customerOrderModel.OrderType);
                    dynamicParameters.Add("@OrderDate", customerOrderModel.OrderDate);
                    dynamicParameters.Add("@TableId", customerOrderModel.TableId);
                    dynamicParameters.Add("@TockenNumber", customerOrderModel.TockenNumber);
                    dynamicParameters.Add("@GrossAmount", customerOrderModel.GrossAmount);
                    dynamicParameters.Add("@DiscountPercentage", customerOrderModel.DiscountPercentage);
                    dynamicParameters.Add("@DiscountAmount", customerOrderModel.DiscountAmount);
                    dynamicParameters.Add("@DeliveryCharges", customerOrderModel.DeliveryCharges);
                    dynamicParameters.Add("@TaxAmount", customerOrderModel.TaxAmount);
                    dynamicParameters.Add("@TotalPayable", customerOrderModel.TotalPayable);
                    dynamicParameters.Add("@CustomerPaid", customerOrderModel.CustomerPaid);
                    dynamicParameters.Add("@CustomerNote", customerOrderModel.CustomerNote);
                    dynamicParameters.Add("@OrderStatus", customerOrderModel.OrderStatus);
                    dynamicParameters.Add("@AnyReason", customerOrderModel.AnyReason);
                    dynamicParameters.Add("@UserIdInserted", customerOrderModel.UserIdInserted);
                    dynamicParameters.Add("@DateInserted", customerOrderModel.DateInserted);

                    insertedId = connection.Query<int>
                        (StoredProcedure.PX_INSERT_CUSTOMER_ORDER, dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: 0, transaction: transaction).FirstOrDefault();

                    transaction.Commit();
                    return insertedId;
                }
            }
        }
    }
}
