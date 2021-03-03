using RocketPOS.Core.Constants;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for PReportHeader.xaml
    /// </summary>
    public partial class PReportHeader : Page
    {
        public PReportHeader()
        {
            InitializeComponent();
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            DateTime dtWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            DateTime dtProductWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

            DateTime baseDate = DateTime.Now;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthProductStart = dtProductWithTime.AddDays(1 - dtProductWithTime.Day);

            dpFromDatePayment.SelectedDate = thisMonthStart;
            dpToDatePayment.SelectedDate = today;

            lblreportTitle.Content = ReportDetail.ReportName;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
//            ReportDetail.ReportName = "ModeOfPayment";
            ReportDetail.ReportFromDate = dpFromDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat);
            ReportDetail.ReportToDate = dpToDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat);

         //   FrameReportViewer.Navigate(new ReportViewer());
            FrameReportViewer.Navigate(new PReportViewer());
        }
    }
}
