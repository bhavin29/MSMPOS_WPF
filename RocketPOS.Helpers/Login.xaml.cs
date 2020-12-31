using System.Collections.Generic;
using System.Windows;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
using NLog;

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
            InitializeComponent();
            txtUsername.Text = "Admin";
            txtPassword.Password = "Admin";

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Error("Loggly Error");
            logger.Info("Start logging");
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            loginModel = loginViewModel.GetUserLogin(txtUsername.Text, txtPassword.Password);
            if (loginModel.Count > 0)
            {
                LoginMerge(loginModel);

                MainWindow mainWin = new MainWindow();
                OpenRegister openRegister = new OpenRegister();

                if (loginModel[0].OutletRegisterStatus == 1)
                {
                    mainWin.Show();
                }
                else
                {
                    openRegister.Show();
                }
                this.Close();
            }
            else
            {

                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Wrong UserName/PassWord", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                //var messageBoxResult = WpfMessageBox.Show("Message Box Title", "Are you sure?", MessageBoxButton.YesNo, RocketPOS.Core.Constants.EnumUtility.MessageBoxImage.Warning);
                //if (messageBoxResult != MessageBoxResult.Yes) return;

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
            LoginDetail.TimeZone = loginModel[0].TimeZone;
            LoginDetail.Header = loginModel[0].Header;
            LoginDetail.Footer = loginModel[0].Footer;
            LoginDetail.Footer1 = loginModel[0].Footer1;
            LoginDetail.Footer2 = loginModel[0].Footer2;
            LoginDetail.Footer3 = loginModel[0].Footer3;
            LoginDetail.Footer4 = loginModel[0].Footer4;
        }
    }
}
