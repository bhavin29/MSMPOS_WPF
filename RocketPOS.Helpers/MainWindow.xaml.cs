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

            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            List<CustomerOrderModel> customerOrderList = new List<CustomerOrderModel>();
            customerOrderList = customerOrderViewModel.GetCustomerOrderList();
            lbCustomerOrderList.ItemsSource = customerOrderList;
        }

        #region Methods
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
            txtbTotalPayableAmount.Text = "0";
            txtbSubTotalAmount.Text = "0";
            txtbTotalItemCount.Text = "0";
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
                GenerateDynamicFoodItems(foodMenu, rootPath,string.Empty,type);
            }
        }
        private void GetSearchFoodItems(string searchKey)
        {
            string rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            spSubCategory.Children.Clear();
            FoodMenuModel foodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];
            GenerateDynamicFoodItems(foodMenu, rootPath, searchKey, string.Empty);
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
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            CustomerOrderModel customerOrderModel = new CustomerOrderModel();
            CustomerOrderItemModel customerOrderItemModel = new CustomerOrderItemModel();
            List<CustomerOrderItemModel> customerOrderItemModels = new List<CustomerOrderItemModel>();

            int insertedId = 0;
            if (Convert.ToInt32(txtbOrderId.Text)==0)
            {
                DataTable customerOrderItem = new DataTable();
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
                customerOrderModel.CustomerId = 1;
                customerOrderModel.WaiterEmployeeId = 1;
                customerOrderModel.OrderType = 1;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = 1;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = 0;
                customerOrderModel.DiscountAmount = 0;
                customerOrderModel.DeliveryCharges = 0;
                customerOrderModel.TaxAmount = 0;
                customerOrderModel.TotalPayable = Convert.ToDecimal(txtbTotalPayableAmount.Text);
                customerOrderModel.CustomerPaid = 0;
                customerOrderModel.CustomerNote = null;
                customerOrderModel.OrderStatus = 0;
                customerOrderModel.AnyReason = null;
                customerOrderModel.UserIdInserted = 1;
                customerOrderModel.DateInserted = System.DateTime.Now;

                insertedId = customerOrderViewModel.InsertCustomerOrder(customerOrderModel, customerOrderItem); 
            }
            else
            {
               

                DataTable customerOrderItem = new DataTable();
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
                customerOrderModel.CustomerId = 1;
                customerOrderModel.WaiterEmployeeId = 1;
                customerOrderModel.OrderType = 1;
                customerOrderModel.OrderDate = System.DateTime.Now;
                customerOrderModel.TableId = 1;
                customerOrderModel.TockenNumber = "0";
                customerOrderModel.GrossAmount = Convert.ToDecimal(txtbSubTotalAmount.Text);
                customerOrderModel.DiscountPercentage = 0;
                customerOrderModel.DiscountAmount = 0;
                customerOrderModel.DeliveryCharges = 0;
                customerOrderModel.TaxAmount = 0;
                customerOrderModel.TotalPayable = Convert.ToDecimal(txtbTotalPayableAmount.Text);
                customerOrderModel.CustomerPaid = 0;
                customerOrderModel.CustomerNote = null;
                customerOrderModel.OrderStatus = 0;
                customerOrderModel.AnyReason = null;
                customerOrderModel.UserIdInserted = 1;
                customerOrderModel.DateInserted = System.DateTime.Now;

                insertedId = customerOrderViewModel.InsertCustomerOrder(customerOrderModel, customerOrderItem);
            }

            if (insertedId > 0)
            {
                MessageBox.Show(StatusMessages.PlaceOrderSuccess);
                ClearCustomerOrderItemControll();
            }
            else
            {
                MessageBox.Show(StatusMessages.PlaceOrderFailed);
            }
        }
        private void btnPopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty = Convert.ToDecimal(txtbPopUpQtyCount.Text);
            saleItem[0].Total = Convert.ToDouble(txtbPopUpItemTotal.Text);
            if (txtbPopUpQtyCount.Text != "1")
            {
                txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)) + Convert.ToInt32(txtbTotalItemCount.Text)).ToString();
                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
            }
            else if (txtbPopUpQtyCount.Text == "1" && txtbPopUpQtyCount.Text != txtbPopUpOriginalQtyCount.Text)
            {
                txtbTotalItemCount.Text = ((Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)) + Convert.ToInt32(txtbTotalItemCount.Text)).ToString();
                txtbSubTotalAmount.Text = (Convert.ToDecimal(txtbSubTotalAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToDecimal(txtbTotalPayableAmount.Text) + (Convert.ToDecimal(saleItem[0].Price) * (Convert.ToDecimal(txtbPopUpQtyCount.Text) - Convert.ToDecimal(txtbPopUpOriginalQtyCount.Text)))).ToString();
            }
            EditSaleItemPopUp.IsOpen = false;
            dgSaleItem.Items.Refresh();
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
        private void btnCreateInvoiceAndClose_Click(object sender, RoutedEventArgs e)
        {
            ReceiptPrintView pj = new ReceiptPrintView();

            pj.Print("Microsoft Print to PDF");
        }
        private void epOrder_LostFocus(object sender, RoutedEventArgs e)
        {
            var expander = sender as Expander;
            expander.IsExpanded = false;
            expander.Background = Brushes.LightGray;
        }
        private void epOrder_Expanded(object sender, RoutedEventArgs e)
        {
            var expander = sender as Expander;
            expander.Background = Brushes.DarkGray;
        }
        private void btnModifyOrder_Click(object sender, RoutedEventArgs e)
        {
            ClearCustomerOrderItemControll();
            var st = (CustomerOrderModel)lbCustomerOrderList.SelectedItem;
            CustomerOrderModel customerOrderModel = new CustomerOrderModel();
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            customerOrderModel = customerOrderViewModel.GetCustomerOrderByOrderId(st.Id);

            txtbSubTotalAmount.Text = customerOrderModel.GrossAmount.ToString();
            txtbTotalPayableAmount.Text = customerOrderModel.TotalPayable.ToString();
            txtbOrderId.Text = customerOrderModel.Id.ToString();

            List <SaleItemModel> saleItems = new List<SaleItemModel>();
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
                    CustomerOrderItemId= orderItem.CustomerOrderItemId,
                });
                dgSaleItem.Items.Add(saleItems);
            }
        }
        #endregion


    }
}
