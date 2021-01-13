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

    public class ReportDetailedDailyView
    {

        private PrintDocument PrintDocument;
        private Graphics graphics;
        private int InitialHeight = 360;
        private string FromDate,Todate ;
        int pageWidthHeader = 50;

        public ReportDetailedDailyView()
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

        public void Print(string printername, string fromDate, string toDate)
        {
            FromDate = fromDate;
            Todate = toDate;

           


            PrintDocument = new PrintDocument();

            PrintDocument.PrinterSettings.PrinterName = printername;

            PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
            PrintDocument.Print();
        }

        void DrawAtStart(string text, int Offset)
        {
            int startX = 10;
            int startY = 5;
            Font minifont = new Font("Arial", 7);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 5, startY + Offset);
        }
        //NEW
        void DrawAtStartCenter(string text, int Offset, int OffsetX)
        {
            //  int intPadding = 
            int startX = OffsetX  + ((pageWidthHeader - text.Length) / 2) * 4;
            int startY = 5;
            Font minifont = new Font("Arial", 7);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 5, startY + Offset);
        }

        void InsertItem(string key, string value, int Offset)
        {
            Font minifont = new Font("Arial", 7);
            int startX = 10;
            int startY = 5;

            graphics.DrawString(key, minifont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);

            graphics.DrawString(value, minifont,
                     new SolidBrush(Color.Black), startX + 130, startY + Offset);
        }

        void InsertItemList(string key, string value, int OffsetY, int OffsetX)
        {
            Font minifont = new Font("Arial", 7);
            int startX = 10;
            int startY = 5;

            graphics.DrawString(key, minifont,
                         new SolidBrush(Color.Black), startX + OffsetX, startY + OffsetY);

        }

        void InsertHeaderStyleItem(string key, string value, int Offset)
        {
            int startX = 10;
            int startY = 5;
            Font itemfont = new Font("Arial", 7, FontStyle.Bold);

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
            Font smallfont = new Font("Arial", 10);
            Font mediumfont = new Font("Arial", 14);
            Font largefont = new Font("Arial", 16);

            int Offset = 10;
            int smallinc = 10, mediuminc = 12, largeinc = 17;

            //Getting Receipt data 
            List<DetailedDailyReportModel> detailedDailyReportModels = new List<DetailedDailyReportModel>();
   
            ReportViewModel reportViewModel = new ReportViewModel();

            detailedDailyReportModels = reportViewModel.GetDetailedDailyByDate(FromDate,Todate);

            Offset = Offset + Offset;
            graphics.DrawString(LoginDetail.ClientName, smallfont, new SolidBrush(Color.Black), 14, Offset);
            Offset = Offset + mediuminc;

            InsertItemList(LoginDetail.OutletName, "", Offset, 5);
            Offset = Offset + mediuminc;
            Offset = Offset + mediuminc;

            //InsertItemList("DETAILED DAILY REPORT", "", Offset, 5);
            //Offset = Offset + mediuminc;
            //Offset = Offset + mediuminc;

            //InsertItemList("DATE: " + FromDate, "", Offset, 5);
            //Offset = Offset + mediuminc;

            foreach (var item in detailedDailyReportModels)
            {
                InsertItemList(item.RegisterTitle.ToString(), "", Offset, 5);
                InsertItemList(item.RegisterValue.ToString(), "", Offset, 175  + (50 - (item.RegisterValue.ToString().Length * 4)));
                Offset = Offset + mediuminc;
            }
            Offset = Offset + mediuminc;

            Offset = Offset + mediuminc;
        }
    }
}
