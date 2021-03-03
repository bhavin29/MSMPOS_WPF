using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using RocketPOS.Core.Constants;

// By @naadydev
//    naadydev@gmail.com 
//    http://blog.mohnady.com
// *** You should modify this class by adding your business validation & Exception Handling 
// *** Line 98 -> Global formatting properties for the table, you can modify it to meet your design
// ============== References & More Information =====================
// https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/flow-document-overview
// https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/how-to-build-a-table-programmatically
// https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/table-overview
// ==================================================================

namespace RocketPOS.Helpers.Reports.WPFPrintHelper
{
    /// <summary>
    /// Utility Class For printing Datatable 
    /// </summary>
    public class WPFPrintUtility
    {
        #region How To Use
        //
        // -------------- In XAML -------------------------
        // <FlowDocumentReader>
        //    <FlowDocument Name = "flowDocument" ColumnWidth= "999999" >
        //        < Paragraph Name= "headParagraph" FontSize= "20" FontWeight= "Bold" > Header Paragraph</Paragraph>
        //        <Paragraph Name = "ndHeadParagraph" FontSize= "15" > nd Header Paragraph</Paragraph>
        //    </FlowDocument>
        //</FlowDocumentReader>
        //<Button Name = "printButton" Content= "Print" Click= "printButton_Click" />
        // -------------- In Code Behind ------------------
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DataTable mockDataTable = wPFPrintHelper.CreateMockDataTableForTest();
        //    wPFPrintHelper.CreateAndVisualizeDataTable(flowDocument, mockDataTable, "2017 Sales Project", "My Footer");
        //}

        //private void printButton_Click(object sender, RoutedEventArgs e)
        //{
        //    wPFPrintHelper.Print(flowDocument);
        //} 
        // -------------- Global Formating -----------------
        //DataTable mockDataTable = wPFPrintUtility.CreateMockDataTableForTest();
        //GlobalFormating.DataRowsEvenBrush = Brushes.AliceBlue;
        //GlobalFormating.TableTitleBackground = Brushes.BurlyWood;            
        //wPFPrintUtility.CreateAndVisualizeDataTable(flowDocument, mockDataTable, "2017 Sales Project", "My Footer");

        // ------------- Overloading-------------------------
        //Sure you should replace this dataTable by your actual datatable.
        //DataTable mockDataTable = wPFPrintUtility.CreateMockDataTableForTest();
        //GlobalFormating.DataRowsEvenBrush = Brushes.AliceBlue;
        //wPFPrintUtility.CreateAndVisualizeDataTable(flowDocument, mockDataTable, "2017 Sales Project", "My Footer");
        // --------------- OR ----------------------
        //Sure you should replace this dataTable by your actual datatable.
        //DataTable mockDataTable = wPFPrintUtility.CreateMockDataTableForTest();
        //GlobalFormating.DataRowsEvenBrush = Brushes.AliceBlue;
        //Table table = wPFPrintUtility.CreateAndVisualizeDataTable(mockDataTable, "2017 Sales Project", "My Footer");
        //flowDocument.Blocks.Add(table);
        // --------------- OR -----------------------
        //Sure you should replace this dataTable by your actual datatable.
        //DataTable mockDataTable = wPFPrintUtility.CreateMockDataTableForTest();                   
        //wPFPrintUtility.Datatable = mockDataTable;
        //wPFPrintUtility.HeaderTitleText = "2017 Sales Project";
        //wPFPrintUtility.FooterText = "My Footer";
        //GlobalFormating.DataRowsEvenBrush = Brushes.AliceBlue;
        //Table table = wPFPrintUtility.CreateAndVisualizeDataTable();
        //flowDocument.Blocks.Add(table);
        #endregion

        #region Class Fields
        private Table _winTable;
        private TableRow _winTableRow;
        #endregion

        #region Properties
        public string HeaderTitleText { get; set; }
        public string FooterText { get; set; }
        public DataTable Datatable { get; set; }
        #endregion

