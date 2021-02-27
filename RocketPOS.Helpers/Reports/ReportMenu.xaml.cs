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
using RocketPOS.Core.Constants;
namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for ReportMenu.xaml
    /// </summary>
    public partial class ReportMenu : Window
    {
        public ReportMenu()
        {
            InitializeComponent();
        }

        private void txtModeOdPayment_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ReportDetail.ReportName = "ModeOfPayment";
            FrameHeader.Navigate(new PReportHeader());
        }
    }
}
