using NLog;
using NLog.Fluent;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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

            var st = new StackTrace(ex, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();
            logger.Error().Exception(ex).Property("line-number", line).Write(); //using NLog.Fluent, .NET 4.5 

            WpfMessageBox.Show(StatusMessages.AppTitle, ex.Message, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
        }
    }
}
