using Microsoft.Win32;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Views;
using System;
using System.Collections.Generic;
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

            DateTime dt = new DateTime();
            dt = DateTime.Now;

            DateTime dtWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

            dpDetailedDailyFromDate.Value = dtWithTime;
            dpDetailedDailyToDate.Value = DateTime.Now;

            DateTime baseDate = DateTime.Now;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            dpFromDate.SelectedDate = thisMonthStart;
            dpToDate.SelectedDate = today;
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
                }
                firstLine = LoginDetail.ClientName;
                commonMethods.WriteCessExcelFile(commonMethods.ConvertToDataTable(cessReportModel.CessSummaryList), commonMethods.ConvertToDataTable(cessReportModel.CessDetailList), path, firstLine);

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
    }
}
