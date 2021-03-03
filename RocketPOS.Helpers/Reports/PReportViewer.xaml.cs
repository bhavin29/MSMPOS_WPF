﻿using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using RocketPOS.Helpers.Reports.WPFPrintHelper;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Windows.Media;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for PReportViewer.xaml
    /// </summary>
    public partial class PReportViewer : Page
    {
        CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
        WPFPrintUtility wPFPrintHelper = new WPFPrintUtility();
        List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();
        DataTable dtData = new DataTable();
        DataTable dtDataResult = new DataTable();
        CommonMethods commonMethods = new CommonMethods();
        string reportTitle = "";
        string reportFooter = "------------------------------------------------------------------------END OF REPORT----------------------------------------------------------------------";
        string _reportName = "";
        public PReportViewer()
        {
            InitializeComponent();
            ReportLoad(ReportDetail.ReportName);
        }
        public void ReportLoad(string reportName)
        {

            _reportName = reportName;

            if (reportName == "ModeOfPayment")
            {
                modeofPaymentReportModel = customerOrderViewModel.GetModOfPaymentReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);
                dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);
                dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);
                reportTitle = "Mode Of Payment Report";
            }

            //common call
            DataTable mockDataTable = wPFPrintHelper.CreateMockDataTableForTest();
            wPFPrintHelper.CreateAndVisualizeDataTable(flowDocument, dtDataResult, reportTitle, reportFooter);
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            Print(flowDocument);
          //  wPFPrintHelper.Print(flowDocument);
        }

        private void excelButton_Click(object sender, RoutedEventArgs e)
        {
            string path = string.Empty, firstLine = string.Empty;

            if (_reportName == "ModeOfPayment")
            {
                string fileName = "ModeOfPaymentReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = fileName != "" ? fileName : "gpmfca-exportedDocument",
                    DefaultExt = ".xlsx",
                    Filter = "Common Seprated Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    firstLine = LoginDetail.ClientName;

                    dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);

                    //X axis column: PaymentMethodName
                    //Y axis column: BillDate
                    //Z axis column: BillAmount
                    //Null value: "-";
                    //Sum of values: true

                    dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);

                    commonMethods.WriteExcelModeOfPaymentFile(dtDataResult, path, firstLine);
                }

            }
        }

        void Print(FlowDocument flowDocument)
        {
            PrintDialog printDialog = new PrintDialog();
            bool? result = printDialog.ShowDialog();
            if (!result.HasValue)
                return;
            if (!result.Value)
                return;

            double pageWidth = printDialog.PrintableAreaWidth;
            double pageHeight = printDialog.PrintableAreaHeight;
            flowDocument = CreateFlowDocument(pageWidth, pageHeight);

            printDialog.PrintDocument(
             ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
             "Test print job");
        }

        FlowDocument CreateFlowDocument(double pageWidth, double pageHeight)
        {
            FlowDocument flowDocument = new FlowDocument();
            flowDocument.PageWidth = pageWidth;
            flowDocument.PageHeight = pageHeight;
            flowDocument.PagePadding = new Thickness(30.0, 50.0, 20.0, 30.0);
            flowDocument.IsOptimalParagraphEnabled = true;
            flowDocument.IsHyphenationEnabled = true;
            flowDocument.IsColumnWidthFlexible = true;

            Paragraph header = new Paragraph();
            header.FontSize = 18;
            header.Foreground = new SolidColorBrush(Colors.Black);
            header.FontWeight = FontWeights.Bold;
            header.Inlines.Add(new Run("Title of my document (will be cut off in XPS)"));
            flowDocument.Blocks.Add(header);

            Paragraph test = new Paragraph();
            test.FontSize = 12;
            test.Foreground = new SolidColorBrush(Colors.Black);
            test.FontWeight = FontWeights.Bold;
            test.Inlines.Add(new Run("This text should stretch across the entire width of the page. Let's see if it really does, though."));
            flowDocument.Blocks.Add(test);

            return flowDocument;
        }

    }
}
