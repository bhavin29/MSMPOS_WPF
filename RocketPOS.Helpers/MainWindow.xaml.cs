using System;
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
using RocketPOS.Helpers.Reports;
using RocketPOS.Helpers.Tables;
using NLog;
using System.Diagnostics;
using NLog.Fluent;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        LoginViewModel loginViewModel = new LoginViewModel();
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                CenterWindowOnScreen();
                Timer();
                HeaderFooter();
                GenerateDynamicFoodMenu();
                GetWaiterList();
                GetCustomerList();
                txtbTotalPayableAmount.Text = "0.00";
                rdbPendingSales.IsChecked = true;
                rdbAllSales.IsChecked = true;
                GetOrderList((int)EnumUtility.OrderPaidStatus.Pending, (int)EnumUtility.OrderType.All, string.Empty);
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        #region Methods
        private void GetCustomerList()
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
                cmbCustomer.SelectedIndex = -1;
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
                    btnCategory.Width = 100;
                    btnCategory.Height = 50;
                    btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFADADAD"));
                    btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    btnCategory.Margin = new Thickness(1, 0, 0, 0);
                    btnCategory.Click += GetSubCategory;
                    spCategory.Children.Add(btnCategory);
                }

                foreach (var foodCategory in foodMenu.FoodList)
                {
                    if (foodCategory.IsFavourite == 0)
                    {
                        Button btnCategory = new Button();
                        btnCategory.Content = foodCategory.FoodCategory;
                        btnCategory.Name = "btn" + foodCategory.Id;
                        btnCategory.FontSize = 15;
                        btnCategory.Width = 100;
                        btnCategory.Height = 50;
                        btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));
                        btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnCategory.Margin = new Thickness(1, 0, 0, 0);
                        btnCategory.Click += GetSubCategory;
                        spCategory.Children.Add(btnCategory);
                    }
                    else
                    {
                        Button btnCategory = new Button();
                        btnCategory.Content = foodCategory.FoodCategory;
                        btnCategory.Name = "btn" + foodCategory.Id;
                        btnCategory.FontSize = 15;
                        btnCategory.Width = 100;
                        btnCategory.Height = 50;
                        btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));
                        btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        btnCategory.Margin = new Thickness(1, 0, 0, 0);
                        btnCategory.Click += GetSubCategory;
                        spFavouriteCategory.Children.Add(btnCategory);
                    }
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

                TextBlock txtSmallName = new TextBlock();
                txtSmallName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtSmallName.Text = itemSubCat.SmallName;
                txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
                menuListPanel.Children.Add(txtSmallName);

                TextBlock txtSalePrice = new TextBlock();
                txtSalePrice.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtSalePrice.Text = Convert.ToDecimal(itemSubCat.SalesPrice).ToString("0.00");
                txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
                menuListPanel.Children.Add(txtSalePrice);

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
                imgFood.Width = 80;
                imgFood.Height = 70;
                imgFood.Margin = new Thickness(2, 2, 2, 2);
                imgFood.Stretch = Stretch.UniformToFill;
                imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
                menuListPanel.Children.Add(imgFood);

                TextBlock txtFoodMenuId = new TextBlock();
                txtFoodMenuId.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtFoodMenuId.Text = itemSubCat.FoodMenuId.ToString();
                txtFoodMenuId.FontSize = 2;
                txtFoodMenuId.Name = "txtFoodMenuId" + itemSubCat.FoodMenuId;
                txtFoodMenuId.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtFoodMenuId);

                TextBlock txtFoodVat = new TextBlock();
                txtFoodVat.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtFoodVat.Text = Convert.ToDecimal(itemSubCat.FoodVat).ToString("0.00");
                txtFoodVat.FontSize = 2;
                txtFoodVat.Name = "txtFoodVat" + itemSubCat.FoodCategoryId;
                txtFoodVat.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtFoodVat);

                TextBlock txtFoodcess = new TextBlock();
                txtFoodcess.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                txtFoodcess.Text = Convert.ToDecimal(itemSubCat.Foodcess).ToString("0.00");
                txtFoodcess.FontSize = 2;
                txtFoodcess.Name = "txtFoodcess" + itemSubCat.FoodCategoryId;
                txtFoodcess.Visibility = Visibility.Hidden;
                menuListPanel.Children.Add(txtFoodcess);

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
                dgSaleItem.Items.Clear();
                cmbWaiter.Text = "Select Waiter";
                cmbWaiter.SelectedIndex = -1;
                cmbCustomer.Text = "Select Customer";
                cmbCustomer.SelectedIndex = -1;
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
                btnEditCustomer.IsEnabled = false;
                txtPPPayAmount.Text = "";
                lblPPChangeAmountTotal.Content = "";
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void CommonOrderCalculation(object sender, string type)
        {
            try
            {
                if (type == "FoodMenu")
                {
                    var menuListPanel = sender as StackPanel;
                    var salePrice = menuListPanel.Children[1] as TextBlock;
                    var foodVat = menuListPanel.Children[4] as TextBlock;
                    var foodCess = menuListPanel.Children[5] as TextBlock;

                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
                    txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
                    txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + Convert.ToDecimal(foodVat.Text) + Convert.ToDecimal(foodCess.Text)).ToString();
                }

                if (type == "FoodMenuGridList")
                {
                    FoodMenu foodMenuItem = sender as FoodMenu;
                    if (foodMenuItem == null) return;

                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(foodMenuItem.SalesPrice)).ToString();
                    txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
                    txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + Convert.ToDecimal(foodMenuItem.FoodVat) + Convert.ToDecimal(foodMenuItem.Foodcess)).ToString();
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
                    txtbTotalDiscountAmount.Text = txtSubTotalDiscountAmount.Text;
                }

                if (type == "DeliveryCharge")
                {
                    decimal percentage = Convert.ToDecimal(lbPPPercentageDelivery.SelectedValue);
                    txtbServiceDeliveryChargeLabel.Text = percentage.ToString("0.00");
                    txtbTotalDeliveryChargeAmt.Text = (percentage).ToString("0.00");
                }

                if (string.IsNullOrEmpty(txtSubTotalDiscountAmount.Text))
                    txtbTotalDiscountAmount.Text = "0.00";

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
            }

            insertedId = customerOrderViewModel.InsertCustomerOrder(customerOrderModel, customerOrderItem);
            txtbOrderId.Text = insertedId.ToString();

            if (insertedId > 0)
            {
                if (type != "DirectInvoice")
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.PlaceOrderSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
                }
                ClearCustomerOrderItemControll();
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
                cmbCustomer.SelectedIndex = -1;
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

                btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFADADAD"));

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
                var salePrice = menuListPanel.Children[1] as TextBlock;
                var itemName = menuListPanel.Children[0] as TextBlock;
                var foodMenuId = menuListPanel.Children[3] as TextBlock;
                var foodVat = menuListPanel.Children[4] as TextBlock;
                var foodcess = menuListPanel.Children[5] as TextBlock;

                CommonOrderCalculation(sender, "FoodMenu");


                List<SaleItemModel> saleItems = new List<SaleItemModel>();
                saleItems.Add(new SaleItemModel()
                {
                    FoodMenuId = foodMenuId.Text,
                    Product = itemName.Text,
                    Price = Convert.ToDecimal(salePrice.Text),
                    Qty = 1,
                    Discount = 0,
                    Total = Convert.ToDecimal(salePrice.Text) * 1,
                    CustomerOrderItemId = 0,
                    FoodVat = Convert.ToDecimal(foodVat.Text),
                    Foodcess = Convert.ToDecimal(foodcess.Text)
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
                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(saleItem[0].Price)).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(saleItem[0].Price) + (Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess))).ToString();
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
                txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) + Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess)).ToString();
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
                saleItem[0].Total -= saleItem[0].Price * 1;
                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(saleItem[0].Price)).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) - Convert.ToDecimal(saleItem[0].Price) - (Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess))).ToString();
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) - 1).ToString();
                txtTaxAmount.Text = (Convert.ToDecimal(txtTaxAmount.Text) - (Convert.ToDecimal(saleItem[0].FoodVat) + Convert.ToDecimal(saleItem[0].Foodcess))).ToString();
                if (saleItem[0].Qty == 0)
                {
                    dgSaleItem.Items.RemoveAt(dgSaleItem.SelectedIndex);
                }
                dgSaleItem.Items.Refresh();
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
                txtbPopUpItemOriginalTotal.Text = saleItem[0].Price.ToString();
                txtbPopUpOriginalQtyCount.Text = saleItem[0].Qty.ToString();
                txtbPopUpItemOriginalSubTotalAmount.Text = saleItem[0].Price.ToString();
                txtbPopUpItemName.Text = saleItem[0].Product;
                txtbPopUpQtyCount.Text = saleItem[0].Qty.ToString();
                txtbPopUpItemTotal.Text = saleItem[0].Total.ToString();
                txtPopUpDiscount.Text = saleItem[0].Discount.ToString();
                txtbPopUpItemSubTotalAmount.Text = Convert.ToDecimal(saleItem[0].Total).ToString();
                EditSaleItemPopUp.IsOpen = true;
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
                saleItem[0].Total = Convert.ToDecimal(txtbPopUpItemSubTotalAmount.Text);
                saleItem[0].Discount = Convert.ToDecimal(txtPopUpDiscount.Text);
                if (txtbPopUpQtyCount.Text != "1")
                {
                    txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)) + Convert.ToDecimal(txtbTotalItemCount.Text)).ToString();
                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
                    txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
                }
                else if (txtbPopUpQtyCount.Text == "1" && txtbPopUpQtyCount.Text != txtbPopUpOriginalQtyCount.Text)
                {
                    txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)) + Convert.ToDecimal(txtbTotalItemCount.Text)).ToString();
                    txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
                    txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
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
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.CancelOrderTitle, StatusMessages.CancelOrder, MessageBoxButton.YesNo, EnumUtility.MessageBoxImage.Question);
                if (messageBoxResult.ToString() == "Yes")
                {
                    tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Open);
                    orderId = txtbOrderId.Text;
                    if (!string.IsNullOrEmpty(orderId) && orderId != "0")
                    {

                        insertedId = customerOrderViewModel.UpdateOrderStatus(orderId, (int)EnumUtility.OrderPaidStatus.Cancelled);
                        if (insertedId > 0)
                        {
                            var messageBoxSuccessResult = WpfMessageBox.Show(StatusMessages.CancelOrderTitle, StatusMessages.CancelOrderSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
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
                OutletRegisterViewModel outletRegisterViewModel = new OutletRegisterViewModel();
                OutletRegisterModel outletRegisterModel = new OutletRegisterModel();

                //Open regiter
                //outletRegisterModel.USerID = LoginDetail.UserId;
                //outletRegisterModel.OutletId = LoginDetail.OutletId;
                //outletRegisterModel.OpeningBalance = 500;

                // outletRegisterViewModel.InsertOutletRegister(outletRegisterModel);

                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, "Are you sure to close register? ", MessageBoxButton.YesNo, EnumUtility.MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    outletRegisterViewModel.UpdateOutletRegister(outletRegisterModel);
                    WpfMessageBox.Show(StatusMessages.AppTitle, "Register closed successfully");

                    OutletRegisterReport outletRegisterReport = new OutletRegisterReport();

                    Login frmlogin = new Login();

                    frmlogin.Show();

                    outletRegisterReport.Show();

                    this.Close();
                }
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
                txtbKitchenStatus.Visibility = Visibility.Visible;
                txtbKitchenStatusTitle.Visibility = Visibility.Visible;
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
                        Foodcess = orderItem.Foodcess
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
                if (cmbWaiter.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectWaiter, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbWaiter.Focus();
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
                lbPPPaymentMethod.ItemsSource = paymentMethodModels;
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
                if (lbPPPaymentMethod.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.BillPaymentTitle, StatusMessages.PaymentMethodSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
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
                customerBillModel.VatableAmount = customerOrderModel.TaxAmount;
                customerBillModel.TaxAmount = customerOrderModel.TaxAmount;
                customerBillModel.TotalAmount = Convert.ToDecimal(lblPPTotalPayableAmount.Content);
                customerBillModel.BillStatus = (int)EnumUtility.OrderPaidStatus.FullPaid;

                customerBillModel.UserId = LoginDetail.UserId;
                customerBillModel.PaymentMethodId = Convert.ToInt32(lbPPPaymentMethod.SelectedValue);
                customerBillModel.PaymentNumber = string.Empty;

                insertedId = customerBillViewModel.InsertBillDetail(customerBillModel);
                ppDirectInvoice.IsOpen = false;
                if (insertedId > 0)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, StatusMessages.BillDetailSaveSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
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
                if (cmbCustomer.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSelectRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(cmbCustomer);
                    return;
                }
                else
                {
                    CustomerViewModel customerViewModel = new CustomerViewModel();
                    CustomerModel customerModel = new CustomerModel();
                    customerModel = customerViewModel.GetCustomerById(Convert.ToInt32(cmbCustomer.SelectedValue));
                    txtPPCName.Text = customerModel.CustomerName;
                    txtPPCPhone.Text = customerModel.CustomerPhone;
                    txtPPCEmail.Text = customerModel.CustomerEmail;
                    txtPPCAddress.Text = customerModel.CustomerAddress1;
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
                btnEditCustomer.IsEnabled = false;
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
                if (string.IsNullOrEmpty(txtPPCName.Text))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSelectRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(txtPPCName);
                    return;
                }

                if (string.IsNullOrEmpty(txtPPCPhone.Text))
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerPhoneRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    Keyboard.Focus(txtPPCPhone);
                    return;
                }

                customerModel.Id = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerModel.CustomerName = txtPPCName.Text;
                customerModel.CustomerPhone = txtPPCPhone.Text;
                customerModel.CustomerEmail = txtPPCEmail.Text;
                customerModel.CustomerAddress1 = txtPPCAddress.Text;
                customerModel.UserId = LoginDetail.UserId;
                insertedId = customerViewModel.InsertUpdateCustomer(customerModel);

                if (insertedId > 0)
                {
                    GetCustomerList();
                    txtPPCName.Text = string.Empty;
                    txtPPCPhone.Text = string.Empty;
                    txtPPCEmail.Text = string.Empty;
                    txtPPCAddress.Text = string.Empty;
                    ppCustomerAdd.IsOpen = false;
                }
                else
                {
                    ppCustomerAdd.IsOpen = false;
                    btnEditCustomer.IsEnabled = false;
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSaveFailed, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }
        private void cmbCustomer_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                btnEditCustomer.IsEnabled = true;
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
                    ppPassword.IsOpen = false;
                    ppDiscountPopUp.IsOpen = true;
                    lbPPDiscountPercent.ItemsSource = Discounts;
                }
                else
                {
                    ppPassword.Focus();
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyPasswordTitle, StatusMessages.WrongPassword, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
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
                if (cmbWaiter.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectWaiter, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbWaiter.Focus();
                    return;
                }
                if (cmbCustomer.SelectedIndex == -1)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.SelectCustomer, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                    cmbCustomer.Focus();
                    return;
                }
                PlaceOrder("Hold");
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
                Login frmLogin = new Login();

                loginViewModel.UpdateLoginLogout("logout");
                loginViewModel.LoginHistory(2);
                frmLogin.Show();
                this.Close();
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
                        return;
                    }
                    else if (Convert.ToInt32(txtAllocatedPerson.Text) > tableModel.PersonCapacity)
                    {
                        var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInSelect, StatusMessages.AddMinimumPerson + tableModel.PersonCapacity, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        txtbDineInTableId.Text = lbTablesList.SelectedValue.ToString();
                        tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Occupied);
                    }
                }
                else
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInSelect, StatusMessages.DineInSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
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
                tables = tableViewModel.GetTables(LoginDetail.OutletId);//outletId
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
                txtWebsite.Text = LoginDetail.WebSite;
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
                customerOrderHistoryList.Show();
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

                CommonOrderCalculation(foodMenuItem, "FoodMenuGridList");

                List<SaleItemModel> saleItems = new List<SaleItemModel>();
                saleItems.Add(new SaleItemModel()
                {
                    FoodMenuId = foodMenuItem.FoodMenuId.ToString(),
                    Product = foodMenuItem.SmallName,
                    Price = Convert.ToDecimal(foodMenuItem.SalesPrice),
                    Qty = 1,
                    Discount = 0,
                    Total = Convert.ToDecimal(foodMenuItem.SalesPrice) * 1,
                    CustomerOrderItemId = 0
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
               
                //Set Header Marquee Text
                txtHeaderTitle.Text = LoginDetail.HeaderMarqueeText;
                canMain.Height = 50;
                canMain.Width = 1200;

                double height = 50;// canMain.ActualHeight - txtHeaderTitle.ActualHeight;
                txtHeaderTitle.Margin = new Thickness(1);// new Thickness(0, height / 2, 0, 0);
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = -200;// txtHeaderTitle.ActualWidth;
                doubleAnimation.To = 1200;// canMain.ActualWidth;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:20"));
                txtHeaderTitle.BeginAnimation(Canvas.RightProperty, doubleAnimation);

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
                dineInTables.Show();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);

            }
        }

        private void txtSubTotalDiscountAmount_LostFocus(object sender, RoutedEventArgs e)
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
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

        private void txtPPPayAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
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
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }

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

    }
}
