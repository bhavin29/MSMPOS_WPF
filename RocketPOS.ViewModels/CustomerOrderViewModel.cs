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
                dynamicParameters.Add("@AllocatedPerson", customerOrderModel.AllocatedPerson);
                dynamicParameters.Add("@TockenNumber", customerOrderModel.TockenNumber);
                dynamicParameters.Add("@GrossAmount", customerOrderModel.GrossAmount);
                dynamicParameters.Add("@DiscountPercentage", customerOrderModel.DiscountPercentage);
                dynamicParameters.Add("@DiscountAmount", customerOrderModel.DiscountAmount);
                dynamicParameters.Add("@DeliveryCharges", customerOrderModel.DeliveryCharges);
                dynamicParameters.Add("@NonVatableAmount", customerOrderModel.NonVatableAmount);
                dynamicParameters.Add("@VatableAmount", customerOrderModel.VatableAmount);
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
                query = "SELECT CO.Id,CO.CustomerOrderNo,CustomerId," +
                        "  CASE WHEN LEN(CUSTOMERNAME) >10 THEN " +
                        " (SELECT substring(customername, 0, 10) + '...' from customer where id = CO.CustomerId) " +
                        " ELSE " +
                        " (SELECT substring(customername, 0, 10) + RIGHT(SPACE(10 - LEN(customername)), 10 - LEN(customername)) + '   ' from customer where id = CO.CustomerId) " +
                        " END as CustomerName, " +
                        " CO.CustomerId, CO.WaiterEmployeeId,E.FirstName+' '+E.LastName AS WaiterName,OrderType," +
                        "  CASE WHEN CO.OrderType = 1 THEN 'DI' " +
                                      "  WHEN CO.OrderType = 2 THEN 'TA' " +
                                      "  WHEN CO.OrderType = 3 THEN 'DY' END as OrderTypeValue,  " +
                        "TableId,' #' + T.Tablename as Tablename,AllocatedPerson " +
                                                                    " FROM CustomerOrder CO" +
                                                                    " INNER JOIN Customer C ON CO.CustomerId = C.Id" +
                                                                    " LEFT JOIN Tables T ON T.Id = CO.TableId " +
                                                                    " LEFT JOIN Employee E" +
                                                                    " ON CO.WaiterEmployeeId = E.Id" +
                                                                    " Where CO.OrderStatus= " + orderStatus;

                if (orderType != (int)EnumUtility.OrderType.All)
                {
                    query += " And CO.OrderType=" + orderType;
                }

                if (!string.IsNullOrEmpty(searchKey))
                {
                    query += " And CO.CustomerOrderNo Like '%" + searchKey + "%'";
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
                var query = "SELECT CO.Id,CO.CustomerOrderNo,CO.OutletId,CO.SalesInvoiceNumber,CO.CustomerId,CO.WaiterEmployeeId,CO.OrderType,CO.TableId,CO.AllocatedPerson,CO.GrossAmount,CO.DiscountPercentage,CO.DiscountAmount,CO.DeliveryCharges,CO.VatableAmount,CO.NonVatableAmount,CO.TaxAmount,CO.TotalPayable,CO.CustomerNote,CO.OrderStatus, " +
                          " COI.Id AS CustomerOrderItemId,COI.FoodMenuId,COI.FoodMenuRate,COI.FoodMenuQty,COI.AddonsId,COI.AddonsQty,COI.VarientId,COI.Discount,COI.Price,FM.FoodCategoryId,FM.FoodMenuName,FM.FoodMenuCode,FM.ColourCode,FM.SmallThumb,FMR.SalesPrice,ISNULL(FMR.FoodVat,0) AS FoodVat,ISNULL(FMR.Foodcess,0) AS Foodcess,FM.Notes,1 as KOTStatus,T.TableName,ISNULL(Ta.TaxPercentage,0) As TaxPercentage,Case When ISNULL(Ta.TaxPercentage,0)>0 Then 1 Else 0 End AS IsVatable " +
                          " FROM dbo.CustomerOrder CO  INNER JOIN dbo.CustomerOrderItem COI  ON CO.Id = COI.CustomerOrderId " +
                         // " INNER JOIN dbo.CustomerOrderKOT COKOT  ON CO.Id = COKOT.CustomerOrderId " +
                          " INNER JOIN dbo.FoodMenu FM  ON FM.Id = COI.FoodMenuId " +
                          " INNER JOIN  FoodMenuRate FMR ON FM.Id = FMR.FoodMenuId " +
                          " LEFT JOIN dbo.[Tables] T On T.Id=CO.TableId LEFT JOIN Tax Ta On Ta.Id=FMR.FoodVatTaxId WHERE FMR.OutletId = CO.OutletId and CO.Id = " + id;

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

        public List<CustomerOrderHistoryModel> GetCustomerOrderHistoryList(string fromDate, string toDate)
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
                                      "  END as OrderStatus,  " +
                                      " (SELECT Stuff( " +
                                       "  (" +
                                       "      SELECT ', ' + PaymentMethodName " +
                                       "      FROM PaymentMethod Where Id In(Select BD.PaymentMethodId From Bill B " +
                                       "      Left Join BillDetail BD On BD.BillId = B.Id " +
                                       "      Left Join PaymentMethod PM On PM.Id = BD.PaymentMethodId Where B.CustomerOrderId = CO.Id) " +
                                       "     FOR XML PATH('') " +
                                       "  ), 1, 2, '')) AS Payment, " +
                            "(select Sum(TotalPayable) from CustomerOrder where convert(varchar(10), Orderdate, 103) between '"+ fromDate + "' AND '" + toDate +"' ) As InvoiceTotal, " +
                              "(select count(*) from CustomerOrder where convert(varchar(10), Orderdate, 103) between  '" + fromDate + "' AND '" + toDate + "' ) As InvoiceCount " +
                            " FROM CustomerOrder CO  " +
                            " INNER JOIN Customer C ON C.ID = CO.CustomerId  " +
                            " WHERE OutletId =" + LoginDetail.OutletId +
                            " AND convert(varchar(10),CO.Orderdate,103) between '" + fromDate + "' AND '" + toDate + "'" +
                            " ORDER BY CO.Orderdate desc;";

                customerOrderHistoryModels = db.Query<CustomerOrderHistoryModel>(query).ToList();

                return customerOrderHistoryModels;
            }
        }

        public int CancelOrder(string orderId)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@orderId", orderId);

                insertedId = connection.Query<int>
                        (StoredProcedure.CANCEL_ORDER, dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: 0).FirstOrDefault();
                return insertedId;
            }
            return insertedId;
        }

        public void UpdateBillDetailPaymentMethod(string orderId,int paymentMethodId)
        {
            int billId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "select Id from Bill where CustomerOrderId="+ orderId;
                billId = connection.Query<int>(query).FirstOrDefault();

                var updaeQuery = "Update BillDetail Set PaymentMethodId="+ paymentMethodId + " Where BillId=" + billId;
                connection.Query<bool>(updaeQuery).FirstOrDefault();
            }
        }

        public CessReportModel GetCessReport(string fromDate,string toDate)
        {
            CessReportModel cessReport = new CessReportModel();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string cessSummaryQuery = string.Empty;
                string cessDetailQuery = string.Empty;

                cessSummaryQuery = "SELECT convert(varchar(10), B.BillDateTime,103) AS BillDate,SUM(isnull(B.VatableAmount,0.00)+isnull(CO.NonVatableAmount,0.00)) AS NetSales, SUM(isnull(B.VatableAmount,0.00)) AS Vatable,SUM(isnull(CO.NonVatableAmount,0.00)) AS NonVatable, SUM(isnull(B.TaxAmount,0.00)) AS TotalTax, SUM(ISNULL(B.TotalAmount,0.00)) AS GrandTotal " +
                                   ", convert(numeric(18,2),round(((SUM(isnull(B.VatableAmount,0)+isnull(CO.NonVatableAmount,0)))*2)/100,2)) As CateringLevy " +
                                   "FROM BILL B " +
                                  "INNER JOIN CustomerOrder CO ON CO.ID = B.CustomerOrderId " +
                                  "Where convert(varchar(10), B.BillDateTime,103) Between '" + fromDate + "' And '" + toDate + "' And B.BillStatus = 4 AND CO.OutletId = "+LoginDetail.OutletId +
                                  "GROUP BY convert(varchar(10), B.BillDateTime, 103) " +
                                  "ORDER BY BillDate";

                cessDetailQuery =  " SELECT convert(varchar(10), B.BillDateTime, 103) AS BillDate, CO.SalesInvoiceNumber AS InvoiceNumber,(ISNULL(B.VatableAmount, 0.00) + ISNULL(CO.NonVatableAmount, 0.00)) AS NetSales, ISNULL(B.VatableAmount, 0.00) AS Vatable, ISNULL(CO.NonVatableAmount, 0.00) AS NonVatable, ISNULL(B.TaxAmount, 0.00) AS TotalTax, ISNULL(B.TotalAmount, 0.00) AS GrandTotal " +
                                   ", convert(numeric(18, 2), round(((isnull(B.VatableAmount, 0.00) + isnull(CO.NonVatableAmount, 0.00)) * 2) / 100, 2)) As  CateringLevy" +
                                   "  FROM BILL B " +
                                   " INNER JOIN CustomerOrder CO ON CO.ID = B.CustomerOrderId " +
                                   " Where convert(varchar(10), B.BillDateTime,103) Between '" + fromDate + "' And '" + toDate + "' And B.BillStatus = 4 AND CO.OutletId = " + LoginDetail.OutletId +
                                   " ORDER BY BillDate";


                cessReport.CessSummaryList = db.Query<CessSummaryModel>(cessSummaryQuery).ToList();
                cessReport.CessDetailList = db.Query<CessDetailModel>(cessDetailQuery).ToList();

                return cessReport;
            }
        }

        public List<ModeofPaymentReportModel> GetModOfPaymentReport(string fromDate, string toDate)
        {
            List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = "(select convert(varchar(10), BD.BillDate,103) as BillDate,PaymentMethodName,sum(BillAmount) As BillAmount from Bill B " +
                                    " INNER JOIN BillDetail BD ON B.ID = BD.BillId " +
                                    " Inner join PaymentMethod PM ON BD.PaymentMethodId = PM.ID " +
                                    " Where B.IsDeleted = 0 AND " +
                                    " convert(varchar(10), BD.BillDate, 103) between '" + fromDate + "' And '" + toDate + "' And B.BillStatus = 4 AND B.OutletId = " + LoginDetail.OutletId +
                                    " Group by convert(varchar(10), BD.BillDate,103),PaymentMethodName )" +
                                    "  union all" +
                                    " (SELECT convert(varchar(10), B.BillDateTime,103) as BillDate, ' SALES' AS PaymentMethodName, SUM(TotalAmount) as Sales from Bill B " +
                                    " Where B.IsDeleted = 0  and convert(varchar(10), B.BillDateTime, 103) between '" + fromDate + "' And '" + toDate + "' And B.BillStatus = 4 AND B.OutletId = " + LoginDetail.OutletId +
                                    " group by convert(varchar(10), B.BillDateTime, 103))" +
                                    " Order by convert(varchar(10), BD.BillDate, 103)";

                modeofPaymentReportModel = db.Query<ModeofPaymentReportModel>(Query).ToList();

                return modeofPaymentReportModel;
            }
        }
    }
}
