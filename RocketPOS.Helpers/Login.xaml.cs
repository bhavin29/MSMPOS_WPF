using System.Collections.Generic;
using System.Windows;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
using NLog;
using System;
using NLog.Fluent;
using System.Diagnostics;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 

    public partial class Login : Window
    {
        List<LoginModel> loginModel = new List<LoginModel>();
        LoginViewModel loginViewModel = new LoginViewModel();
        public Login()
        {
            try
            {
                InitializeComponent();

                txtUsername.Text = "Admin";
                txtPassword.Password = "Admin";
                
                CenterWindowOnScreen();
                throw new DivideByZeroException();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            loginModel = loginViewModel.GetUserLogin(txtUsername.Text, txtPassword.Password);
            if (loginModel.Count > 0)
            {
                LoginMerge(loginModel);

                if (loginModel[0].OutletRegisterStatus == 1)
                {
                    DateTime dt = new DateTime();

                    string v = DateTime.Now.ToShortDateString();
                    dt = DateTime.Parse(v);

                    int result = DateTime.Compare(loginModel[0].SystemDate, dt);

                    if (result < 0)
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "You are running with past date,please close your register and start new register with current date", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
                    }

                    loginViewModel.UpdateLoginLogout("login");
                    loginViewModel.LoginHistory(1);

                    MainWindow mainWin = new MainWindow();
                    mainWin.Show();
                    this.Close();
                }
                else
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Please contact admin to open your register.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                }
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Wrong UserName/PassWord", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                ResetControl();
            }
        }

        private void ResetControl()
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtUsername.Focus();
        }
        private void LoginMerge(List<LoginModel> loginModel)
        {
            LoginDetail.UserId = loginModel[0].UserId;
            LoginDetail.Username = loginModel[0].Username;
            LoginDetail.OutletName = loginModel[0].OutletName;
            LoginDetail.OutletId = loginModel[0].OutletId;
            LoginDetail.RoleTypeId = loginModel[0].RoleTypeId;
            LoginDetail.OutletRegisterStatus = loginModel[0].OutletRegisterStatus;
            LoginDetail.ClientName = loginModel[0].ClientName;
            LoginDetail.Address1 = loginModel[0].Address1;
            LoginDetail.Address2 = loginModel[0].Address2;
            LoginDetail.Email = loginModel[0].Email;
            LoginDetail.Phone = loginModel[0].Phone;
            LoginDetail.Logo = loginModel[0].Logo;
            LoginDetail.WebSite = loginModel[0].WebSite;
            LoginDetail.ReceiptPrefix = loginModel[0].ReceiptPrefix;
            LoginDetail.OrderPrefix = loginModel[0].OrderPrefix;
            LoginDetail.TimeZone = loginModel[0].TimeZone;
            LoginDetail.Header = loginModel[0].Header;
            LoginDetail.Footer = loginModel[0].Footer;
            LoginDetail.Footer1 = loginModel[0].Footer1;
            LoginDetail.Footer2 = loginModel[0].Footer2;
            LoginDetail.Footer3 = loginModel[0].Footer3;
            LoginDetail.Footer4 = loginModel[0].Footer4;
            LoginDetail.SystemDate = loginModel[0].SystemDate;
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = ((screenHeight / 2) - (windowHeight / 2));
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
