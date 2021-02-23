using RocketPOS.Core.Constants;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RocketPOS.Helpers.Settings
{
    /// <summary>
    /// Interaction logic for pSync.xaml
    /// </summary>
    public partial class pSync : Page
    {
        public pSync()
        {
            InitializeComponent();
        }
        private void btnStartSync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtOutput.Text = "";
                UpdateStatus(DateTime.Now.ToString() + " Process Started " + "\n");
                var thread = new Thread(LoadDevices);
                thread.Start();
                return;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void UpdateStatus(string status)
        {
            txtOutput.AppendText(status);
        }
        private void LoadDevices()
        {
            List<SyncErrorModel> syncErrorModel = new List<SyncErrorModel>();
            SettingsViewModel settingsViewModel = new SettingsViewModel();
            Dispatcher.BeginInvoke((Action)(() => btnStartSync.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke((Action)(() => UpdateStatus("Pleae wait..." + "\n")));

            syncErrorModel = settingsViewModel.SyncData();

            foreach (var item in syncErrorModel)
            {
                Dispatcher.BeginInvoke((Action)(() => UpdateStatus(DateTime.Now.ToString() + " " + item.ErrorMessage + "\n")));
            }
            for (int i = 0; i < 5; i++)
            {
                Dispatcher.BeginInvoke((Action)(() => UpdateStatus(".")));
                Thread.Sleep(250);
            }
            Dispatcher.BeginInvoke((Action)(() => UpdateStatus("\n" + DateTime.Now.ToString() + " Process Completed" + "\n")));
            Dispatcher.BeginInvoke((Action)(() => btnStartSync.Visibility = Visibility.Visible));
        }
      }
}
