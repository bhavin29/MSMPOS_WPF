using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Linq;

namespace RocketPOS.Core.Constants
{
    public class CommonMethods
    {
        public DataTable ConvertToDataTable<T>(List<T> models)
        {
            // creating a data table instance and typed it as our incoming model   
            // as I make it generic, if you want, you can make it the model typed you want.  
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties of that model  
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Loop through all the properties              
            // Adding Column name to our datatable  
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names    
                dataTable.Columns.Add(prop.Name);
            }
            // Adding Row and its value to our dataTable  
            foreach (T item in models)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows    
                    values[i] = Props[i].GetValue(item, null);
                }
                // Finally add value to datatable    
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public void WriteExcelFile(DataTable table, string path, string firstLine)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = document.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                WorkbookStylesPart stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                SheetData sheetData = new SheetData();

                //add a row
                Row firstRow = new Row();
                //firstRow.RowIndex = (UInt32)1;

                //create a cell in C1 (the upper left most cell of the merged cells)
                Cell dataCell = new Cell();
                dataCell.CellReference = "A1";
                dataCell.DataType = CellValues.String;
                CellValue cellValue = new CellValue();
                cellValue.Text = firstLine;
                
                dataCell.Append(cellValue);

                firstRow.AppendChild(dataCell);

                sheetData.AppendChild(firstRow);
                // Add a WorkbookPart to the document.
                worksheetPart.Worksheet = new Worksheet(sheetData);

                //create a MergeCells class to hold each MergeCell
                MergeCells mergeCells = new MergeCells();

                //append a MergeCell to the mergeCells for each set of merged cells
                mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:J1") });

                worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                //this is the part that was missing from your code
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.AppendChild(new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(document.WorkbookPart.WorksheetParts.First()),
                    SheetId = 1,
                    Name = "Sheet1"
                });

                Row headerRow = new Row();
                List<String> columns = new List<string>();
             
                //skip//
                    foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);
                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        if (col == "GrossAmount" || col == "DiscountAmount" || col == "DeliveryCharges" || col == "TaxAmount" || col == "TotalPayable" || col == "RegisterValue")
                        {
                            cell.DataType =CellValues.Number;
                            cell.StyleIndex = 3;
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                        }
                        cell.CellValue = new CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(newRow);
                }
                workbookpart.Workbook.Save();
            }
        }

        public Stylesheet GenerateStyleSheet()
        {
            Stylesheet ss = new Stylesheet();

            Fonts fts = new Fonts();
            DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontName ftn = new FontName();
            ftn.Val = "Calibri";
            FontSize ftsz = new FontSize();
            ftsz.Val = 11;
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            fts.Append(ft);
            fts.Count = (uint)fts.ChildElements.Count;

            Fills fills = new Fills();
            Fill fill;
            PatternFill patternFill;
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.None;
            fill.PatternFill = patternFill;
            fills.Append(fill);
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Gray125;
            fill.PatternFill = patternFill;
            fills.Append(fill);
            fills.Count = (uint)fills.ChildElements.Count;

            Borders borders = new Borders();
            Border border = new Border();
            border.LeftBorder = new LeftBorder();
            border.RightBorder = new RightBorder();
            border.TopBorder = new TopBorder();
            border.BottomBorder = new BottomBorder();
            border.DiagonalBorder = new DiagonalBorder();
            borders.Append(border);
            borders.Count = (uint)borders.ChildElements.Count;

            CellStyleFormats csfs = new CellStyleFormats();
            CellFormat cf = new CellFormat();
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            csfs.Append(cf);
            csfs.Count = (uint)csfs.ChildElements.Count;

            uint iExcelIndex = 164;
            NumberingFormats nfs = new NumberingFormats();
            CellFormats cfs = new CellFormats();

            cf = new CellFormat();
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cfs.Append(cf);

            NumberingFormat nf;
            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "dd/mm/yyyy hh:mm:ss";
            nfs.Append(nf);
            cf = new CellFormat();
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "#,##0.0000";
            nfs.Append(nf);
            cf = new CellFormat();
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            // #,##0.00 is also Excel style index 4
            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "#,##0.00";
            nfs.Append(nf);
            cf = new CellFormat();
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            // @ is also Excel style index 49
            nf = new NumberingFormat();
            nf.NumberFormatId = iExcelIndex++;
            nf.FormatCode = "@";
            nfs.Append(nf);
            cf = new CellFormat();
            cf.NumberFormatId = nf.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = true;
            cfs.Append(cf);

            nfs.Count = (uint)nfs.ChildElements.Count;
            cfs.Count = (uint)cfs.ChildElements.Count;

            ss.Append(nfs);
            ss.Append(fts);
            ss.Append(fills);
            ss.Append(borders);
            ss.Append(csfs);
            ss.Append(cfs);

            CellStyles css = new CellStyles();
            CellStyle cs = new CellStyle();
            cs.Name = "Normal";
            cs.FormatId = 0;
            cs.BuiltinId = 0;
            css.Append(cs);
            css.Count = (uint)css.ChildElements.Count;
            ss.Append(css);

            DifferentialFormats dfs = new DifferentialFormats();
            dfs.Count = 0;
            ss.Append(dfs);

            TableStyles tss = new TableStyles();
            tss.Count = 0;
            tss.DefaultTableStyle = "TableStyleMedium9";
            tss.DefaultPivotStyle = "PivotStyleLight16";
            ss.Append(tss);

            return ss;
        }

        public static readonly string DateFormat = "dd/MM/yyyy";

        public void WriteCessExcelFile(DataTable cessSummary, DataTable cessDetail, string path, string firstLine)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = document.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                // Begin: Code block for Excel sheet 1  // Cess Summary
                WorksheetPart worksheetPart1 = workbookpart.AddNewPart<WorksheetPart>();
                Worksheet workSheet1 = new Worksheet();
                SheetData sheetData1 = new SheetData();

                WorkbookStylesPart stylesPart1 = workbookpart.AddNewPart<WorkbookStylesPart>();
                stylesPart1.Stylesheet = GenerateStyleSheet();
                stylesPart1.Stylesheet.Save();

                // the data for sheet 1 // Cess Summary
                Row firstRow1 = new Row();
                Cell dataCell1 = new Cell();
                dataCell1.CellReference = "A1";
                dataCell1.DataType = CellValues.String;

                CellValue cellValue1 = new CellValue();
                cellValue1.Text = firstLine;
                dataCell1.Append(cellValue1);
                firstRow1.AppendChild(dataCell1);
                sheetData1.AppendChild(firstRow1);
                workSheet1.AppendChild(sheetData1);
                worksheetPart1.Worksheet = workSheet1;

                MergeCells mergeCells1 = new MergeCells();
                mergeCells1.Append(new MergeCell() { Reference = new StringValue("A1:G1") });
                worksheetPart1.Worksheet.InsertAfter(mergeCells1, worksheetPart1.Worksheet.Elements<SheetData>().First());

                Row headerRow1 = new Row();
                List<String> columns1 = new List<string>();
                foreach (System.Data.DataColumn column in cessSummary.Columns)
                {
                    columns1.Add(column.ColumnName);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow1.AppendChild(cell);
                }

                sheetData1.AppendChild(headerRow1);
                foreach (DataRow dsrow1 in cessSummary.Rows)
                {
                    Row newRow1 = new Row();
                    foreach (String col in columns1)
                    {
                        Cell cell = new Cell();
                        if (col == "NetSales" || col == "Vatable" || col == "NonVatable" || col == "TotalTax" || col == "GrandTotal" || col == "CateringLevy")
                        {
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = 3;
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                        }
                        cell.CellValue = new CellValue(dsrow1[col].ToString());
                        newRow1.AppendChild(cell);
                    }
                    sheetData1.AppendChild(newRow1);
                }

                Sheet sheets1 = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart1),
                    SheetId = 1,
                    Name = "Summary"
                };
                sheets.Append(sheets1);
                // End: Code block for Excel sheet 1 // Cess Summary

                // Begin: Code block for Excel sheet 2 // Cess Details
                WorksheetPart worksheetPart2 = workbookpart.AddNewPart<WorksheetPart>();
                Worksheet workSheet2 = new Worksheet();
                SheetData sheetData2 = new SheetData();

                // the data for sheet 1 // Cess Details
                Row firstRow2 = new Row();
                Cell dataCell2 = new Cell();
                dataCell2.CellReference = "A1";
                dataCell2.DataType = CellValues.String;

                CellValue cellValue2 = new CellValue();
                cellValue2.Text = firstLine;
                dataCell2.Append(cellValue2);
                firstRow2.AppendChild(dataCell2);
                sheetData2.AppendChild(firstRow2);
                workSheet2.AppendChild(sheetData2);
                worksheetPart2.Worksheet = workSheet2;

                MergeCells mergeCells2 = new MergeCells();
                mergeCells2.Append(new MergeCell() { Reference = new StringValue("A1:G1") });
                worksheetPart2.Worksheet.InsertAfter(mergeCells2, worksheetPart2.Worksheet.Elements<SheetData>().First());

                Row headerRow2= new Row();
                List<String> columns2 = new List<string>();
                foreach (System.Data.DataColumn column in cessDetail.Columns)
                {
                    columns2.Add(column.ColumnName);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow2.AppendChild(cell);
                }

                sheetData2.AppendChild(headerRow2);
                foreach (DataRow dsrow2 in cessDetail.Rows)
                {
                    Row newRow2 = new Row();
                    foreach (String col in columns2)
                    {
                        Cell cell = new Cell();
                        if (col == "NetSales" || col == "Vatable" || col == "NonVatable" || col == "TotalTax" || col == "GrandTotal" || col== "CateringLevy")
                        {
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = 3;
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                        }
                        cell.CellValue = new CellValue(dsrow2[col].ToString());
                        newRow2.AppendChild(cell);
                    }
                    sheetData2.AppendChild(newRow2);
                }

                Sheet sheets2 = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart2),
                    SheetId = 2,
                    Name = "Detail"
                };
                sheets.Append(sheets2);
                // End: Code block for Excel sheet 1 // Cess Details
                workbookpart.Workbook.Save();
            }
        }

        public void WriteExcelDetailDailySalesFile(DataTable table, string path, string firstLine)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = document.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                WorkbookStylesPart stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                SheetData sheetData = new SheetData();

                //add a row
                Row firstRow = new Row();
                //firstRow.RowIndex = (UInt32)1;

                //create a cell in C1 (the upper left most cell of the merged cells)

                //Cell dataCell = new Cell();
                //dataCell.CellReference = "A1";
                //dataCell.DataType = CellValues.String;
                //CellValue cellValue = new CellValue();
                //cellValue.Text = firstLine;

                //dataCell.Append(cellValue);

                //firstRow.AppendChild(dataCell);

                //sheetData.AppendChild(firstRow);
               
                // Add a WorkbookPart to the document.
                worksheetPart.Worksheet = new Worksheet(sheetData);

                //create a MergeCells class to hold each MergeCell
                //MergeCells mergeCells = new MergeCells();

                //append a MergeCell to the mergeCells for each set of merged cells
                //mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:J1") });

                //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                //this is the part that was missing from your code
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
             
                sheets.AppendChild(new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(document.WorkbookPart.WorksheetParts.First()),
                    SheetId = 1,
                    Name = "Datailed Daily Report"
                });

                Row headerRow = new Row();
                List<String> columns = new List<string>();

                //Hide Coulumn name//
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(null);// (column.ColumnName);
                    headerRow.AppendChild(cell);
                }

                //sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    if (!dsrow[0].ToString().Contains("=="))
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            if (col == "GrossAmount" || col == "DiscountAmount" || col == "DeliveryCharges" || col == "TaxAmount" || col == "TotalPayable" || col == "RegisterValue")
                            {
                                cell.DataType = CellValues.Number;
                                cell.StyleIndex = 3;
                            }
                            else
                            {
                                cell.DataType = CellValues.String;
                            }
                            cell.CellValue = new CellValue(dsrow[col].ToString());

                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }
                workbookpart.Workbook.Save();
            }
        }

        public void WriteExcelModeOfPaymentFile(DataTable table, string path, string firstLine)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = document.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                WorkbookStylesPart stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                SheetData sheetData = new SheetData();

                //add a row
                Row firstRow = new Row();
                //firstRow.RowIndex = (UInt32)1;

                //create a cell in C1 (the upper left most cell of the merged cells)
                Cell dataCell = new Cell();
                dataCell.CellReference = "A1";
                dataCell.DataType = CellValues.String;
                CellValue cellValue = new CellValue();
                cellValue.Text = firstLine;

                dataCell.Append(cellValue);

                firstRow.AppendChild(dataCell);

                sheetData.AppendChild(firstRow);
                // Add a WorkbookPart to the document.
                worksheetPart.Worksheet = new Worksheet(sheetData);

                //create a MergeCells class to hold each MergeCell
                MergeCells mergeCells = new MergeCells();

                //append a MergeCell to the mergeCells for each set of merged cells
                mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:J1") });

                worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                //this is the part that was missing from your code
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.AppendChild(new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(document.WorkbookPart.WorksheetParts.First()),
                    SheetId = 1,
                    Name = "Sheet1"
                });

                Row headerRow = new Row();
                List<String> columns = new List<string>();

                //skip//
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);
                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        if (col == " SALES" || col == "CASH" || col == "PAISA-I" || col == "CREDIT CARD" || col == "DEBIT CARD" || col == "CHQEUE")
                        {
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = 3;
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                        }
                        cell.CellValue = new CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(newRow);
                }
                workbookpart.Workbook.Save();
            }
        }

        /// <summary>
        /// Gets a Inverted DataTable
        /// </summary>
        /// <param name="table">Provided DataTable</param>
        /// <param name="columnX">X Axis Column</param>
        /// <param name="columnY">Y Axis Column</param>
        /// <param name="columnZ">Z Axis Column (values)</param>
        /// <param name="columnsToIgnore">Whether to ignore some column, it must be 
        /// provided here</param>
        /// <param name="nullValue">null Values to be filled</param> 
        /// <returns>C# Pivot Table Method  - Felipe Sabino</returns>
        public DataTable GetInversedDataTable(DataTable table, string columnX,
             string columnY, string columnZ, string nullValue, bool sumValues)
        {
            //Create a DataTable to Return
            DataTable returnTable = new DataTable();

            if (columnX == "")
                columnX = table.Columns[0].ColumnName;

            //Add a Column at the beginning of the table
            returnTable.Columns.Add(columnY);


            //Read all DISTINCT values from columnX Column in the provided DataTale
            List<string> columnXValues = new List<string>();

            foreach (DataRow dr in table.Rows)
            {
                string columnXTemp = dr[columnX].ToString();
                if (!columnXValues.Contains(columnXTemp))
                {
                    //Read each row value, if it's different from others provided, add to 
                    //the list of values and creates a new Column with its value.
                    columnXValues.Add(columnXTemp);
                    returnTable.Columns.Add(columnXTemp);
                }
            }

            //Verify if Y and Z Axis columns re provided
            if (columnY != "" && columnZ != "")
            {
                //Read DISTINCT Values for Y Axis Column
                List<string> columnYValues = new List<string>();

                foreach (DataRow dr in table.Rows)
                {
                    if (!columnYValues.Contains(dr[columnY].ToString()))
                        columnYValues.Add(dr[columnY].ToString());
                }

                //Loop all Column Y Distinct Value
                foreach (string columnYValue in columnYValues)
                {
                    //Creates a new Row
                    DataRow drReturn = returnTable.NewRow();
                    drReturn[0] = columnYValue;
                    //foreach column Y value, The rows are selected distincted
                    DataRow[] rows = table.Select(columnY + "='" + columnYValue + "'");

                    //Read each row to fill the DataTable
                    foreach (DataRow dr in rows)
                    {
                        string rowColumnTitle = dr[columnX].ToString();

                        //Read each column to fill the DataTable
                        foreach (DataColumn dc in returnTable.Columns)
                        {
                            if (dc.ColumnName == rowColumnTitle)
                            {
                                //If Sum of Values is True it try to perform a Sum
                                //If sum is not possible due to value types, the value 
                                // displayed is the last one read
                                if (sumValues)
                                {
                                    try
                                    {
                                        drReturn[rowColumnTitle] =
                                             Convert.ToDecimal(drReturn[rowColumnTitle]) +
                                             Convert.ToDecimal(dr[columnZ]);
                                    }
                                    catch
                                    {
                                        drReturn[rowColumnTitle] = dr[columnZ];
                                    }
                                }
                                else
                                {
                                    drReturn[rowColumnTitle] = dr[columnZ];
                                }
                            }
                        }
                    }
                    returnTable.Rows.Add(drReturn);
                }
            }
            else
            {
                throw new Exception("The columns to perform inversion are not provided");
            }

            //if a nullValue is provided, fill the datable with it
            if (nullValue != "")
            {
                foreach (DataRow dr in returnTable.Rows)
                {
                    foreach (DataColumn dc in returnTable.Columns)
                    {
                        if (dr[dc.ColumnName].ToString() == "")
                            dr[dc.ColumnName] = nullValue;
                    }
                }
            }

            return returnTable;
        }

    }
}
