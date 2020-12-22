using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RocketPOS.Model;
using System.Reflection;
using RocketPOS.ViewModels;

namespace RocketPOS.Helpers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // StyleManager.ApplicationTheme = new Expression_DarkTheme();

            InitializeComponent();
            FoodMenuViewModel foodMenuViewModel = new FoodMenuViewModel();

            FoodMenuModel foodMenu = foodMenuViewModel.GetFoodMenu();
            string rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;

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

            foreach (var foodCategory in foodMenu.FoodList)
            {
                foreach (var itemSubCat in foodCategory.SubCategory)
                {
                    StackPanel menuListPanel = new StackPanel();
                    menuListPanel.Orientation = Orientation.Vertical;

                    TextBlock txtSalePrice = new TextBlock();
                    txtSalePrice.Text = itemSubCat.SalesPrice.ToString();
                    txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
                    menuListPanel.Children.Add(txtSalePrice);

                    Image imgFood = new Image();
                    imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\RocketPOS.StartUp\Images\" + itemSubCat.SmallThumb));
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
            }
        }
        private void GetPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var menuListPanel = sender as StackPanel;
            var salePrice = menuListPanel.Children[0] as TextBlock;
            var itemName = menuListPanel.Children[2] as TextBlock;
            var foodMenuId = menuListPanel.Children[3] as TextBlock;

            txtbSubTotalAmount.Text = (Convert.ToInt64(txtbSubTotalAmount.Text) + Convert.ToInt64(salePrice.Text)).ToString();
            txtbTotalPayableAmount.Text = (Convert.ToInt64(txtbTotalPayableAmount.Text) + Convert.ToInt64(salePrice.Text)).ToString();
            txtbTotalItemCount.Text = (Convert.ToInt64(txtbTotalItemCount.Text) + 1).ToString();

            List<SaleItemModel> saleItems = new List<SaleItemModel>();
            saleItems.Add(new SaleItemModel()
            {
                FoodMenuId = foodMenuId.Text,
                Product = itemName.Text,
                Price = Convert.ToUInt64(salePrice.Text),
                Qty = 1,
                Discount = 0,
                Total = Convert.ToUInt64(salePrice.Text) * 1
            });
            dgSaleItem.Items.Add(saleItems);
        }

        private void GetSubCategory(object sender, RoutedEventArgs e)
        {
            var btnCategory = sender as Button;
            var categoryId = btnCategory.Name.Substring(3);//Get the button id
            GetFoodItems(categoryId);
        }

        private void GetFoodItems(string type)
        {
            string rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            if (type == "All")
            {
                spSubCategory.Children.Clear();
                FoodMenuModel FoodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];

                foreach (var foodCategory in FoodMenu.FoodList)
                {
                    foreach (var itemSubCat in foodCategory.SubCategory)
                    {
                        StackPanel menuListPanel = new StackPanel();
                        menuListPanel.Orientation = Orientation.Vertical;

                        TextBlock txtSalePrice = new TextBlock();
                        txtSalePrice.Text = itemSubCat.SalesPrice.ToString();
                        txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
                        menuListPanel.Children.Add(txtSalePrice);

                        Image imgFood = new Image();
                        imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\RocketPOS.StartUp\Images\" + itemSubCat.SmallThumb));
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
                }
            }
            else
            {
                spSubCategory.Children.Clear();
                FoodMenuModel FoodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];

                foreach (var foodCategory in FoodMenu.FoodList)
                {
                    foreach (var itemSubCat in foodCategory.SubCategory)
                    {
                        if (itemSubCat.FoodCategoryId.ToString() == type)
                        {
                            StackPanel menuListPanel = new StackPanel();
                            menuListPanel.Orientation = Orientation.Vertical;

                            TextBlock txtSalePrice = new TextBlock();
                            txtSalePrice.Text = itemSubCat.SalesPrice.ToString();
                            txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
                            menuListPanel.Children.Add(txtSalePrice);

                            Image imgFood = new Image();
                            imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\RocketPOS.StartUp\Images\" + itemSubCat.SmallThumb));
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
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetSearchFoodItems(txtSearch.Text.ToLower());
        }

        private void GetSearchFoodItems(string searchKey)
        {
            string rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            spSubCategory.Children.Clear();
            FoodMenuModel FoodMenu = (FoodMenuModel)Application.Current.Resources["FoodList"];

            foreach (var foodCategory in FoodMenu.FoodList)
            {
                foreach (var itemSubCat in foodCategory.SubCategory)
                {
                    StackPanel menuListPanel = new StackPanel();
                    menuListPanel.Orientation = Orientation.Vertical;

                    if (itemSubCat.SmallName.ToLower().Contains(searchKey) || itemSubCat.SalesPrice.ToString().ToLower().Contains(searchKey))
                    {
                        TextBlock txtSalePrice = new TextBlock();
                        txtSalePrice.Text = itemSubCat.SalesPrice.ToString();
                        txtSalePrice.Name = "txtSalePrice" + itemSubCat.FoodCategoryId;
                        menuListPanel.Children.Add(txtSalePrice);

                        Image imgFood = new Image();
                        imgFood.Source = new BitmapImage(new System.Uri(rootPath + @"\RocketPOS.StartUp\Images\" + itemSubCat.SmallThumb));
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
                }
            }
        }

        private void btnPlusQty_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty += 1;
            saleItem[0].Total += saleItem[0].Price * 1;
            txtbSubTotalAmount.Text = (Convert.ToInt64(txtbSubTotalAmount.Text) + Convert.ToInt64(saleItem[0].Price)).ToString();
            txtbTotalPayableAmount.Text = (Convert.ToInt64(txtbTotalPayableAmount.Text) + Convert.ToInt64(saleItem[0].Price)).ToString();
            txtbTotalItemCount.Text = (Convert.ToInt64(txtbTotalItemCount.Text) + 1).ToString();
            dgSaleItem.Items.Refresh();
        }

        private void btnMinusQty_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty -= 1;
            saleItem[0].Total -= saleItem[0].Price * 1;
            txtbSubTotalAmount.Text = (Convert.ToInt64(txtbSubTotalAmount.Text) - Convert.ToInt64(saleItem[0].Price)).ToString();
            txtbTotalPayableAmount.Text = (Convert.ToInt64(txtbTotalPayableAmount.Text) - Convert.ToInt64(saleItem[0].Price)).ToString();
            txtbTotalItemCount.Text = (Convert.ToInt64(txtbTotalItemCount.Text) - 1).ToString();
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
            txtbPopUpItemSubTotalAmount.Text = Convert.ToInt64(saleItem[0].Total).ToString();
            EditSaleItemPopUp.IsOpen = true;
        }

        private void btnPopUpCancel_Click(object sender, RoutedEventArgs e)
        {
            EditSaleItemPopUp.IsOpen = false;
            dgSaleItem.Items.Refresh();
        }

        private void btnPopUpPlusQty_Click(object sender, RoutedEventArgs e)
        {
            txtbPopUpQtyCount.Text = (Convert.ToInt32(txtbPopUpQtyCount.Text) + 1).ToString();
            txtbPopUpItemTotal.Text = (Convert.ToInt32(txtbPopUpItemTotal.Text) + (Convert.ToInt64(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
            txtbPopUpItemSubTotalAmount.Text = (Convert.ToInt32(txtbPopUpItemSubTotalAmount.Text) + (Convert.ToInt64(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
        }

        private void btnPopUpMinusQty_Click(object sender, RoutedEventArgs e)
        {
            if (txtbPopUpQtyCount.Text != "1")
            {
                txtbPopUpQtyCount.Text = (Convert.ToInt32(txtbPopUpQtyCount.Text) - 1).ToString();
                txtbPopUpItemTotal.Text = (Convert.ToInt32(txtbPopUpItemTotal.Text) - (Convert.ToInt64(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
                txtbPopUpItemSubTotalAmount.Text = (Convert.ToInt32(txtbPopUpItemSubTotalAmount.Text) - (Convert.ToInt64(txtbPopUpItemOriginalTotal.Text) * 1)).ToString();
            }
        }

        private void btnPopUpAddToCart_Click(object sender, RoutedEventArgs e)
        {
            List<SaleItemModel> saleItem = new List<SaleItemModel>();
            object foodItem = dgSaleItem.SelectedItem;
            saleItem = (List<SaleItemModel>)foodItem;
            saleItem[0].Qty = Convert.ToInt32(txtbPopUpQtyCount.Text);
            saleItem[0].Total = Convert.ToInt64(txtbPopUpItemTotal.Text);
            if (txtbPopUpQtyCount.Text != "1")
            {
                txtbTotalItemCount.Text = ((Convert.ToInt32(txtbPopUpQtyCount.Text) - Convert.ToInt32(txtbPopUpOriginalQtyCount.Text)) + Convert.ToInt32(txtbTotalItemCount.Text)).ToString();
                txtbSubTotalAmount.Text = (Convert.ToInt64(txtbSubTotalAmount.Text) + (Convert.ToInt64(saleItem[0].Price) * (Convert.ToInt32(txtbPopUpQtyCount.Text) - Convert.ToInt32(txtbPopUpOriginalQtyCount.Text)))).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToInt64(txtbTotalPayableAmount.Text) + (Convert.ToInt64(saleItem[0].Price) * (Convert.ToInt32(txtbPopUpQtyCount.Text) - Convert.ToInt32(txtbPopUpOriginalQtyCount.Text)))).ToString();
            }
            else if (txtbPopUpQtyCount.Text == "1" && txtbPopUpQtyCount.Text != txtbPopUpOriginalQtyCount.Text)
            {
                txtbTotalItemCount.Text = ((Convert.ToInt32(txtbPopUpQtyCount.Text) - Convert.ToInt32(txtbPopUpOriginalQtyCount.Text)) + Convert.ToInt32(txtbTotalItemCount.Text)).ToString();
                txtbSubTotalAmount.Text = (Convert.ToInt64(txtbSubTotalAmount.Text) + (Convert.ToInt64(saleItem[0].Price) * (Convert.ToInt32(txtbPopUpQtyCount.Text) - Convert.ToInt32(txtbPopUpOriginalQtyCount.Text)))).ToString();
                txtbTotalPayableAmount.Text = (Convert.ToInt64(txtbTotalPayableAmount.Text) + (Convert.ToInt64(saleItem[0].Price) * (Convert.ToInt32(txtbPopUpQtyCount.Text) - Convert.ToInt32(txtbPopUpOriginalQtyCount.Text)))).ToString();
            }
            EditSaleItemPopUp.IsOpen = false;
            dgSaleItem.Items.Refresh();
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            int insertedId = 0;
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();
            CustomerOrderModel customerOrderModel = new CustomerOrderModel();
            CustomerOrderItemModel customerOrderItemModel = new CustomerOrderItemModel();
            List<CustomerOrderItemModel> customerOrderItemModels = new List<CustomerOrderItemModel>();

            var saleItems = dgSaleItem.Items.OfType<List<SaleItemModel>>().ToList();
            foreach (var saleItem in saleItems)
            {
                customerOrderItemModel = new CustomerOrderItemModel();
                customerOrderItemModel.FoodMenuId = Convert.ToInt32(saleItem[0].FoodMenuId);
                customerOrderItemModel.FoodMenuRate = Convert.ToDecimal(saleItem[0].Price);
                customerOrderItemModel.FoodMenuQty = saleItem[0].Qty;
                customerOrderItemModel.Price = Convert.ToDecimal(saleItem[0].Total);
                customerOrderItemModel.Discount = Convert.ToDecimal(saleItem[0].Discount);
                customerOrderItemModels.Add(customerOrderItemModel);
            }
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

            insertedId = customerOrderViewModel.AddCustomerOrder(customerOrderModel, customerOrderItemModels);

            if (insertedId > 0)
            {
                MessageBox.Show("Order Placed Successfully.");
                ClearCustomerOrderItemControll();
            }
            else
            {
                MessageBox.Show("Order Placed Failed.");
            }

        }

        private void ClearCustomerOrderItemControll()
        {
            dgSaleItem.Items.Clear();
            txtbTotalPayableAmount.Text = "0";
            txtbSubTotalAmount.Text = "0";
            txtbTotalItemCount.Text = "0";
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
    }
}
