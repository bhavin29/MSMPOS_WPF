using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
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

            DateTime dtWithTime = new DateTime(dt.Year,dt.Month,dt.Day,0,0,0);
  
            dpDetailedDailyFromDate.Value = dtWithTime;
            dpDetailedDailyToDate.Value = DateTime.Now;
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

                dtFrom = (DateTime) dpDetailedDailyFromDate.Value;
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
    }
}
