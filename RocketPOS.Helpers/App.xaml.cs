using System.Windows;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
          public App()
        {
            Current.DispatcherUnhandledException += (sender, args) => MessageBox.Show(args.Exception.InnerException?.Message ?? args.Exception.Message);
        }
    }
}
