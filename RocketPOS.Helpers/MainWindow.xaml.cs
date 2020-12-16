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
            JObject foodJson = new JObject();
            if (Application.Current.Resources["FoodList"] == null)
            {
                foodJson = JObject.Parse(File.ReadAllText(@"E:\Project\My Docs\FoodJson.json"));
                Application.Current.Resources["FoodList"] = foodJson;
            }
            else
            {
                foodJson = (JObject)Application.Current.Resources["FoodList"];
            }

            //read JSON directly from a file
            FoodMenu foodMenu = JsonConvert.DeserializeObject<FoodMenu>(foodJson.ToString());
            
            foreach (var foodCategory in foodMenu.FoodList)
            {
                Button btnCategory = new Button();
                btnCategory.Content = foodCategory.Category;
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
                    imgFood.Source = new BitmapImage(new System.Uri(@"E:\Visual Studio Exercise\WPF Practices\Images\" + itemSubCat.SmallThumb));
                    imgFood.MaxWidth = 150;
                    imgFood.MaxHeight = 100;
                    imgFood.Margin = new Thickness(5, 0, 5, 10);
                    imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
                    menuListPanel.Children.Add(imgFood);

                    TextBlock txtSmallName = new TextBlock();
                    txtSmallName.Text = itemSubCat.SmallName;
                    txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
                    menuListPanel.Children.Add(txtSmallName);

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
            var smallName = menuListPanel.Children[2] as TextBlock;
            //MessageBox.Show(salePrice.Text + " - " + smallName.Text);

            List<SaleItem> saleItems = new List<SaleItem>();
            saleItems.Add(new SaleItem()
            {
                Product = smallName.Text,
                Price = Convert.ToUInt64(salePrice.Text),
                Qty = 1,
                Discount = 0,
                Total = Convert.ToUInt64(salePrice.Text) * 1
            });
            dgSaleItem.Items.Add(saleItems);
            //dgSaleItem.ItemsSource = saleItems;
        }

        private void GetSubCategory(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var categoryId = btn.Name.Substring(3);//Get the button id
            GetFoodItems(categoryId);
        }

        private void GetFoodItems(string type)
        {
            if (type == "All")
            {
                spSubCategory.Children.Clear();
                FoodMenu FoodMenu = JsonConvert.DeserializeObject<FoodMenu>(Application.Current.Resources["FoodList"].ToString());

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
                        imgFood.Source = new BitmapImage(new System.Uri(@"E:\Visual Studio Exercise\WPF Practices\Images\" + itemSubCat.SmallThumb));
                        imgFood.MaxWidth = 150;
                        imgFood.MaxHeight = 100;
                        imgFood.Margin = new Thickness(5, 0, 5, 10);
                        imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
                        menuListPanel.Children.Add(imgFood);

                        TextBlock txtSmallName = new TextBlock();
                        txtSmallName.Text = itemSubCat.SmallName;
                        txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
                        menuListPanel.Children.Add(txtSmallName);

                        menuListPanel.Name = "childPanel" + itemSubCat.FoodCategoryId;
                        menuListPanel.MouseDown += GetPrice_MouseDown;
                        spSubCategory.Children.Add(menuListPanel);
                    }
                }
            }
            else
            {
                spSubCategory.Children.Clear();
                FoodMenu FoodMenu = JsonConvert.DeserializeObject<FoodMenu>(Application.Current.Resources["FoodList"].ToString());

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
                            imgFood.Source = new BitmapImage(new System.Uri(@"E:\Visual Studio Exercise\WPF Practices\Images\" + itemSubCat.SmallThumb));
                            imgFood.MaxWidth = 150;
                            imgFood.MaxHeight = 100;
                            imgFood.Margin = new Thickness(5, 0, 5, 10);
                            imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
                            menuListPanel.Children.Add(imgFood);

                            TextBlock txtSmallName = new TextBlock();
                            txtSmallName.Text = itemSubCat.SmallName;
                            txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
                            menuListPanel.Children.Add(txtSmallName);

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

            spSubCategory.Children.Clear();
            FoodMenu FoodMenu = JsonConvert.DeserializeObject<FoodMenu>(Application.Current.Resources["FoodList"].ToString());

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
                        imgFood.Source = new BitmapImage(new System.Uri(@"E:\Visual Studio Exercise\WPF Practices\Images\" + itemSubCat.SmallThumb));
                        imgFood.MaxWidth = 150;
                        imgFood.MaxHeight = 100;
                        imgFood.Margin = new Thickness(5, 0, 5, 10);
                        imgFood.Name = "imgFood" + itemSubCat.FoodCategoryId;
                        menuListPanel.Children.Add(imgFood);

                        TextBlock txtSmallName = new TextBlock();
                        txtSmallName.Text = itemSubCat.SmallName;
                        txtSmallName.Name = "txtSmallName" + itemSubCat.FoodCategoryId;
                        menuListPanel.Children.Add(txtSmallName);

                        menuListPanel.Name = "childPanel" + itemSubCat.FoodCategoryId;
                        menuListPanel.MouseDown += GetPrice_MouseDown;
                        spSubCategory.Children.Add(menuListPanel);
                    }
                }
            }
        }
    }
}
