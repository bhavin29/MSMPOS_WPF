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

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : Window
    {
        WPFPrintUtility wPFPrintHelper = new WPFPrintUtility();
        public ReportViewer()
        {
            InitializeComponent();

            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();

            List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();
            modeofPaymentReportModel = customerOrderViewModel.GetModOfPaymentReport("01/01/2021","31/01/2021");
            DataTable dtData = new DataTable();

            CommonMethods commonMethods = new CommonMethods();
            DataTable dtDataResult = new DataTable();

            dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);
            dtDataResult = commonMethods.GetInversedDataTable(dtData    , "PaymentMethodName", "BillDate", "BillAmount", " ", true);


            DataTable mockDataTable = wPFPrintHelper.CreateMockDataTableForTest();
            wPFPrintHelper.CreateAndVisualizeDataTable(flowDocument, dtDataResult, "Mode Of Payment Report", "My Footer");

        }
        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            wPFPrintHelper.Print(flowDocument);
        }

        private void excelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
