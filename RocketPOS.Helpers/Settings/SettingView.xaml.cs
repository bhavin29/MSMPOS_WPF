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
                List<SyncErrorModel> syncErrorModel = new List<SyncErrorModel>();

                SettingsViewModel settingsViewModel = new SettingsViewModel();

                lblWait.Visibility = Visibility.Visible;
                Thread.Sleep(100);
                txtOutput.AppendText(DateTime.Now.ToString() + " Process Starting " + "\n");
                txtOutput.UpdateLayout();
                syncErrorModel = settingsViewModel.SyncData();

                foreach (var item in syncErrorModel)
                {
                    txtOutput.Text = txtOutput.Text + DateTime.Now.ToString() + " " + item.ErrorMessage + "\n";
                }
                txtOutput.Text = txtOutput.Text + DateTime.Now.ToString() +  " Process Completed" + "\n";
                lblWait.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
                lblWait.Visibility = Visibility.Hidden;
            }
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

                /*
                string settings = LoginDetail.MainWindowSettings;
                string[] wordsSettings = settings.Split('$');

                foreach (var word in wordsSettings)
                {
                    string[] words = word.Split('=');

                    if (words[0] == "ShowInTaskbar")
                    {

                        this.ShowInTaskbar = bool.Parse(words[1].ToString());

                    }
                    else if (words[0] == "Topmost")
                    {
                        this.Topmost = bool.Parse(words[1].ToString());

                    }
                    else if (words[0] == "WindowStyle")
                    {
                        if (words[1] == "None")
                            this.WindowStyle = WindowStyle.None;

                    }
                    else if (words[0] == "ResizeMode")
                    {
                        if (words[1] == "NoResize")
                            this.ResizeMode = ResizeMode.NoResize;

                    }
                }
                */
  
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }


    }
}
