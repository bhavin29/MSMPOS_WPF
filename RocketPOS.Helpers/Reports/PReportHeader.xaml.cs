using RocketPOS.Core.Constants;
using RocketPOS.Model;
using RocketPOS.ViewModels;
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
            FillDropdown();
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
                cmbCategory.IsReadOnly = true;
                cmbCategory.SelectedValuePath = "Id";
                cmbCategory.DisplayMemberPath = "Name";

                reportDropDownModels = reportViewModel.GetDropdown("Foodmenu");
                cmbProduct.ItemsSource = reportDropDownModels;
  //              cmbProduct.Text = " All";
                cmbProduct.IsEditable = true;
                cmbProduct.IsReadOnly = true;
                cmbProduct.SelectedValuePath = "Id";
                cmbProduct.DisplayMemberPath = "Name";

               cmbCategory.SelectedValue = ReportDetail.CategoryId;
               cmbProduct.SelectedValue= ReportDetail.ProductId ;

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

            if (cmbCategory.SelectedValue !=null)
            ReportDetail.CategoryId =(int) cmbCategory.SelectedValue;

            if (cmbProduct.SelectedValue != null)
                ReportDetail.ProductId =(int) cmbProduct.SelectedValue;

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
    }
}
