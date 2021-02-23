using RocketPOS.Core.Constants;
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

namespace RocketPOS.Helpers.Settings
{
    /// <summary>
    /// Interaction logic for SettingsList.xaml
    /// </summary>
    public partial class SettingsList : Window
    {
        public SettingsList()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }

  
        private void txtSyncProcess_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RightFrame.Navigate(new pSync());
            
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

        private void txtSystemSetup_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RightFrame.Navigate(new PSystemSetup());
        }

        private void txtReceiptOffset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RightFrame.Navigate(new PReceiptOffset());
        }

        private void txtTallySetup_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RightFrame.Navigate(new PTallySetup());
        }
    }
}
