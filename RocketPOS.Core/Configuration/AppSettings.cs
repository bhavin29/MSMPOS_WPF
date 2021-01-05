using System.Configuration;

namespace RocketPOS.Core.Configuration
{
    public class AppSettings
    {
        public string GetConnectionString()
        {
            return ConfigurationSettings.AppSettings["ConnectionString"];
        }

        public string GetPrinterName()
        {
            return ConfigurationSettings.AppSettings["PrinterName"];
        }

        public string GetAppPath()
        {
            return ConfigurationSettings.AppSettings["AppPath"];
        }
    }
}
