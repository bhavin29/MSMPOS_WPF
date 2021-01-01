using NLog;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            Exception ex = e.Exception as Exception;
            WpfMessageBox.Show(StatusMessages.AppTitle, ex.Message, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);

            logger.Error(ex.Message);

        }
    }
}
