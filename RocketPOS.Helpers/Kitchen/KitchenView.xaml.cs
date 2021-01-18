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
using System.Windows.Shapes;

namespace RocketPOS.Helpers.Kitchen
{
    /// <summary>
    /// Interaction logic for KitchenView.xaml
    /// </summary>
    public partial class KitchenView : Window
    {
        public KitchenView()
        {
            InitializeComponent();
            GetKitchenPending();

        }

        private void GetKitchenPending()
        {
            KitchenViewModel kitchenViewModel = new KitchenViewModel();
            KitchenModel kitchenModel = new KitchenModel();
            kitchenModel = kitchenViewModel.GetKitchenStaus();

            if (kitchenModel.kotStatusList.Count > 0)
            {

                foreach (var kot in kitchenModel.kotStatusList)
                {
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
                    txtbOrderId.Text = "Order# " + kot.OrderId.ToString();
                    txtbOrderId.Name = "txtbOrderId_" + kot.OrderId;
                    txtbOrderId.FontSize = 10;
                    txtbOrderId.Width = 70;
                    txtbOrderId.HorizontalAlignment = HorizontalAlignment.Left;
                    kotHeaderPanel.Children.Add(txtbOrderId);

                    //KOTNumber
                    TextBlock txtbKOTNumber = new TextBlock();
                    txtbKOTNumber.Text = "KOT# " + kot.KOTNumber;
                    txtbKOTNumber.Name = "txtbKOTNumber" + kot.KOTNumber;
                    txtbKOTNumber.FontSize = 10;
                    txtbKOTNumber.Width = 80;
                    txtbKOTNumber.HorizontalAlignment = HorizontalAlignment.Right;
                    kotHeaderPanel.Children.Add(txtbKOTNumber);

                    //Table Name
                    TextBlock txtbTableName = new TextBlock();
                    txtbTableName.Text = "Table : " + kot.TableName;
                    txtbTableName.Name = "txtbTableName_" + kot.TableId;
                    txtbTableName.FontSize = 10;
                    txtbTableName.Width = 80;
                    kotHeaderPanel.Children.Add(txtbTableName);

                    if (kot.kOTItems.Count > 0)
                    {
                        foreach (var kotItem in kot.kOTItems)
                        {
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#32a836"));

                            WrapPanel kotDetailPanel = new WrapPanel();
                            kotDetailPanel.Orientation = Orientation.Horizontal;
                            kotDetailPanel.VerticalAlignment = VerticalAlignment.Top;
                            kotDetailPanel.Width = 150;
                            kotDetailPanel.Background = solidColorBrush;
                            kotDetailPanel.Margin = new Thickness(5);


                            //Food Menu Name
                            TextBlock txtbFoodMenuName = new TextBlock();
                            txtbFoodMenuName.Text = kotItem.FoodMenuName;
                            txtbFoodMenuName.Name = "txtbFoodMenuName_" + kotItem.KOTItemId;
                            txtbFoodMenuName.FontSize = 10;
                            txtbFoodMenuName.Width = 70;
                            txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Left;
                            kotDetailPanel.Children.Add(txtbFoodMenuName);

                            //KOT Item Id
                            TextBlock txtbKOTItemId = new TextBlock();
                            txtbKOTItemId.Text = kotItem.KOTItemId.ToString();
                            txtbKOTItemId.Name = "txtbKOTItemId_" + kotItem.KOTItemId;
                            txtbKOTItemId.Visibility = Visibility.Hidden;
                            kotDetailPanel.Children.Add(txtbKOTItemId);

                            //KOT Status
                            TextBlock txtbKOTStatus = new TextBlock();
                            txtbKOTStatus.Text = kotItem.KOTStatus.ToString();
                            txtbKOTStatus.Name = "txtbKOTStatus_" + kotItem.KOTItemId;
                            txtbKOTStatus.FontSize = 10;
                            txtbKOTStatus.Width = 50;
                            txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Right;
                            kotDetailPanel.Children.Add(txtbKOTStatus);

                            //Food Menu Qty
                            TextBlock txtbFoodMenuQty = new TextBlock();
                            txtbFoodMenuQty.Text = "Qty : "+kotItem.FoodMenuQty.ToString();
                            txtbFoodMenuQty.Name = "txtbFoodMenuQty_" + kotItem.KOTItemId;
                            txtbFoodMenuQty.FontSize = 10;
                            txtbFoodMenuQty.Width = 60;
                            txtbFoodMenuName.HorizontalAlignment = HorizontalAlignment.Left;
                            kotDetailPanel.Children.Add(txtbFoodMenuQty);

                            kotHeaderPanel.Children.Add(kotDetailPanel);
                        }
                    }
                    wpKitchenView.Children.Add(kotHeaderPanel);
                }
            }
        }
    }
}
