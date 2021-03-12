using System.Windows.Controls;
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
using RocketPOS.Helpers.RMessageBox;
using RocketPOS.Views;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Data;
using Separator = LiveCharts.Wpf.Separator;
using System.Runtime.Serialization;

namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for PReportViewer.xaml
    /// </summary>
    public partial class PReportViewer : Page
    {
        CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
        ReportViewModel reportViewModel = new ReportViewModel();
        WPFPrintUtility wPFPrintHelper = new WPFPrintUtility();
        ExportExcel exportExcel = new ExportExcel();

        List<ModeofPaymentReportModel> modeofPaymentReportModel = new List<ModeofPaymentReportModel>();
        List<DetailSaleSummaryModel> detailSaleSummaryModels = new List<DetailSaleSummaryModel>();
        List<MasterSalesReportModel> masterSalesReportModels = new List<MasterSalesReportModel>();
        List<SalesByCategoryProductModel> salesByCategoryProductModel = new List<SalesByCategoryProductModel>();
        List<TableStatisticsModel> tableStatisticsModel = new List<TableStatisticsModel>();
        List<SalesSummaryModel> salesSummaryModel = new List<SalesSummaryModel>();
        List<SalesSummaryByFoodCategoryFoodMenuModel> salesSummaryByFoodCategoryFoodMenuModel = new List<SalesSummaryByFoodCategoryFoodMenuModel>();
        List<SalesSummaryBySectionModel> salesSummaryBySectionModel = new List<SalesSummaryBySectionModel>();
        List<CustomerRewardModel> customerRewardModel = new List<CustomerRewardModel>();
        List<SalesSummaryByWeek> salesSummaryByWeek = new List<SalesSummaryByWeek>();
        List<SalesSummaryByHours> salesSummaryByHours = new List<SalesSummaryByHours>();
        List<DatatableColumnName> datatableColumnNames;

        CessReportModel cessReportModel = new CessReportModel();
        CessCategoryReportModel cessCategoryReportModel = new CessCategoryReportModel();

        DataTable dtData = new DataTable();
        DataTable dtDataResult = new DataTable();
        CommonMethods commonMethods = new CommonMethods();
        string reportTitle = "";
        string reportFooter = "*****END OF REPORT*****";
        string _reportName = "";
        int categoryId = -1, foodMenuId = -1;
        public PReportViewer()
        {
            InitializeComponent();
            ReportLoad(ReportDetail.ReportName);
        }
        public void ReportLoad(string reportName)
        {

            _reportName = reportName;
            LoadReport();
        }

        private void LoadReport()
        {
            int isChart = 0;

            if (!DateValidated(ReportDetail.ReportFromDate, ReportDetail.ReportToDate))
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Please select FROM DATE grater than or equal to TO DATE", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);

                return;
            }

            if (_reportName == "ModeOfPayment")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date" , DataType="Date"},
                    new DatatableColumnName{ id =2, Cname=""},
                    new DatatableColumnName{ id =3, Cname=""},
                    new DatatableColumnName{ id =4, Cname=""},
                    new DatatableColumnName{ id =5, Cname=""},
                    new DatatableColumnName{ id =6, Cname=""},
                    new DatatableColumnName{ id =7, Cname=""},
                    new DatatableColumnName{ id =8, Cname=""}
                };

                modeofPaymentReportModel = customerOrderViewModel.GetModOfPaymentReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);
                dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);
                dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);
            }
            else if (_reportName == "CESS")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date" , DataType="Date"},
                    new DatatableColumnName{ id =2, Cname="Net Sales"},
                    new DatatableColumnName{ id =3, Cname="Vatable"},
                    new DatatableColumnName{ id =4, Cname="Non Vatable"},
                    new DatatableColumnName{ id =5, Cname="Tax"},
                    new DatatableColumnName{ id =6, Cname="Total"},
                    new DatatableColumnName{ id =7, Cname="Catering Levy"},
                    new DatatableColumnName{ id =8, Cname="8"},
                    new DatatableColumnName{ id =9, Cname="8"}
                };

                cessReportModel = customerOrderViewModel.GetCessReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);

                dtDataResult = commonMethods.ConvertToDataTable(cessReportModel.CessSummaryList);
            }
            else if (_reportName == "CESS_Detail")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date", DataType="Date"},
                    new DatatableColumnName{ id =2, Cname="Invoice#", DataType="String"},
                    new DatatableColumnName{ id =3, Cname="Net Sales"},
                    new DatatableColumnName{ id =4, Cname="Vatable"},
                    new DatatableColumnName{ id =5, Cname="Non Vatable"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Catering Levy"},
                };
                cessReportModel = customerOrderViewModel.GetCessReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);

                dtDataResult = commonMethods.ConvertToDataTable(cessReportModel.CessDetailList);
            }

            else if (_reportName == "CESS_Category")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Category",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Net Sales"},
                    new DatatableColumnName{ id =4, Cname="Vatable"},
                    new DatatableColumnName{ id =5, Cname="Non Vatable"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Catering Levy"},
                    new DatatableColumnName{ id =9, Cname="Value %"}
                };
                cessCategoryReportModel = customerOrderViewModel.GetCessCategoryReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate,categoryId,foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(cessCategoryReportModel.CessSummaryList);
            }
            else if (_reportName == "DetailSaleSummaryReport")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date", DataType="Date", Width=80},
                    new DatatableColumnName{ id =2, Cname="Product", DataType="String",Width=120},
                    new DatatableColumnName{ id =3, Cname="Gross"},
                    new DatatableColumnName{ id =4, Cname="Qty"},
                    new DatatableColumnName{ id =5, Cname="Net"},
                    new DatatableColumnName{ id =6, Cname="Dist."},
                    new DatatableColumnName{ id =7, Cname="Tax %"},
                    new DatatableColumnName{ id =8, Cname="Tax"},
                    new DatatableColumnName{ id =9, Cname="Total"},
                    new DatatableColumnName{ id =10, Cname="Cash"},
                    new DatatableColumnName{ id =11, Cname="Card"}
                };
                detailSaleSummaryModels = reportViewModel.GetDetailSaleSummaryReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(detailSaleSummaryModels);
            }
            else if (_reportName == "MasterSale")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date",Width=80, DataType="Date"},
                    new DatatableColumnName{ id =2, Cname="Time", Width=60, DataType="String"},
                    new DatatableColumnName{ id =3, Cname="Invoice#", DataType="String"},
                    new DatatableColumnName{ id =4, Cname="Product", DataType="String", Width=120},
                    new DatatableColumnName{ id =5, Cname="Rate"},
                    new DatatableColumnName{ id =6, Cname="Qty"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Discount"},
                    new DatatableColumnName{ id =9, Cname="Tax"},
                    new DatatableColumnName{ id =10, Cname="Gross"},
                    new DatatableColumnName{ id =11, Cname="Category"}
                };
                masterSalesReportModels = reportViewModel.GetMasterSaleReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(masterSalesReportModels);
            }
            else if (_reportName == "SalesByCategoryProductQtyDesc" || _reportName == "SalesByCategoryProductQtyAsc" || _reportName == "SalesByCategoryProductAmountDesc" || _reportName == "SalesBySectionCategoryProductAmountAsc"
                        || _reportName == "SalesBySectionCategoryProductAmountDesc" || _reportName == "SalesBySectionCategoryProductQtyAsc" || _reportName == "SalesBySectionCategoryProductQtyDesc"
                        || _reportName == "SalesBySectionCategory" || _reportName == "SalesBySectionProductAmountDesc" || _reportName == "SalesBySectionProductQtyDesc"
                        )
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Section", DataType="String"},
                    new DatatableColumnName{ id =2, Cname="Category", DataType="String",Width=120},
                    new DatatableColumnName{ id =2, Cname="Product", DataType="String",Width=120},
                    new DatatableColumnName{ id =4, Cname="Rate"},
                    new DatatableColumnName{ id =5, Cname="Qty"},
                    new DatatableColumnName{ id =6, Cname="Amount"},
                    new DatatableColumnName{ id =7, Cname="Disc."},
                    new DatatableColumnName{ id =8, Cname="Tax"},
                    new DatatableColumnName{ id =9, Cname="Total"},
                    new DatatableColumnName{ id =9, Cname="Value%"}
                };
                salesByCategoryProductModel = reportViewModel.GetSaleByCategorySectionReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, _reportName, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(salesByCategoryProductModel);
            }
            else if (_reportName == "TableStatistics")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Table Name", DataType="String", Width=300},
                    new DatatableColumnName{ id =2, Cname="Actual Capacity"},
                    new DatatableColumnName{ id =3, Cname="Expected Occupancy"},
                    new DatatableColumnName{ id =4, Cname="Occupancy"},
                    new DatatableColumnName{ id =5, Cname="Occupancy %"}
                };
                tableStatisticsModel = reportViewModel.GetTableStatisticsReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);

                dtDataResult = commonMethods.ConvertToDataTable(tableStatisticsModel);
            }
            else if (_reportName == "SalesSummarybyProductCategory")
            {
                isChart = 1;

                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Category", DataType="String", Width=175},
                    new DatatableColumnName{ id =2, Cname="Qty"},
                    new DatatableColumnName{ id =3, Cname="Net Sales"},
                    new DatatableColumnName{ id =4, Cname="Discount"},
                    new DatatableColumnName{ id =5, Cname="Tax"},
                    new DatatableColumnName{ id =6, Cname="Total"},
                    new DatatableColumnName{ id =7, Cname="Value %"},
                };
                salesSummaryModel = reportViewModel.GetSalesSummaryByFoodCategoryReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(salesSummaryModel);
            }
            else if (_reportName == "SalesSummarybyProduct")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Category", DataType="String", Width=150},
                    new DatatableColumnName{ id =2, Cname="Product", DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Qty"},
                    new DatatableColumnName{ id =4, Cname="Net Sales"},
                    new DatatableColumnName{ id =5, Cname="Discount"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Value %"}
                };
                salesSummaryByFoodCategoryFoodMenuModel = reportViewModel.GetSalesSummaryByFoodCategoryFoodMenuReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(salesSummaryByFoodCategoryFoodMenuModel);
            }
            else if (_reportName == "SalesSummarybySection")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Section", DataType="String", Width=150},
                    new DatatableColumnName{ id =2, Cname="Date", DataType="String", Width=80},
                    new DatatableColumnName{ id =3, Cname="Inv Count"},
                    new DatatableColumnName{ id =4, Cname="Net Sales"},
                    new DatatableColumnName{ id =5, Cname="Discount"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"}
                };
                salesSummaryBySectionModel = reportViewModel.GetSalesSummaryBySectionReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(salesSummaryBySectionModel);
            }
            else if (_reportName == "CustomerReward")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Customer Name",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Phone Number",DataType="String", Width=100},
                    new DatatableColumnName{ id =4, Cname="Date",DataType="Date", Width=160},
                    new DatatableColumnName{ id =5, Cname="Credit"},
                    new DatatableColumnName{ id =6, Cname="Debit"},
                    new DatatableColumnName{ id =7, Cname="Balance"}
                };
                customerRewardModel = reportViewModel.GetCustomerRewardReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, "", "");

                dtDataResult = commonMethods.ConvertToDataTable(customerRewardModel);
            }
            else if (_reportName == "SalesSummaryFiveWeeks")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Week",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Inv Count"},
                    new DatatableColumnName{ id =4, Cname="Net Sales"},
                    new DatatableColumnName{ id =5, Cname="Discount"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Gross"},
                };
                salesSummaryByWeek = reportViewModel.GetSalesSummaryByWeekReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate, categoryId, foodMenuId);

                dtDataResult = commonMethods.ConvertToDataTable(salesSummaryByWeek);
            }
            else if (_reportName == "SalesSummarybyHour")
            {
                datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="OrderDate",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="StartHour"},
                    new DatatableColumnName{ id =4, Cname="EndHour"},
                    new DatatableColumnName{ id =5, Cname="TotalInvoice"},
                    new DatatableColumnName{ id =6, Cname="NetSalesAmount"},
                    new DatatableColumnName{ id =7, Cname="TotalDiscount"},
                    new DatatableColumnName{ id =8, Cname="TotalTax"},
                    new DatatableColumnName{ id =9, Cname="TotalGrossAmount"},
                };
                salesSummaryByHours = reportViewModel.GetSalesSummaryByHoursReport(ReportDetail.ReportFromDate, ReportDetail.ReportToDate);

                dtDataResult = commonMethods.ConvertToDataTable(salesSummaryByHours);
            }
            reportTitle = _reportName;



            //common call
            DataTable mockDataTable = wPFPrintHelper.CreateMockDataTableForTest();
            wPFPrintHelper.CreateAndVisualizeDataTable(flowDocument, dtDataResult, reportTitle, reportFooter, datatableColumnNames);

            //create chart
            if (isChart == 1)
            {
                string[] labels = new string[dtDataResult.Rows.Count];
                ChartValues<double> chartValues = new ChartValues<double>();

                CartesianChart chart = new CartesianChart { Margin = new Thickness(10, 10, 10, 10), LegendLocation = LegendLocation.None, DataTooltip = new DefaultTooltip { SelectionMode = TooltipSelectionMode.SharedYValues } };

                for (int index = 0; index < dtDataResult.Rows.Count; index++)
                {
                    labels[index] = dtDataResult.Rows[index][0].ToString();
                    chartValues.Add(Convert.ToDouble(dtDataResult.Rows[index][2].ToString()));
                }

                RowSeries rowSeries = new RowSeries();
                rowSeries.Values = chartValues;
                rowSeries.DataLabels = true;
                rowSeries.FontSize = 16;
                rowSeries.FontWeight = FontWeights.Bold;
                rowSeries.MaxRowHeigth = 150;
                rowSeries.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5928b1"));

                SeriesCollection SeriesCollection = new SeriesCollection();
                SeriesCollection.Add(rowSeries);

                Separator separator = new Separator();
                separator.Step = double.NaN;

                Axis xAxis = new Axis { Foreground = Brushes.Black, FontSize = 16d, FontWeight = FontWeights.Bold, Title = "" };
                Axis yAxis = new Axis { Foreground = Brushes.Black, FontSize = 16d, FontWeight = FontWeights.Bold, Title = "", Separator = separator };

               // Formatter = value => value.ToString("N");
               // xAxis.LabelFormatter = Formatter;
                yAxis.Labels = labels;

                chart.Series = SeriesCollection;
                chart.AxisX.Add(xAxis);
                chart.AxisY.Add(yAxis);

                chart.Height = 500;
                chart.Width = 750;

                BlockUIContainer c = new BlockUIContainer(chart);
                flowDocument.Blocks.Add(c);
            }
        }
        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            Print(flowDocument);
            //  wPFPrintHelper.Print(flowDocument);
        }

        private void excelButton_Click(object sender, RoutedEventArgs e)
        {
            string path = string.Empty, firstLine = string.Empty;
            string fileName = _reportName + "_"+ DateTime.Now.ToString("MM-dd-yyyy_HHmmss").ToString().Replace("-", "_");
            
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

                if (_reportName == "ModeOfPayment")
                {
                    dtData = commonMethods.ConvertToDataTable(modeofPaymentReportModel);
                    dtDataResult = commonMethods.GetInversedDataTable(dtData, "PaymentMethodName", "BillDate", "BillAmount", " ", true);
                    commonMethods.WriteExcelModeOfPaymentFile(dtDataResult, path, firstLine);
                }
                else if (_reportName == "CESS")
                {
                    commonMethods.WriteCessExcelFile(commonMethods.ConvertToDataTable(cessReportModel.CessSummaryList), commonMethods.ConvertToDataTable(cessReportModel.CessDetailList), path, firstLine);
                }
                else if (_reportName == "CESS_Detail")
                {
                    commonMethods.WriteCessExcelFile(commonMethods.ConvertToDataTable(cessReportModel.CessDetailList), commonMethods.ConvertToDataTable(cessReportModel.CessDetailList), path, firstLine);
                }
                else if (_reportName == "CESS_Category")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date", DataType="Date"},
                    new DatatableColumnName{ id =2, Cname="Category",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Net Sales"},
                    new DatatableColumnName{ id =4, Cname="Vatable"},
                    new DatatableColumnName{ id =5, Cname="Non Vatable"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Catering Levy"}
                };

                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(cessCategoryReportModel.CessSummaryList), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "DetailSaleSummaryReport")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date", DataType="Date", Width=80},
                    new DatatableColumnName{ id =2, Cname="Product", DataType="String",Width=120},
                    new DatatableColumnName{ id =3, Cname="Gross"},
                    new DatatableColumnName{ id =4, Cname="Qty"},
                    new DatatableColumnName{ id =5, Cname="Net"},
                    new DatatableColumnName{ id =6, Cname="Dist."},
                    new DatatableColumnName{ id =7, Cname="Tax %"},
                    new DatatableColumnName{ id =8, Cname="Tax"},
                    new DatatableColumnName{ id =9, Cname="Total"},

                    new DatatableColumnName{ id =10, Cname="Cash"},
                    new DatatableColumnName{ id =11, Cname="Card"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(detailSaleSummaryModels), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "MasterSale")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Date",Width=80, DataType="Date"},
                    new DatatableColumnName{ id =2, Cname="Time", Width=60, DataType="String"},
                    new DatatableColumnName{ id =3, Cname="Invoice#", DataType="String"},
                    new DatatableColumnName{ id =4, Cname="Product", DataType="String", Width=120},
                    new DatatableColumnName{ id =5, Cname="Rate"},
                    new DatatableColumnName{ id =6, Cname="Qty"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Discount"},
                    new DatatableColumnName{ id =9, Cname="Tax"},
                    new DatatableColumnName{ id =10, Cname="Gross"},
                    new DatatableColumnName{ id =11, Cname="Category"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(masterSalesReportModels), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "SalesByCategoryProductQtyDesc" || _reportName == "SalesByCategoryProductQtyAsc" || _reportName == "SalesByCategoryProductAmountDesc" || _reportName == "SalesBySectionCategoryProductAmountAsc"
                            || _reportName == "SalesBySectionCategoryProductAmountDesc" || _reportName == "SalesBySectionCategoryProductQtyAsc" || _reportName == "SalesBySectionCategoryProductQtyDesc"
                            || _reportName == "SalesBySectionCategory" || _reportName == "SalesBySectionProductAmountDesc" || _reportName == "SalesBySectionProductQtyDesc"
                            )
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Section", DataType="String"},
                    new DatatableColumnName{ id =2, Cname="Category", DataType="String",Width=120},
                    new DatatableColumnName{ id =2, Cname="Product", DataType="String",Width=120},
                    new DatatableColumnName{ id =4, Cname="Rate"},
                    new DatatableColumnName{ id =5, Cname="Qty"},
                    new DatatableColumnName{ id =6, Cname="Amount"},
                    new DatatableColumnName{ id =7, Cname="Disc."},
                    new DatatableColumnName{ id =8, Cname="Tax"},
                    new DatatableColumnName{ id =9, Cname="Total"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(salesByCategoryProductModel), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "TableStatistics")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Table Name", DataType="String", Width=300},
                    new DatatableColumnName{ id =2, Cname="Actual Capacity"},
                    new DatatableColumnName{ id =3, Cname="Expected Occupancy"},
                    new DatatableColumnName{ id =4, Cname="Occupancy"},
                    new DatatableColumnName{ id =5, Cname="Occupancy %"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(tableStatisticsModel), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "SalesSummarybyProductCategory")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Category", DataType="String", Width=175},
                    new DatatableColumnName{ id =2, Cname="Qty"},
                    new DatatableColumnName{ id =3, Cname="Net Sales"},
                    new DatatableColumnName{ id =4, Cname="Discount"},
                    new DatatableColumnName{ id =5, Cname="Tax"},
                    new DatatableColumnName{ id =6, Cname="Total"},
                    new DatatableColumnName{ id =7, Cname="Value %"},
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(salesSummaryModel), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "SalesSummarybyProduct")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Category", DataType="String", Width=150},
                    new DatatableColumnName{ id =2, Cname="Product", DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Qty"},
                    new DatatableColumnName{ id =4, Cname="Net Sales"},
                    new DatatableColumnName{ id =5, Cname="Discount"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"},
                    new DatatableColumnName{ id =8, Cname="Value %"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(salesSummaryByFoodCategoryFoodMenuModel), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "SalesSummarybySection")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =1, Cname="Section", DataType="String", Width=150},
                    new DatatableColumnName{ id =2, Cname="Date", DataType="String", Width=80},
                    new DatatableColumnName{ id =3, Cname="Inv Count"},
                    new DatatableColumnName{ id =4, Cname="Net Sales"},
                    new DatatableColumnName{ id =5, Cname="Discount"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Total"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(salesSummaryBySectionModel), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "CustomerReward")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Customer Name",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Phone Number",DataType="String", Width=100},
                    new DatatableColumnName{ id =4, Cname="Date",DataType="Date", Width=160},
                    new DatatableColumnName{ id =5, Cname="Credit"},
                    new DatatableColumnName{ id =6, Cname="Debit"},
                    new DatatableColumnName{ id =7, Cname="Balance"}
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(customerRewardModel), datatableColumnNames, path, firstLine);
                }
                else if (_reportName == "SalesSummaryFiveWeeks")
                {
                    datatableColumnNames = new List<DatatableColumnName>
                {
                    new DatatableColumnName{ id =2, Cname="Week",DataType="String", Width=150},
                    new DatatableColumnName{ id =3, Cname="Inv Count"},
                    new DatatableColumnName{ id =4, Cname="Net Sales"},
                    new DatatableColumnName{ id =5, Cname="Discount"},
                    new DatatableColumnName{ id =6, Cname="Tax"},
                    new DatatableColumnName{ id =7, Cname="Gross"},
                };
                    exportExcel.ExportExcelFile(commonMethods.ConvertToDataTable(salesSummaryByWeek), datatableColumnNames, path, firstLine);
                }
            }
        }
        void Print(FlowDocument flowDocument)
        {

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;

            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
            flowDocument.PageWidth = printDialog.PrintableAreaWidth;

            //var elementWithName = (Paragraph)flowDocument.FindResource("headParagraph");
            //elementWithName.Name = null;

            IDocumentPaginatorSource idocument = flowDocument as IDocumentPaginatorSource;

            printDialog.PrintDocument(idocument.DocumentPaginator, "Printing ...");


            //PrintDialog printDialog = new PrintDialog();
            //bool? result = printDialog.ShowDialog();
            //if (!result.HasValue)
            //    return;
            //if (!result.Value)
            //    return;

            //double pageWidth = printDialog.PrintableAreaWidth;
            //double pageHeight = printDialog.PrintableAreaHeight;
            //flowDocument = CreateFlowDocument(pageWidth, pageHeight);

            //printDialog.PrintDocument(
            // ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
            // "Test print job");
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
        private bool DateValidated(string fromDate, string toDate)
        {
            DateTime _fromDate = Convert.ToDateTime(fromDate);// ((System.Windows.Controls.DatePicker)(fromDate)).SelectedDate;
            DateTime _toDate = Convert.ToDateTime(toDate);// ((System.Windows.Controls.DatePicker)(toDate)).SelectedDate;

            if (_fromDate > _toDate)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
