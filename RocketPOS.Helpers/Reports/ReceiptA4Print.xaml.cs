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
using System.Windows.Shapes;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for ReceiptA4Print.xaml
    /// </summary>
    public partial class ReceiptA4Print : Window
    {
        CommonMethods commonMethods = new CommonMethods();
        public ReceiptA4Print()
        {
            InitializeComponent();
            LoadData();
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(print, "invoice");
            }
        }

        public void LoadData()
        {
            int billId = ReportDetail.BillId;
            //Getting Receipt data 
            List<PrintReceiptA4Model> printReceiptA4Models = new List<PrintReceiptA4Model>();
            List<PrintReceiptItemModel> printReceiptItemModel = new List<PrintReceiptItemModel>();

            //Parameter pass global Customer Order Id
            PrintReceiptViewModel printReceiptViewModel = new PrintReceiptViewModel();

            if (billId > 0)
            {
                printReceiptA4Models = printReceiptViewModel.GetPrintReceiptA4ByBillId(billId);
                printReceiptItemModel = printReceiptViewModel.GetPrintReceiptItemA4ByBillId(billId);

                dgItemList.ItemsSource = printReceiptItemModel;

                if (printReceiptA4Models.Count>0)
                {
                    txtClientName.Text = printReceiptA4Models[0].ClientName;
                     //= printReceiptA4Models[0].ClientAddress1;
                    // = printReceiptA4Models[0].ClientAddress2;
                    txtClientEmail.Text = printReceiptA4Models[0].ClientEmail;
                    txtClientPIN.Text = printReceiptA4Models[0].ClientPhone;
                    txtSupplierName.Text = printReceiptA4Models[0].CustomerName;
                    txtSupplierEmail.Text= printReceiptA4Models[0].CustomerEmail;
                    txtSupplierAddress1.Text= printReceiptA4Models[0].CustomerAddress1;
                    txtSupplierAddres2.Text= printReceiptA4Models[0].CustomerAddress2;
                    //= printReceiptA4Models.CustomerPhone;
                    txtInvoiceDate.Text = printReceiptA4Models[0].BillDateTime.ToString("dd/MM/yyyy");
                    txtInvoiceNo.Text = printReceiptA4Models[0].SalesInvoiceNumber;
                    txtAmountinWords.Text = commonMethods.ConvertNumbertoWords((long) printReceiptA4Models[0].TotalAmount) + " ONLY";
                    txtTotalAmount.Text = printReceiptA4Models[0].TotalAmount.ToString("0.00");

                    txtHeader.Text = "INVOICE";
                    if (printReceiptA4Models[0].SalesInvoiceNumber =="" || printReceiptA4Models[0].SalesInvoiceNumber == null)
                    txtHeader.Text = "PROFORMA INVOICE";
                }
            }
        }
    }
}
