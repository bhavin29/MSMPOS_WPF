using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for OpenRegister.xaml
    /// </summary>
    public partial class OpenRegister : Window
    {
        MainWindow mainWin = new MainWindow(); 
        OutletRegisterViewModel outletRegisterViewModel = new OutletRegisterViewModel();
        OutletRegisterModel outletRegisterModel = new OutletRegisterModel();

        public OpenRegister()
        {
            InitializeComponent();
        }

        private void btnOpenRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtOpeningBal.Text.Length > 0)
            {
                outletRegisterModel.OutletId = LoginDetail.OutletId;
                outletRegisterModel.USerID = LoginDetail.UserId;
                outletRegisterModel.OpeningBalance = Convert.ToInt32(txtOpeningBal.Text);

                outletRegisterViewModel.InsertOutletRegister(outletRegisterModel);

                mainWin.Show();
                this.Close();
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show("ROCKET POS", "Please enter Opening Balance", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);

               // MessageBox.Show("Please enter Opening Balance");
                txtOpeningBal.Focus();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
