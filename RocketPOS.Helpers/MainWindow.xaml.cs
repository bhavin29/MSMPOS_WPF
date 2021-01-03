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

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();

            CenterWindowOnScreen();
            Timer();
            HeaderFooter();
            GenerateDynamicFoodMenu();
            GetWaiterList();
            GetCustomerList();
            txtbTotalPayableAmount.Text = "0.0";
            rdbPendingSales.IsChecked = true;
            rdbAllSales.IsChecked = true;
            GetOrderList((int)EnumUtility.OrderPaidStatus.Pending, (int)EnumUtility.OrderType.All, string.Empty);
        }

        #region Methods
        private void GetCustomerList()
        {
            CustomerViewModel customerViewModel = new CustomerViewModel();
            List<CustomerModel> customers = new List<CustomerModel>();
            customers = customerViewModel.GetCustomers();
            cmbCustomer.ItemsSource = customers;
            cmbCustomer.Text = "-- Select Customer --";
            cmbCustomer.IsEditable = true;
            cmbCustomer.IsReadOnly = true;
            cmbCustomer.SelectedValuePath = "Id";
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedIndex = -1;
        }
        private void GenerateDynamicFoodMenu()
        {
            string rootPath = string.Empty;
            FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
            FoodMenuModel foodMenu = new FoodMenuModel();
            rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;

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
                btnCategory.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));
                btnCategory.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                btnCategory.Margin = new Thickness(1);
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
                    btnCategory.Margin = new Thickness(1);
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
                    btnCategory.Margin = new Thickness(1);
                    btnCategory.Click += GetSubCategory;
                    spFavouriteCategory.Children.Add(btnCategory);
                }
                //Button btnCategory = new Button();
                //btnCategory.Content = foodCategory.FoodCategory;
                //btnCategory.Name = "btn" + foodCategory.Id;
                //btnCategory.Width = 100;
                //btnCategory.Height = 50;
                //btnCategory.Margin = new Thickness(5, 0, 5, 10);
                //btnCategory.Click += GetSubCategory;
                //spCategory.Children.Add(btnCategory);
            }
            GenerateDynamicFoodItems(foodMenu, rootPath, string.Empty, "All");
        }
        private void GenerateDynamicFoodItems(FoodMenuModel foodMenu, string rootPath, string searchKey, string type)
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
        private void GenerateDyanmicFoodItemsList(SubCategory itemSubCat, string rootPath)
        {
            StackPanel menuListPanel = new StackPanel();
            menuListPanel.Orientation = Orientation.Vertical;

            TextBlock txtSalePrice = new TextBlock();
            txtSalePrice.Text = itemSubCat.SalesPrice.ToString();
            txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
            menuListPanel.Children.Add(txtSalePrice);

            Image imgFood = new Image();
            try
            {
                imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\RocketPOS.StartUp\Images\" + itemSubCat.SmallThumb));
            }
            catch (Exception)
            {
                imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\RocketPOS.StartUp\Images\defaultimage.png"));
            }
            imgFood.MaxWidth = 150;
            imgFood.MaxHeight = 100;
            imgFood.Margin = new Thickness(5, 0, 5, 10);
            imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
            menuListPanel.Children.Add(imgFood);

            TextBlock txtSmallName = new TextBlock();
            txtSmallName.Text = itemSubCat.SmallName;
            txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
            menuListPanel.Children.Add(txtSmallName);

            TextBlock txtFoodMenuId = new TextBlock();
            txtFoodMenuId.Text = itemSubCat.FoodMenuId.ToString();
            txtFoodMenuId.Name = "txtFoodMenuId" + itemSubCat.FoodMenuId;
            txtFoodMenuId.Visibility = Visibility.Hidden;
            menuListPanel.Children.Add(txtFoodMenuId);

            menuListPanel.Name = "childPanel" + itemSubCat.FoodCategoryId;
            menuListPanel.MouseDown += GetPrice_MouseDown;
            spSubCategory.Children.Add(menuListPanel);
        }
        private void ClearCustomerOrderItemControll()
        {
            dgSaleItem.Items.Clear();
            cmbWaiter.Text = "-- Select Waiter --";
            cmbWaiter.SelectedIndex = -1;
            cmbCustomer.Text = "-- Select Customer --";
            cmbCustomer.SelectedIndex = -1;
            txtbTotalPayableAmount.Text = "0.0";
            txtbSubTotalAmount.Text = "0.0";
            txtbTotalItemCount.Text = "0.0";
            txtbOrderId.Text = "0";
            rdbDeliveryOrderType.IsChecked = false;
            rdbDineInOrderType.IsChecked = false;
            rdbTakeAwayOrderType.IsChecked = false;
            txtbtxtDiscount.Text = "0.0";
            txtbServiceDeliveryChargeLabel.Text = "0.0";
            txtSubTotalDiscountAmount.Text = "0.0";
            txtbTotalDiscountAmount.Text = "0.0";
            txtbTotalDeliveryChargeAmt.Text = "0.0";
            lbTablesList.SelectedIndex = -1;
            cmbPPDiscountNos.SelectedIndex = 0;
            cmbPPPercentageDelivery.SelectedIndex = 0;
            txtbKitchenStatusTitle.Visibility = Visibility.Hidden;
            txtbKitchenStatus.Visibility = Visibility.Hidden;
            txtDiscountPassword.Password = string.Empty;
        }
        private void CommonOrderCalculation(object sender, string type)
        {
            if (type == "FoodMenu")
            {
                var menuListPanel = sender as StackPanel;
                var salePrice = menuListPanel.Children[0] as TextBlock;

                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
            }

            if (type == "FoodMenuGridList")
            {
                FoodMenu foodMenuItem = sender as FoodMenu;
                if (foodMenuItem == null) return;

                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(foodMenuItem.SalesPrice)).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(foodMenuItem.SalesPrice)).ToString();
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
            }

            if (type == "DiscountPercent")
            {
                decimal percentage = Convert.ToDecimal(cmbPPDiscountNos.SelectionBoxItem);
                txtbtxtDiscount.Text = Convert.ToDecimal(percentage).ToString("0.00");
                txtbTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString("0.00");
                txtSubTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100)).ToString();
            }

            if (type == "DiscountAmount")
            {
                txtbTotalDiscountAmount.Text = txtSubTotalDiscountAmount.Text;
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(txtSubTotalDiscountAmount.Text)).ToString();
            }

            if (type == "DeliveryCharge")
            {
                decimal percentage = Convert.ToDecimal(cmbPPPercentageDelivery.SelectionBoxItem);
                txtbServiceDeliveryChargeLabel.Text = percentage.ToString("0.00");
                txtbTotalDeliveryChargeAmt.Text = (percentage).ToString("0.00");
                txtbTotalPayableAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) + percentage) - Convert.ToDecimal(txtSubTotalDiscountAmount.Text)).ToString();
            }

        }
        private void GetFoodItems(string type)
        {
            string rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
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
        private void GetSearchFoodItems(string searchKey)
        {
            string rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            spSubCategory.Children.Clear();
            FoodMenuModel foodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];
            GenerateDynamicFoodItems(foodMenu, rootPath, searchKey, string.Empty);
        }
        private void GetWaiterList()
        {
            CommonViewModel commonViewModel = new CommonViewModel();
            List<WaiterModel> waiters = new List<WaiterModel>();
            waiters = commonViewModel.GetWaiters();
            cmbWaiter.ItemsSource = waiters;
            cmbWaiter.Text = "-- Select Waiter --";
            cmbWaiter.IsEditable = true;
            cmbCustomer.IsReadOnly = true;
            cmbWaiter.SelectedValuePath = "Id";
            cmbWaiter.DisplayMemberPath = "FullName";
            cmbWaiter.SelectedIndex = -1;
        }
        private void GetOrderList(int orderStatus, int orderType, string searchKey)
        {
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            List<CustomerOrderModel> customerOrderList = new List<CustomerOrderModel>();
            customerOrderList = customerOrderViewModel.GetCustomerOrderList(orderStatus, orderType, searchKey);
            lbCustomerOrderList.ItemsSource = customerOrderList;
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
                                        Convert.ToDecimal(saleItem[0].Total));
                }
                customerOrderModel.Id = 0;
                customerOrderModel.OutletId = LoginDetail.OutletId;
                customerOrderModel.SalesInvoiceNumber = "0";
                customerOrderModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerOrderModel.WaiterEmployeeId = Convert.ToInt32(cmbWaiter.SelectedValue);
                customerOrderModel.OrderType = orderType;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = tableId;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = Convert.ToDecimal(txtbtxtDiscount.Text);
                customerOrderModel.DiscountAmount = Convert.ToDecimal(txtSubTotalDiscountAmount.Text);
                customerOrderModel.DeliveryCharges = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
                customerOrderModel.TaxAmount = 0;
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
                                        Convert.ToDecimal(saleItem[0].Total));
                }
                customerOrderModel.Id = Convert.ToInt32(txtbOrderId.Text);
                customerOrderModel.OutletId = LoginDetail.OutletId;
                customerOrderModel.SalesInvoiceNumber = "0";
                customerOrderModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerOrderModel.WaiterEmployeeId = Convert.ToInt32(cmbWaiter.SelectedValue);
                customerOrderModel.OrderType = orderType;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = tableId;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = Convert.ToDecimal(txtbtxtDiscount.Text);
                customerOrderModel.DiscountAmount = Convert.ToDecimal(txtSubTotalDiscountAmount.Text);
                customerOrderModel.DeliveryCharges = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
                customerOrderModel.TaxAmount = 0;
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
                customerOrderModel.CustomerPaid = Convert.ToDecimal(txtPPPayAmount.Text);
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
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.PlaceOrderTitle, StatusMessages.PlaceOrderSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Information);
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
            txtPPCName.Text = string.Empty;
            txtPPCPhone.Text = string.Empty;
            txtPPCEmail.Text = string.Empty;
            txtPPCAddress.Text = string.Empty;
            cmbCustomer.SelectedIndex = -1;
        }
        #endregion
        #region Events
        private void GetSubCategory(object sender, RoutedEventArgs e)
        {
            var btnCategory = sender as Button;
            var categoryId = btnCategory.Name.Substring(3);//Get the button id
            GetFoodItems(categoryId);
        }
        private void GetPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var menuListPanel = sender as StackPanel;
            var salePrice = menuListPanel.Children[0] as TextBlock;
            var itemName = menuListPanel.Children[2] as TextBlock;
            var foodMenuId = menuListPanel.Children[3] as TextBlock;

            CommonOrderCalculation(sender, "FoodMenu");

            /*
          txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
          txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
          txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
         
            List<SaleItemModel> saleItems = new List<SaleItemModel>();
            saleItems.Add(new SaleItemModel()
            {
                FoodMenuId = foodMenuId.Text,
                Product = itemName.Text,
                Price = Convert.ToDecimal(salePrice.Text),
                Qty = 1,
                Discount = 0,
                Total = Convert.ToDecimal(salePrice.Text) * 1,
                CustomerOrderItemId = 0
            });
            dgSaleItem.Items.Add(saleItems);
             */

            List<SaleItemModel> saleItems = new List<SaleItemModel>();
            saleItems.Add(new SaleItemModel()
            {
                FoodMenuId = foodMenuId.Text,
                Product = itemName.Text,
                Price = Convert.ToDecimal(salePrice.Text),
                Qty = 1,
                Discount = 0,
                Total = Convert.ToDecimal(salePrice.Text) * 1,
                CustomerOrderItemId = 0
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
        //private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    GetSearchFoodItems(txtSearch.Text.ToLower());
        //}
        private void btnPlusQty_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty += 1;
            saleItem[0].Total += saleItem[0].Price * 1;
            txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(saleItem[0].Price)).ToString();
            txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(saleItem[0].Price)).ToString();
            txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();
            dgSaleItem.Items.Refresh();
        }
        private void btnMinusQty_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty -= 1;
            saleItem[0].Total -= saleItem[0].Price * 1;
            txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(saleItem[0].Price)).ToString();
            txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) - Convert.ToDecimal(saleItem[0].Price)).ToString();
            txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) - 1).ToString();
            if (saleItem[0].Qty == 0)
            {
                dgSaleItem.Items.RemoveAt(dgSaleItem.SelectedIndex);
            }
            dgSaleItem.Items.Refresh();
        }
        private void btnEditSaleItem_Click(object sender, RoutedEventArgs e)
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
        private void btnPopUpPlusQty_Click(object sender, RoutedEventArgs e)
        {
            txtbPopUpQtyCount.Text = (Convert.ToDecimal(txtbPopUpQtyCount.Text) + 1).ToString();
            txtbPopUpItemTotal.Text = (Convert.ToInt32(txtbPopUpItemTotal.Text) + (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
            txtbPopUpItemSubTotalAmount.Text = (Convert.ToInt32(txtbPopUpItemSubTotalAmount.Text) + (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
        }
        private void btnPopUpMinusQty_Click(object sender, RoutedEventArgs e)
        {
            if (txtbPopUpQtyCount.Text != "1")
            {
                txtbPopUpQtyCount.Text = (Convert.ToDecimal(txtbPopUpQtyCount.Text) - 1).ToString();
                txtbPopUpItemTotal.Text = (Convert.ToInt32(txtbPopUpItemTotal.Text) - (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
                txtbPopUpItemSubTotalAmount.Text = (Convert.ToInt32(txtbPopUpItemSubTotalAmount.Text) - (Convert.ToDecimal(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
            }
        }
        private void btnPopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            EditSaleItemPopUp.IsOpen = false;
            dgSaleItem.Items.Refresh();
        }
        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
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
            txtbKitchenStatusTitle.Visibility = Visibility.Hidden;
            txtbKitchenStatus.Visibility = Visibility.Hidden;
            PlaceOrder("Pending");
        }
        private void btnPopUpAddToCart_Click(object sender, RoutedEventArgs e)
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
        private void txtPopUpDiscount_TextChanged(object sender, TextChangedEventArgs e)
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
        private void btnCancel_Click(object sender, RoutedEventArgs e)
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

                    insertedId = customerOrderViewModel.UpdateOrderStatus(orderId, (int)EnumUtility.OrderPaidStatus.Cancel);
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
        private void btnCalculator_Click(object sender, RoutedEventArgs e)
        {
            Calculator winCalCulator = new Calculator();

            winCalCulator.Width = 237;
            winCalCulator.Height = 310;
            winCalCulator.Top = 100;
            winCalCulator.Left = 500;

            winCalCulator.ShowDialog();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
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
                outletRegisterReport.Show();

                Login frmlogin = new Login();
                frmlogin.Show();
                this.Close();

            }
        }
        #region Search Order Left
        private void epOrder_LostFocus(object sender, RoutedEventArgs e)
        {
            var expander = sender as Expander;
            expander.IsExpanded = false;
            // expander.Background = Brushes.LightGray;
        }
        private void epOrder_Expanded(object sender, RoutedEventArgs e)
        {
            var expander = sender as Expander;
            // expander.Background = Brushes.DarkGray;
        }
        private void btnModifyOrder_Click(object sender, RoutedEventArgs e)
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

            txtbSubTotalAmount.Text = customerOrderModel.GrossAmount.ToString();
            txtbTotalPayableAmount.Text = customerOrderModel.TotalPayable.ToString();
            txtbOrderId.Text = customerOrderModel.Id.ToString();
            cmbCustomer.SelectedValue = customerOrderModel.CustomerId;
            cmbWaiter.SelectedValue = customerOrderModel.WaiterEmployeeId;
            txtbDineInTableId.Text = customerOrderModel.TableId;
            txtSubTotalDiscountAmount.Text = customerOrderModel.DiscountAmount.ToString();
            txtbTotalDiscountAmount.Text = customerOrderModel.DiscountAmount.ToString();
            txtbTotalDeliveryChargeAmt.Text = customerOrderModel.DeliveryCharges.ToString();

            if (customerOrderModel.KotStatus == 1)
            {
                txtbKitchenStatus.Text = "Pending";
                txtbKitchenStatus.Text += " [Table# " + customerOrderModel.TableId + "]";
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
                });
                dgSaleItem.Items.Add(saleItems);
                txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + orderItem.FoodMenuQty).ToString();
            }
        }
        private void searchSalesOrder(object sender, RoutedEventArgs e)
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
        #endregion
        #region Direct Invoice PopUp
        private void btnDirectInvoice_Click(object sender, RoutedEventArgs e)
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
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            List<PaymentMethodModel> paymentMethodModels = new List<PaymentMethodModel>();
            ppDirectInvoice.IsOpen = true;
            lblPPTotalPayableAmount.Content = txtbTotalPayableAmount.Text;
            txtPPPayAmount.Text = txtbTotalPayableAmount.Text;
            paymentMethodModels = customerOrderViewModel.GetPaymentMethod();
            cmbPPPaymentMethod.ItemsSource = paymentMethodModels;
        }
        private void btnDirectInvoicePopupCancel_Click(object sender, RoutedEventArgs e)
        {
            ppDirectInvoice.IsOpen = false;
        }
        private void txtPPGivenAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPPGivenAmount.Text))
            {
                txtPPGivenAmount.Text = "0";
            }
            lblPPChangeAmountTotal.Content = (Convert.ToDecimal(txtPPGivenAmount.Text) - Convert.ToDecimal(lblPPTotalPayableAmount.Content)).ToString();
        }
        private void btnDirectInvoiceSubmit_Click(object sender, RoutedEventArgs e)
        {
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
            customerBillModel.VatableAmount = 0;
            customerBillModel.TotalAmount = customerOrderModel.TotalPayable;
            customerBillModel.BillStatus = (int)EnumUtility.OrderPaidStatus.FullPaid;
            customerBillModel.OutletRegisterId = 4;
            customerBillModel.UserId = LoginDetail.UserId;
            customerBillModel.PaymentMethodId = Convert.ToInt32(cmbPPPaymentMethod.SelectedValue);
            customerBillModel.PaymentNumber = string.Empty;

            //customerBillModel.OutletId = LoginDetail.OutletId;
            //customerBillModel.CustomerOrderId = orderId;
            //customerBillModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
            //customerBillModel.GrossAmount = Convert.ToDecimal(lblPPTotalPayableAmount.Content);
            //customerBillModel.Discount = Convert.ToDecimal(txtSubTotalDiscountAmount.Text);
            //customerBillModel.ServiceCharge = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
            //customerBillModel.VatableAmount = 0;
            //customerBillModel.TotalAmount = Convert.ToDecimal(lblPPTotalPayableAmount.Content);
            //customerBillModel.BillStatus = (int)EnumUtility.OrderPaidStatus.FullPaid;
            //customerBillModel.OutletRegisterId = 4;
            //customerBillModel.UserId = LoginDetail.UserId;
            //customerBillModel.PaymentMethodId = Convert.ToInt32(cmbPPPaymentMethod.SelectedValue);
            //customerBillModel.PaymentNumber = string.Empty;

            insertedId = customerBillViewModel.InsertBillDetail(customerBillModel);
            ppDirectInvoice.IsOpen = false;
            if (insertedId > 0)
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, StatusMessages.BillDetailSaveSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                ClearCustomerOrderItemControll();
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.AppTitle, StatusMessages.BillDetailSaveSuccess, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
            }
            printReceipt.Print(appSettings.GetPrinterName(), customerBillModel.CustomerOrderId);
        }
        #endregion
        #region Customer Add/Edit
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            ResetCustomer();
            ppCustomerAdd.IsOpen = true;
        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCustomer.SelectedIndex != -1)
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSelectRequired, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                Keyboard.Focus(cmbCustomer);
                return;
            }
            CustomerViewModel customerViewModel = new CustomerViewModel();
            CustomerModel customerModel = new CustomerModel();
            customerModel = customerViewModel.GetCustomerById(Convert.ToInt32(cmbCustomer.SelectedValue));
            txtPPCName.Text = customerModel.CustomerName;
            txtPPCPhone.Text = customerModel.CustomerPhone;
            txtPPCEmail.Text = customerModel.CustomerEmail;
            txtPPCAddress.Text = customerModel.CustomerAddress1;
            ppCustomerAdd.IsOpen = true;
        }
        private void btnPPCCancel_Click(object sender, RoutedEventArgs e)
        {
            ppCustomerAdd.IsOpen = false;
        }
        private void btnPPCAddCustomer_Click(object sender, RoutedEventArgs e)
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
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.CustomerTitle, StatusMessages.CustomerSaveFailed, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
            }
        }
        #endregion
        #region Discount Service Charge PopUp
        private void btnDicountPopUp_Click(object sender, RoutedEventArgs e)
        {
            ppPassword.IsOpen = true;
        }
        private void btnPPPaswordApply_Click(object sender, RoutedEventArgs e)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            int validId = 0;
            validId = loginViewModel.ValidateDiscountPassword(txtDiscountPassword.Password);
            if (validId > 0)
            {
                ppPassword.IsOpen = false;
                ppDiscountPopUp.IsOpen = true;
            }
            else
            {
                ppPassword.Focus();
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyPasswordTitle, StatusMessages.WrongPassword, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Error);
                txtDiscountPassword.Focus();
                return;
            }
        }

        private void btnPPPaswordCancel_Click(object sender, RoutedEventArgs e)
        {
            ppPassword.IsOpen = false;
            txtDiscountPassword.Password = string.Empty;
        }
        private void btnPPDiscountCancel_Click(object sender, RoutedEventArgs e)
        {
            cmbPPDiscountNos.SelectedIndex = -1;
            ppDiscountPopUp.IsOpen = false;
        }
        private void btnPPDiscountApply_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPPDiscountNos.SelectedIndex != 0)
            {
                CommonOrderCalculation(sender, "DiscountPercent");
                //txtbtxtDiscount.Text = Convert.ToDecimal(percentage).ToString("0.00");
                //txtbTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString("0.00");
                //txtSubTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString();
                //txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100)).ToString();
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyDiscountTitle, StatusMessages.PercentageSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                return;
            }
            cmbPPDiscountNos.SelectedIndex = 0;
            ppDiscountPopUp.IsOpen = false;
        }
        private void btnServiceDeliveryPopUp_Click(object sender, RoutedEventArgs e)
        {
            ppDeliveryServicePopUp.IsOpen = true;
        }
        private void btnPPDeliveryApply_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPPPercentageDelivery.SelectedIndex != 0)
            {
                CommonOrderCalculation(sender, "DeliveryCharge");
                //decimal percentage = Convert.ToDecimal(cmbPPPercentageDelivery.SelectionBoxItem);
                //txtbServiceDeliveryChargeLabel.Text = percentage.ToString("0.00");
                //txtbTotalDeliveryChargeAmt.Text = (percentage).ToString("0.00");
                //txtbTotalPayableAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) + percentage) - Convert.ToDecimal(txtSubTotalDiscountAmount.Text)).ToString();
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.ApplyServiceChargeTitle, StatusMessages.ServiceChargeSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                return;
            }
            ppDeliveryServicePopUp.IsOpen = false;
            cmbPPPercentageDelivery.SelectedIndex = 0;
        }
        private void btnPPDeliveryCancel_Click(object sender, RoutedEventArgs e)
        {
            cmbPPPercentageDelivery.SelectedIndex = -1;
            ppDeliveryServicePopUp.IsOpen = false;
        }
        private void txtSubTotalDiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSubTotalDiscountAmount.Text))
            {
                CommonOrderCalculation(sender, "DiscountAmount");
                //txtbTotalDiscountAmount.Text = txtSubTotalDiscountAmount.Text;
                //txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - Convert.ToDecimal(txtSubTotalDiscountAmount.Text)).ToString();
            }
            else
            {
                txtbTotalPayableAmount.Text = txtbSubTotalAmount.Text;
                txtbTotalDiscountAmount.Text = "0.00";
                txtSubTotalDiscountAmount.Text = "0.00";
            }
        }
        #endregion
        private void btnHold_Click(object sender, RoutedEventArgs e)
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
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Login frmLogin = new Login();

            frmLogin.Show();
            this.Close();
        }
        #endregion
        private void btnPPCancelTable_Click(object sender, RoutedEventArgs e)
        {
            ppDineInTables.IsOpen = false;
        }
        private void btnPPSelectTable_Click(object sender, RoutedEventArgs e)
        {
            TableViewModel tableViewModel = new TableViewModel();
            if (lbTablesList.SelectedIndex != -1)
            {
                txtbDineInTableId.Text = lbTablesList.SelectedValue.ToString();
                tableViewModel.UpdateTableStatus(txtbDineInTableId.Text, (int)EnumUtility.TableStatus.Occupied);
            }
            else
            {
                var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInSelect, StatusMessages.DineInSelect, MessageBoxButton.OK, EnumUtility.MessageBoxImage.Warning);
                return;
            }
            ppDineInTables.IsOpen = false;
        }
        private void rdbDineInOrderType_Click(object sender, RoutedEventArgs e)
        {
            TableViewModel tableViewModel = new TableViewModel();
            List<TableModel> tables = new List<TableModel>();
            ppDineInTables.IsOpen = true;
            tables = tableViewModel.GetTables(LoginDetail.OutletId);//outletId
            lbTablesList.ItemsSource = tables;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            txtDatetime.Text =   DateTime.Now.ToLongTimeString();
        }
        private void Timer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void HeaderFooter()
        {
            txtClientName.Text = LoginDetail.ClientName + "  |  ";
            txbOutletName.Text = LoginDetail.OutletName;
            txtbUserName.Text = "User: " + LoginDetail.Username;
            txtWebsite.Text = LoginDetail.WebSite;
            txtSystemDate.Text = LoginDetail.SystemDate.ToShortDateString();
        }

        private void btnLastSale_Click(object sender, RoutedEventArgs e)
        {
            CustomerOrderHistoryList customerOrderHistoryList = new CustomerOrderHistoryList();
            customerOrderHistoryList.Show();
        }

        #region FoodMenuList PopUp
        private void btnFoodMenuList_Click(object sender, RoutedEventArgs e)
        {
            FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
            List<FoodMenu> foodMenus = new List<FoodMenu>();
            foodMenus = foodMenuViewModel.GetFoodMenuPopUpList(LoginDetail.OutletId, string.Empty);
            ppFoodMenuList.IsOpen = true;
            dgFoodMenuList.ItemsSource = foodMenus;
        }

        private void btnPPFoodListCancel_Click(object sender, RoutedEventArgs e)
        {
            ppFoodMenuList.IsOpen = false;

        }

        private void txtSearchFoodMenuList_TextChanged(object sender, TextChangedEventArgs e)
        {
            FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
            List<FoodMenu> foodMenus = new List<FoodMenu>();
            foodMenus = foodMenuViewModel.GetFoodMenuPopUpList(LoginDetail.OutletId, txtSearchFoodMenuList.Text);
            dgFoodMenuList.ItemsSource = foodMenus;
        }

        private void dgFoodMenuList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FoodMenu foodMenuItem = new FoodMenu();
            var foodItem = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;

            if (foodItem == null) return;
            foodMenuItem = (FoodMenu)foodItem.Item;

            //Add Into Grid
            //txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(foodMenuItem.SalesPrice)).ToString();
            //txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(foodMenuItem.SalesPrice)).ToString();
            //txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();

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
        #endregion

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = ((screenHeight / 2) - (windowHeight / 2));
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            ClearCustomerOrderItemControll();
        }

        private void btnDineTableView_Click(object sender, RoutedEventArgs e)
        {
            DineInTables dineInTables = new DineInTables();
            dineInTables.Show();
        }
    }
}
