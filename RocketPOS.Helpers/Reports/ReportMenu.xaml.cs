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
            
            ReportItem node1 = new ReportItem() { Title = "Sales" };
            node1.Items.Add(new ReportItem() { Title = " Sales by Category Department Product qty desc" });
            node1.Items.Add(new ReportItem() { Title = " Sales by Category Department Product qty ASC" });
            node1.Items.Add(new ReportItem() { Title = " Sales By Catrgory Department Product Amount desc" });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Category Department Product Amount asc " });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Category Department Product Amount desc" });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Category Department Product qty asc" });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Category Department Product qty desc" });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Catrgory Department " });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Product Amount desc" });
            node1.Items.Add(new ReportItem() { Title = " Sales by Section Product qty desc" });
            root.Items.Add(node1);

            root.Items.Add(new ReportItem() { Title = " Sales by Category Department Product qty desc" });
            root.Items.Add(new ReportItem() { Title = " Detailed Sales Summary by Product Depertment" });
            root.Items.Add(new ReportItem() { Title = " Detailed Sales Summary Report" });
            root.Items.Add(new ReportItem() { Title = " Master Sales Report" });
            root.Items.Add(new ReportItem() { Title = " Mode of Payment" });


            ReportItem childItem1 = new ReportItem() { Title = "Child item #1" };
            childItem1.Items.Add(new ReportItem() { Title = "Child item #1.1 Child item #1.1 Child item #1.1 Child item #1.1" });
            childItem1.Items.Add(new ReportItem() { Title = "Child item #1.2" });
            root.Items.Add(childItem1);
            
            root.Items.Add(new ReportItem() { Title = "Child item #2" });

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

        private void trvMenu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (sender is TreeViewItem)
            //{
            //    if (((TreeViewItem)sender).IsSelected)
            //    {
            //        MessageBox.Show(((TreeViewItem)sender).va ue.ToString()); 
            //    }
            //}

           
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
