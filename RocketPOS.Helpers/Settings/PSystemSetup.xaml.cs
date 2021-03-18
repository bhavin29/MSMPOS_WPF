using RocketPOS.Model;
using RocketPOS.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RocketPOS.Helpers.Settings
{
    /// <summary>
    /// Interaction logic for PSystemSetup.xaml
    /// </summary>
    public partial class PSystemSetup : Page
    {
        SettingsViewModel settingsViewModel = new SettingsViewModel();
        public PSystemSetup()
        {
            InitializeComponent();
            ClientSettingModel clientSettingModel = new ClientSettingModel();
            clientSettingModel = settingsViewModel.GetClientSetting();
            txtClientName.Text = clientSettingModel.ClientName;
            txtAddress1.Text = clientSettingModel.Address1;
            txtAddress2.Text = clientSettingModel.Address2;
            txtEmail.Text = clientSettingModel.Email;
            txtPhone.Text = clientSettingModel.Phone;
            txtHeader.Text = clientSettingModel.Header;
            txtFooter.Text = clientSettingModel.Footer;
            txtFooter1.Text = clientSettingModel.Footer1;
            txtFooter2.Text = clientSettingModel.Footer2;
            txtFooter3.Text = clientSettingModel.Footer3;
            txtFooter4.Text = clientSettingModel.Footer4;
            txtWebsite.Text = clientSettingModel.Website;
            txtOrderPrefix.Text = clientSettingModel.OrderPrefix;
            txtReceiptPrefix.Text = clientSettingModel.ReceiptPrefix;
            txtHeaderMarqueeText.Text = clientSettingModel.HeaderMarqueeText;
            txtDeliveryList.Text = clientSettingModel.DeliveryList;
            txtPowerby.Text = clientSettingModel.Powerby;
            chkIsItemOverright.IsChecked = (bool)clientSettingModel.IsItemOverright;
            txtLinkedServer.Text = clientSettingModel.LinkedServer;
            txtWebAppUrl.Text = clientSettingModel.WebAppUrl;
            txtCurrentOutletId.Text = clientSettingModel.CurrentOutletId;
            txtInvoiceTerms.Text = clientSettingModel.InvoiceTerms;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (btnSave.Content.ToString() == "Edit")
            {
                btnSave.Content = "Save";
                btnCancel.Visibility = Visibility.Visible;

                txtClientName.IsEnabled = true;
                txtAddress1.IsEnabled = true;
                txtAddress2.IsEnabled = true;
                txtEmail.IsEnabled = true;
                txtPhone.IsEnabled = true;
                txtHeader.IsEnabled = true;
                txtFooter.IsEnabled = true;
                txtFooter1.IsEnabled = true;
                txtFooter2.IsEnabled = true;
                txtFooter3.IsEnabled = true;
                txtFooter4.IsEnabled = true;
                txtWebsite.IsEnabled = true;
                txtOrderPrefix.IsEnabled = true;
                txtReceiptPrefix.IsEnabled = true;
                txtHeaderMarqueeText.IsEnabled = true;
                txtDeliveryList.IsEnabled = true;
                txtPowerby.IsEnabled = true;
                chkIsItemOverright.IsEnabled = true;
                txtLinkedServer.IsEnabled = true;
                txtWebAppUrl.IsEnabled = true;
                txtCurrentOutletId.IsEnabled = true;
                txtInvoiceTerms.IsEnabled = true;
            }
            else
            {
                ClientSettingModel clientSettingModel = new ClientSettingModel();
                clientSettingModel.ClientName = txtClientName.Text;
                clientSettingModel.Address1 = txtAddress1.Text;
                clientSettingModel.Address2 = txtAddress2.Text;
                clientSettingModel.Email = txtEmail.Text;
                clientSettingModel.Phone = txtPhone.Text;
                clientSettingModel.Header = txtHeader.Text;
                clientSettingModel.Footer = txtFooter.Text;
                clientSettingModel.Footer1 = txtFooter1.Text;
                clientSettingModel.Footer2 = txtFooter2.Text;
                clientSettingModel.Footer3 = txtFooter3.Text;
                clientSettingModel.Footer4 = txtFooter4.Text;
                clientSettingModel.Website = txtWebsite.Text;
                clientSettingModel.OrderPrefix = txtOrderPrefix.Text;
                clientSettingModel.ReceiptPrefix = txtReceiptPrefix.Text;
                clientSettingModel.HeaderMarqueeText = txtHeaderMarqueeText.Text;
                clientSettingModel.DeliveryList = txtDeliveryList.Text;
                clientSettingModel.Powerby = txtPowerby.Text;
                clientSettingModel.IsItemOverright =(bool)chkIsItemOverright.IsChecked;
                clientSettingModel.LinkedServer = txtLinkedServer.Text;
                clientSettingModel.WebAppUrl = txtWebAppUrl.Text;
                clientSettingModel.CurrentOutletId = txtCurrentOutletId.Text;
                clientSettingModel.InvoiceTerms = txtInvoiceTerms.Text;
                var result = settingsViewModel.UpdateClientSetting(clientSettingModel);
                btnCancel_Click(null, null);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "Edit";
            btnCancel.Visibility = Visibility.Hidden;
            txtClientName.IsEnabled = false;
            txtAddress1.IsEnabled = false;
            txtAddress2.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtPhone.IsEnabled = false;
            txtHeader.IsEnabled = false;
            txtFooter.IsEnabled = false;
            txtFooter1.IsEnabled = false;
            txtFooter2.IsEnabled = false;
            txtFooter3.IsEnabled = false;
            txtFooter4.IsEnabled = false;
            txtWebsite.IsEnabled = false;
            txtOrderPrefix.IsEnabled = false;
            txtReceiptPrefix.IsEnabled = false;
            txtHeaderMarqueeText.IsEnabled = false;
            txtDeliveryList.IsEnabled = false;
            txtPowerby.IsEnabled = false;
            chkIsItemOverright.IsEnabled = false;
            txtLinkedServer.IsEnabled = false;
            txtWebAppUrl.IsEnabled = false;
            txtCurrentOutletId.IsEnabled = false;
            txtInvoiceTerms.IsEnabled = false;
        }
    }
}
