using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using System.Windows;

namespace RocketPOS.Views
{
    //https://stackoverflow.com/questions/28096578/writing-nice-receipt-in-c-sharp-wpf-for-printing-on-thermal-printer-pos

    public class ReceiptPrintView
    {

        private PrintDocument PrintDocument;
        private Graphics graphics;
        private int InitialHeight = 360;
        private int billId = 0;
        int pageWidthHeader = 50;

        public ReceiptPrintView()
        {
            //  this.order = order;
            //  this.shop = shop;
            AdjustHeight();
        }
        private void AdjustHeight()
        {
            var capacity = 5 * 1;// order.ItemTransactions.Capacity;
            InitialHeight += capacity;

            capacity = 5 * 1;// order.DealTransactions.Capacity;
            InitialHeight += capacity;
        }
        public void Print(string printername, int id)
        {
            billId = id;
            PrintDocument = new PrintDocument();

            PrintDocument.PrinterSettings.PrinterName = printername;

            PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
            PrintDocument.Print();
        }
        void DrawAtStart(string text, int Offset)
        {
            int startX = 10;
            int startY = 5;
            Font minifont = new Font("Arial", 5);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 5, startY + Offset);
        }
        //NEW
        void DrawAtStartCenter(string text, int Offset,int OffsetX)
        {
            //  int intPadding = 
            int startX = OffsetX + 25 + ((pageWidthHeader - text.Length) / 2) * 4;
            int startY = 5;
            Font minifont = new Font("Arial", 5);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 5, startY + Offset);
        }
        void InsertItem(string key, string value, int Offset)
        {
            Font minifont = new Font("Arial", 5);
            int startX = 10;
            int startY = 5;

            graphics.DrawString(key, minifont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);

            graphics.DrawString(value, minifont,
                     new SolidBrush(Color.Black), startX + 130, startY + Offset);
        }

        void InsertItemList(string key, string value, int OffsetY, int OffsetX)
        {
            Font minifont = new Font("Arial", 5);
            int startX = 10;
            int startY = 5;

            graphics.DrawString(key, minifont,
                         new SolidBrush(Color.Black), startX + OffsetX, startY + OffsetY);

        }

        void InsertHeaderStyleItem(string key, string value, int Offset)
        {
            int startX = 10;
            int startY = 5;
            Font itemfont = new Font("Arial", 6, FontStyle.Bold);

            graphics.DrawString(key, itemfont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);

            graphics.DrawString(value, itemfont,
                     new SolidBrush(Color.Black), startX + 130, startY + Offset);
        }
        void DrawLine(string text, Font font, int Offset, int xOffset)
        {
            int startX = 10;
            int startY = 5;
            graphics.DrawString(text, font,
                     new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
        }
        void DrawSimpleString(string text, Font font, int Offset, int xOffset)
        {
            int startX = 10;
            int startY = 5;
            graphics.DrawString(text, font,
                     new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
        }
        private void FormatPage(object sender, PrintPageEventArgs e)
        {
            graphics = e.Graphics;
            Font minifont = new Font("Arial", 5);
            Font itemfont = new Font("Arial", 6);
            Font smallfont = new Font("Arial", 12);
            Font mediumfont = new Font("Arial", 14);
            Font largefont = new Font("Arial", 16);

            int Offset = 10;
            int smallinc = 10, mediuminc = 12, largeinc = 17;

            //Getting Receipt data 
            List<PrintReceiptModel> printReceiptModel = new List<PrintReceiptModel>();
            List<PrintReceiptItemModel> printReceiptItemModel = new List<PrintReceiptItemModel>();

            //Parameter pass global Customer Order Id
            PrintReceiptViewModel printReceiptViewModel = new PrintReceiptViewModel();

            List<ReportOffsetModel> reportOffsetModels = new List<ReportOffsetModel>();

            printReceiptModel = printReceiptViewModel.GetPrintReceiptByBillId(billId);
            printReceiptItemModel = printReceiptViewModel.GetPrintReceiptItemByBillId(billId);
            reportOffsetModels = printReceiptViewModel.GetReportOffsetByReportName("R1");

            int intClientName = reportOffsetModels.Find(x => x.ReportColumn.Contains("ClientName")).ColumnOffset;
            int intHeader = reportOffsetModels.Find(x => x.ReportColumn.Contains("Header")).ColumnOffset;
            int intAddress1 = reportOffsetModels.Find(x => x.ReportColumn.Contains("Address1")).ColumnOffset;
            int intAddress2 = reportOffsetModels.Find(x => x.ReportColumn.Contains("Address2")).ColumnOffset;
            int intEmail = reportOffsetModels.Find(x => x.ReportColumn.Contains("Email")).ColumnOffset;
            int intPhone = reportOffsetModels.Find(x => x.ReportColumn.Contains("Phone")).ColumnOffset;
            int intDate = reportOffsetModels.Find(x => x.ReportColumn.Contains("Date")).ColumnOffset;
            int intItemHeader = reportOffsetModels.Find(x => x.ReportColumn.Contains("Item-header")).ColumnOffset;
            int intItemFoodMenuName = reportOffsetModels.Find(x => x.ReportColumn.Contains("Item-FoodMenuName")).ColumnOffset;
            int intItemFoodMenuRate = reportOffsetModels.Find(x => x.ReportColumn.Contains("Item-FoodMenuRate")).ColumnOffset;
            int intItemFoodMenuQty = reportOffsetModels.Find(x => x.ReportColumn.Contains("Item-FoodMenuQty")).ColumnOffset;
            int intItemPrice = reportOffsetModels.Find(x => x.ReportColumn.Contains("Item-Price")).ColumnOffset;
            int intGROSSTotal = reportOffsetModels.Find(x => x.ReportColumn.Contains("GROSSTotal")).ColumnOffset;
            int intVATABLE = reportOffsetModels.Find(x => x.ReportColumn.Contains("VATABLE")).ColumnOffset;
            int intDISCOUNT = reportOffsetModels.Find(x => x.ReportColumn.Contains("DISCOUNT")).ColumnOffset;
            int intDELIVERYCharge = reportOffsetModels.Find(x => x.ReportColumn.Contains("DELIVERYCharge")).ColumnOffset;
            int intTOTAL = reportOffsetModels.Find(x => x.ReportColumn.Contains("TOTAL")).ColumnOffset;
            int intPaid = reportOffsetModels.Find(x => x.ReportColumn.Contains("Paid")).ColumnOffset;
            int intFooter = reportOffsetModels.Find(x => x.ReportColumn.Contains("Footer")).ColumnOffset;
            int intFooter1 = reportOffsetModels.Find(x => x.ReportColumn.Contains("Footer1")).ColumnOffset;
            int intFooter2 = reportOffsetModels.Find(x => x.ReportColumn.Contains("Footer2")).ColumnOffset;
            int intFooter3 = reportOffsetModels.Find(x => x.ReportColumn.Contains("Footer3")).ColumnOffset;
            int intFooter4 = reportOffsetModels.Find(x => x.ReportColumn.Contains("Footer4")).ColumnOffset;

            //Image image = Image.FromFile("d:\\2.jpg");

            //e.Graphics.DrawImage(image, startX + 50, startY + Offset, 100, 30);
            // e.Graphics.DrawImage(image, 50, 10 + Offset, 100, 30);

            Offset = Offset + Offset;

            //Name
            int intPadding = 10 + ((pageWidthHeader - LoginDetail.ClientName.Length) / 2) * 4;

 
            graphics.DrawString(LoginDetail.ClientName, smallfont, new SolidBrush(Color.Black), intClientName, Offset);//50 + 22

            Offset = Offset + mediuminc;
            DrawAtStartCenter(LoginDetail.Header, Offset,intHeader);

            Offset = Offset + mediuminc;
            DrawAtStartCenter(LoginDetail.Address1, Offset,intAddress1);

            Offset = Offset + mediuminc;
            DrawAtStartCenter(LoginDetail.Address2, Offset,intAddress2);

            Offset = Offset + mediuminc;
            DrawAtStartCenter("Email : " + LoginDetail.Email, Offset,intEmail);

            Offset = Offset + mediuminc;
            DrawAtStartCenter("Phone : " + LoginDetail.Phone, Offset,intPhone);

            String underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + mediuminc + 6;
            DrawAtStart("Receipt Number: " + printReceiptModel[0].SalesInvoiceNumber.ToString().PadRight((intDate - printReceiptModel[0].SalesInvoiceNumber.ToString().Length) + 10) + "Date: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm"), Offset); ;

            Offset = Offset + mediuminc;
            DrawAtStart("Customer: " + printReceiptModel[0].CustomerName, Offset);

            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;

            InsertHeaderStyleItem("ITEM".PadRight(intItemHeader) +  "RATE     QTY    AMOUNT ", "", Offset);

            Offset = Offset + largeinc;

            foreach (var item in printReceiptItemModel)
            {
                InsertItemList(item.FoodMenuName.ToString(), "", Offset, intItemFoodMenuName);
                InsertItemList(item.FoodMenuRate.ToString("F"), "", Offset, intItemFoodMenuRate + (50 - (item.FoodMenuRate.ToString().Length * 4)));
                InsertItemList(item.FoodMenuQty.ToString("F"), "", Offset, intItemFoodMenuQty + (50 - (item.FoodMenuQty.ToString().Length * 4)));
                InsertItemList(item.Price.ToString("F") + " " + item.FoodVat.ToString(), "", Offset, intItemPrice + (50 - (item.Price.ToString().Length * 4)));
                Offset = Offset + smallinc;
            }
            Offset = Offset - smallinc;

            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;
            InsertItem("GROSS TOTAL: ", printReceiptModel[0].GrossAmount.ToString("F").PadLeft(intGROSSTotal - printReceiptModel[0].GrossAmount.ToString().Length), Offset);

            Offset = Offset + smallinc;
            InsertItem("VATABLE: ", printReceiptModel[0].VatableAmount.ToString("F").PadLeft(intVATABLE - printReceiptModel[0].VatableAmount.ToString().Length), Offset);
            
            Offset = Offset + smallinc;
            InsertItem("DISCOUNT: ", printReceiptModel[0].Discount.ToString("F").PadLeft(intDISCOUNT - printReceiptModel[0].Discount.ToString().Length), Offset);
 
            Offset = Offset + smallinc;
            InsertItem("DELIVERY CHARGE: ", printReceiptModel[0].ServiceCharge.ToString("F").PadLeft(intDELIVERYCharge - printReceiptModel[0].ServiceCharge.ToString().Length), Offset);

            Offset = Offset + smallinc;
            InsertItem("TOTAL: ", printReceiptModel[0].TotalAmount.ToString("F").PadLeft(intTOTAL - printReceiptModel[0].TotalAmount.ToString().Length), Offset);

            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;
            InsertItem("Paid: ", printReceiptModel[0].PaymentMethodName.ToString().PadRight(2) + " " + printReceiptModel[0].BillAmount.ToString("F").PadLeft(intPaid), Offset);

            Offset = Offset + largeinc;
            Offset = Offset + largeinc;

            intPadding = 10 + ((pageWidthHeader - LoginDetail.ClientName.Length) / 2) * 3;
            graphics.DrawString(LoginDetail.Footer, smallfont, new SolidBrush(Color.Black), intFooter, Offset);//50 + 22

            Offset = Offset + largeinc;

            DrawAtStartCenter(LoginDetail.Footer1, Offset,intFooter1);
            Offset = Offset + mediuminc;

            DrawAtStartCenter(LoginDetail.Footer2, Offset,intFooter2);
            Offset = Offset + mediuminc;

            DrawAtStartCenter(LoginDetail.Footer3, Offset,intFooter3);
            Offset = Offset + mediuminc;

            DrawAtStartCenter(LoginDetail.Footer4, Offset,intFooter4);
            Offset = Offset + mediuminc;
        }
    }
}
