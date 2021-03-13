using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RocketPOS.Model;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for ReportMenu.xaml
    /// </summary>
    /// 
    public partial class ReportMenu : Window
    {


        public ReportMenu()
        {
            InitializeComponent();

            ReportItem root = new ReportItem() { Title = "Reports" };


            root.Items.Add(new ReportItem() { Title = "Catering Levy Summary" });
            root.Items.Add(new ReportItem() { Title = "Catering Levy Summary Category Wise" });
            root.Items.Add(new ReportItem() { Title = "Sales Summary by Payment Method" });
            root.Items.Add(new ReportItem() { Title = "Invoicewise Tax Summary Report" });
            root.Items.Add(new ReportItem() { Title = "Detailed Sales Summary by Product" });
            root.Items.Add(new ReportItem() { Title = "Master Sales" });
            root.Items.Add(new ReportItem() { Title = "Table Statistics" });
            root.Items.Add(new ReportItem() { Title = "Sales Summary by Product Category" });
            root.Items.Add(new ReportItem() { Title = "Sales Summary by Product" });
            root.Items.Add(new ReportItem() { Title = "Sales Summary by Section" });
            root.Items.Add(new ReportItem() { Title = "Customer Reward" });
            root.Items.Add(new ReportItem() { Title = "Sales Summary by Hour" });
            root.Items.Add(new ReportItem() { Title = "Sales Summary by Weeks" });

            //root.Items.Add(new ReportItem() { Title = "Sales by Category Department Product qty desc" });
            //root.Items.Add(new ReportItem() { Title = "Detailed Sales Summary Report" });
            //root.Items.Add(new ReportItem() { Title = "Sales Summary by Hour" });
            //root.Items.Add(new ReportItem() { Title = "Sales Summary Five Weeks" });


            ReportItem node1 = new ReportItem() { Title = "Sales" };
            node1.Items.Add(new ReportItem() { Title = "Sales by Category Product qty desc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Category Product qty asc" });
            node1.Items.Add(new ReportItem() { Title = "Sales By Catrgory Product Amount desc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Category Product Amount asc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Category Product Amount desc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Category Product qty asc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Category Product qty desc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Catrgory" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Product Amount desc" });
            node1.Items.Add(new ReportItem() { Title = "Sales by Section Product qty desc" });
            root.Items.Add(node1);

            trvMenu.Items.Add(root);

            CenterWindowOnScreen();
        }

        private void txtModeOdPayment_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ReportDetail.ReportName = "ModeOfPayment";
            FrameHeader.Navigate(new PReportHeader());
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


        private void trvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var SelectedItem = e.NewValue as ReportItem;
            txtReportPreview.Visibility = Visibility.Hidden;
            txtReportPreview.Height = 0;
            FrameHeader.Visibility = Visibility.Visible;
            ShowReport(SelectedItem.Title.ToString());
        }
        private void ShowReport(string ReportName)
        {
            ReportDetail.ReportTitle = ReportName;
            int Reports = 1;

            if (ReportName == "Catering Levy Summary")
            {
                ReportDetail.ReportName = "CESS";
            }
            else if (ReportName == "Catering Levy Summary Category Wise")
            {
                ReportDetail.ReportName = "CESS_Category";
            }
            else if (ReportName == "Sales Summary by Payment Method")
            {
                ReportDetail.ReportName = "ModeOfPayment";
            }
            else if (ReportName == "Invoicewise Tax Summary Report")
            {
                ReportDetail.ReportName = "CESS_Detail";
            }
            else if (ReportName == "Detailed Sales Summary by Product")
            {
                ReportDetail.ReportName = "DetailSaleSummaryReport";
            }
            else if (ReportName == "Master Sales")
            {
                ReportDetail.ReportName = "MasterSale";
            }
            else if (ReportName == "Sales by Category Product qty desc")
            {
                ReportDetail.ReportName = "SalesByCategoryProductQtyDesc";
            }
            else if (ReportName == "Sales by Category Product qty asc")
            {
                ReportDetail.ReportName = "SalesByCategoryProductQtyAsc";
            }
            else if (ReportName == "Sales By Catrgory Product Amount desc")
            {
                ReportDetail.ReportName = "SalesByCategoryProductAmountDesc";
            }
            else if (ReportName == "Sales by Section Category Product Amount asc")
            {
                ReportDetail.ReportName = "SalesBySectionCategoryProductAmountAsc";
            }
            else if (ReportName == "Sales by Section Category Product Amount desc")
            {
                ReportDetail.ReportName = "SalesBySectionCategoryProductAmountDesc";
            }
            else if (ReportName == "Sales by Section Category Product qty asc")
            {
                ReportDetail.ReportName = "SalesBySectionCategoryProductQtyAsc";
            }
            else if (ReportName == "Sales by Section Category Product qty desc")
            {
                ReportDetail.ReportName = "SalesBySectionCategoryProductQtyDesc";
            }
            else if (ReportName == "Sales by Section Catrgory")
            {
                ReportDetail.ReportName = "SalesBySectionCategory";
            }
            else if (ReportName == "Sales by Section Product Amount desc")
            {
                ReportDetail.ReportName = "SalesBySectionProductAmountDesc";
            }
            else if (ReportName == "Sales by Section Product qty desc")
            {
                ReportDetail.ReportName = "SalesBySectionProductQtyDesc";
            }
            else if (ReportName == "Table Statistics")
            {
                ReportDetail.ReportName = "TableStatistics";
            }
            else if (ReportName == "Table Statistics")
            {
                ReportDetail.ReportName = "TableStatistics";
            }
            else if (ReportName == "Table Statistics")
            {
                ReportDetail.ReportName = "TableStatistics";
            }
            else if (ReportName == "Table Statistics")
            {
                ReportDetail.ReportName = "TableStatistics";
            }
            else if (ReportName == "Sales Summary by Product Category")
            {
                ReportDetail.ReportName = "SalesSummarybyProductCategory";
            }
            else if (ReportName == "Sales Summary by Product")
            {
                ReportDetail.ReportName = "SalesSummarybyProduct";
            }
            else if (ReportName == "Sales Summary by Section")
            {
                ReportDetail.ReportName = "SalesSummarybySection";
            }
            else if (ReportName == "Customer Reward")
            {
                ReportDetail.ReportName = "CustomerReward";
            }
            else if (ReportName == "Sales Summary by Hour")
            {
                ReportDetail.ReportName = "SalesSummarybyHour";
            }
            else if (ReportName == "Sales Summary by Weeks")
            {
                ReportDetail.ReportName = "SalesSummaryFiveWeeks";
            }
            else
            {
                Reports = 0;
            }

            if (Reports == 0)
            {
                txtReportPreview.Visibility = Visibility.Visible;
                txtReportPreview.Height = 40;
                FrameHeader.Visibility = Visibility.Hidden;
            }
            else
            {
                FrameHeader.Navigate(new PReportHeader());
            }

        }
    }

    public class ReportItem
    {
        public ReportItem()
        {
            this.Items = new ObservableCollection<ReportItem>();
        }

        public string Title { get; set; }

        public ObservableCollection<ReportItem> Items { get; set; }
    }

}
