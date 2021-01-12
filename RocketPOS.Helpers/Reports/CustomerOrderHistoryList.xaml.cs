using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Views;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for OrderList.xaml
    /// </summary>
    public partial class CustomerOrderHistoryList : Window
    {
        List<CustomerOrderHistoryModel> customerOrderHistoryModel = new List<CustomerOrderHistoryModel>();
        public CustomerOrderHistoryList()
        {
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            InitializeComponent();
            CenterWindowOnScreen();

            dpFromDate.SelectedDate = System.DateTime.Now;
            dpToDate.SelectedDate = System.DateTime.Now;
            customerOrderHistoryModel = customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat));
            this.dgOrderList.ItemsSource = customerOrderHistoryModel;
        }

        private void dgOrderList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void btnSearchOrderList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<CustomerOrderHistoryModel> customerOrderHistoryModel = new List<CustomerOrderHistoryModel>();
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                customerOrderHistoryModel = customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat));
                this.dgOrderList.ItemsSource = customerOrderHistoryModel;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnReceiptPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReceiptPrintView printReceipt = new ReceiptPrintView();
                AppSettings appSettings = new AppSettings();
                var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
                printReceipt.Print(appSettings.GetPrinterName(), order.Id);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnSalesExcelExport_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            CommonMethods commonMethods = new CommonMethods();
            string path = string.Empty,firstLine = string.Empty;
            List<CustomerOrderHistoryModel> customerOrderHistoryModel = new List<CustomerOrderHistoryModel>();
            
            customerOrderHistoryModel = (List<CustomerOrderHistoryModel>)dgOrderList.ItemsSource;

            string fileName = "SalesReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
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

            DataTable table = new DataTable();
            table = commonMethods.ConvertToDataTable(customerOrderHistoryModel);
            table.Columns.Remove("Id");
            firstLine = "Sale List for " + dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat) + " to " + dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat);
            commonMethods.WriteExcelFile(table, path, firstLine);
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
