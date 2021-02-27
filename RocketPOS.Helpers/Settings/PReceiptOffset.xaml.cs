using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
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

namespace RocketPOS.Helpers.Settings
{
    /// <summary>
    /// Interaction logic for PReceiptOffset.xaml
    /// </summary>
    public partial class PReceiptOffset : Page
    {
        PrintReceiptViewModel printReceiptViewModel = new PrintReceiptViewModel();
        List<ReportOffsetModel> reportOffsetModels = new List<ReportOffsetModel>();
        public PReceiptOffset()
        {
            InitializeComponent();
            reportOffsetModels = printReceiptViewModel.GetReportOffsetByReportName("R1");
            dgReportOffset.ItemsSource = reportOffsetModels;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (btnSave.Content.ToString() == "Edit")
            {
                btnSave.Content = "Save";
                btnCancel.Visibility = Visibility.Visible;
                int j = 1;
                for (int i = 0; i < dgReportOffset.Items.Count; i++)
                {
                    var reportOffsetModel = (ReportOffsetModel)dgReportOffset.Items[i];
                    ContentPresenter myCp = dgReportOffset.Columns[j].GetCellContent(reportOffsetModel) as ContentPresenter;
                    var myTemplate = myCp.ContentTemplate;
                    TextBox mytxtbox = myTemplate.FindName("txtColumnOffset", myCp) as TextBox;
                    mytxtbox.IsEnabled = true;
                }
            }
            else
            {
                int j = 1;
                for (int i = 0; i < dgReportOffset.Items.Count; i++)
                {
                    var reportOffsetModel = (ReportOffsetModel)dgReportOffset.Items[i];
                    ContentPresenter myCp = dgReportOffset.Columns[j].GetCellContent(reportOffsetModel) as ContentPresenter;
                    var myTemplate = myCp.ContentTemplate;
                    TextBox mytxtbox = myTemplate.FindName("txtColumnOffset", myCp) as TextBox;
                    if (string.IsNullOrEmpty(mytxtbox.Text))
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Enter valid data", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        ReportOffsetModel reportOffsetModel1 = (ReportOffsetModel)myCp.Content;
                        reportOffsetModel1.ColumnOffset = Convert.ToInt32(mytxtbox.Text);
                        var result = printReceiptViewModel.UpdateReportOffsetById(reportOffsetModel1);
                    }
                }
                btnCancel_Click(null,null);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "Edit";
            btnCancel.Visibility = Visibility.Hidden;
            int j = 1;
            for (int i = 0; i < dgReportOffset.Items.Count; i++)
            {
                var reportOffsetModel = (ReportOffsetModel)dgReportOffset.Items[i];
                ContentPresenter myCp = dgReportOffset.Columns[j].GetCellContent(reportOffsetModel) as ContentPresenter;
                var myTemplate = myCp.ContentTemplate;
                TextBox mytxtbox = myTemplate.FindName("txtColumnOffset", myCp) as TextBox;
                mytxtbox.IsEnabled = false;
            }
        }
    }
}