        #region Generate DataTable Just for Test
        public DataTable CreateMockDataTableForTest()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Product");
            dt.Columns.Add("Quarter1");
            dt.Columns.Add("Quarter2");
            dt.Columns.Add("Quarter3");
            dt.Columns.Add("Quarter4");
            dt.Columns.Add("TOTAL");
            // -------------------------------
            DataRow dr1 = dt.NewRow();
            dr1["Product"] = "Widgets";
            dr1["Quarter1"] = "$1,000";
            dr1["Quarter2"] = "$2,000";
            dr1["Quarter3"] = "$3,000";
            dr1["Quarter4"] = "$4,000";
            dr1["TOTAL"] = "$10,000";
            // -------------------------------
            DataRow dr2 = dt.NewRow();
            dr2["Product"] = "TV";
            dr2["Quarter1"] = "$3,000";
            dr2["Quarter2"] = "$3,000";
            dr2["Quarter3"] = "$2,000";
            dr2["Quarter4"] = "$1,000";
            dr2["TOTAL"] = "$9,000";
            // -------------------------------
            DataRow dr3 = dt.NewRow();
            dr3["Product"] = "AC";
            dr3["Quarter1"] = "$4,000";
            dr3["Quarter2"] = "$4,000";
            dr3["Quarter3"] = "$1,000";
            dr3["Quarter4"] = "$1,000";
            dr3["TOTAL"] = "$10,000";
            // -------------------------------

            dt.Rows.Add(dr1);
            dt.Rows.Add(dr2);
            dt.Rows.Add(dr3);

            return dt;
        }
        #endregion

        #region CreateAndVisualizeDataTable Overloading
        /// <summary>
        /// Create And Visualize The DataTable
        /// </summary>
        /// <returns>System.Windows.Documents.Table</returns>
        public Table CreateAndVisualizeDataTable()
        {
            return CreateVisualTableFromDataTable();
        }

        /// <summary>
        /// Create And Visualize The DataTable
        /// </summary>
        /// <param name="flowDocument"></param>
        public void CreateAndVisualizeDataTable(FlowDocument flowDocument)
        {
            flowDocument.Blocks.Add(CreateVisualTableFromDataTable());
        }

        /// <summary>
        /// Create And Visualize The DataTable
        /// </summary>
        /// <param name="flowDocument"></param>
        /// <param name="datatable"></param>
        /// <param name="headerTitle"></param>
        /// <param name="footer"></param>
        public void CreateAndVisualizeDataTable(FlowDocument flowDocument, DataTable datatable, string headerTitle, string footer)
        {
            var section = new Section();
            section.LineHeight = Double.NaN;

            var pHeader = new Paragraph();
            pHeader.Inlines.Add(new Run(LoginDetail.ClientName));
            pHeader.LineHeight = Double.NaN;
           flowDocument.Blocks.Add(pHeader);
         //   section.Blocks.Add(pHeader);

            var pndHeader = new Paragraph();
            pndHeader.Inlines.Add(new Run(LoginDetail.Address2));
            pndHeader.LineHeight = Double.NaN;
            flowDocument.Blocks.Add(pndHeader);
            //section.Blocks.Add(pndHeader);
          //  flowDocument.Blocks.Add(section);

            HeaderTitleText = headerTitle;
            FooterText = footer;
            Datatable = datatable;
            flowDocument.Blocks.Add(CreateVisualTableFromDataTable());
        }

        /// <summary>
        /// Create And Visualize The DataTable
        /// </summary>
        /// <param name="datatable"></param>
        /// <param name="headerTitle"></param>
        /// <param name="footer"></param>
        /// <returns>System.Windows.Documents.Table</returns>
        public Table CreateAndVisualizeDataTable(DataTable datatable, string headerTitle, string footer)
        {
    
            
            HeaderTitleText = headerTitle;
            FooterText = footer;
            Datatable = datatable;
            return CreateVisualTableFromDataTable();
        }
        #endregion

        /// <summary>
        /// Create Visual Table From DataTable
        /// </summary>
        /// <returns>Table</returns>
        private Table CreateVisualTableFromDataTable()
        {
            // Create the Table
            _winTable = new Table();

            // Global formatting properties for the table, you can modify it to meet your design 
            _winTable.CellSpacing = GlobalFormating.TableCellSpacing;
            _winTable.Background = GlobalFormating.TableBackground;
            _winTable.BorderThickness = GlobalFormating.TableBorderThickness;
            _winTable.BorderBrush = GlobalFormating.TableBorderBrush;
            _winTable.FontFamily = new FontFamily(GlobalFormating.Fontname);

            // Create Columns
            CreateColumns();
            // Create Rows
            CreateRows();

            return _winTable;
        }

