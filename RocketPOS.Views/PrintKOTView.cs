using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;

namespace RocketPOS.Views
{
    public class PrintKOTView
    {
        //https://www.c-sharpcorner.com/UploadFile/mahesh/create-a-text-file-in-C-Sharp/
        //https://www.c-sharpcorner.com/article/printing-text-file-in-C-Sharp/

        private Font verdana10Font;
        private StreamReader reader;
        public void PrintKOT(int kotId)
        {
            int result=0;

            string fileName = @"C:\Temp\KOTTX.dat";
            result=CreateKOTTextFile(fileName, kotId);

            if (result == 1)
            {
                PrintTextFile(fileName);
            }
            else
            {
                Console.WriteLine("Opps sothings went worng.");
            }
        }

        private int CreateKOTTextFile(string fileName,int kotId)
        {
            int result= 0;
          //  FormattedText ft = new FormattedText();
            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Author: Mahesh Chand");
                    sw.WriteLine("Add one more line ");
                    sw.WriteLine("Add one more line ");
                    sw.WriteLine("Done! ");
                    result = 1;
                }

                return result;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                return result;

            }
        }

        private void CreateText(object sender, PrintPageEventArgs e) 
        {
            Graphics graphics = e.Graphics;

            Font regular = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Regular);
            Font bold = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Bold);

            //print header
            graphics.DrawString("FERREIRA MATERIALS PARA CONSTRUCAO LTDA", bold, Brushes.Black, 20, 10);
            graphics.DrawString("EST ENGENHEIRO MARCILAC, 116, SAO PAOLO - SP", regular, Brushes.Black, 30, 30);
            graphics.DrawString("Telefone: (11)5921-3826", regular, Brushes.Black, 110, 50);
            graphics.DrawLine(Pens.Black, 80, 70, 320, 70);
            graphics.DrawString("CUPOM NAO FISCAL", bold, Brushes.Black, 110, 80);
            graphics.DrawLine(Pens.Black, 80, 100, 320, 100);

            //print items
            graphics.DrawString("COD | DESCRICAO                      | QTY | X | Vir Unit | Vir Total |", bold, Brushes.Black, 10, 120);
            graphics.DrawLine(Pens.Black, 10, 140, 430, 140);

            //for (int i = 0; i < itemList.Count; i++)
            //{
            //    graphics.DrawString(itemList[i].ToString(), regular, Brushes.Black, 20, 150 + i * 20);
            //}

            //print footer
            //...

            regular.Dispose();
            bold.Dispose();

            // Check to see if more pages are to be printed.
           // e.HasMorePages = (itemList.Count > 20);

        }


        private void PrintTextFile(string fileName)
        {                // Write file contents on console.     
            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
            reader = new StreamReader(fileName);

            verdana10Font = new Font("Verdana", 10);
            PrintDocument pd = new PrintDocument();

            pd.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);

            pd.Print();

            if (reader != null)
                reader.Close();
        }
        private void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            //Get the Graphics object  
            Graphics g = ppeArgs.Graphics;
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = ppeArgs.MarginBounds.Left;
            float topMargin = ppeArgs.MarginBounds.Top;
            string line = null;
            //Calculate the lines per page on the basis of the height of the page and the height of the font  
            linesPerPage = ppeArgs.MarginBounds.Height / verdana10Font.GetHeight(g);
            //Now read lines one by one, using StreamReader  
            while (count < linesPerPage && ((line = reader.ReadLine()) != null))
            {
                //Calculate the starting position  
                yPos = topMargin + (count * verdana10Font.GetHeight(g));
                //Draw text  
                g.DrawString(line, verdana10Font, Brushes.Black, leftMargin, yPos, new StringFormat());
                //Move to next line  
                count++;
            }
            //If PrintPageEventArgs has more pages to print  
            if (line != null)
            {
                ppeArgs.HasMorePages = true;
            }
            else
            {
                ppeArgs.HasMorePages = false;
            }
        }
    }
}
