using Microsoft.Win32;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Views;
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

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for ReportList.xaml
    /// </summary>
    public partial class ReportList : Window
    {
        public ReportList()
        {
            InitializeComponent();
            CenterWindowOnScreen();

            DateTime dt = new DateTime();
            dt = DateTime.Now;

            DateTime dtWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            DateTime dtProductWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

            DateTime baseDate = DateTime.Now;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthProductStart = dtProductWithTime.AddDays(1 - dtProductWithTime.Day);

            dpDetailedDailyFromDate.Value = dtWithTime;
            dpDetailedDailyToDate.Value = DateTime.Now;

            dpProductwiseFromDate.Value = thisMonthProductStart;
            dpProductWiseToDate.Value = DateTime.Now;

    
            dpFromDate.SelectedDate = thisMonthStart;
            dpToDate.SelectedDate = today;

            dpFromDatePayment.SelectedDate = thisMonthStart;
            dpToDatePayment.SelectedDate = today;

            dpFromDateSales.SelectedDate = thisMonthStart;
            dpToDateSales.SelectedDate = today;
        }

        private void btnDetailedDailyReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailedDailyReportModel detailedDailyReportModel = new DetailedDailyReportModel();
                AppSettings appSettings = new AppSettings();

                ReportDetailedDailyView reportDetailedDailyView = new ReportDetailedDailyView();
                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();

                dtFrom = (DateTime)dpDetailedDailyFromDate.Value;
                dtTo = (DateTime)dpDetailedDailyToDate.Value;

                reportDetailedDailyView.Print(appSettings.GetPrinterName(), dtFrom.ToString("yyyy-MM-dd HH:mi:ss"), dtTo.ToString("yyyy-MM-dd HH:mi:ss"));
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnProductWiseReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProductWiseSalesReportModel detailedDailyReportModel = new ProductWiseSalesReportModel();
                AppSettings appSettings = new AppSettings();

                ReportProductWiseSalesView reportProductWiseSalesView = new ReportProductWiseSalesView();
                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();

                dtFrom = (DateTime)dpProductwiseFromDate.Value;
                dtTo = (DateTime)dpProductWiseToDate.Value;

                reportProductWiseSalesView.Print(appSettings.GetPrinterName(), dtFrom.ToString("yyyy-MM-dd HH:mi:ss"), dtTo.ToString("yyyy-MM-dd HH:mi:ss"));
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
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

        private void btnCessReportExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonMethods commonMethods = new CommonMethods();
                string path = string.Empty, firstLine = string.Empty;
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                CessReportModel cessReportModel = new CessReportModel();
                CessCategoryReportModel cessCategoryReportModel = new CessCategoryReportModel();
                
                if (chkCess.IsChecked == false)
                {
                    cessReportModel = customerOrderViewModel.GetCessReport(dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat));

                    string fileName = "CessReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
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
                        commonMethods.WriteCessExcelFile(commonMethods.ConvertToDataTable(cessReportModel.CessSummaryList), commonMethods.ConvertToDataTable(cessReportModel.CessDetailList), path, firstLine);
                    }
                }
                else
                {
                    cessCategoryReportModel = customerOrderViewModel.GetCessCategoryReport(dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat));

                    string fileName = "CategoryWiseCessReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
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
                        commonMethods.WriteCessCategoryExcelFile(commonMethods.ConvertToDataTable(cessCategoryReportModel.CessSummaryList), commonMethods.ConvertToDataTable(cessCategoryReportModel.CessDetailList), path, firstLine);
                    }
                }

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnDetailedDailyExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();

                dtFrom = (DateTime)dpDetailedDailyFromDate.Value;
                dtTo = (DateTime)dpDetailedDailyToDate.Value;


                List<DetailedDailyReportModel> detailedDailyReportModels = new List<DetailedDailyReportModel>();
                ReportViewModel reportViewModel = new ReportViewModel();

                detailedDailyReportModels = reportViewModel.GetDetailedDailyByDate(dtFrom.ToString("yyyy-MM-dd HH:mi:ss"), dtTo.ToString("yyyy-MM-dd HH:mi:ss"));

                CommonMethods commonMethods = new CommonMethods();
                string path = string.Empty, firstLine = string.Empty;

                string fileName = "DetailedDailyReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = fileName != "" ? fileName : "gpmfca-exportedDocument",
                    DefaultExt = ".xlsx",
                    Filter = "Common Seprated Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    DataTable table = new DataTable();

                    table = commonMethods.ConvertToDataTable(detailedDailyReportModels);
                    firstLine = "Detailed Daily List for " + dtFrom.ToString("yyyy-MM-dd HH:mi:ss") + " to " + dtTo.ToString("yyyy-MM-dd HH:mi:ss");
                    commonMethods.WriteExcelDetailDailySalesFile(table, path, firstLine);
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnModeofPaymentReportExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonMethods commonMethods = new CommonMethods();
                string path = string.Empty, firstLine = string.Empty;

                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();

                List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();
                modeofPaymentReportModel = customerOrderViewModel.GetModOfPaymentReport(dpFromDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat));

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

                    DataTable dtData = new DataTable();
                    DataTable dtDataResult = new DataTable();

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
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }

        }

        private void btnSalesVoucherExport_Click(object sender, RoutedEventArgs e)
        {
            CommonMethods commonMethods = new CommonMethods();
            string path = string.Empty, firstLine = string.Empty;

            TallyXMLView tallyXMLView = new TallyXMLView();

            List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();

            string fileName = LoginDetail.OutletName.Trim().ToString() + "_TallySalesVoucher_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
            var saveFileDialog = new SaveFileDialog
            {
                FileName = fileName != "" ? fileName : "gpmfca-exportedDocument",
                DefaultExt = ".XML",
                Filter = "Common Seprated Documents (.XML)|*.XML"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;

                tallyXMLView.GenerateSalesVoucher(dpFromDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat), path);
            }
        }

        private void btnProductWiseExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();

                dtFrom = (DateTime)dpProductwiseFromDate.Value;
                dtTo = (DateTime)dpProductWiseToDate.Value;

                List<ProductWiseSalesReportModel> productWiseSalesReportModels = new List<ProductWiseSalesReportModel>();
                ReportViewModel reportViewModel = new ReportViewModel();

                productWiseSalesReportModels = reportViewModel.GetProductWiseSales(dtFrom.ToString("yyyy-MM-dd HH:mi:ss"), dtTo.ToString("yyyy-MM-dd HH:mi:ss"),"Excel");

                CommonMethods commonMethods = new CommonMethods();
                string path = string.Empty, firstLine = string.Empty;

                string fileName = "ProductWiseSalesReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = fileName != "" ? fileName : "gpmfca-exportedDocument",
                    DefaultExt = ".xlsx",
                    Filter = "Common Seprated Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    DataTable table = new DataTable();

                    table = commonMethods.ConvertToDataTable(productWiseSalesReportModels);
                    firstLine = "Productwise Sales List for " + dtFrom.ToString("yyyy-MM-dd HH:mi:ss") + " to " + dtTo.ToString("yyyy-MM-dd HH:mi:ss");
                    commonMethods.WriteExcelProductWiseSaleFile(table, path, firstLine);
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
    }
}
