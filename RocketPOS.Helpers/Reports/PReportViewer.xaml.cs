using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using RocketPOS.Helpers.Reports.WPFPrintHelper;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using Microsoft.Win32;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for PReportViewer.xaml
    /// </summary>
    public partial class PReportViewer : Page
    {
        CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
        WPFPrintUtility wPFPrintHelper = new WPFPrintUtility();
        List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();
        DataTable dtData = new DataTable();
        DataTable dtDataResult = new DataTable();
        CommonMethods commonMethods = new CommonMethods();
        string reportTitle = "";
        string reportFooter = "";
        string _reportName = "";
        public PReportViewer()
        {
            InitializeComponent();
            ReportLoad(ReportDetail.ReportName);
        }
        public void ReportLoad(string reportName)
        {

            _reportName = reportName;

            if (reportName == "ModeOfPayment")
            {
                modeofPaymentReportModel = customerOrderViewModel.GetModOfPaymentReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);
                dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);
                dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);
                reportTitle = "Mode Of Payment Report";
            }

            //common call
            DataTable mockDataTable = wPFPrintHelper.CreateMockDataTableForTest();
            wPFPrintHelper.CreateAndVisualizeDataTable(flowDocument, dtDataResult, reportTitle, reportFooter);
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            wPFPrintHelper.Print(flowDocument);
        }

        private void excelButton_Click(object sender, RoutedEventArgs e)
        {
            string path = string.Empty, firstLine = string.Empty;

            if (_reportName == "ModeOfPayment")
            {
                string fileName = "ModeOfPaymentReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = fileName != "" ? fileName : "gpmfca-exportedDocument",
                    DefaultExt = ".xlsx",
                    Filter = "Common Seprated Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    firstLine = LoginDetail.ClientName;

                    dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);

                    //X axis column: PaymentMethodName
                    //Y axis column: BillDate
                    //Z axis column: BillAmount
                    //Null value: "-";
                    //Sum of values: true

                    dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);

                    commonMethods.WriteExcelModeOfPaymentFile(dtDataResult, path, firstLine);
                }

            }
        }
    }
}
