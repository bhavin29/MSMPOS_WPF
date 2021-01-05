using NLog;
using RocketPOS.Helpers.RMessageBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RocketPOS.Core.Constants
{
    public static class SystemError
    {
        static Logger logger;
 
        public static void Register(Exception ex)
        {
            WpfMessageBox.Show(StatusMessages.AppTitle, ex.ToString(), MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
            
            logger = LogManager.GetCurrentClassLogger();
            logger.Error(ex.ToString());
        }
    }
}
