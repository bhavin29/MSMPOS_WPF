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
        int _KotTimerLimit;
        List<int> KitchenOrder = new List<int>();
        public KitchenView()
        {
            InitializeComponent();

            AppSettings appSettings = new AppSettings();
            _KotTimerLimit = Convert.ToInt32(appSettings.GetKotTimerLimit());
            txtTimer.Text = _KotTimerLimit.ToString();

            CenterWindowOnScreen();
            TimerForKOTStatus(_KotTimerLimit);
            GetKitchenPending(KitchenOrder);
        }
        private void GetKitchenPending(List<int> KitchenOrder)
        {
            KitchenViewModel kitchenViewModel = new KitchenViewModel();
            List<KitchenModel> kitchenModel = new List<KitchenModel>();
            kitchenModel = kitchenViewModel.GetKitchenStaus(KitchenOrder);

            try
            {
                wpKitchenView.Children.Clear();

                if (kitchenModel.Count > 0)
                {
                    foreach (var item in kitchenModel)
                    {
                        WrapPanel wpKOT = new WrapPanel();
                        wpKOT.Orientation = Orientation.Vertical;
                        wpKOT.VerticalAlignment = VerticalAlignment.Top;
                        wpKOT.Width = 212;
                        wpKOT.Height = 375;
                        wpKOT.Margin = new Thickness(1);

                        SolidColorBrush solidColorBrush = new SolidColorBrush();
                        solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E8E8E8"));

                        WrapPanel kotHeaderPanel = new WrapPanel();
                        kotHeaderPanel.Orientation = Orientation.Horizontal;
                        kotHeaderPanel.VerticalAlignment = VerticalAlignment.Top;
                        kotHeaderPanel.Width = 211;
                        kotHeaderPanel.Height = 325;
                        kotHeaderPanel.Background = solidColorBrush;
                        kotHeaderPanel.Margin = new Thickness(1);

                        ////Order Id
                        TextBlock txtbOrderId = new TextBlock();
                        txtbOrderId.Text = "Order# " + item.CustomerOrderId.ToString();
                        txtbOrderId.Name = "txtbOrderId_" + item.CustomerOrderId.ToString();
                        txtbOrderId.FontSize = 1;
                        txtbOrderId.Width = 0;
                        txtbOrderId.Visibility = Visibility.Hidden;
                        txtbOrderId.TextAlignment = TextAlignment.Left;
                        kotHeaderPanel.Children.Add(txtbOrderId);

                        //Table Name
                        TextBlock txtbTableName = new TextBlock();

                        string strPerson = "";
                        if (item.AllocatedPerson > 0)
                            strPerson = " [ " + item.AllocatedPerson + " ]";

                        txtbTableName.Text = "# " + item.TableName + strPerson;
                        txtbTableName.Name = "txtbTableName_" + item.TableId;
                        txtbTableName.FontSize = 20;

                        txtbTableName.Width = 166;
                        if (item.CustomerOrderId == 0)
                        {
                            txtbTableName.Width = 211;
                        }
                        txtbTableName.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                        txtbTableName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                        txtbTableName.TextAlignment = TextAlignment.Left;
                        kotHeaderPanel.Children.Add(txtbTableName);

                        if (item.CustomerOrderId != 0)
                        {
                            ToggleButton btnAllKOT = new ToggleButton();
                            btnAllKOT.Content = "...";
                            btnAllKOT.Name = "btnAllKOT_" + item.TableId;
                            btnAllKOT.FontSize = 16;
                            btnAllKOT.FontWeight = FontWeights.Bold;
                            btnAllKOT.Width = 45;
                            btnAllKOT.Height = 27;
                            btnAllKOT.BorderThickness = new Thickness(0);
                            btnAllKOT.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                            btnAllKOT.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                            btnAllKOT.Margin = new Thickness(0, 0, 0, 0);
                            btnAllKOT.Checked += DisplayAllKotItem;
                            btnAllKOT.Unchecked += DisplayCurrentKotItem;

                            for (var i = 0; i < KitchenOrder.Count; i++)
                            {
                                if (KitchenOrder[i] == item.TableId)
                                {
                                    btnAllKOT.IsChecked = true;
                                }
                            }


                            kotHeaderPanel.Children.Add(btnAllKOT);
                        }
                        if (item.kotStatusList.Count > 0)
                        {
                            foreach (var kotItem in item.kotStatusList)
                            {
                                if (kotItem.KOTStatus == EnumUtility.KOTStatus.Completed.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
                                }
                                else if (kotItem.KOTStatus == EnumUtility.KOTStatus.Pending.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff726f"));
                                }
                                else if (kotItem.KOTStatus == EnumUtility.KOTStatus.Cooking.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#fbff99"));
                                }
                                else if (kotItem.KOTStatus == EnumUtility.KOTStatus.Ready.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5cff7e"));
                                }
                                else if (kotItem.KOTStatus == EnumUtility.KOTStatus.Served.ToString())
                                {
                                    solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ccd9ff"));
                                }

                                WrapPanel kotDetailPanel = new WrapPanel();
                                kotDetailPanel.Orientation = Orientation.Horizontal;
                                kotDetailPanel.VerticalAlignment = VerticalAlignment.Top;
                                kotDetailPanel.Width = 226;
                                kotDetailPanel.Background = solidColorBrush;
                                kotDetailPanel.Margin = new Thickness(1);
                                kotDetailPanel.MouseDown += ChangeKOTStatus;

                                //Food Menu Name
                                TextBlock txtbFoodMenuName = new TextBlock();
                                txtbFoodMenuName.Text = kotItem.FoodMenuName;
                                txtbFoodMenuName.Name = "txtbFoodMenuName_" + kotItem.KOTItemId;
                                txtbFoodMenuName.FontSize = 18;
                                txtbFoodMenuName.Width = 225;
                                txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Left;
                                kotDetailPanel.Children.Add(txtbFoodMenuName);

                                //Food Menu Qty
                                TextBlock txtbFoodMenuQty = new TextBlock();
                                txtbFoodMenuQty.Text = "Qty: " + kotItem.FoodMenuQty.ToString();
                                txtbFoodMenuQty.Name = "txtbFoodMenuQty_" + kotItem.KOTItemId;
                                txtbFoodMenuQty.FontSize = 18;
                                txtbFoodMenuQty.Width = 70;
                                txtbFoodMenuQty.TextAlignment = TextAlignment.Left;
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
                                txtbKOTId.Text = kotItem.KOTId.ToString();
                                txtbKOTId.Name = "txtbKOTId_" + kotItem.KOTId;
                                txtbKOTId.Visibility = Visibility.Hidden;
                                txtbKOTId.Width = 5;
                                kotDetailPanel.Children.Add(txtbKOTId);

                                //KOT Status
                                TextBlock txtbKOTStatus = new TextBlock();
                                txtbKOTStatus.Text = kotItem.KOTStatus.ToString();
                                txtbKOTStatus.Name = "txtbKOTStatus_" + kotItem.KOTItemId.ToString();
                                txtbKOTStatus.FontSize = 1;
                                txtbKOTStatus.Width = 2;
                                txtbKOTStatus.TextAlignment = TextAlignment.Right;
                                kotDetailPanel.Children.Add(txtbKOTStatus);

                                kotHeaderPanel.Children.Add(kotDetailPanel);
                            }
                        }

                        wpKOT.Children.Clear();
                        wpKOT.Children.Add(kotHeaderPanel);

                        if (item.kotStatusList.Count > 0)
                        {
                            WrapPanel kotButtonPanel = new WrapPanel();
                            kotButtonPanel.Orientation = Orientation.Horizontal;
                            kotButtonPanel.VerticalAlignment = VerticalAlignment.Top;
                            kotButtonPanel.Width = 220;
                            kotButtonPanel.Margin = new Thickness(1);

                            ToggleButton btnSelectAll = new ToggleButton();
                            btnSelectAll.Content = "Select All";
                            btnSelectAll.Name = "btnSelectAll";
                            btnSelectAll.FontSize = 16;
                            btnSelectAll.FontWeight = FontWeights.Bold;
                            btnSelectAll.Width = 90;
                            btnSelectAll.Height = 40;
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
                            btnCooking.Name = "btnC" + item.CustomerOrderId.ToString();
                            btnCooking.FontSize = 16;
                            btnCooking.FontWeight = FontWeights.Bold;
                            btnCooking.Width = 40;
                            btnCooking.Height = 40;
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
                            btnPending.Name = "btnP" + item.CustomerOrderId.ToString();
                            btnPending.FontSize = 16;
                            btnPending.FontWeight = FontWeights.Bold;
                            btnPending.Width = 40;
                            btnPending.Height = 40;
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
                            btnCompleted.Name = "btnD" + item.CustomerOrderId.ToString();
                            btnCompleted.FontSize = 16;
                            btnCompleted.FontWeight = FontWeights.Bold;
                            btnCompleted.Width = 40;
                            btnCompleted.Height = 40;
                            btnCompleted.BorderThickness = new Thickness(1);
                            btnCompleted.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5733"));
                            btnCompleted.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                            btnCompleted.Margin = new Thickness(0, 0, 0, 0);
                            btnCompleted.Visibility = Visibility.Hidden;
                            btnCompleted.Click += ChangeKOTAllItemStatus;
                            kotButtonPanel.Children.Add(btnCompleted);
                            wpKOT.Children.Add(kotButtonPanel);
                        }

                        wpKitchenView.Children.Add(wpKOT);
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }


        private void DisplayAllKotItem(object sender, RoutedEventArgs e)
        {
            var btnAllKot = sender as ToggleButton;
            string tableId = btnAllKot.Name.Substring(10);
            if (!KitchenOrder.Contains(Convert.ToInt32(tableId)))
            {
                KitchenOrder.Add(Convert.ToInt32(tableId));
                GetKitchenPending(KitchenOrder);
            }
        }
        private void DisplayCurrentKotItem(object sender, RoutedEventArgs e)
        {
            var btnAllKot = sender as ToggleButton;
            string tableId = btnAllKot.Name.Substring(10);
            KitchenOrder.Remove(Convert.ToInt32(tableId));
            GetKitchenPending(KitchenOrder);
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
                    status = (int)EnumUtility.KOTStatus.Ready;
                }
                else if (txtbKOTStatus.Text == EnumUtility.KOTStatus.Ready.ToString())
                {
                    status = (int)EnumUtility.KOTStatus.Served;
                }
                else if (txtbKOTStatus.Text == EnumUtility.KOTStatus.Served.ToString())
                {
                    status = (int)EnumUtility.KOTStatus.Completed;
                }

                if (status != 0)
                {
                    kitchenViewModel.ChangeKOTStatus(txtbKOTItemId.Text, txtbKOTId.Text, status);
                    GetKitchenPending(KitchenOrder);
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
                string kotId = btnChangeStatus.Name.Substring(4);

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
                GetKitchenPending(KitchenOrder);
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
                _KotTimerLimit = Convert.ToInt32(txtTimer.Text);

                timer.Stop();
                timer.Interval = TimeSpan.FromMinutes(_KotTimerLimit);
                timer.Start();

                GetKitchenPending(KitchenOrder);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void TimerForKOTStatus(int kotTimerLimit)
        {
            try
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMinutes(kotTimerLimit);
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
                GetKitchenPending(KitchenOrder);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void CenterWindowOnScreen()
        {
            try
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double windowWidth = this.Width;
                double windowHeight = this.Height;
                this.Left = (screenWidth / 2) - (windowWidth / 2);
                this.Top = ((screenHeight / 2) - (windowHeight / 2));
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
    }
}
