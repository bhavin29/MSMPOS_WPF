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
            dpFromDate.SelectedDate = System.DateTime.Now;
            dpToDate.SelectedDate = System.DateTime.Now;
            customerOrderHistoryModel = customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString("yyyy/MM/dd"), dpToDate.SelectedDate.Value.ToString("yyyy/MM/dd"));
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
                customerOrderHistoryModel = customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString("yyyy/MM/dd"), dpToDate.SelectedDate.Value.ToString("yyyy/MM/dd"));
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
            CommonMethods commonMethods = new CommonMethods();
            string path = string.Empty;
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
            commonMethods.WriteExcelFile(commonMethods.ConvertToDataTable(customerOrderHistoryModel), path);
        }
    }
}
