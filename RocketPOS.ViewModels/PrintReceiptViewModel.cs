using Dapper; 
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class PrintReceiptViewModel
    {
        AppSettings appSettings = new AppSettings();
        List<PrintReceiptModel> printReceiptModel = new List<PrintReceiptModel>();
        List<PrintReceiptItemModel> printReceiptItemModel = new List<PrintReceiptItemModel>();

        public List<PrintReceiptModel> GetPrintReceiptByBillId(int billId)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                var query = "SELECT b.CustomerOrderId,b.Id As BillId,CO.SalesInvoiceNumber,ISNULL(CO.RewardPoints,0) as RewardAmount,B.BillDateTime,O.OutletName,U.Username,C.CustomerName,B.GrossAmount,B.TaxAmount,B.VatableAmount,CO.NonVatableAmount, B.Discount,B.ServiceCharge,B.TotalAmount,PM.PaymentMethodName,BD.BillAmount FROM Bill B " +
                            "  INNER JOIN CustomerOrder CO ON B.CustomerOrderId = CO.Id " +
                            "  INNER JOIN BillDetail BD ON B.Id = BD.BillId " +
                            "  INNER JOIN Outlet O ON O.Id = B.OutletId " +
                            "  INNER JOIN[User] U ON U.ID = B.UserIdInserted " +
                            "  INNER JOIN Customer C ON C.Id = b.CustomerId " +
                            "  INNER JOIN PaymentMethod PM ON PM.Id = BD.PaymentMethodId " +
                            "   WHERE b.CustomerOrderId =  " + billId.ToString();

                printReceiptModel = connection.Query<PrintReceiptModel>(query).ToList();

            }

            return printReceiptModel;
        }

        public List<PrintReceiptItemModel> GetPrintReceiptItemByBillId(int billId)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();

                var query = " SELECT FM.FoodMenuName,COI.FoodMenuQty,COI.FoodMenuRate,COI.Price, " +
                            " (select case when foodmenutaxtype = 1 then 'V' when foodmenutaxtype = 2 then 'E' when foodmenutaxtype = 3 then 'Z' ELSE '' end) AS FOODVAT " + 
                            " FROM CustomerOrderItem  COI " +
                            " INNER JOIN FoodMenu FM ON FM.ID = COI.FoodMenuId " +
                            " WHERE CustomerOrderId = " + billId.ToString();

                printReceiptItemModel = connection.Query<PrintReceiptItemModel>(query).ToList();
            }

            return printReceiptItemModel;
        }

        public List<PrintKOTItemModel> GetPrintKOTItemByBillId(int billId)
        {
            List<PrintKOTItemModel> printKOTItemModels = new List<PrintKOTItemModel>();

            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();

                var query = "SELECT CO.CustomerOrderNo +' / '+ COK.KOTNumber as KOTNumber, T.TableName, " +
                            " Case When CO.OrderType = 1 then 'DineIN' when OrderType = 2 then 'TakeAway' else 'Delivery' end as OrderType, " +
                            " COK.KOTDateTime,  U.Username, CustomerOrderId,FM.FoodMenuName,COKI.FoodMenuQty" +
                            " From CustomerOrder CO" +
                            " inner join CustomerOrderKOT COK on CO.Id = COK.CustomerOrderId " +
                            " Inner join CustomerOrderKOTItem COKI on COK.Id = COKI.CustomerOrderKOTId " +
                            " INNER JOIN FoodMenu FM ON FM.ID = COKI.FoodMenuId" +
                            " INNER JOIN[User] U ON U.ID = COK.UserIdInserted" +
                            " left join Tables T On T.Id = CO.TableId" +
                            " WHERE COK.ID = " + billId.ToString();

                printKOTItemModels = connection.Query<PrintKOTItemModel>(query).ToList();
            }

            return printKOTItemModels;
        }
        public List<ReportOffsetModel> GetReportOffsetByReportName(string reportName)
        {
            List<ReportOffsetModel> reportOffsetModels = new List<ReportOffsetModel>();

            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();

                var query = " Select Id,ReportName, ReportColumn, ColumnOffset from ReportOffset " +
                            " WHERE ReportName = '" + reportName.ToString() + "'";

                reportOffsetModels = connection.Query<ReportOffsetModel>(query).ToList();
            }

            return reportOffsetModels;

        }

        public bool UpdateReportOffsetById(ReportOffsetModel reportOffsetModel)
        {
            bool result=false;
            if (!string.IsNullOrEmpty(reportOffsetModel.Id.ToString()))
            {
                using (var connection = new SqlConnection(appSettings.GetConnectionString()))
                {
                    var query = "Update [ReportOffset] set Columnoffset=" + reportOffsetModel.ColumnOffset + " Where Id=" + reportOffsetModel.Id;
                    result = connection.Query<bool>(query).FirstOrDefault();
                }
            }
            return result;
        }
        public List<PrintReceiptA4Model> GetPrintReceiptA4ByBillId(int billId)
        {
            List<PrintReceiptA4Model> printReceiptA4Models = new List<PrintReceiptA4Model>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                var query = "SELECT CL.ClientName,CL.Address1 as ClientAddress1,CL.Address2 as ClientAddress2,CL.Email as ClientEmail,CL.Phone as ClientPhone, " + 
                            " CustomerName,CustomerEmail,CustomerAddress1,CustomerAddress2,CustomerPhone, C.Id as CustomerOrderId,CO.SalesInvoiceNumber," +
                            " ISNULL(CO.RewardPoints,0) as RewardAmount,CO.Orderdate as BillDateTime,O.OutletName,U.Username,CO.GrossAmount, " +
                            " CO.TaxAmount,CO.VatableAmount," +
                            " CO.NonVatableAmount, CO.DiscountAmount,CO.TotalPayable as TotalAmount " +
                            " FROM CustomerOrder CO " +
                            " INNER JOIN Outlet O ON O.Id = CO.OutletId " +
                            " INNER JOIN[User] U ON U.ID = CO.UserIdInserted " +
                            " INNER JOIN Customer C ON C.Id = CO.CustomerId " +
                            " cross join Client CL " +
                            "   WHERE CO.Id =  " + billId.ToString();

                printReceiptA4Models = connection.Query<PrintReceiptA4Model>(query).ToList();

            }

            return printReceiptA4Models;
        }

        public List<PrintReceiptItemModel> GetPrintReceiptItemA4ByBillId(int billId)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();

                var query = " SELECT FM.FoodMenuName,COI.FoodMenuQty,COI.FoodMenuRate,COI.Price,  U.UnitShortname as Unitname, " +
                            " (select case when foodmenutaxtype = 1 then 'V' when foodmenutaxtype = 2 then 'E' when foodmenutaxtype = 3 then 'Z' ELSE '' end) AS FOODVAT " +
                            " FROM CustomerOrderItem  COI " +
                            " INNER JOIN FoodMenu FM ON FM.ID = COI.FoodMenuId " +
                            "  left join Units U on U.Id = FM.Unitsid " +
                            " WHERE CustomerOrderId = " + billId.ToString();

                printReceiptItemModel = connection.Query<PrintReceiptItemModel>(query).ToList();
            }

            return printReceiptItemModel;
        }
    }
}
