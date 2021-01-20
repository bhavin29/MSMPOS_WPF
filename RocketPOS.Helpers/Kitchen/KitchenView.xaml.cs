using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RocketPOS.Helpers.Kitchen
{
    /// <summary>
    /// Interaction logic for KitchenView.xaml
    /// </summary>
    public partial class KitchenView : Window
    {
        DispatcherTimer timer;
        public KitchenView()
        {
            InitializeComponent();
            TimerForKOTStatus();
            GetKitchenPending();
        }
        private void GetKitchenPending()
        {
            KitchenViewModel kitchenViewModel = new KitchenViewModel();
            KitchenModel kitchenModel = new KitchenModel();
            kitchenModel = kitchenViewModel.GetKitchenStaus();

            try
            {
                wpKitchenView.Children.Clear();
                

                if (kitchenModel.kotStatusList.Count > 0)
                {
                    foreach (var kot in kitchenModel.kotStatusList)
                    {
                        WrapPanel wpKOT = new WrapPanel();
                        wpKOT.Orientation = Orientation.Vertical;
                        wpKOT.VerticalAlignment = VerticalAlignment.Top;
                        wpKOT.Width = 150;
                        wpKOT.Height = 300;
                        wpKOT.Margin = new Thickness(5);

                        SolidColorBrush solidColorBrush = new SolidColorBrush();
                        solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#dce309"));

                        WrapPanel kotHeaderPanel = new WrapPanel();
                        kotHeaderPanel.Orientation = Orientation.Horizontal;
                        kotHeaderPanel.VerticalAlignment = VerticalAlignment.Top;
                        kotHeaderPanel.Width = 150;
                        kotHeaderPanel.Height = 200;
                        kotHeaderPanel.Background = solidColorBrush;
                        kotHeaderPanel.Margin = new Thickness(5);

                        //Order Id
                        TextBlock txtbOrderId = new TextBlock();
                        txtbOrderId.Text = "Order# " + kot.CustomerOrderNo.ToString();
                        txtbOrderId.Name = "txtbOrderId_" + kot.OrderId.ToString();
                        txtbOrderId.FontSize = 10;
                        txtbOrderId.Width = 100;
                        txtbOrderId.HorizontalAlignment = HorizontalAlignment.Left;
                        kotHeaderPanel.Children.Add(txtbOrderId);

                        //KOTNumber
                        TextBlock txtbKOTNumber = new TextBlock();
                        txtbKOTNumber.Text = "KOT# " + kot.KOTNumber;
                        txtbKOTNumber.Name = "txtbKOTNumber_" + kot.KOTNumber;
                        txtbKOTNumber.FontSize = 10;
                        txtbKOTNumber.Width = 50;
                        txtbKOTNumber.HorizontalAlignment = HorizontalAlignment.Right;
                        kotHeaderPanel.Children.Add(txtbKOTNumber);


                        //Table Name
                        TextBlock txtbTableName = new TextBlock();
                        if (!string.IsNullOrEmpty(kot.TableName))
                        {
                            txtbTableName.Text = "Table : " + kot.TableName;
                        }
                        else
                        {
                            txtbTableName.Text = string.Empty;
                        }
                        txtbTableName.Name = "txtbTableName_" + kot.TableId;
                        txtbTableName.FontSize = 10;
                        txtbTableName.Width = 80;
                        kotHeaderPanel.Children.Add(txtbTableName);

                        if (kot.kOTItems.Count > 0)
                        {
                            foreach (var kotItem in kot.kOTItems)
                            {
                                if (kotItem.KOTStatus == EnumUtility.KOTStatus.Completed.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#32a836"));
                                }
                                else if (kotItem.KOTStatus == EnumUtility.KOTStatus.Pending.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66FFFF"));
                                }
                                else if (kotItem.KOTStatus == EnumUtility.KOTStatus.Cooking.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AF4D4D"));
                                }

                                WrapPanel kotDetailPanel = new WrapPanel();
                                kotDetailPanel.Orientation = Orientation.Horizontal;
                                kotDetailPanel.VerticalAlignment = VerticalAlignment.Top;
                                kotDetailPanel.Width = 150;
                                kotDetailPanel.Background = solidColorBrush;
                                kotDetailPanel.Margin = new Thickness(5);
                                kotDetailPanel.MouseDown += ChangeKOTStatus;


                                //Food Menu Name
                                TextBlock txtbFoodMenuName = new TextBlock();
                                txtbFoodMenuName.Text = kotItem.FoodMenuName;
                                txtbFoodMenuName.Name = "txtbFoodMenuName_" + kotItem.KOTItemId;
                                txtbFoodMenuName.FontSize = 10;
                                txtbFoodMenuName.Width = 150;
                                txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Left;
                                kotDetailPanel.Children.Add(txtbFoodMenuName);

                                //Food Menu Qty
                                TextBlock txtbFoodMenuQty = new TextBlock();
                                txtbFoodMenuQty.Text = "Qty : " + kotItem.FoodMenuQty.ToString();
                                txtbFoodMenuQty.Name = "txtbFoodMenuQty_" + kotItem.KOTItemId;
                                txtbFoodMenuQty.FontSize = 10;
                                txtbFoodMenuQty.Width = 75;
                                txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Left;
                                kotDetailPanel.Children.Add(txtbFoodMenuQty);

                                //KOT Item Id
                                TextBlock txtbKOTItemId = new TextBlock();
                                txtbKOTItemId.Text = kotItem.KOTItemId.ToString();
                                txtbKOTItemId.Name = "txtbKOTItemId_" + kotItem.KOTItemId;
                                txtbKOTItemId.Visibility = Visibility.Hidden;
                                txtbKOTItemId.Width = 5;
                                kotDetailPanel.Children.Add(txtbKOTItemId);

                                //KOT Id
                                TextBlock txtbKOTId = new TextBlock();
                                txtbKOTId.Text = kot.KOTId.ToString();
                                txtbKOTId.Name = "txtbKOTId_" + kot.KOTId;
                                txtbKOTId.Visibility = Visibility.Hidden;
                                txtbKOTId.Width = 5;
                                kotDetailPanel.Children.Add(txtbKOTId);

                                //KOT Status
                                TextBlock txtbKOTStatus = new TextBlock();
                                txtbKOTStatus.Text = kotItem.KOTStatus.ToString();
                                txtbKOTStatus.Name = "txtbKOTStatus_" + kotItem.KOTItemId.ToString();
                                txtbKOTStatus.FontSize = 10;
                                txtbKOTStatus.Width = 65;
                                txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Right;
                                kotDetailPanel.Children.Add(txtbKOTStatus);

                                kotHeaderPanel.Children.Add(kotDetailPanel);
                            }
                        }

                        WrapPanel kotButtonPanel = new WrapPanel();
                        kotButtonPanel.Orientation = Orientation.Horizontal;
                        kotButtonPanel.VerticalAlignment = VerticalAlignment.Top;
                        kotButtonPanel.Width = 150;
                        kotButtonPanel.Margin = new Thickness(5);

                        ToggleButton btnSelectAll = new ToggleButton();
                        btnSelectAll.Content = "Select All";
                        btnSelectAll.Name = "btnSelectAll";
                        btnSelectAll.FontSize = 10;
                        btnSelectAll.FontWeight = FontWeights.Bold;
                        btnSelectAll.Width = 70;
                        btnSelectAll.Height = 30;
                        btnSelectAll.BorderThickness = new Thickness(1);
                        btnSelectAll.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5733"));
                        btnSelectAll.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnSelectAll.Margin = new Thickness(0, 0, 0, 0);
                        btnSelectAll.Checked += SelectAllKotItem;
                        btnSelectAll.Unchecked += DeSelectAllKotItem;
                        kotButtonPanel.Children.Add(btnSelectAll);

                        //Cooking Button
                        Button btnCooking = new Button();
                        btnCooking.Content = "C";
                        btnCooking.Name = "btn" + kot.KOTId.ToString();
                        btnCooking.FontSize = 10;
                        btnCooking.FontWeight = FontWeights.Bold;
                        btnCooking.Width = 25;
                        btnCooking.Height = 30;
                        btnCooking.BorderThickness = new Thickness(1);
                        btnCooking.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5733"));
                        btnCooking.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnCooking.Margin = new Thickness(0, 0, 0, 0);
                        btnCooking.Visibility = Visibility.Hidden;
                        btnCooking.Click += ChangeKOTAllItemStatus;
                        kotButtonPanel.Children.Add(btnCooking);

                        //Pending Button
                        Button btnPending = new Button();
                        btnPending.Content = "P";
                        btnPending.Name = "btn" + kot.KOTId.ToString();
                        btnPending.FontSize = 10;
                        btnPending.FontWeight = FontWeights.Bold;
                        btnPending.Width = 25;
                        btnPending.Height = 30;
                        btnPending.BorderThickness = new Thickness(1);
                        btnPending.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5733"));
                        btnPending.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnPending.Margin = new Thickness(0, 0, 0, 0);
                        btnPending.Visibility = Visibility.Hidden;
                        btnPending.Click += ChangeKOTAllItemStatus;
                        kotButtonPanel.Children.Add(btnPending);

                        //Completed Button
                        Button btnCompleted = new Button();
                        btnCompleted.Content = "D";
                        btnCompleted.Name = "btn" + kot.KOTId.ToString();
                        btnCompleted.FontSize = 10;
                        btnCompleted.FontWeight = FontWeights.Bold;
                        btnCompleted.Width = 25;
                        btnCompleted.Height = 30;
                        btnCompleted.BorderThickness = new Thickness(1);
                        btnCompleted.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5733"));
                        btnCompleted.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnCompleted.Margin = new Thickness(0, 0, 0, 0);
                        btnCompleted.Visibility = Visibility.Hidden;
                        btnCompleted.Click += ChangeKOTAllItemStatus;
                        kotButtonPanel.Children.Add(btnCompleted);

                        wpKOT.Children.Clear();
                        wpKOT.Children.Add(kotHeaderPanel);
                        wpKOT.Children.Add(kotButtonPanel);
                        wpKitchenView.Children.Add(wpKOT);
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void SelectAllKotItem(object sender, RoutedEventArgs e)
        {
            try
            {
                var btnSelectAll = sender as ToggleButton;
                var kotButtonPanel = (WrapPanel)btnSelectAll.Parent;
                var btnCooking = kotButtonPanel.Children[1] as Button;
                var btnPending = kotButtonPanel.Children[2] as Button;
                var btnCompleted = kotButtonPanel.Children[3] as Button;

                btnSelectAll.Content = "De-Select All";
                btnSelectAll.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bcddee"));
                btnCooking.Visibility = Visibility.Visible;
                btnPending.Visibility = Visibility.Visible;
                btnCompleted.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void DeSelectAllKotItem(object sender, RoutedEventArgs e)
        {
            try
            {
                var btnDeSelectAll = sender as ToggleButton;
                var kotButtonPanel = (WrapPanel)btnDeSelectAll.Parent;
                var btnCooking = kotButtonPanel.Children[1] as Button;
                var btnPending = kotButtonPanel.Children[2] as Button;
                var btnCompleted = kotButtonPanel.Children[3] as Button;

                btnDeSelectAll.Content = "Select All";
                btnDeSelectAll.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5733"));
                btnCooking.Visibility = Visibility.Hidden;
                btnPending.Visibility = Visibility.Hidden;
                btnCompleted.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void ChangeKOTStatus(object sender, MouseButtonEventArgs e)
        {
            try
            {
                KitchenViewModel kitchenViewModel = new KitchenViewModel();
                var kotDetailPanel = sender as WrapPanel;
                var txtbKOTItemId = kotDetailPanel.Children[2] as TextBlock;
                var txtbKOTId = kotDetailPanel.Children[3] as TextBlock;
                var txtbKOTStatus = kotDetailPanel.Children[4] as TextBlock;
                int status = 0;

                if (txtbKOTStatus.Text == EnumUtility.KOTStatus.Pending.ToString())
                {
                    status = (int)EnumUtility.KOTStatus.Cooking;
                }
                else if (txtbKOTStatus.Text == EnumUtility.KOTStatus.Cooking.ToString())
                {
                    status = (int)EnumUtility.KOTStatus.Completed;
                }

                if (status != 0)
                {
                    kitchenViewModel.ChangeKOTStatus(txtbKOTItemId.Text, txtbKOTId.Text, status);
                    GetKitchenPending();
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void ChangeKOTAllItemStatus(object sender, RoutedEventArgs e)
        {
            try
            {
                KitchenViewModel kitchenViewModel = new KitchenViewModel();
                var btnChangeStatus = sender as Button;
                string kotId = btnChangeStatus.Name.Substring(3);

                if (btnChangeStatus.Content.ToString() == "C")
                {
                    kitchenViewModel.ChangeAllKOTItemStatus(kotId, (int)EnumUtility.KOTStatus.Cooking);
                }
                else if (btnChangeStatus.Content.ToString() == "P")
                {
                    kitchenViewModel.ChangeAllKOTItemStatus(kotId, (int)EnumUtility.KOTStatus.Pending);
                }
                else if (btnChangeStatus.Content.ToString() == "D")
                {
                    kitchenViewModel.ChangeAllKOTItemStatus(kotId, (int)EnumUtility.KOTStatus.Completed);
                }
                GetKitchenPending();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnRefreshAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetKitchenPending();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void TimerForKOTStatus()
        {
            AppSettings appSettings = new AppSettings();
            try
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMinutes(Convert.ToDouble(appSettings.GetKotTimerLimit()));
                timer.Tick += TimerForKOTStatus_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        void TimerForKOTStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                GetKitchenPending();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
    }
}
