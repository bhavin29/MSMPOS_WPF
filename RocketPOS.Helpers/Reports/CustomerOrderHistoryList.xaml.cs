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
using RocketPOS.Core.Configuration;
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
            customerOrderHistoryModel =  customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString("yyyy/MM/dd"), dpToDate.SelectedDate.Value.ToString("yyyy/MM/dd"));
            this.dgOrderList.ItemsSource = customerOrderHistoryModel;
        }

        private void dgOrderList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void btnSearchOrderList_Click(object sender, RoutedEventArgs e)
        {
            List<CustomerOrderHistoryModel> customerOrderHistoryModel = new List<CustomerOrderHistoryModel>();
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            customerOrderHistoryModel = customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString("yyyy/MM/dd"), dpToDate.SelectedDate.Value.ToString("yyyy/MM/dd"));
            this.dgOrderList.ItemsSource = customerOrderHistoryModel;
        }

        private void btnReceiptPrint_Click(object sender, RoutedEventArgs e)
        {
            ReceiptPrintView printReceipt = new ReceiptPrintView();
            AppSettings appSettings = new AppSettings();
            var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
            printReceipt.Print(appSettings.GetPrinterName(), order.Id);
        }
    }
}
