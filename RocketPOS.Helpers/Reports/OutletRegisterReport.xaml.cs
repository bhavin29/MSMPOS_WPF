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
    /// Interaction logic for OutletRegisterReport.xaml
    /// </summary>
    public partial class OutletRegisterReport : Window
    {
        public OutletRegisterReport()
        {
            InitializeComponent();
            string strUri = "https://localhost:44319/Report/OutletRegister?outletRegisterId=21";
            Uri uri = new Uri(strUri, UriKind.RelativeOrAbsolute);

            this.webBrowser.Navigate(uri);

            //PrintDialog printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            //{
            //    printDialog.PrintVisual(print, "OutletRegisterReport");
            //}

        }


    }
}
