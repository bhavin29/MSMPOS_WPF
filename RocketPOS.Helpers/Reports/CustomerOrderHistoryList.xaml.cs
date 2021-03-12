using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
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
                ReportDetail.BillId = order.Id;
                this.Height = 2;
                ReceiptA4Print form = new ReceiptA4Print();
             //   form.Owner = this;
           //     form.ShowDialog();
               this.Height = 800;
               return;
  
               // var order = (CustomerOrderHistoryModel)dgOrderList.SelectedItem;
                //if (string.IsNullOrEmpty(order.SalesInvoiceNumber))
                //{
                //    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerOrderHistory, StatusMessages.ReceiptNotReady, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                //    return;
                //}

                FlowDocument flowDocument;
               // flowDocument = PrintA4(order.Id);

                this.Height = 200;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {

                    flowDocument.PageHeight = 1200;// printDialog.PrintableAreaHeight;
                    flowDocument.PageWidth = 800;// printDialog.PrintableAreaWidth;

                    IDocumentPaginatorSource idocument = flowDocument as IDocumentPaginatorSource;

                    printDialog.PrintDocument(idocument.DocumentPaginator, "Printing ...");
                }
                this.Height = 800;

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }


        private FlowDocument PrintA4(int billId)
        {
            //Getting Receipt data 
            List<PrintReceiptA4Model> printReceiptModel = new List<PrintReceiptA4Model>();
            List<PrintReceiptItemModel> printReceiptItemModel = new List<PrintReceiptItemModel>();

            //Parameter pass global Customer Order Id
            PrintReceiptViewModel printReceiptViewModel = new PrintReceiptViewModel();

            printReceiptModel = printReceiptViewModel.GetPrintReceiptA4ByBillId(billId);
            printReceiptItemModel = printReceiptViewModel.GetPrintReceiptItemA4ByBillId(billId);

            var document = new FlowDocument();
            document.PagePadding = new Thickness(20, 20, 20, 20);
            document.PageWidth = 210;
            document.PageHeight = 297;
            document.ColumnWidth = 999999;
            var section = new Section();
            section.LineHeight = Double.NaN;

            var pReportTitle = new Paragraph();
            pReportTitle.Inlines.Add(new Run(ReportDetail.ReportTitle));
            pReportTitle.LineHeight = Double.NaN;
            pReportTitle.FontSize = 24;
            document.Blocks.Add(pReportTitle);

            var pHeader = new Paragraph();
            pHeader.Inlines.Add(new Run(LoginDetail.ClientName + "\n" + LoginDetail.Address1 + "\n" + LoginDetail.Address2));
            pHeader.LineHeight = Double.NaN;
            document.Blocks.Add(pHeader);

            document.Blocks.Add(new BlockUIContainer(new Separator()));
            var pParameter = new Paragraph();
            pParameter.Inlines.Add(new Run("From Date: " + ReportDetail.ReportFromDate + "         fsdfsfsfsfsfsdfsdfsdsdf sfsd sdf sdfsdfsdfs 111sdfs fdsdfsdfsdf222s3d sdf sdfsdfsd       33           To Date: " + ReportDetail.ReportToDate));
            pParameter.LineHeight = Double.NaN;
            document.Blocks.Add(pParameter);

            document.ColumnGap = 0;

            //Table A
            var tableA = new Table();
            tableA.CellSpacing = 0;
            tableA.BorderThickness = new Thickness(1);
            tableA.BorderBrush = Brushes.Black;
                       var supplierColumn = new TableColumn();
            supplierColumn.Width = new GridLength(375);
            var invoiceNoColumn = new TableColumn();
            invoiceNoColumn.Width = new GridLength(188);
            var invoiceDate = new TableColumn();
            invoiceDate.Width = new GridLength(188);

  
            tableA.Columns.Add(supplierColumn);
            tableA.Columns.Add(invoiceNoColumn);
            tableA.Columns.Add(invoiceDate);

            var rowGroupA = new TableRowGroup();
            var itemRowA = new TableRow();
            var tcell = new TableCell();
            var para = new Paragraph();
            tcell.BorderThickness = new Thickness(2);

             para = new Paragraph();
            para.Inlines.Add(new Run(""));
            tcell.Blocks.Add(para);


            //Assuming your data item has Quantity, Price and Text
            itemRowA.Cells.Add(new TableCell(new Paragraph(new Run("FoodMenuQty"))));
            itemRowA.Cells.Add(new TableCell(new Paragraph(new Run("Rate"))));
            itemRowA.Cells.Add(new TableCell(new Paragraph(new Run("Total"))));

           // TableCell a =new

            //rowGroupA.Rows.Add(itemRowA);
          //  tableA.RowGroups.Add(rowGroupA);
          //  document.Blocks.Add(tableA);
            //End Table A

            //            var table = new Table();
            //            table.CellSpacing = 0;

            //            var quantityColumn = new TableColumn();
            //            quantityColumn.Width = new GridLength(80);
            //            var priceColumn = new TableColumn();
            //            priceColumn.Width = new GridLength(80);
            //            var textColumn = new TableColumn();
            //            textColumn.Width = new GridLength(597);

            //            table.Columns.Add(quantityColumn);
            //            table.Columns.Add(priceColumn);
            //            table.Columns.Add(textColumn);

            //             var rowGroup = new TableRowGroup();
            //            var itemRow1 = new TableRow();

            //            //Assuming your data item has Quantity, Price and Text
            //            itemRow1.Cells.Add(new TableCell(new Paragraph(new Run("FoodMenuQty"))));
            //            itemRow1.Cells.Add(new TableCell(new Paragraph(new Run("Rate"))));
            //            itemRow1.Cells.Add(new TableCell(new Paragraph(new Run("Total"))));

            //            rowGroup.Rows.Add(itemRow1);
            ////            document.Blocks.Add(table);

            //            foreach (var item in printReceiptItemModel)
            //            {
            //                //Add your data
            //                var itemRow = new TableRow();

            //                //Assuming your data item has Quantity, Price and Text
            //                itemRow.Cells.Add(new TableCell(new Paragraph(new Run(item.FoodMenuQty.ToString()))));
            //                itemRow.Cells.Add(new TableCell(new Paragraph(new Run(item.FoodMenuRate.ToString()))));
            //                itemRow.Cells.Add(new TableCell(new Paragraph(new Run(item.Price.ToString()))));

            //                rowGroup.Rows.Add(itemRow);
            //                //Etc.
            //            }

            //            table.RowGroups.Add(rowGroup);

            //            table.BorderThickness = new Thickness(1);
            //            table.BorderBrush = Brushes.Black;


            //            document.Blocks.Add(table);


            //FlowDocument flowDocument = new FlowDocument();


            return document;
        }
    }
}
