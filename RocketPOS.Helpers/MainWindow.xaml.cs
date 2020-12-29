using System;
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

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GenerateDynamicFoodMenu();
            GetWaiterList();
            GetCustomerList();
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
        }
        private void GenerateDynamicFoodMenu()
        {
            string rootPath = string.Empty;
            FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();
            FoodMenuModel foodMenu = new FoodMenuModel();
            rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;

            if (Application.Current.Resources["FoodList"] == null)
            {
                foodMenu = foodMenuViewModel.GetFoodMenu();
                Application.Current.Resources["FoodList"] = foodMenu;
            }
            else
            {
                foodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];
            }

            foreach (var foodCategory in foodMenu.FoodList)
            {
                Button btnCategory = new Button();
                btnCategory.Content = foodCategory.FoodCategory;
                btnCategory.Name = "btn" + foodCategory.Id;
                btnCategory.Width = 100;
                btnCategory.Height = 50;
                btnCategory.Margin = new Thickness(5, 0, 5, 10);
                btnCategory.Click += GetSubCategory;
                spCategory.Children.Add(btnCategory);
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
            cmbWaiter.SelectedIndex = 0;
            cmbCustomer.SelectedIndex = 0;
            txtbTotalPayableAmount.Text = "0";
            txtbSubTotalAmount.Text = "0";
            txtbTotalItemCount.Text = "0";
            txtbOrderId.Text = "0";
            rdbDeliveryOrderType.IsChecked = false;
            rdbDineInOrderType.IsChecked =false; 
            rdbTakeAwayOrderType.IsChecked = false;
            txtDiscount.Text = "0.0";
            txtServiceDeliveryCharge.Text = "0.0";
            txtbSubTotalDiscountAmount.Text = "0";
            txtbTotalDeliveryChargeAmt.Text = "0";
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
        }
        private void GetOrderList(int orderStatus, int orderType,string searchKey)
        {
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            List<CustomerOrderModel> customerOrderList = new List<CustomerOrderModel>();
            customerOrderList = customerOrderViewModel.GetCustomerOrderList(orderStatus, orderType,searchKey);
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
                customerOrderModel.OutletId = 1;
                customerOrderModel.SalesInvoiceNumber = "0";
                customerOrderModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerOrderModel.WaiterEmployeeId = Convert.ToInt32(cmbWaiter.SelectedValue);
                customerOrderModel.OrderType = 1;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = 1;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = Convert.ToDecimal(txtDiscount.Text);
                customerOrderModel.DiscountAmount = Convert.ToDecimal(txtbSubTotalDiscountAmount.Text);
                customerOrderModel.DeliveryCharges = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
                customerOrderModel.TaxAmount = 0;
                customerOrderModel.TotalPayable = Convert.ToDecimal(txtbTotalPayableAmount.Text);
                customerOrderModel.CustomerPaid = 0;
                customerOrderModel.CustomerNote = null;
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
                customerOrderModel.AnyReason = null;
                customerOrderModel.UserIdInserted = 1;
                customerOrderModel.DateInserted = System.DateTime.Now;
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
                customerOrderModel.OutletId = 1;
                customerOrderModel.SalesInvoiceNumber = "0";
                customerOrderModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                customerOrderModel.WaiterEmployeeId = Convert.ToInt32(cmbWaiter.SelectedValue);
                customerOrderModel.OrderType = 1;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = 1;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = Convert.ToDecimal(txtDiscount.Text);
                customerOrderModel.DiscountAmount = Convert.ToDecimal(txtbSubTotalDiscountAmount.Text);
                customerOrderModel.DeliveryCharges = Convert.ToDecimal(txtbTotalDeliveryChargeAmt.Text);
                customerOrderModel.TaxAmount = 0;
                customerOrderModel.TotalPayable = Convert.ToDecimal(txtbTotalPayableAmount.Text);
                customerOrderModel.CustomerPaid = 0;
                customerOrderModel.CustomerNote = null;
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
                customerOrderModel.AnyReason = null;
                customerOrderModel.UserIdInserted = 1;
                customerOrderModel.DateInserted = System.DateTime.Now;
            }


            if (type == "DirectInvoice")
            {
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.FullPaid;
            }
            else if (type == "Hold")
            {
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Hold;
            }
            else
            {
                customerOrderModel.OrderStatus = (int)EnumUtility.OrderPaidStatus.Pending;
            }

            insertedId = customerOrderViewModel.InsertCustomerOrder(customerOrderModel, customerOrderItem);
            txtbOrderId.Text = insertedId.ToString();

            if (insertedId > 0)
            {
                //MessageBox.Show(StatusMessages.PlaceOrderSuccess);
                MessageBox.Show(StatusMessages.PlaceOrderSuccess, "Place Order", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                ClearCustomerOrderItemControll();
            }
            else
            {
                //MessageBox.Show(StatusMessages.PlaceOrderFailed);
                MessageBox.Show(StatusMessages.PlaceOrderFailed, "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            return insertedId;
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

            txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
            txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + Convert.ToDecimal(salePrice.Text)).ToString();
            txtbTotalItemCount.Text = (Convert.ToDecimal(txtbTotalItemCount.Text) + 1).ToString();

            List<SaleItemModel> saleItems = new List<SaleItemModel>();
            saleItems.Add(new SaleItemModel()
            {
                FoodMenuId = foodMenuId.Text,
                Product = itemName.Text,
                Price = Convert.ToUInt64(salePrice.Text),
                Qty = 1,
                Discount = 0,
                Total = Convert.ToUInt64(salePrice.Text) * 1,
                CustomerOrderItemId = 0
            });
            dgSaleItem.Items.Add(saleItems);
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetSearchFoodItems(txtSearch.Text.ToLower());
        }
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
            if (dgSaleItem.Items.Count==0)
            {
                MessageBox.Show("Cart is empty!", "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            if (rdbDeliveryOrderType.IsChecked == false && rdbDineInOrderType.IsChecked == false && rdbTakeAwayOrderType.IsChecked == false)
            {
                MessageBox.Show("You must select Dine In or Take Away or Delivery!", "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            if (cmbWaiter.SelectedItem == null)
            {
                MessageBox.Show("Please Select Waiter.", "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                cmbWaiter.Focus();
                return;
            }

            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Please Select Customer.", "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                cmbCustomer.Focus();
                return;
            }
            btnKitchenStatus.Visibility = Visibility.Hidden;
            PlaceOrder("Pending");
        }
        private void btnPopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty = Convert.ToDecimal(txtbPopUpQtyCount.Text);
            saleItem[0].Total = Convert.ToDouble(txtbPopUpItemSubTotalAmount.Text);
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
            ClearCustomerOrderItemControll();
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
            outletRegisterModel.USerID = 1;
            outletRegisterModel.OutletId = 1;
            outletRegisterModel.OpeningBalance = 500;

            outletRegisterViewModel.InsertOutletRegister(outletRegisterModel);

            //Close register
            outletRegisterViewModel.UpdateOutletRegister(outletRegisterModel);
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
            ClearCustomerOrderItemControll();
            btnKitchenStatus.Visibility = Visibility.Visible;
            var st = (CustomerOrderModel)lbCustomerOrderList.SelectedItem;
            CustomerOrderModel customerOrderModel = new CustomerOrderModel();
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            customerOrderModel = customerOrderViewModel.GetCustomerOrderByOrderId(st.Id);

            txtbSubTotalAmount.Text = customerOrderModel.GrossAmount.ToString();
            txtbTotalPayableAmount.Text = customerOrderModel.TotalPayable.ToString();
            txtbOrderId.Text = customerOrderModel.Id.ToString();
            cmbCustomer.SelectedValue = customerOrderModel.CustomerId;
            cmbWaiter.SelectedValue = customerOrderModel.WaiterEmployeeId;

            List<SaleItemModel> saleItems = new List<SaleItemModel>();
            foreach (var orderItem in customerOrderModel.CustomerOrderItemModels)
            {
                saleItems = new List<SaleItemModel>();
                saleItems.Add(new SaleItemModel()
                {
                    FoodMenuId = orderItem.FoodMenuId.ToString(),
                    Product = orderItem.FoodMenuName,
                    Price = Convert.ToDouble(orderItem.FoodMenuRate),
                    Qty = orderItem.FoodMenuQty,
                    Discount = orderItem.Discount,
                    Total = Convert.ToUInt64(orderItem.Price),
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

            CustomerBillViewModel customerBillViewModel = new CustomerBillViewModel();
            CustomerBillModel customerBillModel = new CustomerBillModel();
            int insertedId = 0;

            customerBillModel.OutletId = 1;
            customerBillModel.CustomerOrderId = orderId;
            customerBillModel.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
            customerBillModel.GrossAmount = Convert.ToDecimal(lblPPTotalPayableAmount.Content);
            customerBillModel.Discount = 0;
            customerBillModel.ServiceCharge = 0;
            customerBillModel.VatableAmount = 0;
            customerBillModel.TotalAmount = Convert.ToDecimal(lblPPTotalPayableAmount.Content);
            customerBillModel.BillStatus = 1;
            customerBillModel.OutletRegisterId = 4;
            customerBillModel.UserId = 2;
            customerBillModel.PaymentMethodId = Convert.ToInt32(cmbPPPaymentMethod.SelectedValue);
            customerBillModel.PaymentNumber = "12345";

            insertedId = customerBillViewModel.InsertBillDetail(customerBillModel);
            ppDirectInvoice.IsOpen = false;
            if (insertedId > 0)
            {
                MessageBox.Show(StatusMessages.BillDetailSaveSuccess);
                ClearCustomerOrderItemControll();
            }
            else
            {
                MessageBox.Show(StatusMessages.BillDetailSaveFailed);
            }

            ReceiptPrintView pj = new ReceiptPrintView();
            pj.Print("Microsoft Print to PDF", customerBillModel.CustomerOrderId);
        }
        #endregion
        #region Customer Add/Edit
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            ppCustomerAdd.IsOpen = true;

        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show(StatusMessages.CustomerSelectRequired, "Add Customer", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
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
                MessageBox.Show(StatusMessages.CustomerNameRequired, "Add Customer", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Keyboard.Focus(txtPPCName);
                return;
            }

            if (string.IsNullOrEmpty(txtPPCPhone.Text))
            {
                MessageBox.Show(StatusMessages.CustomerPhoneRequired, "Add Customer", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Keyboard.Focus(txtPPCPhone);
                return;
            }

            customerModel.Id = Convert.ToInt32(cmbCustomer.SelectedValue);
            customerModel.CustomerName = txtPPCName.Text;
            customerModel.CustomerPhone = txtPPCPhone.Text;
            customerModel.CustomerEmail = txtPPCEmail.Text;
            customerModel.CustomerAddress1 = txtPPCAddress.Text;
            customerModel.UserId = 1;
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
                MessageBox.Show(StatusMessages.CustomerSaveFailed, "Add Customer", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        #endregion
        #region Discount Service Charge PopUp
        private void btnDicountPopUp_Click(object sender, RoutedEventArgs e)
        {
            ppDiscountPopUp.IsOpen = true;
        }
        private void btnPPDiscountCancel_Click(object sender, RoutedEventArgs e)
        {
            ppDiscountPopUp.IsOpen = false;
        }
        private void btnPPDiscountApply_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbPPDiscountNos.SelectionBoxItem.ToString()))
            {
                decimal percentage = Convert.ToDecimal(cmbPPDiscountNos.SelectionBoxItem);
                txtDiscount.Text = percentage.ToString();
                txtbSubTotalDiscountAmount.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) - ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100)).ToString();
            }
            ppDiscountPopUp.IsOpen = false;
        }
        private void btnServiceDeliveryPopUp_Click(object sender, RoutedEventArgs e)
        {
            ppDeliveryServicePopUp.IsOpen = true;
        }
        private void btnPPDeliveryApply_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbPPPercentageDelivery.SelectionBoxItem.ToString()))
            {
                decimal percentage = Convert.ToDecimal(cmbPPPercentageDelivery.SelectionBoxItem);
                txtServiceDeliveryCharge.Text = percentage.ToString();
                txtbTotalDeliveryChargeAmt.Text = ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + ((Convert.ToDecimal(txtbSubTotalAmount.Text) * percentage) / 100)).ToString();
            }
            ppDeliveryServicePopUp.IsOpen = false;
        }
        private void btnPPDeliveryCancel_Click(object sender, RoutedEventArgs e)
        {
            ppDeliveryServicePopUp.IsOpen = false;
        }
        #endregion
        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            if (cmbWaiter.SelectedItem == null)
            {
                MessageBox.Show("Please Select Waiter.", "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                cmbWaiter.Focus();
                return;
            }

            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Please Select Customer.", "Place Order", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                cmbCustomer.Focus();
                return;
            }
            PlaceOrder("Hold");
        }
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
