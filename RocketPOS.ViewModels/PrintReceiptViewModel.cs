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
                var query = "SELECT b.CustomerOrderId,b.Id As BillId,CO.SalesInvoiceNumber,B.BillDateTime,O.OutletName,U.Username,C.CustomerName,B.GrossAmount,B.TaxAmount,B.VatableAmount,(B.GrossAmount - (B.VatableAmount + B.TaxAmount))  as NonVATAmount, B.Discount,B.ServiceCharge,B.TotalAmount,PM.PaymentMethodName,BD.BillAmount FROM Bill B " +
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

        public List<ReportOffsetModel> GetReportOffsetByReportName(string reportName)
        {
            List<ReportOffsetModel> reportOffsetModels = new List<ReportOffsetModel>();

            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();

                var query = " Select ReportName, ReportColumn, ColumnOffset from ReportOffset " +
                            " WHERE ReportName = '" + reportName.ToString() + "'";

                reportOffsetModels = connection.Query<ReportOffsetModel>(query).ToList();
            }

            return reportOffsetModels;

        }
    }
}
