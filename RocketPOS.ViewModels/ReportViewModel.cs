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
                        " FM.FoodMenuName ,COI.FoodMenuRate,FoodMenuQty,COI.Price,COI.Discount,0.00 As Tax,COI.GrossAmount,FMC.FoodMenuCategoryName " +
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
                    Query = " select FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                    Query = " select FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                    Query = " select FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FMC.FoodMenuCategoryName,FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FMC.FoodMenuCategoryName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
                        " FM.FoodMenuName,Sum(COI.FoodMenuRate) As TotalUnitPrice,Sum(COI.FoodMenuQty) As TotalQty,Sum(COI.Price) As TotalPrice,Sum(COI.Discount)  As TotalDiscount,0.00 As TotalTax,Sum(COI.GrossAmount) As TotalGrossAmount " +
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
    }
}