        /// <summary>
        /// Create The Columns
        /// </summary>
        /// <param name="table">Table</param>
        private void CreateColumns()
        {
            for (int x = 0; x < Datatable.Columns.Count; x++)
            {
                _winTable.Columns.Add(new TableColumn());

                // (odd even) background colors.
                if (x % 2 == 0)
                    _winTable.Columns[x].Background = GlobalFormating.TableOddColumnBrush;
                else
                    _winTable.Columns[x].Background = GlobalFormating.TableEvenColumnBrush;
            }
        }

        /// <summary>
        /// Create Rows
        /// </summary>
        /// <param name="table"></param>
        private void CreateRows()
        {
            // Create an empty Row Group to hold the table's Rows.
            _winTable.RowGroups.Add(new TableRowGroup());
            _winTable.RowGroups[0].Rows.Add(new TableRow());
            _winTableRow = _winTable.RowGroups[0].Rows[0];
            // ---------------------------
            // Add the first Row (title) if exists.
            if (!string.IsNullOrEmpty(this.HeaderTitleText)) CreateTableTitleRow();
            // ==========================================
            CreateColumnsHeaderRow();
            // ============================================
            CreateTableDataRows();
            // ===================================================

           //  Craete Table Footer Row Text
              if (!string.IsNullOrEmpty(this.FooterText)) CreateFooter(Datatable.Rows.Count);
        }

        /// <summary>
        /// Create Table Title Row
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private void CreateTableTitleRow()
        {
            // Global formatting for the title row.
            _winTableRow.Background = GlobalFormating.TableTitleBackground;
            _winTableRow.FontSize = GlobalFormating.TableTitleFontSize;
            _winTableRow.FontWeight = GlobalFormating.TableTitleFontWeight;
            _winTableRow.FontFamily = new FontFamily(GlobalFormating.Fontname);

            // Add the header row with content, 
            _winTableRow.Cells.Add(new TableCell(new Paragraph(new Run(HeaderTitleText))));
            // and set the row to span all columns.
            _winTableRow.Cells[0].ColumnSpan = GlobalFormating.TableTitleColumnSpan;
            _winTableRow.Cells[0].TextAlignment = TextAlignment.Center;
        }

        /// <summary>
        /// Create Columns Header Row
        /// </summary>
        /// <param name="table"></param>
        /// <param name="currentRow"></param>
        private void CreateColumnsHeaderRow()
        {
            // Add the second (header) row.
            _winTable.RowGroups[0].Rows.Add(new TableRow());
            _winTableRow = _winTable.RowGroups[0].Rows[1];

            // Global formatting for the header row.
            _winTableRow.FontSize = GlobalFormating.ColumnsHeaderRowFontSize;
            _winTableRow.FontWeight = GlobalFormating.ColumnsHeaderRowFontWeight;

            // Add cells with content to the second row.
            // Columns Header
            for (int i = 0; i < Datatable.Columns.Count; i++)
            {
                _winTableRow.Cells.Add(new TableCell(new Paragraph(new Run(Datatable.Columns[i].ColumnName))));
                _winTableRow.Cells[i].TextAlignment = TextAlignment.Center;
            }
        }

        /// <summary>
        /// Create Table Data Rows
        /// </summary>
        /// <param name="table"></param>
        /// <param name="currentRow"></param>
        private void CreateTableDataRows()
        {
            int i = 0, j = 2;
            for (; i < Datatable.Rows.Count; i++, j++)
            {
                // Add the row.
                _winTable.RowGroups[0].Rows.Add(new TableRow());
                _winTableRow = _winTable.RowGroups[0].Rows[j]; // 2

                // Global formatting for the row.
                _winTableRow.FontSize = GlobalFormating.DataRowsFontSize;
                _winTableRow.FontWeight = GlobalFormating.DataRowsFontWeight;

                //------------------
                // (odd even) background colors.
                if ((i + 2) % 2 == 0)
                    _winTableRow.Background = GlobalFormating.DataRowsOddBrush;
                else
                    _winTableRow.Background = GlobalFormating.DataRowsEvenBrush;
                //-------------------
                // Add cells with content to the  row
                for (int x = 0; x < Datatable.Columns.Count; x++)
                {
                    _winTableRow.Cells.Add(new TableCell(new Paragraph(new Run(Datatable.Rows[i][x].ToString()))));

                    if (x == 1 || x == 2 || x == 3|| x == 4|| x == 5)
                        _winTableRow.Cells[x].TextAlignment = TextAlignment.Right;
                }

                //    if ( (i%21)==0 && i!=0)
                //    {
                //        CreateColumns();

                //        // Add the row.
                //        _winTable.RowGroups[0].Rows.Add(new TableRow());
                //        _winTableRow = _winTable.RowGroups[0].Rows[j + 1];

                //        // Global formatting for the row.
                //        _winTableRow.FontSize = GlobalFormating.ColumnsHeaderRowFontSize;
                //        _winTableRow.FontWeight = GlobalFormating.ColumnsHeaderRowFontWeight;

                //        // Add cells with content to the  row
                //        for (int x = 0; x < Datatable.Columns.Count; x++)
                //        {
                //            _winTableRow.Cells.Add(new TableCell(new Paragraph(new Run(Datatable.Columns[x].ColumnName))));

                //            if (x % 2 == 0)
                //                _winTableRow.Cells[x].Background = GlobalFormating.TableOddColumnBrush;
                //            else
                //                _winTableRow.Cells[x].Background = GlobalFormating.TableEvenColumnBrush;
                //        }
                //        j = j + 1;
                //    }
            }
        }

