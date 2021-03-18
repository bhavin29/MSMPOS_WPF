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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for PReportHeader.xaml
    /// </summary>
    public partial class PReportHeader : Page
    {
        DetailedDailyReportModel detailedDailyReportModel = new DetailedDailyReportModel();
        ProductWiseSalesReportModel productWiseSalesReportModel = new ProductWiseSalesReportModel();
        AppSettings appSettings = new AppSettings();

        ReportProductWiseSalesView reportProductWiseSalesView = new ReportProductWiseSalesView();
        ReportDetailedDailyView reportDetailedDailyView = new ReportDetailedDailyView();
        public PReportHeader()
        {
            InitializeComponent();
            ResetControl();
        }

        private void ResetControl()
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            DateTime dtWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            DateTime dtProductWithTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            DateTime baseDate = DateTime.Now;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthProductStart = dtProductWithTime.AddDays(1 - dtProductWithTime.Day);

            //Date
            if (ReportDetail.ReportFromDate == "")
            {
                dpFromDatePayment.SelectedDate = thisMonthStart;
                dpToDatePayment.SelectedDate = today;
            }
            else
            {
                dpFromDatePayment.Text = ReportDetail.ReportFromDate;
                dpToDatePayment.Text = ReportDetail.ReportToDate;
            }

            lblreportTitle.Content = ReportDetail.ReportTitle;

            //Dropdown
            if (ReportDetail.ReportName == "TallySalesVoucher" || ReportDetail.ReportName == "DetailedDaily" || ReportDetail.ReportName == "ProductwiseSales" || ReportDetail.ReportName == "CESS" ||
                ReportDetail.ReportName == "ModeOfPayment" || ReportDetail.ReportName == "CESS_Detail" || ReportDetail.ReportName == "TableStatistics" ||
                ReportDetail.ReportName == "SalesSummarybySection" || ReportDetail.ReportName == "CustomerReward" || ReportDetail.ReportName == "SalesSummarybyHour" )
            {
                lblCategory.Visibility = Visibility.Hidden;
                lblProduct.Visibility = Visibility.Hidden;
                cmbCategory.Visibility = Visibility.Hidden;
                cmbProduct.Visibility = Visibility.Hidden;
            }
            else if ( ReportDetail.ReportName == "CESS_Category")
            {
                cmbCategory.Visibility = Visibility.Visible;
                cmbProduct.Visibility = Visibility.Hidden;
                lblCategory.Visibility = Visibility.Visible;
                lblProduct.Visibility = Visibility.Hidden;
            }
            else
            {
                lblCategory.Visibility = Visibility.Visible;
                lblProduct.Visibility = Visibility.Visible;
                cmbCategory.Visibility = Visibility.Visible;
                cmbProduct.Visibility = Visibility.Visible;
                FillDropdown();
            }


            //XML & Print Button
            btnGeneric.Visibility = Visibility.Hidden;
            if (ReportDetail.ReportName == "TallySalesVoucher" || ReportDetail.ReportName == "DetailedDaily" || ReportDetail.ReportName == "ProductwiseSales")
            {
                btnGeneric.Visibility = Visibility.Visible;
                btnGeneric.Content = "Print";
                if (ReportDetail.ReportName == "TallySalesVoucher")
                {
                    btnGeneric.Content = "XML";
                }
            }


        }

        private void FillDropdown()
        {
            try
            {
                ReportViewModel reportViewModel = new ReportViewModel();
                List<ReportDropDownModel> reportDropDownModels = new List<ReportDropDownModel>();

                reportDropDownModels = reportViewModel.GetDropdown("FoodmenuCategory");
                cmbCategory.ItemsSource = reportDropDownModels;
                //                cmbCategory.Text = " All";
                cmbCategory.IsEditable = true;
                //cmbCategory.IsReadOnly = true;
                cmbCategory.SelectedValuePath = "Id";
                cmbCategory.DisplayMemberPath = "Name";

                reportDropDownModels = reportViewModel.GetDropdown("Foodmenu");
                cmbProduct.ItemsSource = reportDropDownModels;
                //              cmbProduct.Text = " All";
                cmbProduct.IsEditable = true;
                //cmbProduct.IsReadOnly = true;
                cmbProduct.SelectedValuePath = "Id";
                cmbProduct.DisplayMemberPath = "Name";

                cmbCategory.SelectedValue = ReportDetail.CategoryId;
                cmbProduct.SelectedValue = ReportDetail.ProductId;

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //            ReportDetail.ReportName = "ModeOfPayment";
            ReportDetail.ReportFromDate = dpFromDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat);
            ReportDetail.ReportToDate = dpToDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat);

            if (cmbCategory.SelectedValue != null)
                ReportDetail.CategoryId = (int)cmbCategory.SelectedValue;

            if (cmbProduct.SelectedValue != null)
                ReportDetail.ProductId = (int)cmbProduct.SelectedValue;

            FrameReportViewer.Navigate(new PReportViewer());
        }

        private void dpFromDatePayment_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFromDatePayment.SelectedDate != null)
            {
                ReportDetail.ReportFromDate = dpFromDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat);
            }
        }

        private void dpToDatePayment_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpToDatePayment.SelectedDate != null)
            {
                ReportDetail.ReportToDate = dpToDatePayment.SelectedDate.Value.ToString(CommonMethods.DateFormat);
            }
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCategory.SelectedValue != null)
                ReportDetail.CategoryId = (int)cmbCategory.SelectedValue;
        }

        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProduct.SelectedValue != null)
                ReportDetail.ProductId = (int)cmbProduct.SelectedValue;
        }

        private void btnGeneric_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDetail.ReportName == "TallySalesVoucher")
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

                    tallyXMLView.GenerateSalesVoucher(ReportDetail.ReportFromDate, ReportDetail.ReportToDate,path);
                }

            }
            else if (ReportDetail.ReportName == "DetailedDaily")
            {
                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();

                dtFrom = Convert.ToDateTime(ReportDetail.ReportFromDate);
                dtTo = Convert.ToDateTime(ReportDetail.ReportToDate);

                reportDetailedDailyView.Print(appSettings.GetPrinterName(), dtFrom.ToString("yyyy-MM-dd") + " 00:00:00", dtTo.ToString("yyyy-MM-dd") + " 23:59i:59");
            }
            else if (ReportDetail.ReportName == "ProductwiseSales")
            {
                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();

                dtFrom = Convert.ToDateTime(ReportDetail.ReportFromDate);
                dtTo = Convert.ToDateTime(ReportDetail.ReportToDate);

                reportProductWiseSalesView.Print(appSettings.GetPrinterName(), dtFrom.ToString("yyyy-MM-dd") + " 00:00:00", dtTo.ToString("yyyy-MM-dd") + " 23:59i:59");
            }

        }

    }
}
