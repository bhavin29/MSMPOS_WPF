using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using RocketPOS.Model;
using RocketPOS.ViewModels;
namespace RocketPOS.Views
{
    //https://stackoverflow.com/questions/28096578/writing-nice-receipt-in-c-sharp-wpf-for-printing-on-thermal-printer-pos

    public class ReceiptPrintView
    {

        private PrintDocument PrintDocument;
        private Graphics graphics;
        private int InitialHeight = 360;
        private int billId = 0;
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
        public void Print(string printername,int id)
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


            //This all vluae take from the static after login

            ClientModel clientModel = new ClientModel();

            clientModel.ClientName = "SHAYONA LTD";
            clientModel.Address1 = "P.O.BOX 1234-345";
            clientModel.Address2 = "SHAHIBAUG, AHMEDABAD";
            clientModel.Email = "ADMDABAD123@GMAIL.COM";
            clientModel.Phone = "91-1234456789  || PIN:POB55689562";
            clientModel.Logo = "";
            clientModel.OpenTime = "";
            clientModel.CloseTime = "";
            clientModel.CurrencyId = 1;
            clientModel.TimeZone = "";
            clientModel.Header = "SATVIK VEGITABLE FOOD";
            clientModel.Footer = "JAI SWAMINARAYAN";
            clientModel.Footer1 = "For Catering/Party Arrangment";
            clientModel.Footer2 = "CAll on +658923 569 8956";
            clientModel.Footer3 = "WE12331WEREREW23213WE";
            clientModel.Footer4 = "WE12331WEREREW23213WE";

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

            //Name
            graphics.DrawString(clientModel.ClientName, smallfont, new SolidBrush(Color.Black), 50 + 22, 10 + Offset);
            Offset = Offset + largeinc + 10;

            DrawAtStart(clientModel.Header, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart(clientModel.Address1, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart(clientModel.Address2, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart("Email: " + clientModel.Email, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart("Phone: " + clientModel.Address1, Offset);

            String underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + mediuminc;
            DrawAtStart("Receipt Number: " + printReceiptModel[0].BillId, Offset);

            // if (!String.Equals(order.Customer.Address, "N/A"))
            // {
            //    Offset = Offset + mediuminc;
            //    DrawAtStart("Address: Paldi", Offset);
            // }

            //   if (!String.Equals(order.Customer.Phone, "N/A"))
            //   {
            //       Offset = Offset + mediuminc;
            //       DrawAtStart("Phone # : " + order.Customer.Phone, Offset);
            //   }

            Offset = Offset + mediuminc;
            DrawAtStart("Date: " + DateTime.Now.ToString(), Offset);

            Offset = Offset + mediuminc;
            DrawAtStart("Customer: " + printReceiptModel[0].CustomerName, Offset);

            Offset = Offset + smallinc;
            underLine = "-------------------------";
            DrawLine(underLine, largefont, Offset, 30);

            Offset = Offset + largeinc;

            InsertHeaderStyleItem("ITEM ", "RATE        AMOUNT ", Offset);

            foreach (var item in printReceiptItemModel)
            {
                InsertItem(item.FoodMenuName + " x " + item.FoodMenuRate, "25", Offset);

                Offset = Offset + smallinc;
            }
            ////   foreach (var itran in order.ItemTransactions)
            //   {
            //       InsertItem(itran.Item.Name + " x " + itran.Quantity, itran.Total.CValue, Offset);
            //       Offset = Offset + smallinc;
            //   }
            ////   foreach (var dtran in order.DealTransactions)
            //   {
            //       InsertItem(dtran.Deal.Name, dtran.Total.CValue, Offset);
            //       Offset = Offset + smallinc;

            //       foreach (var di in dtran.Deal.DealItems)
            //       {
            //           InsertItem(di.Item.Name + " x " + (dtran.Quantity * di.Quantity), "", Offset);
            //           Offset = Offset + smallinc;
            //       }
            //   }

            underLine = "-------------------------";
            DrawLine(underLine, largefont, Offset, 30);

            Offset = Offset + largeinc;
            InsertItem("GROSS TOTAL: ", printReceiptModel[0].GrossAmount.ToString(), Offset);

            Offset = Offset + largeinc;
            InsertItem("DISCOUNT : ", printReceiptModel[0].Discount.ToString(), Offset);

            Offset = Offset + largeinc;
            InsertItem("VATABLE: ", printReceiptModel[0].VatableAmount.ToString(), Offset);

            Offset = Offset + largeinc;
            InsertItem("SER CRH: ", printReceiptModel[0].ServiceCharge.ToString(), Offset);

            Offset = Offset + largeinc;
            InsertItem("TOTAL: ", printReceiptModel[0].TotalAmount.ToString(), Offset);

            //   if (!order.Cash.Discount.IsZero())
            //   {
            //       Offset = Offset + smallinc;
            //       InsertItem(" Discount: ", order.Cash.Discount.CValue, Offset);
            //   }

            underLine = "-------------------------";
            DrawLine(underLine, largefont, Offset, 30);

            Offset = Offset + largeinc;
            InsertItem("TOTAL: ", printReceiptModel[0].PaymentMethodName.ToString(), Offset);

            graphics.DrawString(clientModel.Footer, smallfont, new SolidBrush(Color.Black), 50 + 22, 10 + Offset);

            Offset = Offset + largeinc + 10;

            DrawAtStart(clientModel.Footer1, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart(clientModel.Footer2, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart(clientModel.Footer3, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart(clientModel.Footer4, Offset);


            Offset = Offset + 7;
            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + mediuminc;
            String greetings = "Thanks for visiting us.";
            DrawSimpleString(greetings, mediumfont, Offset, 28);

            Offset = Offset + mediuminc;
            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset += (2 * mediuminc);
            string tip = "TIP: -----------------------------";
            InsertItem(tip, "", Offset);

            Offset = Offset + largeinc;
            string DrawnBy = "Meganos Softwares: 0312-0459491 - OR - 0321-6228321";
            DrawSimpleString(DrawnBy, minifont, Offset, 15);
        }
    }
}
