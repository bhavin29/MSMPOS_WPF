using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
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
    /// Interaction logic for PTallySetup.xaml
    /// </summary>
    public partial class PTallySetup : Page
    {
        SettingsViewModel settingsViewModel = new SettingsViewModel();
        List<TallySetupSettingModel> tallySetupSettingModel = new List<TallySetupSettingModel>();
        List<PaymentMethodSettingModel> paymentMethodSettingModel = new List<PaymentMethodSettingModel>();
        public PTallySetup()
        {
            InitializeComponent();
            tallySetupSettingModel = settingsViewModel.GetTallySetupSetting();
            dgTallySetupSetting.ItemsSource = tallySetupSettingModel;
            paymentMethodSettingModel = settingsViewModel.GetPaymentMethodSetting();
            dgPaymentMethod.ItemsSource = settingsViewModel.GetPaymentMethodSetting();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (btnSave.Content.ToString() == "Edit")
            {
                btnSave.Content = "Save";
                btnCancel.Visibility = Visibility.Visible;
                int j = 1;
                for (int i = 0; i < dgTallySetupSetting.Items.Count; i++)
                {
                    var tallySetupSettingModel = (TallySetupSettingModel)dgTallySetupSetting.Items[i];
                    ContentPresenter myCp = dgTallySetupSetting.Columns[j].GetCellContent(tallySetupSettingModel) as ContentPresenter;
                    var myTemplate = myCp.ContentTemplate;
                    TextBox mytxtbox = myTemplate.FindName("txtLedgerName", myCp) as TextBox;
                    mytxtbox.IsEnabled = true;
                }
            }
            else
            {
                int j = 1;
                for (int i = 0; i < dgTallySetupSetting.Items.Count; i++)
                {
                    var tallySetupSettingModel = (TallySetupSettingModel)dgTallySetupSetting.Items[i];
                    ContentPresenter myCp = dgTallySetupSetting.Columns[j].GetCellContent(tallySetupSettingModel) as ContentPresenter;
                    var myTemplate = myCp.ContentTemplate;
                    TextBox mytxtbox = myTemplate.FindName("txtLedgerName", myCp) as TextBox;
                    if (string.IsNullOrEmpty(mytxtbox.Text))
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Enter valid data", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        TallySetupSettingModel tallySetupSettingModel1 = (TallySetupSettingModel)myCp.Content;
                        tallySetupSettingModel1.LedgerName = mytxtbox.Text;
                        var result = settingsViewModel.UpdateTallySetupSetting(tallySetupSettingModel1);
                    }
                }
                btnCancel_Click(null, null);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "Edit";
            btnCancel.Visibility = Visibility.Hidden;
            int j = 1;
            for (int i = 0; i < dgTallySetupSetting.Items.Count; i++)
            {
                var tallySetupSettingModel = (TallySetupSettingModel)dgTallySetupSetting.Items[i];
                ContentPresenter myCp = dgTallySetupSetting.Columns[j].GetCellContent(tallySetupSettingModel) as ContentPresenter;
                var myTemplate = myCp.ContentTemplate;
                TextBox mytxtbox = myTemplate.FindName("txtLedgerName", myCp) as TextBox;
                mytxtbox.IsEnabled = false;
            }
        }

        private void btnCancelPaymentMethod_Click(object sender, RoutedEventArgs e)
        {
            btnSavePaymentMethod.Content = "Edit";
            btnCancelPaymentMethod.Visibility = Visibility.Hidden;
            int j = 1,k = 2,l = 3;
            for (int i = 0; i < dgPaymentMethod.Items.Count; i++)
            {
                var paymentMethodSettingModel = (PaymentMethodSettingModel)dgPaymentMethod.Items[i];
                ContentPresenter myCpLedgerName = dgPaymentMethod.Columns[j].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                var myTemplateLedgerName = myCpLedgerName.ContentTemplate;
                TextBox txtTallyLedgerName = myTemplateLedgerName.FindName("txtTallyLedgerName", myCpLedgerName) as TextBox;
                txtTallyLedgerName.IsEnabled = false;

                ContentPresenter myCpTallyLedgerNamePark = dgPaymentMethod.Columns[k].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                var myTemplateTallyLedgerNamePark = myCpTallyLedgerNamePark.ContentTemplate;
                TextBox txtTallyLedgerNamePark = myTemplateTallyLedgerNamePark.FindName("txtTallyLedgerNamePark", myCpTallyLedgerNamePark) as TextBox;
                txtTallyLedgerNamePark.IsEnabled = false;


                ContentPresenter myCpTallyBillPostfix = dgPaymentMethod.Columns[l].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                var myTemplateTallyBillPostfix = myCpTallyBillPostfix.ContentTemplate;
                TextBox txtTallyBillPostfix = myTemplateTallyBillPostfix.FindName("txtTallyBillPostfix", myCpTallyBillPostfix) as TextBox;
                txtTallyBillPostfix.IsEnabled = false;
            }
        }

        private void btnSavePaymentMethod_Click(object sender, RoutedEventArgs e)
        {
            if (btnSavePaymentMethod.Content.ToString() == "Edit")
            {
                btnSavePaymentMethod.Content = "Save";
                btnCancelPaymentMethod.Visibility = Visibility.Visible;
                int j = 1, k = 2, l = 3;
                for (int i = 0; i < dgPaymentMethod.Items.Count; i++)
                {
                    var paymentMethodSettingModel = (PaymentMethodSettingModel)dgPaymentMethod.Items[i];
                    ContentPresenter myCpLedgerName = dgPaymentMethod.Columns[j].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                    var myTemplateLedgerName = myCpLedgerName.ContentTemplate;
                    TextBox txtTallyLedgerName = myTemplateLedgerName.FindName("txtTallyLedgerName", myCpLedgerName) as TextBox;
                    txtTallyLedgerName.IsEnabled = true;

                    ContentPresenter myCpTallyLedgerNamePark = dgPaymentMethod.Columns[k].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                    var myTemplateTallyLedgerNamePark = myCpTallyLedgerNamePark.ContentTemplate;
                    TextBox txtTallyLedgerNamePark = myTemplateTallyLedgerNamePark.FindName("txtTallyLedgerNamePark", myCpTallyLedgerNamePark) as TextBox;
                    txtTallyLedgerNamePark.IsEnabled = true;


                    ContentPresenter myCpTallyBillPostfix = dgPaymentMethod.Columns[l].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                    var myTemplateTallyBillPostfix = myCpTallyBillPostfix.ContentTemplate;
                    TextBox txtTallyBillPostfix = myTemplateTallyBillPostfix.FindName("txtTallyBillPostfix", myCpTallyBillPostfix) as TextBox;
                    txtTallyBillPostfix.IsEnabled = true;
                }
            }
            else
            {
                int j = 1, k = 2, l = 3;
                for (int i = 0; i < dgPaymentMethod.Items.Count; i++)
                {
                    var paymentMethodSettingModel = (PaymentMethodSettingModel)dgPaymentMethod.Items[i];
                    ContentPresenter myCpLedgerName = dgPaymentMethod.Columns[j].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                    var myTemplateLedgerName = myCpLedgerName.ContentTemplate;
                    TextBox txtTallyLedgerName = myTemplateLedgerName.FindName("txtTallyLedgerName", myCpLedgerName) as TextBox;

                    ContentPresenter myCpTallyLedgerNamePark = dgPaymentMethod.Columns[k].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                    var myTemplateTallyLedgerNamePark = myCpTallyLedgerNamePark.ContentTemplate;
                    TextBox txtTallyLedgerNamePark = myTemplateTallyLedgerNamePark.FindName("txtTallyLedgerNamePark", myCpTallyLedgerNamePark) as TextBox;


                    ContentPresenter myCpTallyBillPostfix = dgPaymentMethod.Columns[l].GetCellContent(paymentMethodSettingModel) as ContentPresenter;
                    var myTemplateTallyBillPostfix = myCpTallyBillPostfix.ContentTemplate;
                    TextBox txtTallyBillPostfix = myTemplateTallyBillPostfix.FindName("txtTallyBillPostfix", myCpTallyBillPostfix) as TextBox;

                    if (string.IsNullOrEmpty(txtTallyLedgerName.Text) && string.IsNullOrEmpty(txtTallyLedgerNamePark.Text) && string.IsNullOrEmpty(txtTallyBillPostfix.Text))
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Enter valid data", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        var paymentMethodSettingModel1 = (PaymentMethodSettingModel)dgPaymentMethod.Items[i];
                        paymentMethodSettingModel1.TallyLedgerName = txtTallyLedgerName.Text;
                        paymentMethodSettingModel1.TallyLedgerNamePark = txtTallyLedgerNamePark.Text;
                        paymentMethodSettingModel1.TallyBillPostfix = txtTallyBillPostfix.Text;
                        var result = settingsViewModel.UpdatePaymentMethodSetting(paymentMethodSettingModel1);
                    }
                }
                btnCancelPaymentMethod_Click(null, null);
            }
        }
    }
}