        /// <summary>
        /// Create Footer
        /// </summary>
        /// <param name="table"></param>
        /// <param name="currentRow"></param>
        /// <param name="rowsCount"></param>
        private void CreateFooter(int rowsCount)
        {
            _winTable.RowGroups[0].Rows.Add(new TableRow());
            _winTableRow = _winTable.RowGroups[0].Rows[rowsCount + 2];

            // Global formatting for the footer row.
            _winTableRow.Background = GlobalFormating.TableFooterBackground;
            _winTableRow.FontSize = GlobalFormating.TableFooterFontSize;
            _winTableRow.FontWeight = GlobalFormating.TableFooterFontWeight;

            // Add the header row with content, 
            _winTableRow.Cells.Add(new TableCell(new Paragraph(new Run(FooterText))));
            // and set the row to span all columns.
            _winTableRow.Cells[0].ColumnSpan = GlobalFormating.TableFooterColumnSpan;
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="flowDocument"></param>
        public void Print(FlowDocument flowDocument)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;

            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
            flowDocument.PageWidth = printDialog.PrintableAreaWidth;

            //var elementWithName = (Paragraph)flowDocument.FindResource("headParagraph");
            //elementWithName.Name = null;

            IDocumentPaginatorSource idocument = flowDocument as IDocumentPaginatorSource;

            printDialog.PrintDocument(idocument.DocumentPaginator, "Printing ...");

            flowDocument.PageHeight = 800;
            flowDocument.PageWidth = 816;
        }
    }

    /// <summary>
    /// Global UI / look & feel formating for the Table
    /// </summary>
    public class GlobalFormating
    {
        // -------------
        public static double TableCellSpacing { get; set; } = 10;
        public static Brush TableBackground { get; set; } = Brushes.White;
        public static Thickness TableBorderThickness { get; set; } = new Thickness() { Top = 1, Left = 1, Bottom = 1, Right = 1 };
        public static Brush TableBorderBrush { get; set; } = Brushes.Black;
        // -------------
        public static Brush TableOddColumnBrush { get; set; } = Brushes.Beige;
        public static Brush TableEvenColumnBrush { get; set; } = Brushes.WhiteSmoke;
        // -------------
        public static int TableTitleColumnSpan { get; set; } = 6;
        public static Brush TableTitleBackground { get; set; } = Brushes.Silver;
        public static double TableTitleFontSize { get; set; } = 18;
        public static FontWeight TableTitleFontWeight { get; set; } = System.Windows.FontWeights.Bold;
        // -------------
        public static int TableFooterColumnSpan { get; set; } = 6;
        public static Brush TableFooterBackground { get; set; } = Brushes.LightGray;
        public static double TableFooterFontSize { get; set; } = 18;
        public static FontWeight TableFooterFontWeight { get; set; } = System.Windows.FontWeights.Normal;
        // -------------
        public static double ColumnsHeaderRowFontSize { get; set; } = 16;
        public static FontWeight ColumnsHeaderRowFontWeight { get; set; } = System.Windows.FontWeights.Bold;
        // -------------
        public static double DataRowsFontSize { get; set; } = 14;
        public static FontWeight DataRowsFontWeight { get; set; } = System.Windows.FontWeights.Normal;
        public static Brush DataRowsOddBrush { get; set; } = Brushes.White;
        public static Brush DataRowsEvenBrush { get; set; } = Brushes.WhiteSmoke;
        public static string Fontname { get; set; } = "Calibri";
    }
}