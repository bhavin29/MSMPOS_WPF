using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RocketPOS.Helpers.Reports.WPFPrintHelper;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using Microsoft.Win32;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : Window
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

        public ReportViewer()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            ReportLoad(LoginDetail.ReportName);
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
        public void ReportLoad(string reportName)
        {

            _reportName = reportName;

            if (reportName == "ModeOfPayment")
            {
                 modeofPaymentReportModel = customerOrderViewModel.GetModOfPaymentReport(LoginDetail.ReportFromDate, LoginDetail.ReportToDate);
                dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);
                dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);
                reportTitle = "Mode Of Payment Report";
            }

            //common call
            DataTable mockDataTable = wPFPrintHelper.CreateMockDataTableForTest();
            wPFPrintHelper.CreateAndVisualizeDataTable(flowDocument, dtDataResult, reportTitle, reportFooter);
        }

        private void CenterWindowOnScreen()
        {
            try
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double windowWidth = this.Width;
                double windowHeight = this.Height;
                this.Left = (screenWidth / 2) - (windowWidth / 2);
                this.Top = ((screenHeight / 2) - (windowHeight / 2));
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ReportList reportList = new ReportList();
            //  reportList.Owner = Application.Current.MainWindow;
            reportList.ShowDialog();
        }
    }
}
