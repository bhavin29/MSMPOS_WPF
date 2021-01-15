using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
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
            try
            {
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                InitializeComponent();
                CenterWindowOnScreen();

                dpFromDate.SelectedDate = System.DateTime.Now;
                dpToDate.SelectedDate = System.DateTime.Now;
                customerOrderHistoryModel = customerOrderViewModel.GetCustomerOrderHistoryList(dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat), dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat));

                if (customerOrderHistoryModel.Count > 0)
                {
                    lblInvoiceCount.Content = customerOrderHistoryModel[0].InvoiceCount;
                    lblInvoiceTotal.Content = customerOrderHistoryModel[0].InvoiceTotal;
                }
                else
                {
                    lblInvoiceCount.Content = "0";
                    lblInvoiceTotal.Content = "0.00";
                }
                this.dgOrderList.ItemsSource = customerOrderHistoryModel;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
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

                if (customerOrderHistoryModel.Count > 0)
                {
                    lblInvoiceCount.Content = customerOrderHistoryModel[0].InvoiceCount;
                    lblInvoiceTotal.Content = customerOrderHistoryModel[0].InvoiceTotal;
                }
                else
                {
                    lblInvoiceCount.Content = "0";
                    lblInvoiceTotal.Content = "0.00";
                }
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
                var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
                if (string.IsNullOrEmpty(order.SalesInvoiceNumber))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerOrderHistory, StatusMessages.ReceiptNotReady, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                ReceiptPrintView printReceipt = new ReceiptPrintView();
                AppSettings appSettings = new AppSettings();
                printReceipt.Print(appSettings.GetPrinterName(), order.Id);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnSalesExcelExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonMethods commonMethods = new CommonMethods();
                string path = string.Empty, firstLine = string.Empty;
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


                    DataTable table = new DataTable();
                    table = commonMethods.ConvertToDataTable(customerOrderHistoryModel);
                    table.Columns.Remove("Id");
                    table.Columns.Remove("InvoiceTotal");
                    table.Columns.Remove("InvoiceCount");
                    firstLine = "Sale List for " + dpFromDate.SelectedDate.Value.ToString(CommonMethods.DateFormat) + " to " + dpToDate.SelectedDate.Value.ToString(CommonMethods.DateFormat);
                    commonMethods.WriteExcelFile(table, path, firstLine);
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
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

        private void btnPPPaymentMethodApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
                if (cmbSelectPaymentMethod.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerOrderHistory, StatusMessages.PaymentMethodSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbSelectPaymentMethod.Focus();
                    return;
                }
                customerOrderViewModel.UpdateBillDetailPaymentMethod(order.Id.ToString(), Convert.ToInt32(cmbSelectPaymentMethod.SelectedValue));
                ppChangePaymentMethod.IsOpen = false;
                btnSearchOrderList_Click(null, null);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnPPChangePaymentCancel_Click(object sender, RoutedEventArgs e)
        {
            ppChangePaymentMethod.IsOpen = false;
        }

        private void btnChangePaymentMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
                if (string.IsNullOrEmpty(order.Payment) || order.Payment.Contains(","))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerOrderHistory, StatusMessages.PaymentMethodModifyNotAllow, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbSelectPaymentMethod.Focus();
                    return;
                }
                ppChangePaymentMethod.IsOpen = true;
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                List<PaymentMethodModel> paymentMethodModels = new List<PaymentMethodModel>();
                paymentMethodModels = customerOrderViewModel.GetPaymentMethod();
                cmbSelectPaymentMethod.ItemsSource = paymentMethodModels;
                cmbSelectPaymentMethod.Text = "Select Payment";
                cmbSelectPaymentMethod.IsEditable = true;
                cmbSelectPaymentMethod.IsReadOnly = true;
                cmbSelectPaymentMethod.SelectedValuePath = "Id";
                cmbSelectPaymentMethod.DisplayMemberPath = "PaymentMethodName";
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnReceiptA4Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
                if (string.IsNullOrEmpty(order.SalesInvoiceNumber))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerOrderHistory, StatusMessages.ReceiptNotReady, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
    }
}
