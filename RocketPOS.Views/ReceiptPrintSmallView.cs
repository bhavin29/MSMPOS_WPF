﻿using System;
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

    public class ReceiptPrintSmallView
    {

        private PrintDocument PrintDocument;
        private Graphics graphics;
        private int InitialHeight = 360;
        private int billId = 0;
        int pageWidthHeader = 50;

        public ReceiptPrintSmallView()
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
        void DrawAtStartCenter(string text, int Offset)
        {
            //  int intPadding = 
            int startX = 25 + ((pageWidthHeader - text.Length) / 2) * 3;
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
            Font smallfont = new Font("Arial", 8);
            Font mediumfont = new Font("Arial", 10);
            Font largefont = new Font("Arial", 12);
            int Offset = 10;
            int smallinc = 10, mediuminc = 12, largeinc = 15;

            //Getting Receipt data 
            List<PrintReceiptModel> printReceiptModel = new List<PrintReceiptModel>();
            List<PrintReceiptItemModel> printReceiptItemModel = new List<PrintReceiptItemModel>();

            //Parameter pass global Customer Order Id
            PrintReceiptViewModel printReceiptViewModel = new PrintReceiptViewModel();
            printReceiptModel = printReceiptViewModel.GetPrintReceiptByBillId(billId);
            printReceiptItemModel = printReceiptViewModel.GetPrintReceiptItemByBillId(billId);

            //Image image = Image.FromFile("d:\\2.jpg");

            //e.Graphics.DrawImage(image, startX + 50, startY + Offset, 100, 30);
            // e.Graphics.DrawImage(image, 50, 10 + Offset, 100, 30);

            Offset = Offset + Offset;

            //Name
            int intPadding = 10 + ((pageWidthHeader - LoginDetail.ClientName.Length) / 2) * 3;
            graphics.DrawString(LoginDetail.ClientName, smallfont, new SolidBrush(Color.Black), intPadding, Offset);//50 + 22

            Offset = Offset + mediuminc;
            DrawAtStartCenter(LoginDetail.Header, Offset);

            Offset = Offset + mediuminc;
            DrawAtStartCenter(LoginDetail.Address1, Offset);

            Offset = Offset + mediuminc;
            DrawAtStartCenter(LoginDetail.Address2, Offset);

            Offset = Offset + mediuminc;
            DrawAtStartCenter("Email : " + LoginDetail.Email, Offset);

            Offset = Offset + mediuminc;
            DrawAtStartCenter("Phone : " + LoginDetail.Phone, Offset);

            String underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + mediuminc;
            DrawAtStart("Receipt Number: " + printReceiptModel[0].SalesInvoiceNumber.ToString().PadRight((20 - printReceiptModel[0].SalesInvoiceNumber.ToString().Length) + 10) + "Date: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm"), Offset); ;

            Offset = Offset + mediuminc;
            DrawAtStart("Customer: " + printReceiptModel[0].CustomerName, Offset);

            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;

            InsertHeaderStyleItem("ITEM                                RATE     QTY    AMOUNT ", "", Offset);

            Offset = Offset + largeinc;

            foreach (var item in printReceiptItemModel)
            {
                InsertItemList(item.FoodMenuName.ToString(), "", Offset, 5);
                InsertItemList(item.FoodMenuRate.ToString("F"), "", Offset, 60 + (50 - (item.FoodMenuRate.ToString().Length * 4)));
                InsertItemList(item.FoodMenuQty.ToString("F"), "", Offset, 90 + (50 - (item.FoodMenuQty.ToString().Length * 4)));
                InsertItemList(item.Price.ToString("F"), "", Offset, 133 + (50 - (item.Price.ToString().Length * 4)));

                // InsertItem(item.FoodMenuName.ToString().PadRight((55- (item.FoodMenuName.ToString().Length)))  + "x " +   item.FoodMenuRate.ToString("F").PadRight((25 - (item.FoodMenuRate.ToString().Length))) + item.Price.ToString("F"), "", Offset);

                Offset = Offset + smallinc;
            }
            Offset = Offset - smallinc;

            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;
            InsertItem("GROSS TOTAL: ", printReceiptModel[0].GrossAmount.ToString("F").PadLeft(30 - printReceiptModel[0].GrossAmount.ToString().Length), Offset);

            Offset = Offset + smallinc;
            InsertItem("DISCOUNT : ", printReceiptModel[0].Discount.ToString("F").PadLeft(30 - printReceiptModel[0].Discount.ToString().Length), Offset);

            Offset = Offset + smallinc;
            InsertItem("VATABLE: ", printReceiptModel[0].VatableAmount.ToString("F").PadLeft(30 - printReceiptModel[0].VatableAmount.ToString().Length), Offset);

            Offset = Offset + smallinc;
            InsertItem("SER CRH: ", printReceiptModel[0].ServiceCharge.ToString("F").PadLeft(30 - printReceiptModel[0].ServiceCharge.ToString().Length), Offset);

            Offset = Offset + smallinc;
            InsertItem("TOTAL: ", printReceiptModel[0].TotalAmount.ToString("F").PadLeft(30 - printReceiptModel[0].TotalAmount.ToString().Length), Offset);


            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;
            InsertItem("TOTAL: ", printReceiptModel[0].PaymentMethodName.ToString().PadRight(10) + " " + printReceiptModel[0].BillAmount.ToString("F").PadLeft(10), Offset);

            Offset = Offset + largeinc;
            Offset = Offset + largeinc;

            intPadding = 10 + ((pageWidthHeader - LoginDetail.ClientName.Length) / 2) * 3;
            graphics.DrawString(LoginDetail.Footer, smallfont, new SolidBrush(Color.Black), intPadding, Offset);//50 + 22

            Offset = Offset + largeinc;

            DrawAtStartCenter(LoginDetail.Footer1, Offset);
            Offset = Offset + mediuminc;

            DrawAtStartCenter(LoginDetail.Footer2, Offset);
            Offset = Offset + mediuminc;

            DrawAtStartCenter(LoginDetail.Footer3, Offset);
            Offset = Offset + mediuminc;

            DrawAtStartCenter(LoginDetail.Footer4, Offset);
            Offset = Offset + mediuminc;


            //DrawAtStart(LoginDetail.Footer1, Offset);

            //Offset = Offset + mediuminc;
            //DrawAtStart(LoginDetail.Footer2, Offset);

            //Offset = Offset + mediuminc;
            //DrawAtStart(LoginDetail.Footer3, Offset);

            //Offset = Offset + mediuminc;
            //DrawAtStart(LoginDetail.Footer4, Offset);

            //Offset = Offset + 7;
            //underLine = "-------------------------------------";
            //DrawLine(underLine, largefont, Offset, 0);

            //Offset = Offset + mediuminc;
            //String greetings = "Thanks for visiting us.";
            //DrawSimpleString(greetings, mediumfont, Offset, 28);

            //Offset = Offset + mediuminc;
            //underLine = "-------------------------------------";
            //DrawLine(underLine, largefont, Offset, 0);

            //Offset += (2 * mediuminc);
            //string tip = "TIP: -----------------------------";
            //InsertItem(tip, "", Offset);

            //Offset = Offset + largeinc;
            //string DrawnBy = "Meganos Softwares: 0312-0459491 - OR - 0321-6228321";
            //DrawSimpleString(DrawnBy, minifont, Offset, 15);
        }
    }
}
