using System.Windows;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MainWindow mainWin = new MainWindow();
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            if (txtUsername.Text == "A" && txtPassword.Password == "A")
            {
                mainWin.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong UserName/PassWord");
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
