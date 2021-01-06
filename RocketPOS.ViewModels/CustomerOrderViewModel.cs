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
        AppSettings appSettings = new AppSettings();
        public int InsertCustomerOrder(CustomerOrderModel customerOrderModel, DataTable customerOrderItem)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@CustomerOrderItemData", customerOrderItem.AsTableValuedParameter(StoredProcedure.TABLE_TYPE_CUST_ORDER_ITEMDATA));
                    dynamicParameters.Add("@Id", customerOrderModel.Id);
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
                    dynamicParameters.Add("@KotStatus", customerOrderModel.KotStatus);
                    dynamicParameters.Add("@OrderPrefix", LoginDetail.OrderPrefix);

                insertedId = connection.Query<int>
                        (StoredProcedure.PX_INSERT_CUSTOMER_ORDER, dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: 0).FirstOrDefault();
                    return insertedId;
            }
        }

        public List<CustomerOrderModel> GetCustomerOrderList(int orderStatus, int orderType, string searchKey)
        {
            List<CustomerOrderModel> customerOrderList = new List<CustomerOrderModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = string.Empty;
                query = "SELECT CO.Id,CO.CustomerOrderNo,CustomerId,C.CustomerName,CO.CustomerId, CO.WaiterEmployeeId,E.FirstName+' '+E.LastName AS WaiterName,OrderType,TableId " +
                                                                    " FROM CustomerOrder CO" +
                                                                    " INNER JOIN Customer C" +
                                                                    " ON CO.CustomerId = C.Id" +
                                                                    " INNER JOIN Employee E" +
                                                                    " ON CO.WaiterEmployeeId = E.Id" +
                                                                    " Where CO.OrderStatus= " + orderStatus;

                if (orderType != (int)EnumUtility.OrderType.All)
                {
                    query += " And CO.OrderType="+ orderType; 
                }

                if (!string.IsNullOrEmpty(searchKey))
                {
                    query += " And CO.Id Like '%" + searchKey + "%'";
                }

                customerOrderList = db.Query<CustomerOrderModel>(query).ToList();

                return customerOrderList;
            }
        }

        public CustomerOrderModel GetCustomerOrderByOrderId(int id)
        {
            CustomerOrderModel customerOrderModel;
            List<OrderDetailModel> orderDetailModel = new List<OrderDetailModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT CO.Id,CO.CustomerOrderNo,CO.OutletId,CO.SalesInvoiceNumber,CO.CustomerId,CO.WaiterEmployeeId,CO.OrderType,CO.TableId,CO.GrossAmount,CO.DiscountPercentage,CO.DiscountAmount,CO.DeliveryCharges,CO.TaxAmount,CO.TotalPayable,CO.CustomerNote,CO.OrderStatus, " +
                            " COI.Id AS CustomerOrderItemId,COI.FoodMenuId,COI.FoodMenuRate,COI.FoodMenuQty,COI.AddonsId,COI.AddonsQty,COI.VarientId,COI.Discount,COI.Price,FM.FoodCategoryId,FM.FoodMenuName,FM.FoodMenuCode,FM.ColourCode,FM.SmallThumb,FM.SalesPrice,ISNULL(FM.FoodVat,0) AS FoodVat,ISNULL(FM.Foodcess,0) AS Foodcess,FM.Notes,COKOT.KOTStatus,T.TableName " +
                            " FROM dbo.CustomerOrder CO  INNER JOIN dbo.CustomerOrderItem COI  ON CO.Id = COI.CustomerOrderId " +
                            " INNER JOIN dbo.CustomerOrderKOT COKOT  ON CO.Id = COKOT.CustomerOrderId " +
                            " INNER JOIN dbo.FoodMenu FM  ON FM.Id = COI.FoodMenuId LEFT JOIN dbo.[Tables] T On T.Id=CO.TableId WHERE CO.Id = " + id;
                orderDetailModel = connection.Query<OrderDetailModel>(query).ToList();

                customerOrderModel = (from order in orderDetailModel select order).FirstOrDefault();
                customerOrderModel.CustomerOrderItemModels = (from order in orderDetailModel select order).ToList<CustomerOrderItemModel>();

                return customerOrderModel;
            }
        }

        public List<PaymentMethodModel> GetPaymentMethod()
        {
            List<PaymentMethodModel> paymentMethodModels = new List<PaymentMethodModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT Id,PaymentMethodName FROM PaymentMethod Where IsActive=1";
                paymentMethodModels = connection.Query<PaymentMethodModel>(query).ToList();
                return paymentMethodModels;
            }
        }

        public List<CustomerOrderHistoryModel> GetCustomerOrderHistoryList( string fromDate, string toDate)
        {
            List<CustomerOrderHistoryModel> customerOrderHistoryModels = new List<CustomerOrderHistoryModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = string.Empty;
                query = "SELECT CO.Id,CO.SalesInvoiceNumber,C.CustomerName, " +
                           "  CASE WHEN CO.OrderType = 1 THEN 'Dine IN' " +
                                      "  WHEN CO.OrderType = 2 THEN 'Take Away' " +
                                      "  WHEN CO.OrderType = 3 THEN 'Delivery' END as OrderType,  " +
                            " CONVERT(VARCHAR(10),CO.OrderDate,105) AS Orderdate,CO.GrossAmount,CO.DiscountAmount,  " +
                            " CO.DeliveryCharges,CO.TaxAmount,CO.TotalPayable ,  " +
                            " CASE WHEN CO.OrderStatus = 1 THEN 'Pending'  " +
                                      "  WHEN CO.OrderStatus = 2 THEN 'Hold'  " +
                                      "  WHEN CO.OrderStatus = 3 THEN 'Partial Paid'  " +
                                      "  WHEN CO.OrderStatus = 4 THEN 'Paid' " +
                                      "  WHEN CO.OrderStatus = 5 THEN 'Cancelled' " +
                                      "  END as OrderStatus  " +
                            " FROM CustomerOrder CO  " +
                            " INNER JOIN Customer C ON C.ID = CO.CustomerId  " +
                            " WHERE OutletId =" + LoginDetail.OutletId +
                            " AND convert(varchar(10),Orderdate,126) between '" + fromDate + "' AND '" + toDate + "'" +
                            " ORDER BY CO.Orderdate desc;";

                customerOrderHistoryModels = db.Query<CustomerOrderHistoryModel>(query).ToList();

                return customerOrderHistoryModels;
            }
        }

        public int UpdateOrderStatus(string orderId,int orderStatus)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = string.Empty;
                    query = @" Update CustomerOrder Set OrderStatus=@OrderStatus ,UserIdUpdated=@UserId,DateUpdated=@DateUpdated Where Id=@Id;
                               SELECT CAST(@Id as int)";

                insertedId = connection.Query<int>(query, new
                {
                    Id = orderId,
                    OrderStatus = orderStatus,
                    UserId = LoginDetail.UserId,
                    DateUpdated = System.DateTime.Now
                }).Single();
            }
            return insertedId;
        }

    }
}
