using System;
using RocketPOS.Helpers.Reports;
using RocketPOS.Helpers.RMessageBox;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using System.Data;
using RocketPOS.Core.Constants;
using RocketPOS.Views;
using System.Windows.Media;
using System.Windows.Threading;
using RocketPOS.Core.Configuration;
using RocketPOS.Helpers.Tables;
using System.Diagnostics;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.Text;
using System.Threading;
using RocketPOS.Helpers.Kitchen;
using RocketPOS.Helpers.Settings;
namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        LoginViewModel loginViewModel = new LoginViewModel();
        SaleItemModel saleItemsFoodMenu = new SaleItemModel();
        int rowId, customerAdd = 0, ladCustomer = 0;
        decimal BalancePoints = 0, ApplyBalancePoints = 0;
        bool applyRedeem = false;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                CenterWindowOnScreen();
                HeaderFooter();
                GenerateDynamicFoodMenu();
                GetWaiterList();
                GetCustomerList(1);
                Timer();

                txtbTotalPayableAmount.Text = "0.00";
                rdbPendingSales.IsChecked = true;
                rdbAllSales.IsChecked = true;
                dgFoodMenuList.Columns[4].Visibility = Visibility.Visible;
                dgSaleItem.Columns[0].Visibility = Visibility.Visible;

                //if (LoginDetail.RoleTypeId == (int)EnumUtility.RoleTypeId.Admin)
                //{
                //    dgFoodMenuList.Columns[3].Visibility = Visibility.Visible;
                //    dgSaleItem.Columns[0].Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    dgSaleItem.Columns[2].Width = 165;
                //}

                GetOrderList((int)EnumUtility.OrderPaidStatus.Pending, (int)EnumUtility.OrderType.All, string.Empty);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void CheckDineInSelect()
        {
            if (DineTable.OrderId != 0)
            {
                rdbDineInOrderType.IsChecked = true;
                txtbDineInTableId.Text = DineTable.TableId.ToString();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        #region Methods
        private void GetCustomerList(int id)
        {
            try
            {
                CustomerViewModel customerViewModel = new CustomerViewModel();
                List<CustomerModel> customers = new List<CustomerModel>();
                customers = customerViewModel.GetCustomers();
                cmbCustomer.ItemsSource = customers;
                cmbCustomer.Text = "Select Customer";
                cmbCustomer.IsEditable = true;
                cmbCustomer.IsReadOnly = true;
                cmbCustomer.SelectedValuePath = "Id";
                cmbCustomer.DisplayMemberPath = "CustomerName";

                if (cmbCustomer.Items.Count >= 1)
                {
                    cmbCustomer.SelectedValue = id;
                    //    txtRedeemPoints.Text = "Redeem " + customers[id].BalancePoints + " Pt";
                    //   BalancePoints = customers[id].BalancePoints;
                }
                else
                {
                    cmbCustomer.SelectedIndex = -1;
                    //      txtRedeemPoints.Text = "Redeem";
                    //     BalancePoints = 0;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void GenerateDynamicFoodMenu()
        {
            try
            {
                spCategory.Children.Clear();
                spFavouriteCategory.Children.Clear();
                spSubCategory.Children.Clear();

                Application.Current.Resources["FoodList"] = null;
                AppSettings appSettings = new AppSettings();
                string rootPath = string.Empty;
                FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
                FoodMenuModel foodMenu = new FoodMenuModel();
                rootPath = appSettings.GetAppPath();

                if (Application.Current.Resources["FoodList"] == null)
                {
                    foodMenu = foodMenuViewModel.GetFoodMenu(LoginDetail.OutletId);
                    Application.Current.Resources["FoodList"] = foodMenu;
                }
                else
                {
                    foodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];
                }

                if (foodMenu.FoodList.Count > 0)
                {
                    Button btnCategory = new Button();
                    btnCategory.Content = "All";
                    btnCategory.Name = "btnAll";
                    btnCategory.FontSize = 15;
                    btnCategory.FontWeight = FontWeights.Bold;
                    btnCategory.Width = 102;
                    btnCategory.Height = 50;
                    btnCategory.BorderThickness = new Thickness(1);
                    btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bcddee"));
                    btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    btnCategory.Margin = new Thickness(0, 0, 0, 0);
                    btnCategory.Click += GetSubCategory;
                    spCategory.Children.Add(btnCategory);
                }

                foreach (var foodCategory in foodMenu.FoodList)
                {
                    // if (foodCategory.IsFavourite == 0)
                    {
                        TextBlock txBlock = new TextBlock();

                        txBlock.Text = foodCategory.FoodCategory;
                        txBlock.TextWrapping = TextWrapping.Wrap;
                        txBlock.FontSize = 15;
                        txBlock.FontWeight = FontWeights.Bold;
                        txBlock.Width = 96;
                        txBlock.TextAlignment = TextAlignment.Center;

                        Button btnCategory = new Button();
                        //btnCategory.Content = foodCategory.FoodCategory;
                        btnCategory.Content = txBlock;
                        btnCategory.Name = "btn" + foodCategory.Id;
                        btnCategory.Width = 102;
                        btnCategory.Height = 50;
                        btnCategory.BorderThickness = new Thickness(1);
                        btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));
                        btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnCategory.Margin = new Thickness(0, 0, 0, 0);
                        btnCategory.Click += GetSubCategory;
                        spCategory.Children.Add(btnCategory);
                    }
                    //else
                    //{
                    //    TextBlock txBlock = new TextBlock();

                    //    txBlock.Text = foodCategory.FoodCategory;
                    //    txBlock.TextWrapping = TextWrapping.Wrap;
                    //    txBlock.FontSize = 15;
                    //    txBlock.FontWeight = FontWeights.Bold;
                    //    txBlock.Width = 100;

                    //    Button btnCategory = new Button();
                    //    // btnCategory.Content = foodCategory.FoodCategory;
                    //    btnCategory.Content = txBlock;
                    //    btnCategory.Name = "btn" + foodCategory.Id;
                    //    btnCategory.Width = 102;
                    //    btnCategory.Height = 50;
                    //    btnCategory.BorderThickness = new Thickness(1);
                    //    btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));
                    //    btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    //    btnCategory.Margin = new Thickness(0, 0, 0, 0);
                    //    btnCategory.Click += GetSubCategory;
                    //   spFavouriteCategory.Children.Add(btnCategory);
                    //}
                }

                if (foodMenu.FoodList.Count > 0)
                {
                    GenerateDynamicFoodItemsALL(foodMenu, rootPath, string.Empty, "All");
                }

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void GenerateDynamicFoodItemsALL(FoodMenuModel foodMenu, string rootPath, string searchKey, string type)
        {
            try
            {
                foreach (var foodCategory in foodMenu.FoodList)
                {
                    foreach (var itemSubCat in foodCategory.SubCategory)
                    {
                        GenerateDyanmicFoodItemsList(itemSubCat, rootPath);
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void GenerateDynamicFoodItems(FoodMenuModel foodMenu, string rootPath, string searchKey, string type)
        {
            try
            {
                foreach (var foodCategory in foodMenu.FoodList)
                {
                    foreach (var itemSubCat in foodCategory.SubCategory)
                    {
                        if (!string.IsNullOrEmpty(searchKey) && (itemSubCat.SmallName.ToLower().Contains(searchKey) || itemSubCat.SalesPrice.ToString().ToLower().Contains(searchKey)))
                        {
                            GenerateDyanmicFoodItemsList(itemSubCat, rootPath);
                        }
                        else if (!string.IsNullOrEmpty(type))
                        {
                            if (type == "All")
                            {
                                GenerateDyanmicFoodItemsList(itemSubCat, rootPath);
                            }
                            else
                            {
                                if (itemSubCat.FoodCategoryId.ToString() == type)
                                {
                                    GenerateDyanmicFoodItemsList(itemSubCat, rootPath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void GenerateDyanmicFoodItemsList(SubCategory itemSubCat, string rootPath)
        {
            try
            {
                StackPanel menuListPanel = new StackPanel();
                menuListPanel.Orientation = Orientation.Vertical;
                menuListPanel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#808080"));

                Image imgFood = new Image();
                try
                {
                    if (!string.IsNullOrEmpty(itemSubCat.SmallThumb))
                    {
                        string directory = Path.GetDirectoryName(rootPath + @"\Images\");
                        string filePath = Path.Combine(directory, itemSubCat.SmallThumb);

                        if (File.Exists(filePath))
                        {
                            imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\" + itemSubCat.SmallThumb));
                        }
                    }
                    else
                    {
                        imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\defaultimage.png"));
                    }
                }
                catch (Exception)
                {
                    imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\defaultimage.png"));
                }
                imgFood.Width = 96;
                imgFood.Height = 70;
                imgFood.Margin = new Thickness(2, 2, 2, 2);
                imgFood.Stretch = Stretch.UniformToFill;
                imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
                menuListPanel.Children.Add(imgFood);

                StackPanel menuName = new StackPanel();
                menuName.Height = 33;
                menuName.Width = 96;

                TextBlock txtSmallName = new TextBlock();
                txtSmallName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtSmallName.Text = itemSubCat.SmallName;
                txtSmallName.TextWrapping = TextWrapping.Wrap;
                txtSmallName.ToolTip = itemSubCat.SmallName;
                txtSmallName.FontWeight = FontWeights.Bold;
                txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
                menuName.Children.Add(txtSmallName);

                menuListPanel.Children.Add(menuName);

                TextBlock txtSalePrice = new TextBlock();
                txtSalePrice.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtSalePrice.Text = Convert.ToDecimal(itemSubCat.SalesPrice).ToString("0.00");
                txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
                txtSalePrice.FontWeight = FontWeights.Bold;
                menuListPanel.Children.Add(txtSalePrice);

                TextBlock txtFoodMenuId = new TextBlock();
                txtFoodMenuId.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtFoodMenuId.Text = itemSubCat.FoodMenuId.ToString();
                txtFoodMenuId.FontSize = 1;
                txtFoodMenuId.Name = "txtFoodMenuId" + itemSubCat.FoodMenuId;
                txtFoodMenuId.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtFoodMenuId);

                TextBlock txtFoodVat = new TextBlock();
                txtFoodVat.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtFoodVat.Text = Convert.ToDecimal(itemSubCat.FoodVat).ToString("0.00");
                txtFoodVat.FontSize = 0.5;
                txtFoodVat.Name = "txtFoodVat" + itemSubCat.FoodCategoryId;
                txtFoodVat.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtFoodVat);

                TextBlock txtFoodcess = new TextBlock();
                txtFoodcess.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtFoodcess.Text = Convert.ToDecimal(itemSubCat.Foodcess).ToString("0.00");
                txtFoodcess.FontSize = 0.5;
                txtFoodcess.Name = "txtFoodcess" + itemSubCat.FoodCategoryId;
                txtFoodcess.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtFoodcess);

                TextBlock txtTaxPercentage = new TextBlock();
                txtTaxPercentage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtTaxPercentage.Text = Convert.ToDecimal(itemSubCat.TaxPercentage).ToString("0.00");
                txtTaxPercentage.FontSize = 0.5;
                txtTaxPercentage.Name = "txtTaxPercentage" + itemSubCat.FoodCategoryId;
                txtTaxPercentage.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtTaxPercentage);

                TextBlock txtIsVatable = new TextBlock();
                txtIsVatable.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtIsVatable.Text = Convert.ToInt32(itemSubCat.IsVatable).ToString();
                txtIsVatable.FontSize = 0.5;
                txtIsVatable.Name = "txtIsVatable" + itemSubCat.FoodCategoryId;
                txtIsVatable.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtIsVatable);

                TextBlock txtIsPriceChange = new TextBlock();
                txtIsPriceChange.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtIsPriceChange.Text = Convert.ToInt32(itemSubCat.IsPriceChange).ToString();
                txtIsPriceChange.FontSize = 0.5;
                txtIsPriceChange.Name = "txtIsPriceChange" + itemSubCat.FoodCategoryId;
                txtIsPriceChange.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtIsPriceChange);

                menuListPanel.Name = "childPanel" + itemSubCat.FoodCategoryId;
                menuListPanel.MouseDown += GetPrice_MouseDown;
                spSubCategory.Children.Add(menuListPanel);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void ClearCustomerOrderItemControll()
        {
            try
            {
                applyRedeem = false;
                btnRedeemPoints.IsChecked = false;
                dgSaleItem.Items.Clear();
                cmbWaiter.Text = "Select Waiter";
                cmbWaiter.SelectedIndex = -1;
                //  cmbCustomer.Text = "Select Customer";

                if (cmbCustomer.Items.Count > 0)
                {
                    cmbCustomer.SelectedIndex = 1;
                }
                else
                {
                    cmbCustomer.SelectedIndex = -1;
                }
                txtbTotalPayableAmount.Text = "0.00";
                txtbSubTotalAmount.Text = "0.00";
                txtbTotalItemCount.Text = "0.00";
                txtbOrderId.Text = "0";
                rdbDeliveryOrderType.IsChecked = false;
                rdbDineInOrderType.IsChecked = false;
                rdbTakeAwayOrderType.IsChecked = false;
                txtbtxtDiscount.Text = "0.00";
                txtbServiceDeliveryChargeLabel.Text = "0.00";
                txtSubTotalDiscountAmount.Text = "0.00";
                txtbTotalDiscountAmount.Text = "0.00";
                txtbTotalDeliveryChargeAmt.Text = "0.00";
                lbTablesList.SelectedIndex = -1;
                txtAllocatedPerson.Text = string.Empty;
                lbPPDiscountPercent.SelectedIndex = -1;
                lbPPPercentageDelivery.SelectedIndex = -1;
                txtbKitchenStatusTitle.Visibility = Visibility.Hidden;
                txtbKitchenStatus.Visibility = Visibility.Hidden;
                txtDiscountPassword.Password = string.Empty;
                txtTaxAmount.Text = "0.00";
                //  btnEditCustomer.IsEnabled = false;
                txtPPPayAmount.Text = "";
                lblPPChangeAmountTotal.Content = "";
                txtTableNumber.Text = "";
                txtbDineInTableId.Text = "";
                txtVatableAmount.Text = "0.00";
                txtNonVatableAmount.Text = "0.00";
                txtRedeemAmount.Text = "";
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private decimal GetPercentageAmount(decimal amount, decimal percent)
        {
            try
            {
                if (percent != 0)
                {
                    if (LoginDetail.TaxInclusive == 1)
                    {
                        return (amount - ((amount / (100 + percent)) * 100));
                    }
                    else
                    {
                        return ((amount * percent) / 100);
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
                return 0;
            }
        }
        private void CommonOrderCalculation(object sender, string type)
        {
            try
            {
                if (type == "FoodMenu")
                {

                    var menuListPanel = sender as StackPanel;
                    var salePrice = menuListPanel.Children[2] as TextBlock;
                    var foodVat = menuListPanel.Children[4] as TextBlock;
                    var foodCess = menuListPanel.Children[5] as TextBlock;
                    var taxPercentage = menuListPanel.Children[6] as TextBlock;
                    var isVatable = menuListPanel.Children[7] as TextBlock;

                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
                    txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
                    txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + GetPercentageAmount(Convert.ToDecimal(salePrice.Text), Convert.ToDecimal(taxPercentage.Text))).ToString("0.00");
                    if (Convert.ToInt32(isVatable.Text) == 1)
                    {
                        txtVatableAmount.Text = (Convert.ToDecimal(txtVatableAmount.Text) + (Convert.ToDecimal(salePrice.Text) - GetPercentageAmount(Convert.ToDecimal(salePrice.Text), Convert.ToDecimal(taxPercentage.Text)))).ToString("0.00");
                    }
                    else
                    {
                        txtNonVatableAmount.Text = (Convert.ToDecimal(txtNonVatableAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString("0.00");
                    }
                }
                if (type == "FoodMenuGridListQty")
                {
                    decimal oldTotal = 0, oldVatable = 0, OldNonVatable = 0, OldTaxPercentage = 0, oldTaxAmount = 0, oldPrice = 0, oldQty = 0;
                    bool isFound = false;
                    if (dgSaleItem != null)
                    {
                        for (int i = 0; i < dgSaleItem.Items.Count; i++)
                        {
                            var gridSaleitem = (List<SaleItemModel>)dgSaleItem.Items[i];
                            if (saleItemsFoodMenu.FoodMenuId.Equals(gridSaleitem[0].FoodMenuId))
                            {
                                isFound = true;
                                oldTotal = gridSaleitem[0].Total;
                                OldTaxPercentage = gridSaleitem[0].TaxPercentage;
                                oldPrice = gridSaleitem[0].Price;
                                oldQty = gridSaleitem[0].Qty;

                                oldTaxAmount = GetPercentageAmount(Convert.ToDecimal(oldTotal), Convert.ToDecimal(OldTaxPercentage));

                                if (Convert.ToInt32(gridSaleitem[0].IsVatable) == 1)
                                {
                                    oldVatable = Convert.ToDecimal(oldTotal) - (GetPercentageAmount(Convert.ToDecimal(oldTotal), Convert.ToDecimal(OldTaxPercentage)));
                                }
                                else
                                {
                                    OldNonVatable = oldTotal;
                                }

                                gridSaleitem[0].Qty = Convert.ToDecimal(txtEditQty.Text.ToString());
                                gridSaleitem[0].Total = gridSaleitem[0].Qty * gridSaleitem[0].Price;
                            }
                        }
                    }

                    if (oldQty == 0)
                    {
                        txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + (Convert.ToDecimal(txtEditQty.Text))).ToString();
                    }
                    else
                    {
                        txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbTotalItemCount.Text) - oldQty) + Convert.ToDecimal(txtEditQty.Text)).ToString();
                    }

                    var salesQty = saleItemsFoodMenu.Qty;
                    var salePrice = saleItemsFoodMenu.Total;
                    var foodVat = saleItemsFoodMenu.FoodVat;
                    var foodCess = saleItemsFoodMenu.Foodcess;
                    var taxPercentage = saleItemsFoodMenu.TaxPercentage;
                    var isVatable = saleItemsFoodMenu.IsVatable;

                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - oldTotal + Convert.ToDecimal(salePrice)).ToString("0.00");
                    txtTaxAmount.Text = ((Convert.ToDecimal(txtTaxAmount.Text) - oldTaxAmount) + GetPercentageAmount(Convert.ToDecimal(salePrice), Convert.ToDecimal(taxPercentage))).ToString("0.00");
                    if (Convert.ToInt32(isVatable) == 1)
                    {
                        txtVatableAmount.Text = ((Convert.ToDecimal(txtVatableAmount.Text) - oldVatable) + (Convert.ToDecimal(salePrice)) - GetPercentageAmount(Convert.ToDecimal(salePrice), Convert.ToDecimal(taxPercentage))).ToString("0.00");
                    }
                    else
                    {
                        txtNonVatableAmount.Text = ((Convert.ToDecimal(txtNonVatableAmount.Text) - OldNonVatable) + Convert.ToDecimal(salePrice)).ToString("0.00");
                    }
                }

                if (type == "FoodMenuGridList")
                {
                    FoodMenu foodMenuItem = new FoodMenu();

                    // foodMenuItem = ;

                    if (saleItemsFoodMenu == null) return;

                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(saleItemsFoodMenu.Total)).ToString();
                    txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
                    //txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + Convert.ToDecimal(foodMenuItem.FoodVat) + Convert.ToDecimal(foodMenuItem.Foodcess)).ToString();
                    txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + GetPercentageAmount(Convert.ToDecimal(saleItemsFoodMenu.Total), Convert.ToDecimal(saleItemsFoodMenu.TaxPercentage))).ToString("0.00");
                    if (Convert.ToInt32(saleItemsFoodMenu.IsVatable) == 1)
                    {
                        txtVatableAmount.Text = (Convert.ToDecimal(txtVatableAmount.Text) + (Convert.ToDecimal(saleItemsFoodMenu.Total) - GetPercentageAmount(Convert.ToDecimal(saleItemsFoodMenu.Total), Convert.ToDecimal(saleItemsFoodMenu.TaxPercentage)))).ToString("0.00");
                    }
                    else
                    {
                        txtNonVatableAmount.Text = (Convert.ToDecimal(txtNonVatableAmount.Text) + Convert.ToDecimal(saleItemsFoodMenu.Total)).ToString("0.00");
                    }
                }

                if (type == "DiscountPercent")
                {
                    decimal percentage = Convert.ToDecimal(lbPPDiscountPercent.SelectedValue);
                    txtbtxtDiscount.Text = Convert.ToDecimal(percentage).ToString("0.00");
                    txtbTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString("0.00");
                    txtSubTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString();
                }

                if (type == "DiscountAmount")
                {
                    txtbTotalDiscountAmount.Text = Convert.ToDecimal(txtSubTotalDiscountAmount.Text).ToString("0.00");
                }

                if (type == "DeliveryCharge")
                {
                    decimal percentage = Convert.ToDecimal(lbPPPercentageDelivery.SelectedValue);
                    txtbServiceDeliveryChargeLabel.Text = percentage.ToString("0.00");
                    txtbTotalDeliveryChargeAmt.Text = (percentage).ToString("0.00");
                }

                //if (string.IsNullOrEmpty(txtSubTotalDiscountAmount.Text))
                //   txtbTotalDiscountAmount.Text = "0.00";

                txtbTotalPayableAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(txtbTotalDiscountAmount.Text)) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void GetFoodItems(string type)
        {
            try
            {
                AppSettings appSettings = new AppSettings();
                string rootPath = appSettings.GetAppPath();// new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                spSubCategory.Children.Clear();
                FoodMenuModel foodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];
                if (type == "All")
                {
                    GenerateDynamicFoodItems(foodMenu, rootPath, string.Empty, type);
                }
                else
                {
                    GenerateDynamicFoodItems(foodMenu, rootPath, string.Empty, type);
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void GetSearchFoodItems(string searchKey)
        {
            try
            {
                AppSettings appSettings = new AppSettings();
                string rootPath = appSettings.GetAppPath();// new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                spSubCategory.Children.Clear();
                FoodMenuModel foodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];
                GenerateDynamicFoodItems(foodMenu, rootPath, searchKey, string.Empty);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void GetWaiterList()
        {
            try
            {
                CommonViewModel commonViewModel = new CommonViewModel();
                List<WaiterModel> waiters = new List<WaiterModel>();
                waiters = commonViewModel.GetWaiters();
                cmbWaiter.ItemsSource = waiters;
                cmbWaiter.Text = "Select Waiter";
                cmbWaiter.IsEditable = true;
                cmbCustomer.IsReadOnly = true;
                cmbWaiter.SelectedValuePath = "Id";
                cmbWaiter.DisplayMemberPath = "FullName";
                cmbWaiter.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void GetOrderList(int orderStatus, int orderType, string searchKey)
        {
            try
            {
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                List<CustomerOrderModel> customerOrderList = new List<CustomerOrderModel>();
                customerOrderList = customerOrderViewModel.GetCustomerOrderList(orderStatus, orderType, searchKey);
                lbCustomerOrderList.ItemsSource = customerOrderList;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private int PlaceOrder(string type)
        {
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            CustomerOrderModel customerOrderModel = new CustomerOrderModel();
            CustomerOrderItemModel customerOrderItemModel = new CustomerOrderItemModel();
            List<CustomerOrderItemModel> customerOrderItemModels = new List<CustomerOrderItemModel>();
            DataTable customerOrderItem = new DataTable();
            int insertedId = 0;
            int orderType = 0;
            string tableId = null;
            string waiterId = null;

            if (rdbDineInOrderType.IsChecked == true)
            {
                orderType = (int)EnumUtility.OrderType.DineIN;
                if (lbTablesList.SelectedIndex != -1)
                {
                    tableId = lbTablesList.SelectedValue.ToString();
                }
                else
                {
                    tableId = txtbDineInTableId.Text;
                }
            }
            else if (rdbTakeAwayOrderType.IsChecked == true)
            {
                orderType = (int)EnumUtility.OrderType.TakeAway;
            }
            else if (rdbDeliveryOrderType.IsChecked == true)
            {
                orderType = (int)EnumUtility.OrderType.Delivery;
            }

            if (cmbWaiter.SelectedValue != null)
            {
                waiterId = cmbWaiter.SelectedValue.ToString();
            }

            if (Convert.ToInt32(txtbOrderId.Text) == 0)
            {
                customerOrderItem.Columns.Add("CustomerOrderItemId", typeof(Int64));
                customerOrderItem.Columns.Add("FoodMenuId", typeof(Int32));
                customerOrderItem.Columns.Add("FoodMenuRate", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuQty", typeof(decimal));
                customerOrderItem.Columns.Add("AddonsId", typeof(Int32));
                customerOrderItem.Columns.Add("AddonsQty", typeof(decimal));
                customerOrderItem.Columns.Add("VarientId", typeof(Int32));
                customerOrderItem.Columns.Add("Discount", typeof(decimal));
                customerOrderItem.Columns.Add("Price", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuVat", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuCess", typeof(decimal));

                var saleItems = dgSaleItem.Items.OfType<List<SaleItemModel>>().ToList();
                foreach (var saleItem in saleItems)
                {
                    customerOrderItem.Rows.Add(0,
                                        Convert.ToInt32(saleItem[0].FoodMenuId),
                                        Convert.ToDecimal(saleItem[0].Price),
                                        saleItem[0].Qty,
                                        0,
                                        0,
                                        0,
                                        Convert.ToDecimal(saleItem[0].Discount),
                                        Convert.ToDecimal(saleItem[0].Total),
                                        Convert.ToDecimal(saleItem[0].FoodVat),
                                        Convert.ToDecimal(saleItem[0].Foodcess));
                }
                customerOrderModel.Id = 0;
                customerOrderModel.OutletId = LoginDetail.OutletId;
                customerOrderModel.SalesInvoiceNumber = null;
                customerOrderModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerOrderModel.WaiterEmployeeId = waiterId;
                customerOrderModel.OrderType = orderType;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = tableId;
                customerOrderModel.AllocatedPerson = txtAllocatedPerson.Text;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = Convert.ToDecimal(txtbtxtDiscount.Text);
                customerOrderModel.DiscountAmount = Convert.ToDecimal(txtbTotalDiscountAmount.Text);
                customerOrderModel.DeliveryCharges = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
                customerOrderModel.VatableAmount = Convert.ToDecimal(txtVatableAmount.Text);
                customerOrderModel.NonVatableAmount = Convert.ToDecimal(txtNonVatableAmount.Text);
                customerOrderModel.TaxAmount = Convert.ToDecimal(txtTaxAmount.Text);
                customerOrderModel.TotalPayable = Convert.ToDecimal(txtbTotalPayableAmount.Text);
                customerOrderModel.CustomerPaid = 0;
                customerOrderModel.CustomerNote = null;
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
                customerOrderModel.AnyReason = null;
                customerOrderModel.UserIdInserted = LoginDetail.UserId;
                customerOrderModel.DateInserted = System.DateTime.Now;
                customerOrderModel.KotStatus = (int)EnumUtility.KOTStatus.Pending;
            }
            else
            {
                customerOrderItem.Columns.Add("CustomerOrderItemId", typeof(Int64));
                customerOrderItem.Columns.Add("FoodMenuId", typeof(Int32));
                customerOrderItem.Columns.Add("FoodMenuRate", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuQty", typeof(decimal));
                customerOrderItem.Columns.Add("AddonsId", typeof(Int32));
                customerOrderItem.Columns.Add("AddonsQty", typeof(decimal));
                customerOrderItem.Columns.Add("VarientId", typeof(Int32));
                customerOrderItem.Columns.Add("Discount", typeof(decimal));
                customerOrderItem.Columns.Add("Price", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuVat", typeof(decimal));
                customerOrderItem.Columns.Add("FoodMenuCess", typeof(decimal));

                var saleItems = dgSaleItem.Items.OfType<List<SaleItemModel>>().ToList();
                foreach (var saleItem in saleItems)
                {
                    customerOrderItem.Rows.Add(Convert.ToInt32(saleItem[0].CustomerOrderItemId),
                                        Convert.ToInt32(saleItem[0].FoodMenuId),
                                        Convert.ToDecimal(saleItem[0].Price),
                                        saleItem[0].Qty,
                                        0,
                                        0,
                                        0,
                                        Convert.ToDecimal(saleItem[0].Discount),
                                        Convert.ToDecimal(saleItem[0].Total),
                                        Convert.ToDecimal(saleItem[0].FoodVat),
                                        Convert.ToDecimal(saleItem[0].Foodcess));
                }
                customerOrderModel.Id = Convert.ToInt32(txtbOrderId.Text);
                customerOrderModel.OutletId = LoginDetail.OutletId;
                customerOrderModel.SalesInvoiceNumber = null;
                customerOrderModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerOrderModel.WaiterEmployeeId = waiterId;
                customerOrderModel.OrderType = orderType;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = tableId;
                customerOrderModel.AllocatedPerson = txtAllocatedPerson.Text;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = Convert.ToDecimal(txtbtxtDiscount.Text);
                customerOrderModel.DiscountAmount = Convert.ToDecimal(txtSubTotalDiscountAmount.Text);
                customerOrderModel.DeliveryCharges = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
                customerOrderModel.VatableAmount = Convert.ToDecimal(txtVatableAmount.Text);
                customerOrderModel.NonVatableAmount = Convert.ToDecimal(txtNonVatableAmount.Text);
                customerOrderModel.TaxAmount = Convert.ToDecimal(txtTaxAmount.Text);
                customerOrderModel.TotalPayable = Convert.ToDecimal(txtbTotalPayableAmount.Text);
                customerOrderModel.CustomerPaid = 0;
                customerOrderModel.CustomerNote = null;
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
                customerOrderModel.AnyReason = null;
                customerOrderModel.UserIdInserted = LoginDetail.UserId;
                customerOrderModel.DateInserted = System.DateTime.Now;
                customerOrderModel.KotStatus = (int)EnumUtility.KOTStatus.Pending;
            }

            if (type == "DirectInvoice")
            {
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.FullPaid;
                customerOrderModel.KotStatus = (int)EnumUtility.KOTStatus.Completed;
                if (rdbDineInOrderType.IsChecked == true)
                {
                    TableViewModel tableViewModel = new TableViewModel();
                    tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Clean);
                }
            }
            else if (type == "Hold")
            {
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Hold;
                customerOrderModel.KotStatus = (int)EnumUtility.KOTStatus.Pending;
            }
            else
            {
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
                customerOrderModel.KotStatus = (int)EnumUtility.KOTStatus.Pending;
                if (rdbDineInOrderType.IsChecked == true)
                {
                    TableViewModel tableViewModel = new TableViewModel();
                    tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Occupied);
                }

            }

            insertedId = customerOrderViewModel.InsertCustomerOrder(customerOrderModel, customerOrderItem);
            txtbOrderId.Text = insertedId.ToString();

            if (insertedId > 0)
            {
                if (type == "Hold")
                {
                    SplashScreen splash = new SplashScreen("Images/OrderHold.PNG");
                    splash.Show(true);

                    Thread.Sleep(2000);

                }
                else if (type != "DirectInvoice")
                {
                    SplashScreen splash = new SplashScreen("Images/PlaceOrder.PNG");
                    splash.Show(true);

                    Thread.Sleep(2000);
                }

                if (rdbDineInOrderType.IsChecked == false && !string.IsNullOrEmpty(txtbDineInTableId.Text))
                {
                    TableViewModel tableViewModel = new TableViewModel();
                    tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Clean);
                }

                GetOrderList((int)EnumUtility.OrderPaidStatus.Pending, (int)EnumUtility.OrderType.All, string.Empty);
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.PlaceOrderFailed, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
            }

            return insertedId;
        }

        private void ResetCustomer()
        {
            try
            {
                txtPPCName.Text = string.Empty;
                txtPPCPhone.Text = string.Empty;
                txtPPCEmail.Text = string.Empty;
                txtPPCAddress.Text = string.Empty;
                txtPPCAddress2.Text = string.Empty;
                dpBirthDate.Text = "";
                dpAnniversaryDate.Text = "";
                txtRedeemPoints.Text = "Redeem";
                //  cmbCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        #endregion
        #region Events
        private void GetSubCategory(object sender, RoutedEventArgs e)
        {
            try
            {
                var btnCategory = sender as Button;

                foreach (var btn in spCategory.Children.OfType<Button>().Where(x => x.Name.StartsWith("btn")))
                    btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));

                foreach (var btn in spFavouriteCategory.Children.OfType<Button>().Where(x => x.Name.StartsWith("btn")))
                    btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));

                btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bcddee"));

                var categoryId = btnCategory.Name.Substring(3);//Get the button id
                GetFoodItems(categoryId);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void GetPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                var menuListPanel = sender as StackPanel;

                var menuNamePanel = menuListPanel.Children[1] as StackPanel;
                var itemName = menuNamePanel.Children[0] as TextBlock;

                var salePrice = menuListPanel.Children[2] as TextBlock;
                var foodMenuId = menuListPanel.Children[3] as TextBlock;
                var foodVat = menuListPanel.Children[4] as TextBlock;
                var foodcess = menuListPanel.Children[5] as TextBlock;
                var taxPercentage = menuListPanel.Children[6] as TextBlock;
                var isVatable = menuListPanel.Children[7] as TextBlock;
                var txtIsPriceChange = menuListPanel.Children[8] as TextBlock;

                //if (Convert.ToInt32(txtIsPriceChange.Text) == 2)
                //{
                txtEditQty.Text = "1";
                saleItemsFoodMenu.FoodMenuId = foodMenuId.Text.ToString();
                saleItemsFoodMenu.Product = itemName.Text;
                saleItemsFoodMenu.Price = Convert.ToDecimal(salePrice.Text);
                saleItemsFoodMenu.Qty = 1.0m;
                saleItemsFoodMenu.Discount = 0;
                saleItemsFoodMenu.Total = Convert.ToDecimal(salePrice.Text) * 1;
                saleItemsFoodMenu.CustomerOrderItemId = 0;
                saleItemsFoodMenu.Foodcess = Convert.ToDecimal(foodcess.Text);
                saleItemsFoodMenu.FoodVat = Convert.ToDecimal(foodVat.Text);
                saleItemsFoodMenu.TaxPercentage = Convert.ToDecimal(taxPercentage.Text);
                saleItemsFoodMenu.IsVatable = Convert.ToInt32(isVatable.Text);

                txtQtyPopUpProductName.Text = saleItemsFoodMenu.Product;
                txtChnageQty.Text = saleItemsFoodMenu.Qty.ToString();

                ppEditQty.IsOpen = true;
                return;
                // }

                CommonOrderCalculation(sender, "FoodMenu");

                List<SaleItemModel> saleItems = new List<SaleItemModel>();
                saleItems.Add(new SaleItemModel()
                {
                    FoodMenuId = foodMenuId.Text,
                    Product = itemName.Text,
                    Price = Convert.ToDecimal(salePrice.Text),
                    Qty = 1.0m,
                    Discount = 0,
                    Total = Convert.ToDecimal(salePrice.Text) * 1,
                    CustomerOrderItemId = 0,
                    FoodVat = Convert.ToDecimal(foodVat.Text),
                    Foodcess = Convert.ToDecimal(foodcess.Text),
                    TaxPercentage = Convert.ToDecimal(taxPercentage.Text),
                    IsVatable = Convert.ToInt32(isVatable.Text)
                });

                bool isFound = false;
                if (dgSaleItem != null)
                {
                    for (int i = 0; i < dgSaleItem.Items.Count; i++)
                    {
                        var gridSaleitem = (List<SaleItemModel>)dgSaleItem.Items[i];
                        if (saleItems[0].FoodMenuId.Equals(gridSaleitem[0].FoodMenuId))
                        {
                            isFound = true;
                            gridSaleitem[0].Qty += 1;
                            gridSaleitem[0].Total = gridSaleitem[0].Qty * gridSaleitem[0].Price;
                        }
                    }
                }

                if (!isFound)
                {
                    dgSaleItem.Items.Add(saleItems);
                }
                else
                {
                    dgSaleItem.Items.Refresh();
                }

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        //private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    GetSearchFoodItems(txtSearch.Text.ToLower());
        //}
        private void btnPlusQty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SaleItemModel> saleItem = new List<SaleItemModel>();
                object foodItem = dgSaleItem.SelectedItem;
                saleItem = (List<SaleItemModel>)foodItem;
                saleItem[0].Qty += 1;
                saleItem[0].Total += saleItem[0].Price * 1;
                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(saleItem[0].Price)).ToString("0.00");
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(saleItem[0].Price) + (Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess))).ToString("0.00");
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString("0.00");
                //txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess)).ToString();

                txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + GetPercentageAmount(Convert.ToDecimal(saleItem[0].Price), Convert.ToDecimal(saleItem[0].TaxPercentage))).ToString("0.00");
                if (Convert.ToInt32(saleItem[0].IsVatable) == 1)
                {
                    txtVatableAmount.Text = (Convert.ToDecimal(txtVatableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) - GetPercentageAmount(Convert.ToDecimal(saleItem[0].Price), Convert.ToDecimal(saleItem[0].TaxPercentage)))).ToString("0.00");
                }
                else
                {
                    txtNonVatableAmount.Text = (Convert.ToDecimal(txtNonVatableAmount.Text) + Convert.ToDecimal(saleItem[0].Price)).ToString("0.00");
                }
                CommonOrderCalculation(null, string.Empty);
                dgSaleItem.Items.Refresh();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnMinusQty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SaleItemModel> saleItem = new List<SaleItemModel>();
                object foodItem = dgSaleItem.SelectedItem;
                saleItem = (List<SaleItemModel>)foodItem;
                saleItem[0].Qty -= 1;

                if (saleItem[0].Qty <= 0)
                {
                    saleItem[0].Price = saleItem[0].Total;
                    saleItem[0].Total = 0;
                }
                else
                {
                    saleItem[0].Total = Math.Round((saleItem[0].Qty * saleItem[0].Price), 2);
                }

                //saleItem[0].Total -= saleItem[0].Price * 1;

                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(saleItem[0].Price)).ToString("0.00");
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) - Convert.ToDecimal(saleItem[0].Price) - (Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess))).ToString("0.00");
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) - 1).ToString();
                txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) - GetPercentageAmount(Convert.ToDecimal(saleItem[0].Price), Convert.ToDecimal(saleItem[0].TaxPercentage))).ToString("0.00");

                if (Convert.ToInt32(saleItem[0].IsVatable) == 1)
                {
                    txtVatableAmount.Text = (Convert.ToDecimal(txtVatableAmount.Text) - (Convert.ToDecimal(saleItem[0].Price) - GetPercentageAmount(Convert.ToDecimal(saleItem[0].Price), Convert.ToDecimal(saleItem[0].TaxPercentage)))).ToString("0.00");
                }
                else
                {
                    txtNonVatableAmount.Text = (Convert.ToDecimal(txtNonVatableAmount.Text) - Convert.ToDecimal(saleItem[0].Price)).ToString("0.00");
                }

                if (saleItem[0].Qty <= 0)
                {
                    dgSaleItem.Items.RemoveAt(dgSaleItem.SelectedIndex);
                }


                if (Convert.ToDecimal(txtbSubTotalAmount.Text) <= 0)
                    txtbSubTotalAmount.Text = "0.00";
                if (Convert.ToDecimal(txtVatableAmount.Text) <= 0)
                    txtVatableAmount.Text = "0.00";
                if (Convert.ToDecimal(txtNonVatableAmount.Text) <= 0)
                    txtNonVatableAmount.Text = "0.00";
                if (Convert.ToDecimal(txtTaxAmount.Text) <= 0)
                {
                    txtTaxAmount.Text = "0.00";
                    txtbTotalItemCount.Text = "0";
                }

                CommonOrderCalculation(null, string.Empty);
                dgSaleItem.Items.Refresh();

                if (dgSaleItem.Items.Count <= 0)
                {
                    ClearCustomerOrderItemControll();
                }

                if (Convert.ToDecimal(txtbTotalDiscountAmount.Text) > Convert.ToDecimal(txtbSubTotalAmount.Text))
                {
                    txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString();
                    txtbTotalDiscountAmount.Text = "0.00";
                    applyRedeem = false;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnEditSaleItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SaleItemModel> saleItem = new List<SaleItemModel>();
                object foodItem = dgSaleItem.SelectedItem;
                saleItem = (List<SaleItemModel>)foodItem;
                txtbPopUpProductName.Text = saleItem[0].Product;
                txtPPPrice.Text = "";// saleItem[0].Price.ToString();
                txtPPOriginalPrice.Text = saleItem[0].Price.ToString();
                ppEditPrice.IsOpen = true;

                //txtbPopUpItemOriginalTotal.Text = saleItem[0].Price.ToString();
                //txtbPopUpOriginalQtyCount.Text = saleItem[0].Qty.ToString();
                //txtbPopUpItemOriginalSubTotalAmount.Text = saleItem[0].Price.ToString();
                //txtbPopUpItemName.Text = saleItem[0].Product;
                //txtbPopUpQtyCount.Text = saleItem[0].Qty.ToString();
                //txtbPopUpItemTotal.Text = saleItem[0].Total.ToString();
                //txtPopUpDiscount.Text = saleItem[0].Discount.ToString();
                //txtbPopUpItemSubTotalAmount.Text = Convert.ToDecimal(saleItem[0].Total).ToString();
                //EditSaleItemPopUp.IsOpen = true;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPopUpPlusQty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtbPopUpQtyCount.Text = (Convert.ToDecimal(txtbPopUpQtyCount.Text) + 1).ToString();
                txtbPopUpItemTotal.Text = (Convert.ToInt32(txtbPopUpItemTotal.Text) + (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
                txtbPopUpItemSubTotalAmount.Text = (Convert.ToInt32(txtbPopUpItemSubTotalAmount.Text) + (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPopUpMinusQty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtbPopUpQtyCount.Text != "1")
                {
                    txtbPopUpQtyCount.Text = (Convert.ToDecimal(txtbPopUpQtyCount.Text) - 1).ToString();
                    txtbPopUpItemTotal.Text = (Convert.ToInt32(txtbPopUpItemTotal.Text) - (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
                    txtbPopUpItemSubTotalAmount.Text = (Convert.ToInt32(txtbPopUpItemSubTotalAmount.Text) - (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditSaleItemPopUp.IsOpen = false;
                dgSaleItem.Items.Refresh();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgSaleItem.Items.Count == 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.CartEmpty, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                if (rdbDeliveryOrderType.IsChecked == false && rdbDineInOrderType.IsChecked == false && rdbTakeAwayOrderType.IsChecked == false)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectOrderType, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                string tableId = null;
                if (lbTablesList.SelectedIndex != -1)
                {
                    tableId = lbTablesList.SelectedValue.ToString();
                }
                else if (!String.IsNullOrEmpty(txtbDineInTableId.Text.ToString()))
                {
                    tableId = txtbDineInTableId.Text;
                }

                if (rdbDineInOrderType.IsChecked == true && String.IsNullOrEmpty(tableId))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.TableNumberNoSelected, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                //if (cmbWaiter.SelectedIndex == -1)
                //{
                //    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectWaiter, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                //    cmbWaiter.Focus();
                //    return;
                //}

                if (cmbCustomer.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectCustomer, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbCustomer.Focus();
                    return;
                }
                txtbKitchenStatusTitle.Visibility = Visibility.Hidden;
                txtbKitchenStatus.Visibility = Visibility.Hidden;
                PlaceOrder("Pending");
                ClearCustomerOrderItemControll();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SaleItemModel> saleItem = new List<SaleItemModel>();
                object foodItem = dgSaleItem.SelectedItem;
                saleItem = (List<SaleItemModel>)foodItem;
                saleItem[0].Qty = Convert.ToDecimal(txtbPopUpQtyCount.Text);
                saleItem[0].Total = Math.Round(Convert.ToDecimal(txtbPopUpItemSubTotalAmount.Text), 2);
                saleItem[0].Discount = Convert.ToDecimal(txtPopUpDiscount.Text);
                if (txtbPopUpQtyCount.Text != "1")
                {
                    txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)) + Convert.ToDecimal(txtbTotalItemCount.Text)).ToString("0.00");
                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString("0.00");
                    txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString("0.00");

                }
                else if (txtbPopUpQtyCount.Text == "1" && txtbPopUpQtyCount.Text != txtbPopUpOriginalQtyCount.Text)
                {
                    txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)) + Convert.ToDecimal(txtbTotalItemCount.Text)).ToString("0.00");
                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString("0.00");
                    txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString("0.00");
                }
                EditSaleItemPopUp.IsOpen = false;
                dgSaleItem.Items.Refresh();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void txtPopUpDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPopUpDiscount.Text))
                {
                    if ((Convert.ToDecimal(txtPopUpDiscount.Text) != 0))
                    {
                        txtbPopUpItemSubTotalAmount.Text = (Convert.ToDecimal(txtbPopUpItemTotal.Text) - ((Convert.ToDecimal(txtPopUpDiscount.Text) / 100) * Convert.ToDecimal(txtbPopUpItemTotal.Text))).ToString();
                    }
                }
                else
                {
                    txtbPopUpItemSubTotalAmount.Text = txtbPopUpItemTotal.Text;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableViewModel tableViewModel = new TableViewModel();
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                string orderId = string.Empty;
                int insertedId = 0;

                if (rdbDeliveryOrderType.IsChecked == false && rdbDineInOrderType.IsChecked == false && rdbTakeAwayOrderType.IsChecked == false)
                {
                    var messageBoxResult1 = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectOrderType, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                if (dgSaleItem.Items.Count == 0)
                {
                    var messageBoxResult2 = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.CartEmpty, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                if (cmbCustomer.SelectedIndex == -1)
                {
                    var messageBoxResult3 = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectCustomer, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbCustomer.Focus();
                    return;
                }

                var messageBoxResult = WpfMessageBox.Show(StatusMessages.CancelOrderTitle, StatusMessages.CancelOrder, MessageBoxButton.YesNo, EnumUtility.MessageBoxImage.Question);
                if (messageBoxResult.ToString() == "Yes")
                {
                    orderId = txtbOrderId.Text;
                    if (!string.IsNullOrEmpty(orderId) && orderId != "0")
                    {

                        insertedId = customerOrderViewModel.CancelOrder(orderId);
                        if (insertedId > 0)
                        {
                            SplashScreen splash = new SplashScreen("Images/OrderCancel.PNG");
                            splash.Show(true);

                            Thread.Sleep(2000);

                            ClearCustomerOrderItemControll();
                            GetOrderList((int)EnumUtility.OrderPaidStatus.Pending, (int)EnumUtility.OrderType.All, string.Empty);
                        }
                        else
                        {
                            var messageBoxFailResult = WpfMessageBox.Show(StatusMessages.CancelOrderTitle, StatusMessages.CancelOrderFail, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        ClearCustomerOrderItemControll();
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnCalculator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Calculator winCalCulator = new Calculator();

                winCalCulator.Width = 237;
                winCalCulator.Height = 310;
                winCalCulator.Top = 100;
                winCalCulator.Left = 500;

                winCalCulator.ShowDialog();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportList reportList = new ReportList();
                //  reportList.Owner = Application.Current.MainWindow;
                reportList.ShowDialog();


                //OutletRegisterViewModel outletRegisterViewModel = new OutletRegisterViewModel();
                //OutletRegisterModel outletRegisterModel = new OutletRegisterModel();

                //Open regiter
                //outletRegisterModel.USerID = LoginDetail.UserId;
                //outletRegisterModel.OutletId = LoginDetail.OutletId;
                //outletRegisterModel.OpeningBalance = 500;

                // outletRegisterViewModel.InsertOutletRegister(outletRegisterModel);

                //if (lbCustomerOrderList.Items.Count > 0)
                //{
                //    var messageBoxResultCount = WpfMessageBox.Show(StatusMessages.AppTitle, "Clear the open order before closing register.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
                //    return;
                //}

                //var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Are you sure to close register? ", MessageBoxButton.YesNo, EnumUtility.MessageBoxImage.Warning);

                //if (messageBoxResult == MessageBoxResult.Yes)
                //{
                //    outletRegisterViewModel.UpdateOutletRegister(outletRegisterModel);
                //    WpfMessageBox.Show(StatusMessages.AppTitle, "Register closed successfully");

                //    OutletRegisterReport outletRegisterReport = new OutletRegisterReport();

                //    Login frmlogin = new Login();

                //    frmlogin.Show();

                //    outletRegisterReport.Show();

                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        #region Search Order Left
        private void btnModifyOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbCustomerOrderList.SelectedItem == null)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectOrder, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                ClearCustomerOrderItemControll();
                //txtbKitchenStatus.Visibility = Visibility.Visible;
                //txtbKitchenStatusTitle.Visibility = Visibility.Visible;
                var st = (CustomerOrderModel)lbCustomerOrderList.SelectedItem;

                CustomerOrderModel customerOrderModel = new CustomerOrderModel();
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                customerOrderModel = customerOrderViewModel.GetCustomerOrderByOrderId(st.Id);

                txtbSubTotalAmount.Text = Convert.ToDecimal(customerOrderModel.GrossAmount).ToString("0.00");
                txtbTotalPayableAmount.Text = customerOrderModel.TotalPayable.ToString();
                txtbOrderId.Text = customerOrderModel.Id.ToString();
                cmbCustomer.SelectedValue = customerOrderModel.CustomerId;
                cmbWaiter.SelectedValue = customerOrderModel.WaiterEmployeeId;
                txtbDineInTableId.Text = customerOrderModel.TableId;
                txtAllocatedPerson.Text = customerOrderModel.AllocatedPerson;
                txtSubTotalDiscountAmount.Text = Convert.ToDecimal(customerOrderModel.DiscountAmount).ToString("0.00");
                txtbTotalDiscountAmount.Text = Convert.ToDecimal(customerOrderModel.DiscountAmount).ToString("0.00");
                txtbTotalDeliveryChargeAmt.Text = Convert.ToDecimal(customerOrderModel.DeliveryCharges).ToString("0.00");
                txtTaxAmount.Text = Convert.ToDecimal(customerOrderModel.TaxAmount).ToString("0.00");
                txtVatableAmount.Text = Convert.ToDecimal(customerOrderModel.VatableAmount).ToString("0.00");
                txtNonVatableAmount.Text = Convert.ToDecimal(customerOrderModel.NonVatableAmount).ToString("0.00");

                if (customerOrderModel.OrderType == (int)EnumUtility.OrderType.DineIN)
                {
                    if (!string.IsNullOrEmpty(customerOrderModel.TableName))
                    {
                        txtTableNumber.Text = " #" + customerOrderModel.TableName.ToString();
                    }
                }
                else
                {
                    txtTableNumber.Text = "";
                }
                if (customerOrderModel.KotStatus == 1)
                {
                    txtbKitchenStatus.Text = "Pending";
                }
                else if (customerOrderModel.KotStatus == 2)
                {
                    txtbKitchenStatus.Text = "Cooking";
                }
                else
                {
                    txtbKitchenStatus.Text = "Completed";
                }

                if (customerOrderModel.OrderType == 1)
                {
                    rdbDineInOrderType.IsChecked = true;
                    txtbDineInTableId.Text = customerOrderModel.TableId;
                }
                else if (customerOrderModel.OrderType == 2)
                {
                    rdbTakeAwayOrderType.IsChecked = true;
                }
                else if (customerOrderModel.OrderType == 3)
                {
                    rdbDeliveryOrderType.IsChecked = true;
                }

                List<SaleItemModel> saleItems = new List<SaleItemModel>();
                foreach (var orderItem in customerOrderModel.CustomerOrderItemModels)
                {
                    saleItems = new List<SaleItemModel>();
                    saleItems.Add(new SaleItemModel()
                    {
                        FoodMenuId = orderItem.FoodMenuId.ToString(),
                        Product = orderItem.FoodMenuName,
                        Price = Convert.ToDecimal(orderItem.FoodMenuRate),
                        Qty = orderItem.FoodMenuQty,
                        Discount = orderItem.Discount,
                        Total = Convert.ToDecimal(orderItem.Price),
                        CustomerOrderItemId = orderItem.CustomerOrderItemId,
                        FoodVat = orderItem.FoodVat,
                        Foodcess = orderItem.Foodcess,
                        TaxPercentage = orderItem.TaxPercentage,
                        IsVatable = orderItem.IsVatable
                    });
                    dgSaleItem.Items.Add(saleItems);
                    txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + orderItem.FoodMenuQty).ToString();
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void searchSalesOrder(object sender, RoutedEventArgs e)
        {
            try
            {
                int orderType = 0, orderStatus = 0;
                if (rdbDineInSales.IsChecked == true)
                {
                    orderType = (int)EnumUtility.OrderType.DineIN;
                }
                else if (rdbTakeAwaySales.IsChecked == true)
                {
                    orderType = (int)EnumUtility.OrderType.TakeAway;
                }
                else if (rdbDeliverySales.IsChecked == true)
                {
                    orderType = (int)EnumUtility.OrderType.Delivery;
                }
                else
                {
                    orderType = (int)EnumUtility.OrderType.All;
                }

                if (rdbHoldSales.IsChecked == true)
                {
                    orderStatus = (int)EnumUtility.OrderPaidStatus.Hold;
                }
                else
                {
                    orderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
                }
                GetOrderList(orderStatus, orderType, txtSearchModifyOrder.Text);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        #endregion
        #region Direct Invoice PopUp
        private void btnDirectInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dgSaleItem.Items.Count == 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.CartEmpty, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                if (rdbDeliveryOrderType.IsChecked == false && rdbDineInOrderType.IsChecked == false && rdbTakeAwayOrderType.IsChecked == false)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectOrderType, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                //if (cmbWaiter.SelectedIndex == -1)
                //{
                //    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectWaiter, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                //    cmbWaiter.Focus();
                //    return;
                //}

                string tableId = null;
                if (lbTablesList.SelectedIndex != -1)
                {
                    tableId = lbTablesList.SelectedValue.ToString();
                }
                else if (!String.IsNullOrEmpty(txtbDineInTableId.Text.ToString()))
                {
                    tableId = txtbDineInTableId.Text;
                }

                if (rdbDineInOrderType.IsChecked == true && String.IsNullOrEmpty(tableId))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.TableNumberNoSelected, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                if (cmbCustomer.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectCustomer, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbCustomer.Focus();
                    return;
                }
                decimal totalPaid = 0;
                CustomerBillViewModel customerBillViewModel = new CustomerBillViewModel();
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                List<PaymentMethodModel> paymentMethodModels = new List<PaymentMethodModel>();
                ppDirectInvoice.IsOpen = true;
                lblPPTotalPayableAmount.Content = txtbTotalPayableAmount.Text;
                paymentMethodModels = customerOrderViewModel.GetPaymentMethod();
                //lbPPPaymentMethod.ItemsSource = paymentMethodModels;
                dgPaymentMethod.ItemsSource = paymentMethodModels;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void btnDirectInvoicePopupCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppDirectInvoice.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        //private void txtPPGivenAmount_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(txtPPGivenAmount.Text))
        //        {
        //            txtPPGivenAmount.Text = "0";
        //        }
        //        lblPPChangeAmountTotal.Content = (Convert.ToDecimal(txtPPGivenAmount.Text) - Convert.ToDecimal(lblPPTotalPayableAmount.Content)).ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemError.Register(ex);
        //        
        //    }
        //}
        private void btnDirectInvoiceSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable multipleBillPayment = new DataTable();
                multipleBillPayment.Columns.Add("PaymentMethodId", typeof(Int32));
                multipleBillPayment.Columns.Add("Amount", typeof(Decimal));

                decimal totalAmount = 0;
                decimal paidAmount = 0;
                int j = 1; //Set this equal to desired column index where you add the textbox amount 
                for (int i = 0; i < dgPaymentMethod.Items.Count; i++)
                {
                    var paymentMethod = (PaymentMethodModel)dgPaymentMethod.Items[i];
                    ContentPresenter myCp = dgPaymentMethod.Columns[j].GetCellContent(paymentMethod) as ContentPresenter;
                    var myTemplate = myCp.ContentTemplate;
                    TextBox mytxtbox = myTemplate.FindName("txtPaymentAmount", myCp) as TextBox;
                    if (!string.IsNullOrEmpty(mytxtbox.Text))
                    {
                        paidAmount = Convert.ToDecimal(mytxtbox.Text);
                        totalAmount = totalAmount + paidAmount;
                        if (paidAmount > 0)
                        {
                            multipleBillPayment.Rows.Add(paymentMethod.Id, paidAmount);
                        }
                    }
                }

                if (totalAmount <= 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.BillPaymentTitle, StatusMessages.PaymentNotZero, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    ppDirectInvoice.IsOpen = true;
                    return;
                }
                if (totalAmount < Convert.ToDecimal(lblPPTotalPayableAmount.Content))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.BillPaymentTitle, StatusMessages.PaymentMustBeHigher, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    ppDirectInvoice.IsOpen = true;
                    return;
                }

                int orderId = 0;
                orderId = PlaceOrder("DirectInvoice");
                TableViewModel tableViewModel = new TableViewModel();
                ReceiptPrintView printReceipt = new ReceiptPrintView();
                AppSettings appSettings = new AppSettings();
                CustomerBillViewModel customerBillViewModel = new CustomerBillViewModel();
                CustomerBillModel customerBillModel = new CustomerBillModel();
                CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                CustomerOrderModel customerOrderModel = new CustomerOrderModel();
                int insertedId = 0;

                //Update Table Status
                tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Clean);

                customerOrderModel = customerOrderViewModel.GetCustomerOrderByOrderId(orderId);
                customerBillModel.OutletId = customerOrderModel.OutletId;
                customerBillModel.CustomerOrderId = customerOrderModel.Id;
                customerBillModel.CustomerId = customerOrderModel.CustomerId;
                customerBillModel.GrossAmount = customerOrderModel.GrossAmount;
                customerBillModel.Discount = customerOrderModel.DiscountAmount;
                customerBillModel.ServiceCharge = customerOrderModel.DeliveryCharges;
                customerBillModel.VatableAmount = customerOrderModel.VatableAmount;
                customerBillModel.TaxAmount = customerOrderModel.TaxAmount;
                customerBillModel.TotalAmount = customerOrderModel.TotalPayable;
                customerBillModel.BillStatus = (int)EnumUtility.OrderPaidStatus.FullPaid;
                customerBillModel.UserId = LoginDetail.UserId;
                //customerBillModel.PaymentMethodId = Convert.ToInt32(lbPPPaymentMethod.SelectedValue);
                customerBillModel.PaymentNumber = string.Empty;
                customerBillModel.applyRedeem = applyRedeem;

                insertedId = customerBillViewModel.InsertBillDetail(customerBillModel, multipleBillPayment);
                ppDirectInvoice.IsOpen = false;
                if (insertedId > 0)
                {
                    SplashScreen splash = new SplashScreen("Images/InvoiceOrder.PNG");
                    splash.Show(true);

                    Thread.Sleep(2000);

                    GetOrderList((int)EnumUtility.OrderPaidStatus.Pending, (int)EnumUtility.OrderType.All, string.Empty);
                }
                else
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, StatusMessages.BillDetailSaveSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                }
                ClearCustomerOrderItemControll();
                printReceipt.Print(appSettings.GetPrinterName(), customerBillModel.CustomerOrderId);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        #endregion
        #region Customer Add/Edit
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetCustomer();
                ppCustomerAdd.IsOpen = true;
                customerAdd = 1;
                txtPPCPhone.Focus();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbCustomer.SelectedIndex == -1 || cmbCustomer.SelectedValue == null)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSelectRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(cmbCustomer);
                    return;
                }
                else
                {
                    ResetCustomer();

                    CustomerViewModel customerViewModel = new CustomerViewModel();
                    CustomerModel customerModel = new CustomerModel();
                    customerModel = customerViewModel.GetCustomerById(Convert.ToInt32(cmbCustomer.SelectedValue));
                    txtPPCName.Text = customerModel.CustomerName;
                    txtPPCPhone.Text = customerModel.CustomerPhone;
                    txtPPCEmail.Text = customerModel.CustomerEmail;
                    txtPPCAddress.Text = customerModel.CustomerAddress1;
                    txtPPCAddress2.Text = customerModel.CustomerAddress2;

                    if (customerModel.BirthDate != DateTime.MinValue)
                        dpBirthDate.SelectedDate = customerModel.BirthDate;

                    if (customerModel.AnniversaryDate != DateTime.MinValue)
                        dpAnniversaryDate.SelectedDate = customerModel.AnniversaryDate;

                    txtPPCPhone.Focus();
                    customerAdd = 2;
                    ppCustomerAdd.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPCCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppCustomerAdd.IsOpen = false;
                dgSearchCustomer.ItemsSource = null;
                dgSearchCustomer.Items.Refresh();
                //   btnEditCustomer.IsEnabled = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPCAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int insertedId = 0;
                CustomerViewModel customerViewModel = new CustomerViewModel();
                Keyboard.Focus(ppCustomerAdd);
                CustomerModel customerModel = new CustomerModel();

                if (string.IsNullOrEmpty(txtPPCPhone.Text))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerPhoneRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(txtPPCPhone);
                    ppCustomerAdd.IsOpen = true;
                    return;
                }
                else if (txtPPCPhone.Text.Length < 10)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerPhoneDigitRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(txtPPCPhone);
                    ppCustomerAdd.IsOpen = true;
                    return;
                }

                if (string.IsNullOrEmpty(txtPPCName.Text))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSelectRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(txtPPCName);
                    ppCustomerAdd.IsOpen = true;
                    return;
                }

                //customerAdd 1 for add and 2 for edit
                if (customerAdd == 1)
                {
                    customerModel.Id = 0;
                }
                else
                {
                    customerModel.Id = Convert.ToInt32(cmbCustomer.SelectedValue);
                }
                customerModel.CustomerName = txtPPCName.Text;
                customerModel.CustomerPhone = txtPPCPhone.Text;
                customerModel.CustomerEmail = txtPPCEmail.Text;
                customerModel.CustomerAddress1 = txtPPCAddress.Text;
                customerModel.CustomerAddress2 = txtPPCAddress2.Text;

                if (dpBirthDate.SelectedDate != null)
                    customerModel.BirthDate = dpBirthDate.SelectedDate.Value;

                if (dpAnniversaryDate.SelectedDate != null)
                    customerModel.AnniversaryDate = dpAnniversaryDate.SelectedDate.Value;

                customerModel.UserId = LoginDetail.UserId;
                insertedId = customerViewModel.InsertUpdateCustomer(customerModel);

                if (insertedId > 0)
                {
                    GetCustomerList(insertedId);
                    txtPPCName.Text = string.Empty;
                    txtPPCPhone.Text = string.Empty;
                    txtPPCEmail.Text = string.Empty;
                    txtPPCAddress.Text = string.Empty;
                    ppCustomerAdd.IsOpen = false;
                }
                else
                {
                    ppCustomerAdd.IsOpen = false;
                    //   btnEditCustomer.IsEnabled = false;
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSaveFailed, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
                }
                dgSearchCustomer.ItemsSource = null;
                dgSearchCustomer.Items.Refresh();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        #endregion
        #region Discount Service Charge PopUp
        private void btnDicountPopUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppPassword.IsOpen = true;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPPaswordApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<int> Discounts = LoginDetail.DiscountList.Split(',').Select(int.Parse).ToList();
                LoginViewModel loginViewModel = new LoginViewModel();
                int validId = 0;
                validId = loginViewModel.ValidateDiscountPassword(txtDiscountPassword.Password);
                if (validId > 0)
                {
                    txtDiscountPassword.Password = "";
                    ppPassword.IsOpen = false;
                    ppDiscountAmount.IsOpen = true;
                }
                else
                {
                    ppPassword.Focus();
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyPasswordTitle, StatusMessages.WrongPassword, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
                    txtDiscountPassword.Password = "";
                    txtDiscountPassword.Focus();

                    return;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnPPPaswordCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppPassword.IsOpen = false;
                txtDiscountPassword.Password = string.Empty;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPDiscountCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //cmbPPDiscountNos.SelectedIndex = -1;
                lbPPDiscountPercent.SelectedIndex = -1;
                ppDiscountPopUp.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPDiscountApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbPPDiscountPercent.SelectedIndex != -1)
                {
                    CommonOrderCalculation(sender, "DiscountPercent");
                }
                else
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyDiscountTitle, StatusMessages.PercentageSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                lbPPDiscountPercent.SelectedIndex = -1;
                ppDiscountPopUp.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnServiceDeliveryPopUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<int> DeliveryList = LoginDetail.DeliveryList.Split(',').Select(int.Parse).ToList();
                lbPPPercentageDelivery.ItemsSource = DeliveryList;
                ppDeliveryServicePopUp.IsOpen = true;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPDeliveryApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbPPPercentageDelivery.SelectedIndex != -1)
                {
                    CommonOrderCalculation(sender, "DeliveryCharge");
                }
                else
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyServiceChargeTitle, StatusMessages.ServiceChargeSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                ppDeliveryServicePopUp.IsOpen = false;
                lbPPPercentageDelivery.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPDeliveryCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lbPPPercentageDelivery.SelectedIndex = -1;
                ppDeliveryServicePopUp.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void txtSubTotalDiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtSubTotalDiscountAmount.Text))
            //{
            //    CommonOrderCalculation(sender, "DiscountAmount");
            //    //txtbTotalDiscountAmount.Text = txtSubTotalDiscountAmount.Text;
            //    //txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(txtSubTotalDiscountAmount.Text)).ToString();
            //}
            //else
            //{
            //    txtbTotalPayableAmount.Text = txtbSubTotalAmount.Text;
            //    txtbTotalDiscountAmount.Text = "0.00";
            //    txtSubTotalDiscountAmount.Text = "0.00";
            //}
        }
        #endregion
        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dgSaleItem.Items.Count == 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.CartEmpty, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }
                if (rdbDeliveryOrderType.IsChecked == false && rdbDineInOrderType.IsChecked == false && rdbTakeAwayOrderType.IsChecked == false)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectOrderType, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                string tableId = null;
                if (lbTablesList.SelectedIndex != -1)
                {
                    tableId = lbTablesList.SelectedValue.ToString();
                }
                else if (!String.IsNullOrEmpty(txtbDineInTableId.Text.ToString()))
                {
                    tableId = txtbDineInTableId.Text;
                }

                if (rdbDineInOrderType.IsChecked == true && String.IsNullOrEmpty(tableId))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.TableNumberNoSelected, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    return;
                }

                //if (cmbWaiter.SelectedIndex == -1)
                //{
                //    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectWaiter, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                //    cmbWaiter.Focus();
                //    return;
                //}
                if (cmbCustomer.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectCustomer, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbCustomer.Focus();
                    return;
                }
                PlaceOrder("Hold");
                ClearCustomerOrderItemControll();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loginViewModel.UpdateLoginLogout("logout");
                loginViewModel.LoginHistory(2);

                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        #endregion
        private void btnPPCancelTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppDineInTables.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void btnPPSelectTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableViewModel tableViewModel = new TableViewModel();
                if (lbTablesList.SelectedIndex != -1)
                {
                    TableModel tableModel = (TableModel)lbTablesList.SelectedItem;
                    if (string.IsNullOrEmpty(txtAllocatedPerson.Text))
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInSelect, StatusMessages.AddTotalPerson, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        ppDineInTables.IsOpen = true;
                        return;
                    }
                    else if (Convert.ToInt32(txtAllocatedPerson.Text) > tableModel.PersonCapacity)
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInSelect, StatusMessages.AddMinimumPerson + tableModel.PersonCapacity, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        ppDineInTables.IsOpen = true;
                        return;
                    }
                    else
                    {
                        txtbDineInTableId.Text = lbTablesList.SelectedValue.ToString();
                    }
                }
                else
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInSelect, StatusMessages.DineInSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    ppDineInTables.IsOpen = true;
                    return;
                }
                ppDineInTables.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void rdbDineInOrderType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TableViewModel tableViewModel = new TableViewModel();
                List<TableModel> tables = new List<TableModel>();
                ppDineInTables.IsOpen = true;
                tables = tableViewModel.GetPendingTables(LoginDetail.OutletId);//outletId
                lbTablesList.ItemsSource = tables;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                txtDatetime.Text = DateTime.Now.ToLongTimeString();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void Timer()
        {
            try
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void HeaderFooter()
        {
            try
            {
                txtClientName.Text = LoginDetail.ClientName + "  |  ";
                txbOutletName.Text = LoginDetail.OutletName + "  |  Ver: " + LoginDetail.AppVersion;
                txtbUserName.Text = "User: " + LoginDetail.Username;
                //txtWebsite.Text = LoginDetail.WebSite;
                btnWebsite.Content = LoginDetail.WebSite;
                txtSystemDate.Text = LoginDetail.SystemDate.ToShortDateString();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnLastSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerOrderHistoryList customerOrderHistoryList = new CustomerOrderHistoryList();
                customerOrderHistoryList.Owner = Application.Current.MainWindow;
                customerOrderHistoryList.ShowDialog();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        #region FoodMenuList PopUp
        private void btnFoodMenuList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
                List<FoodMenu> foodMenus = new List<FoodMenu>();
                foodMenus = foodMenuViewModel.GetFoodMenuPopUpList(LoginDetail.OutletId, string.Empty);
                ppFoodMenuList.IsOpen = true;
                dgFoodMenuList.ItemsSource = foodMenus;
                txtSearchFoodMenuList.Text = "";
                txtSearchFoodMenuList.Focus();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnPPFoodListCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppFoodMenuList.IsOpen = false;
                txtSearchFoodMenuList.Text = "";
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }

        }

        private void txtSearchFoodMenuList_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
                List<FoodMenu> foodMenus = new List<FoodMenu>();
                foodMenus = foodMenuViewModel.GetFoodMenuPopUpList(LoginDetail.OutletId, txtSearchFoodMenuList.Text);
                dgFoodMenuList.ItemsSource = foodMenus;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void dgFoodMenuList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FoodMenu foodMenuItem = new FoodMenu();
                var foodItem = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;

                if (foodItem == null) return;
                foodMenuItem = (FoodMenu)foodItem.Item;

                //if (foodMenuItem.IsPriceChange == true)
                //{
                rowId = dgSaleItem.Items.Count + 1;
                saleItemsFoodMenu.RowId = rowId;
                saleItemsFoodMenu.FoodMenuId = foodMenuItem.FoodMenuId.ToString();
                saleItemsFoodMenu.Product = foodMenuItem.SmallName;
                saleItemsFoodMenu.Price = Convert.ToDecimal(foodMenuItem.SalesPrice);
                saleItemsFoodMenu.Qty = 1.0m;
                saleItemsFoodMenu.Discount = 0;
                saleItemsFoodMenu.Total = Convert.ToDecimal(foodMenuItem.SalesPrice) * 1;
                saleItemsFoodMenu.CustomerOrderItemId = 0;
                saleItemsFoodMenu.Foodcess = Convert.ToDecimal(foodMenuItem.Foodcess);
                saleItemsFoodMenu.FoodVat = Convert.ToDecimal(foodMenuItem.FoodVat);
                saleItemsFoodMenu.TaxPercentage = Convert.ToDecimal(foodMenuItem.TaxPercentage);
                saleItemsFoodMenu.IsVatable = Convert.ToInt32(foodMenuItem.IsVatable);

                txtQtyPopUpProductName.Text = saleItemsFoodMenu.Product;
                txtChnageQty.Text = saleItemsFoodMenu.Qty.ToString();

                ppFoodMenuList.IsOpen = false;

                ppEditQty.IsOpen = true;

                return;
                // }

                CommonOrderCalculation(foodMenuItem, "FoodMenuGridList");

                List<SaleItemModel> saleItems = new List<SaleItemModel>();
                saleItems.Add(new SaleItemModel()
                {
                    RowId = rowId,
                    FoodMenuId = foodMenuItem.FoodMenuId.ToString(),
                    Product = foodMenuItem.SmallName,
                    Price = Convert.ToDecimal(foodMenuItem.SalesPrice),
                    Qty = 1.0m,
                    Discount = 0,
                    Total = Convert.ToDecimal(foodMenuItem.SalesPrice) * 1,
                    CustomerOrderItemId = 0,
                    Foodcess = Convert.ToDecimal(foodMenuItem.Foodcess),
                    FoodVat = Convert.ToDecimal(foodMenuItem.FoodVat),
                    TaxPercentage = Convert.ToDecimal(foodMenuItem.TaxPercentage),
                    IsVatable = Convert.ToInt32(foodMenuItem.IsVatable)
                });
                //dgSaleItem.Items.Add(saleItems);


                bool isFound = false;
                if (dgSaleItem != null)
                {
                    for (int i = 0; i < dgSaleItem.Items.Count; i++)
                    {
                        var gridSaleitem = (List<SaleItemModel>)dgSaleItem.Items[i];
                        if (saleItems[0].FoodMenuId.Equals(gridSaleitem[0].FoodMenuId))
                        {
                            isFound = true;
                            gridSaleitem[0].Qty += 1;
                            gridSaleitem[0].Total = gridSaleitem[0].Qty * gridSaleitem[0].Price;
                        }
                    }
                }

                if (!isFound)
                {
                    dgSaleItem.Items.Add(saleItems);
                }
                else
                {
                    dgSaleItem.Items.Refresh();
                }
                ppFoodMenuList.IsOpen = false;
                txtSearchFoodMenuList.Text = "";


            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        #endregion

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

                /*
                string settings = LoginDetail.MainWindowSettings;
                string[] wordsSettings = settings.Split('$');

                foreach (var word in wordsSettings)
                {
                    string[] words = word.Split('=');

                    if (words[0] == "ShowInTaskbar")
                    {

                        this.ShowInTaskbar = bool.Parse(words[1].ToString());

                    }
                    else if (words[0] == "Topmost")
                    {
                        this.Topmost = bool.Parse(words[1].ToString());

                    }
                    else if (words[0] == "WindowStyle")
                    {
                        if (words[1] == "None")
                            this.WindowStyle = WindowStyle.None;

                    }
                    else if (words[0] == "ResizeMode")
                    {
                        if (words[1] == "NoResize")
                            this.ResizeMode = ResizeMode.NoResize;

                    }
                }
                */
                //Set Header Marquee Text
                txtHeaderTitle.Text = LoginDetail.HeaderMarqueeText;
                canMain.Height = 50;
                canMain.Width = 1200;

                double height = 50;// canMain.ActualHeight - txtHeaderTitle.ActualHeight;
                txtHeaderTitle.Margin = new Thickness(1);// new Thickness(0, height / 2, 0, 0);
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = -600;// txtHeaderTitle.ActualWidth;
                doubleAnimation.To = 1200;// canMain.ActualWidth;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:20"));
                txtHeaderTitle.BeginAnimation(Canvas.RightProperty, doubleAnimation);

                //Hide control

                if (LoginDetail.DeliveryList.Length == 0)
                {
                    btnServiceDeliveryPopUp.Visibility = Visibility.Hidden;
                    txtbTotalDeliveryChargeAmt.Visibility = Visibility.Hidden;
                    txtbServiceDeliveryCharge.Visibility = Visibility.Hidden;
                    txtbServiceDeliveryChargeLabel.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearCustomerOrderItemControll();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnDineTableView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DineInTables dineInTables = new DineInTables();
                dineInTables.Owner = this;
                dineInTables.ShowDialog();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void txtSubTotalDiscountAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            return;
            try
            {
                CommonOrderCalculation(sender, "DiscountAmount");
                if (!string.IsNullOrEmpty(txtSubTotalDiscountAmount.Text))
                    txtSubTotalDiscountAmount.Text = Convert.ToDecimal(txtSubTotalDiscountAmount.Text).ToString("0.00");
                else
                {
                    txtSubTotalDiscountAmount.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        //private void txtPPPayAmount_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(txtPPPayAmount.Text))
        //        {
        //            txtPPPayAmount.Text = "";
        //            lblPPChangeAmountTotal.Content = "";
        //        }
        //        else
        //        {
        //            if ((Convert.ToDecimal(txtPPPayAmount.Text) - Convert.ToDecimal(lblPPTotalPayableAmount.Content)) >= 0)
        //            {
        //                lblPPChangeAmountTotal.Content = (Convert.ToDecimal(txtPPPayAmount.Text) - Convert.ToDecimal(lblPPTotalPayableAmount.Content)).ToString();
        //            }
        //            else
        //            {
        //                lblPPChangeAmountTotal.Content = "";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemError.Register(ex);
        //    }
        //}

        private void btnUploadFoodImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppFoodMenuList.IsOpen = false;
                string newFileName = string.Empty, fileExtension = string.Empty, source = string.Empty, destination = string.Empty;
                AppSettings appSettings = new AppSettings();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();

                var foodMenu = (FoodMenu)dgFoodMenuList.SelectedItem;

                openFileDialog.Title = "Select a picture";
                openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";

                if (openFileDialog.ShowDialog() == true)
                {
                    source = openFileDialog.FileName;
                    newFileName = foodMenu.SmallName.Replace(" ", "") + DateTime.Now.ToString("MM-dd-yyyy_HHmmss");
                    newFileName = RemoveSpecialCharacters(newFileName);
                    fileExtension = Path.GetExtension(source).ToString();

                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        newFileName = newFileName + fileExtension;
                        destination = appSettings.GetAppPath() + @"\Images\" + newFileName;
                        File.Copy(source, destination);
                        var uplpoadStatus = foodMenuViewModel.UploadFoodImage(newFileName, foodMenu.FoodMenuId);
                        if (uplpoadStatus)
                        {
                            var messageBoxResult = WpfMessageBox.Show(StatusMessages.FoodImageUploadTitle, StatusMessages.FoodImageUploadSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                            GenerateDynamicFoodMenu();
                            ppFoodMenuList.IsOpen = true;
                            return;
                        }
                        else
                        {
                            var messageBoxResult = WpfMessageBox.Show(StatusMessages.FoodImageUploadTitle, StatusMessages.FoodImageUploadFailed, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                            ppFoodMenuList.IsOpen = true;
                            return;
                        }

                    }
                    else
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.FoodImageUploadTitle, StatusMessages.FoodImageSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        ppFoodMenuList.IsOpen = true;
                        return;
                    }
                }
                else
                {
                    ppFoodMenuList.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loginViewModel.UpdateLoginLogout("logout");
            loginViewModel.LoginHistory(2);

            App.Current.Shutdown();

        }
        private void txtPaymentAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal paidAmount = 0m;
            decimal totalAmount = 0;
            for (int i = 0; i < dgPaymentMethod.Items.Count; i++)
            {
                var paymentMethod = (PaymentMethodModel)dgPaymentMethod.Items[i];
                ContentPresenter myCp = dgPaymentMethod.Columns[1].GetCellContent(paymentMethod) as ContentPresenter;
                var myTemplate = myCp.ContentTemplate;
                TextBox mytxtbox = myTemplate.FindName("txtPaymentAmount", myCp) as TextBox;
                if (!string.IsNullOrEmpty(mytxtbox.Text))
                {
                    paidAmount = Convert.ToDecimal(mytxtbox.Text);
                    totalAmount = totalAmount + paidAmount;
                }
            }
            txtPPPayAmount.Text = totalAmount.ToString();

            if (string.IsNullOrEmpty(txtPPPayAmount.Text))
            {
                txtPPPayAmount.Text = "";
                lblPPChangeAmountTotal.Content = "";
            }
            else
            {
                if ((Convert.ToDecimal(txtPPPayAmount.Text) - Convert.ToDecimal(lblPPTotalPayableAmount.Content)) >= 0)
                {
                    lblPPChangeAmountTotal.Content = (Convert.ToDecimal(txtPPPayAmount.Text) - Convert.ToDecimal(lblPPTotalPayableAmount.Content)).ToString();
                }
                else
                {
                    lblPPChangeAmountTotal.Content = "";
                }
            }
        }
        private string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void dgPaymentMethod_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var uiElement = e.OriginalSource as UIElement;
                if (e.Key == Key.Enter && uiElement != null)
                {
                    e.Handled = true;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnFoodMenuList_Click(sender, e);
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            btnPlaceOrder_Click(sender, e);
            ClearCustomerOrderItemControll();
        }

        private void btnWebsite_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = btnWebsite.Content.ToString(),
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void btnPaymentMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PaymentMethodModel paymentMethodModel = (PaymentMethodModel)dgPaymentMethod.SelectedItem;
                if (paymentMethodModel != null)
                {
                    for (int i = 0; i < dgPaymentMethod.Items.Count; i++)
                    {
                        var paymentMethod = (PaymentMethodModel)dgPaymentMethod.Items[i];
                        ContentPresenter myCpresenter = dgPaymentMethod.Columns[1].GetCellContent(paymentMethod) as ContentPresenter;
                        if (myCpresenter != null)
                        {
                            var myTemplate = myCpresenter.ContentTemplate;
                            TextBox mytxtbox = myTemplate.FindName("txtPaymentAmount", myCpresenter) as TextBox;
                            if (paymentMethodModel.Id == paymentMethod.Id)
                            {
                                mytxtbox.Text = lblPPTotalPayableAmount.Content.ToString();
                            }
                            else
                            {
                                mytxtbox.Text = string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnPricePopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            ppEditPrice.IsOpen = false;
        }

        private void btnPricePopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            ppEditPrice.IsOpen = false;
            try
            {
                decimal originalPrice = 0, increasedPrice = 0, changedPrice = 0;
                List<SaleItemModel> saleItem = new List<SaleItemModel>();

                object foodItem = dgSaleItem.SelectedItem;
                saleItem = (List<SaleItemModel>)foodItem;
                saleItem[0].Price = Convert.ToDecimal(txtPPPrice.Text);
                originalPrice = Convert.ToDecimal(txtPPOriginalPrice.Text);
                increasedPrice = Convert.ToDecimal(txtPPPrice.Text);

                originalPrice = originalPrice * saleItem[0].Qty;
                increasedPrice = increasedPrice * saleItem[0].Qty;
                changedPrice = increasedPrice - originalPrice;

                saleItem[0].Total = Math.Round(increasedPrice, 2);

                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + changedPrice).ToString("0.00");
                txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + GetPercentageAmount(changedPrice, saleItem[0].TaxPercentage)).ToString("0.00");
                if (Convert.ToInt32(saleItem[0].TaxPercentage) > 0)
                {
                    txtVatableAmount.Text = (Convert.ToDecimal(txtVatableAmount.Text) + (Convert.ToDecimal(changedPrice) - GetPercentageAmount(Convert.ToDecimal(changedPrice), Convert.ToDecimal(saleItem[0].TaxPercentage)))).ToString("0.00");
                }
                else
                {
                    txtNonVatableAmount.Text = (Convert.ToDecimal(txtNonVatableAmount.Text) + Convert.ToDecimal(changedPrice)).ToString("0.00");
                }
                txtbTotalPayableAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(txtbTotalDiscountAmount.Text)) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString("0.00");

                ppEditPrice.IsOpen = false;
                dgSaleItem.Items.Refresh();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportMenu reportMenu = new ReportMenu();
                reportMenu.ShowDialog();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }

        }

        private void btnQtyPricePopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtEditQty.Text) || Convert.ToDecimal(txtEditQty.Text.ToString()) <= 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Pleae enter change qty.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                    return;
                }
                saleItemsFoodMenu.Qty = Convert.ToDecimal(txtEditQty.Text.ToString());
                saleItemsFoodMenu.Total = Math.Round((Convert.ToDecimal(saleItemsFoodMenu.Price) * Convert.ToDecimal(txtEditQty.Text.ToString())), 2);

                rowId += 1;
                List<SaleItemModel> saleItems = new List<SaleItemModel>();
                saleItems.Add(new SaleItemModel()
                {
                    RowId = rowId,
                    FoodMenuId = saleItemsFoodMenu.FoodMenuId.ToString(),
                    Product = saleItemsFoodMenu.Product,
                    Price = Convert.ToDecimal(saleItemsFoodMenu.Price),
                    Qty = Convert.ToDecimal(txtEditQty.Text.ToString()),
                    Discount = 0,
                    Total = Math.Round((Convert.ToDecimal(saleItemsFoodMenu.Price) * Convert.ToDecimal(txtEditQty.Text.ToString())), 2),
                    CustomerOrderItemId = 0,
                    Foodcess = Convert.ToDecimal(saleItemsFoodMenu.Foodcess),
                    FoodVat = Convert.ToDecimal(saleItemsFoodMenu.FoodVat),
                    TaxPercentage = Convert.ToDecimal(saleItemsFoodMenu.TaxPercentage),
                    IsVatable = Convert.ToInt32(saleItemsFoodMenu.IsVatable)
                });



                bool IsItemOverright = LoginDetail.IsItemOverright;
                bool isFound = false;

                if (IsItemOverright)
                {
                    CommonOrderCalculation(sender, "FoodMenuGridListQty");

                    if (dgSaleItem != null)
                    {
                        for (int i = 0; i < dgSaleItem.Items.Count; i++)
                        {
                            var gridSaleitem = (List<SaleItemModel>)dgSaleItem.Items[i];
                            if (saleItems[0].FoodMenuId.Equals(gridSaleitem[0].FoodMenuId))
                            {
                                isFound = true;
                                gridSaleitem[0].Qty = Convert.ToDecimal(txtEditQty.Text.ToString());
                                gridSaleitem[0].Total = gridSaleitem[0].Qty * gridSaleitem[0].Price;
                            }
                        }

                        if (!isFound)
                        {
                            dgSaleItem.Items.Add(saleItems);
                        }
                    }
                }
                else
                {
                    CommonOrderCalculation(sender, "FoodMenuGridList");
                    dgSaleItem.Items.Add(saleItems);
                }

                dgSaleItem.Items.Refresh();

                ppEditQty.IsOpen = false;
                txtSearchFoodMenuList.Text = "";
                txtEditQty.Text = "1";
                //saleItemsFoodMenu = null;

            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void btnQtyPopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ppEditQty.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }

        }

        public void OrderCall(int orderId, int TableStatus, int TableId)
        {
            try
            {
                ClearCustomerOrderItemControll();

                if (TableStatus == 1)
                {
                    ClearCustomerOrderItemControll();
                    rdbDineInOrderType.IsChecked = true;
                    txtbDineInTableId.Text = TableId.ToString();//.Text.ToString();
                    lbTablesList.SelectedValue = TableId;

                }
                else if (TableStatus == 2)
                {
                    CustomerOrderModel customerOrderModel = new CustomerOrderModel();
                    CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
                    customerOrderModel = customerOrderViewModel.GetCustomerOrderByOrderId(orderId);

                    txtbSubTotalAmount.Text = Convert.ToDecimal(customerOrderModel.GrossAmount).ToString("0.00");
                    txtbTotalPayableAmount.Text = customerOrderModel.TotalPayable.ToString();
                    txtbOrderId.Text = customerOrderModel.Id.ToString();
                    cmbCustomer.SelectedValue = customerOrderModel.CustomerId;
                    cmbWaiter.SelectedValue = customerOrderModel.WaiterEmployeeId;
                    txtbDineInTableId.Text = customerOrderModel.TableId;
                    txtAllocatedPerson.Text = customerOrderModel.AllocatedPerson;
                    txtSubTotalDiscountAmount.Text = Convert.ToDecimal(customerOrderModel.DiscountAmount).ToString("0.00");
                    txtbTotalDiscountAmount.Text = Convert.ToDecimal(customerOrderModel.DiscountAmount).ToString("0.00");
                    txtbTotalDeliveryChargeAmt.Text = Convert.ToDecimal(customerOrderModel.DeliveryCharges).ToString("0.00");
                    txtTaxAmount.Text = Convert.ToDecimal(customerOrderModel.TaxAmount).ToString("0.00");
                    txtVatableAmount.Text = Convert.ToDecimal(customerOrderModel.VatableAmount).ToString("0.00");
                    txtNonVatableAmount.Text = Convert.ToDecimal(customerOrderModel.NonVatableAmount).ToString("0.00");

                    if (customerOrderModel.OrderType == (int)EnumUtility.OrderType.DineIN)
                    {
                        if (!string.IsNullOrEmpty(customerOrderModel.TableName))
                        {
                            txtTableNumber.Text = " #" + customerOrderModel.TableName.ToString();
                        }
                    }
                    else
                    {
                        txtTableNumber.Text = "";
                    }

                    if (customerOrderModel.OrderType == 1)
                    {
                        rdbDineInOrderType.IsChecked = true;
                        txtbDineInTableId.Text = customerOrderModel.TableId;
                    }
                    else if (customerOrderModel.OrderType == 2)
                    {
                        rdbTakeAwayOrderType.IsChecked = true;
                    }
                    else if (customerOrderModel.OrderType == 3)
                    {
                        rdbDeliveryOrderType.IsChecked = true;
                    }

                    List<SaleItemModel> saleItems = new List<SaleItemModel>();
                    foreach (var orderItem in customerOrderModel.CustomerOrderItemModels)
                    {
                        saleItems = new List<SaleItemModel>();
                        saleItems.Add(new SaleItemModel()
                        {
                            FoodMenuId = orderItem.FoodMenuId.ToString(),
                            Product = orderItem.FoodMenuName,
                            Price = Convert.ToDecimal(orderItem.FoodMenuRate),
                            Qty = orderItem.FoodMenuQty,
                            Discount = orderItem.Discount,
                            Total = Convert.ToDecimal(orderItem.Price),
                            CustomerOrderItemId = orderItem.CustomerOrderItemId,
                            FoodVat = orderItem.FoodVat,
                            Foodcess = orderItem.Foodcess,
                            TaxPercentage = orderItem.TaxPercentage,
                            IsVatable = orderItem.IsVatable
                        });
                        dgSaleItem.Items.Add(saleItems);
                        txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + orderItem.FoodMenuQty).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsList frmSettings = new SettingsList();
            frmSettings.ShowDialog();
            //SettingView settingView = new SettingView();
            //settingView.ShowDialog();
        }

        private void btnChangePrice_Click(object sender, RoutedEventArgs e)
        {
            ppChnagePrice.IsOpen = true;

            var foodMenu = (FoodMenu)dgFoodMenuList.SelectedItem;
            txtbPopUpChangeProductName.Text = foodMenu.SmallName;
            txtPPOriginalChangePrice.Text = foodMenu.SalesPrice.ToString("0.00");

            txtChnagePPPrice.Text = "";
        }

        private void btnChangePricePopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            ppChnagePrice.IsOpen = false;
        }

        private void btnChangePricePopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();

                var foodMenu = (FoodMenu)dgFoodMenuList.SelectedItem;

                if (String.IsNullOrEmpty(txtChnagePPPrice.Text) || Convert.ToDecimal(txtChnagePPPrice.Text.ToString()) <= 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Pleae enter new rate", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                    ppChnagePrice.IsOpen = true;
                    return;
                }

                decimal changePrice = Convert.ToDecimal(txtChnagePPPrice.Text.ToString());
                var newPrice = foodMenuViewModel.ChagePrice(foodMenu.FoodMenuId, changePrice);
                GenerateDynamicFoodMenu();

                if (Convert.ToDecimal(txtbTotalDiscountAmount.Text) > Convert.ToDecimal(txtbSubTotalAmount.Text))
                {
                    txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString();
                    txtbTotalDiscountAmount.Text = "0.00";
                    applyRedeem = false;
                }

                ppFoodMenuList.IsOpen = false;
                ppChnagePrice.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void rdbTakeAwayOrderType_Click(object sender, RoutedEventArgs e)
        {
            //  txtTableNumber.Text = "";
            // txtbDineInTableId.Text = "";
        }

        private void rdbDeliveryOrderType_Click(object sender, RoutedEventArgs e)
        {
            // txtTableNumber.Text = "";
            // txtbDineInTableId.Text = "";
        }

        private void btnKitchenView_Click(object sender, RoutedEventArgs e)
        {
            KitchenView kitchenView = new KitchenView();
            kitchenView.Owner = Application.Current.MainWindow;
            kitchenView.ShowDialog();
        }

        private void btnPPDAmountApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonOrderCalculation(sender, "DiscountAmount");
                if (!string.IsNullOrEmpty(txtSubTotalDiscountAmount.Text))
                    txtSubTotalDiscountAmount.Text = Convert.ToDecimal(txtSubTotalDiscountAmount.Text).ToString("0.00");
                else
                {
                    txtSubTotalDiscountAmount.Text = "0.00";
                }
                ppDiscountAmount.IsOpen = false;
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnRedeemPoints_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDecimal(BalancePoints) > 0 && dgSaleItem.Items.Count > 0)
            {
                ppRedeem.IsOpen = true;

                txtRedeemCustomerName.Text = " Name: " + cmbCustomer.Text;
                //txtRedeemCustomerPhone
                lblRedeemTotal.Content = "Available Reward Points : " + Convert.ToDecimal(BalancePoints).ToString("0.00");
                txtRedeemAmount.Focus();
            }

            // btnRedeemPoints.IsChecked = false;
            // return;

            //    if (btnRedeemPoints.IsChecked == true)
            //    {
            //        //    txtBalancePoints.Text = Convert.ToDecimal(BalancePoints).ToString("0.00");
            //        txtbTotalDiscountAmount.Text = Convert.ToDecimal(BalancePoints).ToString("0.00");
            //        txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) - Convert.ToDecimal(txtbTotalDiscountAmount.Text)).ToString("0.00");
            //        applyRedeem = true;
            //    }
            //    else
            //    {
            //        txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(txtbTotalDiscountAmount.Text)).ToString("0.00");
            //        txtSubTotalDiscountAmount.Text = "0.00";
            //        //  txtBalancePoints.Text = "0.00";
            //        applyRedeem = false;
            //    }
        }

        private void btnRedeemPopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            ppRedeem.IsOpen = false;
        }

        private void btnKot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgKOTItem.ItemsSource = null;
                ppKOT.IsOpen = true;
                KitchenViewModel kitchenViewModel = new KitchenViewModel();
                KOTCustomerOrderDetail kOTCustomerOrderDetail = new KOTCustomerOrderDetail();
                List<KOTHeaderDetail> kOTHeaderDetails = new List<KOTHeaderDetail>();


                var saleOrder = (CustomerOrderModel)lbCustomerOrderList.SelectedItem;

                if (saleOrder == null)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.SelectOrder, "Pleae Select Sale Order", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                    return;
                }
                else
                {
                    int orderId = saleOrder.Id;

                    kOTCustomerOrderDetail = kitchenViewModel.GetCustomerOrderKOT(orderId.ToString());
                    txtbCustomerOrderNo.Content = kOTCustomerOrderDetail.CustomerOrderNo;
                    txtbOrderDate.Content = kOTCustomerOrderDetail.OrderDate;
                    txtbCustomerName.Content = kOTCustomerOrderDetail.CustomerName;
                    txtbWaiterName.Content = kOTCustomerOrderDetail.WaiterName;
                    txtbTableName.Content = kOTCustomerOrderDetail.TableName;
                    txtbOrderType.Content = kOTCustomerOrderDetail.OrderType;

                    kOTHeaderDetails = kitchenViewModel.GetKOTHeaderDetail(orderId.ToString());

                    lbKOTOrderList.ItemsSource = kOTHeaderDetails;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnPPKOTCancel_Click(object sender, RoutedEventArgs e)
        {
            ppKOT.IsOpen = false;
        }

        private void lbKOTOrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                List<KOTItemDetail> kOTItemDetails = new List<KOTItemDetail>();
                KitchenViewModel kitchenViewModel = new KitchenViewModel();
                var kotId = (KOTHeaderDetail)lbKOTOrderList.SelectedItem;
                if (kotId != null)
                {
                    int kotItemId = kotId.Id;
                    kOTItemDetails = kitchenViewModel.GetKOTItemDetail(kotItemId.ToString());
                    dgKOTItem.ItemsSource = kOTItemDetails;
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void btnPrintKOT_Click(object sender, RoutedEventArgs e)
        {
            var kotId = (KOTHeaderDetail)lbKOTOrderList.SelectedItem;
            AppSettings appSettings = new AppSettings();

            if (kotId != null)
            {
                int kotItemId = kotId.Id;
                PrintKOTView printKOTView = new PrintKOTView();
                printKOTView.Print(appSettings.GetPrinterName(), kotItemId); 
            }
        }

        private void btnKOTCancel_Click(object sender, RoutedEventArgs e)
        {
            ppKOT.IsOpen = false;
        }

        private void btnPPCSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerViewModel customerViewModel = new CustomerViewModel();
            List<CustomerSearchModel> customerSearchModels = new List<CustomerSearchModel>();
            customerSearchModels = customerViewModel.GetSearchCustomers(txtPPSearchCName.Text, txtPPSearchCPhone.Text);

            if (customerSearchModels.Count > 0)
            {
                dgSearchCustomer.ItemsSource = customerSearchModels;
            }
        }

        private void btnPPCSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            ResetCustomer();
            CustomerSearchModel customerSearchModel = ((FrameworkElement)sender).DataContext as CustomerSearchModel;

            CustomerViewModel customerViewModel = new CustomerViewModel();
            CustomerModel customerModel = new CustomerModel();
            customerModel = customerViewModel.GetCustomerById(Convert.ToInt32(customerSearchModel.Id));
            txtPPCName.Text = customerModel.CustomerName;
            txtPPCPhone.Text = customerModel.CustomerPhone;
            txtPPCEmail.Text = customerModel.CustomerEmail;
            txtPPCAddress.Text = customerModel.CustomerAddress1;
            txtPPCAddress2.Text = customerModel.CustomerAddress2;

            if (customerModel.BirthDate != DateTime.MinValue)
                dpBirthDate.SelectedDate = customerModel.BirthDate;

            if (customerModel.AnniversaryDate != DateTime.MinValue)
                dpAnniversaryDate.SelectedDate = customerModel.AnniversaryDate;

            txtPPCPhone.Focus();
            cmbCustomer.SelectedValue = customerSearchModel.Id;
            customerAdd = 2;
            ppCustomerAdd.IsOpen = true;
        }

        private void btnRedeemApply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRedeemAmount.Text))
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Pleae enter reedem points.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                ppRedeem.IsOpen = true;
                return;
            }
            else if (Convert.ToDecimal(txtRedeemAmount.Text.ToString()) < 0)
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Pleae enter valid reedem points.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                ppRedeem.IsOpen = true;
                return;
            }
            else if (Convert.ToDecimal(txtRedeemAmount.Text) > Convert.ToDecimal(BalancePoints))
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Reedem points cannot more then available points.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                ppRedeem.IsOpen = true;
                return;
            }
            else if (Convert.ToDecimal(txtRedeemAmount.Text) > Convert.ToDecimal(txtbSubTotalAmount.Text))
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Reedem points cannot more then payable/invoice amount.", MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                ppRedeem.IsOpen = true;
                return;
            }
            else if (Convert.ToDecimal(txtRedeemAmount.Text) == 0)
            {
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString();

                ApplyBalancePoints = Convert.ToDecimal(txtRedeemAmount.Text);
                txtbTotalDiscountAmount.Text = Convert.ToDecimal(ApplyBalancePoints).ToString("0.00");
                applyRedeem = false;
                ppRedeem.IsOpen = false;
            }
            else if (Convert.ToDecimal(txtRedeemAmount.Text) > 0)
            {
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString();

                ApplyBalancePoints = Convert.ToDecimal(txtRedeemAmount.Text);
                txtbTotalDiscountAmount.Text = Convert.ToDecimal(ApplyBalancePoints).ToString("0.00");
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) - Convert.ToDecimal(ApplyBalancePoints)).ToString("0.00");
                applyRedeem = true;
                ppRedeem.IsOpen = false;
            }

        }

        private void cmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerViewModel customerViewModel = new CustomerViewModel();
            CustomerModel customerModel = new CustomerModel();

            if (cmbCustomer.SelectedIndex > -1)
            {
                customerModel = customerViewModel.GetCustomerById(Convert.ToInt32(cmbCustomer.SelectedValue));

                if (customerModel.BalancePoints > 0 & customerModel.CustomerTypeId != -1)
                {
                    txtRedeemPoints.Text = "Redeem " + customerModel.BalancePoints + " Pt";
                    BalancePoints = customerModel.BalancePoints;
                }
                else
                {
                    txtRedeemPoints.Text = "Redeem";
                    BalancePoints = 0;
                }
            }

            if (applyRedeem == true)
            {
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(txtbServiceDeliveryChargeLabel.Text)).ToString();
                ApplyBalancePoints = 0;
                txtRedeemAmount.Text = "";
                txtbTotalDiscountAmount.Text = "0.00";
                applyRedeem = false;
            }
            ApplyBalancePoints = 0;
        }


        private void btnPPDAmountCancel_Click(object sender, RoutedEventArgs e)
        {
            ppDiscountAmount.IsOpen = false;

        }
    }
}
