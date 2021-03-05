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
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : Window
    {
        public SettingView()
        {
            InitializeComponent();
            CenterWindowOnScreen();
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
            Dispatcher.BeginInvoke((Action)(() => btnStartSync.Visibility=Visibility.Hidden));
            Dispatcher.BeginInvoke((Action)(() => UpdateStatus("Pleae wait..." + "\n")));

          //  syncErrorModel = settingsViewModel.SyncData();

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
        private void CenterWindowOnScreen()
        {
            try
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double windowWidth = this.Width;
                double windowHeight = this.Height;
                this.Left = (screenWidth / 2) - (windowWidth / 2);
                this.Top = ((screenHeight / 2) - (windowHeight / 2));


            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }


    }
}
