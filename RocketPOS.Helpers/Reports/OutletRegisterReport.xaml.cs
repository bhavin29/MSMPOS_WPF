using RocketPOS.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RocketPOS.Core.Constants;
using RocketPOS.ViewModels;
using RocketPOS.Model;
using Microsoft.Win32;

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
            CommonMethods commonMethods = new CommonMethods();
            OutletRegisterViewModel outletRegisterViewModel = new OutletRegisterViewModel();
            AppSettings appSettings = new AppSettings();
            try
            {
                string path = string.Empty, firstLine = string.Empty;
                string strUri = appSettings.GetWebAppUri();
                strUri += "Report/OutletRegister?outletRegisterId=" + LoginDetail.OutletRegisterId;

                Uri uri = new Uri(strUri, UriKind.RelativeOrAbsolute);

                this.webBrowser.Navigate(uri);

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "OutletRegisterReport");
                }

                //Download To Export User Register Report
                List<OutletUserRegister> outletUserRegister = new List<OutletUserRegister>();
                outletUserRegister = outletRegisterViewModel.GetUserRegisterReport();
                
                string fileName = "UserRegisterReport_" + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = fileName != "" ? fileName : "gpmfca-exportedDocument",
                    DefaultExt = ".xlsx",
                    Filter = "Common Seprated Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                }
                firstLine = "Close Register for " + LoginDetail.Username + " at " + System.DateTime.Now;
                commonMethods.WriteExcelFile(commonMethods.ConvertToDataTable(outletUserRegister), path, firstLine);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        
    }
}
