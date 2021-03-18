using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class SettingsViewModel
    {
        AppSettings appSettings = new AppSettings();
        public List<SyncErrorModel> SyncData(int process)
        {
            List<SyncErrorModel> syncErrorModel = new List<SyncErrorModel>();

            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "";
                if (process == 1)
                {
                    query = "EXEC [SyncData] ";
                }
                else if (process == 2)
                {
                    query = "EXEC [SyncDataTransaction] ";
                }

                syncErrorModel = connection.Query<SyncErrorModel>(query, commandTimeout: 300).ToList();
            }
            return syncErrorModel;
        }

        public List<TallySetupSettingModel> GetTallySetupSetting()
        {
            List<TallySetupSettingModel> tallySetupSettingModel = new List<TallySetupSettingModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                var query = " select Id,KeyName,LedgerName from TallySetup ";
                tallySetupSettingModel = connection.Query<TallySetupSettingModel>(query).ToList();
            }
            return tallySetupSettingModel;
        }

        public bool UpdateTallySetupSetting(TallySetupSettingModel tallySetupSettingModel)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(tallySetupSettingModel.Id.ToString()))
            {
                using (var connection = new SqlConnection(appSettings.GetConnectionString()))
                {
                    var query = "Update [TallySetup] set LedgerName='" + tallySetupSettingModel.LedgerName + "' Where Id=" + tallySetupSettingModel.Id;
                    result = connection.Query<bool>(query).FirstOrDefault();
                }
            }
            return result;
        }

        public List<PaymentMethodSettingModel> GetPaymentMethodSetting()
        {
            List<PaymentMethodSettingModel> paymentMethodSettingModel = new List<PaymentMethodSettingModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                var query = " select Id,PaymentMethodName,TallyLedgerName,TallyLedgerNamePark,TallyBillPostfix from PaymentMethod ";
                paymentMethodSettingModel = connection.Query<PaymentMethodSettingModel>(query).ToList();
            }
            return paymentMethodSettingModel;
        }

        public bool UpdatePaymentMethodSetting(PaymentMethodSettingModel paymentMethodSettingModel)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(paymentMethodSettingModel.Id.ToString()))
            {
                using (var connection = new SqlConnection(appSettings.GetConnectionString()))
                {
                    var query = "Update [PaymentMethod] set TallyLedgerName='" + paymentMethodSettingModel.TallyLedgerName + "',TallyLedgerNamePark='" + paymentMethodSettingModel.TallyLedgerNamePark + "',TallyBillPostfix='" + paymentMethodSettingModel.TallyBillPostfix + "' Where Id=" + paymentMethodSettingModel.Id;
                    result = connection.Query<bool>(query).FirstOrDefault();
                }
            }
            return result;
        }

        public ClientSettingModel GetClientSetting()
        {
            ClientSettingModel clientSettingModel = new ClientSettingModel();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                connection.Open();
                var query = " select ClientName,Address1,Address2,Email,Phone,Header,Footer,Footer1,Footer2,Footer3,Footer4,website,OrderPrefix,ReceiptPrefix, " +
                            " HeaderMarqueeText,DeliveryList,Powerby,IsItemOverright,LinkedServer,WebAppUrl,CurrentOutletId,InvoiceTerms from Client ";
                clientSettingModel = connection.Query<ClientSettingModel>(query).FirstOrDefault();
            }
            return clientSettingModel;
        }

        public bool UpdateClientSetting(ClientSettingModel clientSettingModel)
        {
            bool result = false;
            int isItemOverright = 0;

            if (clientSettingModel.IsItemOverright == false)
            {
                isItemOverright = 0;
            }
            else
            {
                isItemOverright = 1;
            }
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Update [Client] set ClientName='" + clientSettingModel.ClientName + "'," +
                                                "Address1 = '" + clientSettingModel.Address1 + "'," +
                                                "Address2 = '" + clientSettingModel.Address2 + "'," +
                                                "Email = '" + clientSettingModel.Email + "'," +
                                                "Phone = '" + clientSettingModel.Phone + "'," +
                                                "Header = '" + clientSettingModel.Header + "'," +
                                                "Footer = '" + clientSettingModel.Footer + "'," +
                                                "Footer1 = '" + clientSettingModel.Footer1 + "'," +
                                                "Footer2 = '" + clientSettingModel.Footer2 + "'," +
                                                "Footer3 = '" + clientSettingModel.Footer3 + "'," +
                                                "Footer4 = '" + clientSettingModel.Footer4 + "'," +
                                                "website = '" + clientSettingModel.Website + "'," +
                                                "OrderPrefix = '" + clientSettingModel.OrderPrefix + "'," +
                                                "ReceiptPrefix = '" + clientSettingModel.ReceiptPrefix + "'," +
                                                "HeaderMarqueeText = '" + clientSettingModel.HeaderMarqueeText + "'," +
                                                "DeliveryList = '" + clientSettingModel.DeliveryList + "'," +
                                                "Powerby = '" + clientSettingModel.Powerby + "'," +
                                                "IsItemOverright = " + isItemOverright + "," +
                                                "LinkedServer = '" + clientSettingModel.LinkedServer + "'," +
                                                "WebAppUrl = '" + clientSettingModel.WebAppUrl + "'," +
                                                "InvoiceTerms='" + clientSettingModel.InvoiceTerms + "',"+
                                                "CurrentOutletId = " + clientSettingModel.CurrentOutletId;
                result = connection.Query<bool>(query).FirstOrDefault();
            }
            return result;
        }
    }
}
