using Dapper;
using Microsoft.SqlServer.Server;
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
        public int AddCustomerOrder(CustomerOrderModel customerOrderModel, List<CustomerOrderItemModel> customerOrderItemModels)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]))
            {
                connection.Open();
                DataTable customerOrderItem = new DataTable();
                customerOrderItem.Columns.Add("CustomerOrderId", typeof(Int64));
                customerOrderItem.Columns.Add("FoodMenuId", typeof(Int32));
                customerOrderItem.Columns.Add("FoodMenuRate", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuQty", typeof(decimal));
                customerOrderItem.Columns.Add("AddonsId", typeof(Int32));
                customerOrderItem.Columns.Add("AddonsQty", typeof(decimal));
                customerOrderItem.Columns.Add("VarientId", typeof(Int32));
                customerOrderItem.Columns.Add("Discount", typeof(decimal));
                customerOrderItem.Columns.Add("Price", typeof(decimal));

                //Begin the transaction
                using (var transaction = connection.BeginTransaction())
                {
                    for (int lineNo = 0; lineNo < customerOrderItemModels.Count; lineNo++)
                    {
                        customerOrderItem.Rows.Add(
                                 customerOrderItemModels[lineNo].CustomerOrderId,
                                 customerOrderItemModels[lineNo].FoodMenuId,
                                 customerOrderItemModels[lineNo].FoodMenuRate,
                                 customerOrderItemModels[lineNo].FoodMenuQty,
                                 customerOrderItemModels[lineNo].AddonsId,
                                 customerOrderItemModels[lineNo].AddonsQty,
                                 customerOrderItemModels[lineNo].VarientId,
                                 customerOrderItemModels[lineNo].Discount,
                                 customerOrderItemModels[lineNo].Price
                            );
                    }

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@CustomerOrderItemData", customerOrderItem.AsTableValuedParameter("CustomerOrderItemData"));
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
                        ("AddCustomerOrder", dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: 0, transaction: transaction).FirstOrDefault();

                    transaction.Commit();
                    return insertedId;
                }
            }
        }
    }
}
