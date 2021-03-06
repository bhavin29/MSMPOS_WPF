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
        public List<DetailSaleSummaryModel> GetDetailSaleSummaryReport(string fromDate, string toDate)
        {
            List<DetailSaleSummaryModel> detailSaleSummaryModel = new List<DetailSaleSummaryModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select CONVERT(VARCHAR(10),CO.OrderDate,103) As OrderDate,FM.FoodMenuName,sum(COI.FoodMenuQty) AS TotalQty,sum(COI.GrossAmount) AS TotalGrossAmount,sum(COI.Discount)  AS TotalDiscountAmount,sum(COI.Price)  AS TotalNetAmount,sum(T.TaxPercentage) AS TotalTaxPercentage,sum(B.GrossAmount)  AS TotalBillGrossAmount, " +
                        " sum(Case When PM.PaymentMethodName ='CASH' Then BD.BillAmount End) AS CashPayment, " +
                        " sum(Case When PM.PaymentMethodName = 'CREDIT CARD' Or PM.PaymentMethodName = 'DEBIT CARD' Then BD.BillAmount End) AS CardPayment  from Bill B " +
                        " Inner Join CustomerOrder CO On Co.Id=B.CustomerOrderId " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " inner join BillDetail BD ON BD.BillId=B.Id " +
                        " Inner join PaymentMethod PM On PM.Id=BD.PaymentMethodId " +
                        " Left Join Tax T On T.Id = COI.foodmenuvattaxid " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CONVERT(VARCHAR(10),CO.OrderDate,103),FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " order by CONVERT(VARCHAR(10),CO.OrderDate,103)  ";
                detailSaleSummaryModel = db.Query<DetailSaleSummaryModel>(Query).ToList();
                return detailSaleSummaryModel;
            }
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
        public List<MasterSalesReportModel> GetMasterSaleReport(string fromDate, string toDate)
        {
            List<MasterSalesReportModel> masterSalesReportModel = new List<MasterSalesReportModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select CONVERT(VARCHAR(10),CO.OrderDate,103) As OrderDate,CONVERT(VARCHAR(8),CO.OrderDate,108)  As OrderTime,CO.SalesInvoiceNumber," +
                        " FM.FoodMenuName ,COI.FoodMenuRate,FoodMenuQty,COI.Price,COI.Discount,COI.FoodMenuVat As Tax,COI.GrossAmount,FMC.FoodMenuCategoryName " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103)";
                masterSalesReportModel = db.Query<MasterSalesReportModel>(Query).ToList();
                return masterSalesReportModel;
            }
        }
        public List<SalesByCategoryProductModel> GetSaleByCategorySectionReport(string fromDate, string toDate,string reportName)
        {
            List<SalesByCategoryProductModel> salesByCategoryProductModel = new List<SalesByCategoryProductModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                if (reportName == "SalesByCategoryProductQtyDesc")
                {
                    Query = " select FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +  // Returns Sub Totals For Group By
                        " Order By Sum(COI.FoodMenuQty) desc ";
                }

                if (reportName == "SalesByCategoryProductQtyAsc")
                {
                    Query = " select FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.FoodMenuQty) asc ";
                }

                if (reportName == "SalesByCategoryProductAmountDesc")
                {
                    Query = " select FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.Price) desc ";
                }

                if (reportName == "SalesBySectionCategoryProductAmountAsc")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.Price) asc ";
                }

                if (reportName == "SalesBySectionCategoryProductAmountDesc")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.Price) desc ";
                }

                if (reportName == "SalesBySectionCategoryProductQtyAsc")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.FoodMenuQty) asc ";
                }

                if (reportName == "SalesBySectionCategoryProductQtyDesc")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FMC.FoodMenuCategoryName,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.FoodMenuQty) desc ";
                }

                if (reportName == "SalesBySectionCategory")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FMC.FoodMenuCategoryName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FMC.FoodMenuCategoryName " +
                        " WITH ROLLUP  " +
                        " Order By CO.OrderType,FMC.FoodMenuCategoryName asc ";
                }

                if (reportName == "SalesBySectionProductAmountDesc")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.Price) desc ";
                }

                if (reportName == "SalesBySectionProductQtyDesc")
                {
                    Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName, " +
                        " FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
                        " from CustomerOrder CO " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId " +
                        " Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,FM.FoodMenuName " +
                        " WITH ROLLUP  " +
                        " Order By Sum(COI.FoodMenuQty) desc ";
                }

                salesByCategoryProductModel = db.Query<SalesByCategoryProductModel>(Query).ToList();
                return salesByCategoryProductModel;
            }
        }

        public List<TableStatisticsModel> GetTableStatisticsReport(string fromDate, string toDate)
        {
            List<TableStatisticsModel> tableStatisticsModel = new List<TableStatisticsModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select  T.TableName,T.PersonCapacity AS ActualCapacity, Sum(T.PersonCapacity) As ExpectedOccupancy,Sum(CO.AllocatedPerson) As Occupancy,((100 * Sum(CO.AllocatedPerson))/Sum(T.PersonCapacity)) As OccupancyPercentage " +
                        " from CustomerOrder CO   " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId   " +
                        " Inner Join [Tables] T On T.Id=CO.TableId " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) "+
                        " group by  T.TableName,T.PersonCapacity "+
                        " Order By T.TableName asc ";
                tableStatisticsModel = db.Query<TableStatisticsModel>(Query).ToList();
                return tableStatisticsModel;
            }
        }

        public List<SalesSummaryModel> GetSalesSummaryByFoodCategoryReport(string fromDate, string toDate)
        {
            List<SalesSummaryModel> salesSummaryModel = new List<SalesSummaryModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select FMC.FoodMenuCategoryName, Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.VatableAmount) As NetSalesAmount,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount, " +
                        " SUM(COI.GrossAmount) * 100.0 / SUM(SUM(COI.GrossAmount)) OVER () AS ValuePercentage   " +
                        " from CustomerOrder CO   " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId   Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId   " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by FMC.FoodMenuCategoryName " +
                        " Order By FMC.FoodMenuCategoryName asc ";
                salesSummaryModel = db.Query<SalesSummaryModel>(Query).ToList();
                return salesSummaryModel;
            }
        }

        public List<SalesSummaryByFoodCategoryFoodMenuModel> GetSalesSummaryByFoodCategoryFoodMenuReport(string fromDate, string toDate)
        {
            List<SalesSummaryByFoodCategoryFoodMenuModel> salesSummaryByFoodCategoryFoodMenuModel = new List<SalesSummaryByFoodCategoryFoodMenuModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select FMC.FoodMenuCategoryName,Fm.FoodMenuName, Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.VatableAmount) As NetSalesAmount,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount, " +
                        " SUM(COI.GrossAmount) * 100.0 / SUM(SUM(COI.GrossAmount)) OVER () AS ValuePercentage   " +
                        " from CustomerOrder CO   " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId   Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId   " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by FMC.FoodMenuCategoryName,Fm.FoodMenuName " +
                        " Order By FMC.FoodMenuCategoryName,Fm.FoodMenuName asc ";
                salesSummaryByFoodCategoryFoodMenuModel = db.Query<SalesSummaryByFoodCategoryFoodMenuModel>(Query).ToList();
                return salesSummaryByFoodCategoryFoodMenuModel;
            }
        }

        public List<SalesSummaryBySectionModel> GetSalesSummaryBySectionReport(string fromDate, string toDate)
        {
            List<SalesSummaryBySectionModel> salesSummaryBySectionModel = new List<SalesSummaryBySectionModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select case when CO.OrderType = 1 then 'DineIN' When CO.OrderType = 2 then 'TakeAway' When CO.OrderType = 2 then 'Delivery' else 'ALL' End As SectionName,  " +
                        " convert(varchar, CO.Orderdate, 103) as Orderdate , Count(CO.SalesInvoiceNumber) As TotalInvoice,Sum(COI.VatableAmount) As NetSalesAmount,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount   " +
                        " from CustomerOrder CO   " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId   Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId   " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by CO.OrderType,convert(varchar, CO.Orderdate, 103) " +
                        " Order By CO.OrderType,convert(varchar, CO.Orderdate, 103) ";
                salesSummaryBySectionModel = db.Query<SalesSummaryBySectionModel>(Query).ToList();
                return salesSummaryBySectionModel;
            }
        }


        public List<CustomerRewardModel> GetCustomerRewardReport(string fromDate, string toDate,string customerPhone ,string customerName)
        {
            List<CustomerRewardModel> customerRewardModel = new List<CustomerRewardModel>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select CustomerName,CustomerPhone,Datetime,case when Credit = 0.00 then null else Credit end As Credit,case when Debit = 0.00 then null else Debit end As Debit, Balance from CustomerRedeem CR inner join customer C on C.Id = CR.CustomerId    " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) ";
                if (!string.IsNullOrEmpty(customerPhone))
                {
                    Query += " And CustomerPhone like '%" + customerPhone + "%' ";
                }

                if (!string.IsNullOrEmpty(customerName))
                {
                    Query += " And CustomerName like '%" + customerName + "%' ";
                }
                Query += " Order By Datetime desc ";
                customerRewardModel = db.Query<CustomerRewardModel>(Query).ToList();
                return customerRewardModel;
            }
        }

        public List<SalesSummaryByWeek> GetSalesSummaryByWeekReport(string fromDate, string toDate)
        {
            List<SalesSummaryByWeek> salesSummaryByWeek = new List<SalesSummaryByWeek>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                string Query = string.Empty;

                Query = " select DATEADD(DAY, -DATEDIFF(DAY, 0, Convert(Date, CO.Orderdate, 103)) % 7, Convert(Date, CO.Orderdate, 103)) AS [WeekStartDate], Count(CO.SalesInvoiceNumber) As TotalInvoice,Sum(COI.VatableAmount) As NetSalesAmount,Sum(COI.Discount)  As TotalDiscount,Sum(COI.FoodMenuVat) As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount  " +
                        " from CustomerOrder CO   " +
                        " Inner Join CustomerOrderItem COI ON CO.Id =COI.CustomerOrderId " +
                        " Inner Join FoodMenu FM On FM.Id = COI.FoodMenuId   Inner Join FoodMenuCategory FMC ON FMC.Id=FM.FoodCategoryId   " +
                        " Where CO.OutletId = " + LoginDetail.OutletId + " And  Convert(Date, CO.Orderdate, 103)  between Convert(Date, '" + fromDate + "', 103)  and Convert(Date, '" + toDate + "' , 103) " +
                        " group by DATEADD(DAY, -DATEDIFF(DAY, 0, Convert(Date, CO.Orderdate, 103)) % 7, Convert(Date, CO.Orderdate, 103)) " +
                        " Order By DATEADD(DAY, -DATEDIFF(DAY, 0, Convert(Date, CO.Orderdate, 103)) % 7, Convert(Date, CO.Orderdate, 103)) ";
                salesSummaryByWeek = db.Query<SalesSummaryByWeek>(Query).ToList();
                return salesSummaryByWeek;
            }
        }
    }
}
