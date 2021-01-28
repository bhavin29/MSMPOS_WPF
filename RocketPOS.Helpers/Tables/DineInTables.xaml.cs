using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Helpers.RMessageBox;
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

namespace RocketPOS.Helpers.Tables
{
    /// <summary>
    /// Interaction logic for DineInTables.xaml
    /// </summary>
    public partial class DineInTables : Window
    {
        List<TableModel> tablesList = new List<TableModel>();
        public DineInTables()
        {
            try
            {
                InitializeComponent();
                CenterWindowOnScreen();
                GetDineInTableList();
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void GetDineInTableList()
        {
            try
            {
                wpTableView.Children.Clear();
                AppSettings appSettings = new AppSettings();
                string rootPath = string.Empty;
                rootPath = appSettings.GetAppPath();
                TableViewModel tableViewModel = new TableViewModel();
                tablesList = tableViewModel.GetTables(LoginDetail.OutletId);

                if (tablesList.Count > 0)
                {

                    foreach (var table in tablesList)
                    {
                        Image imgTableStatus = new Image();
                        SolidColorBrush solidColorBrush = new SolidColorBrush();

                        if (Convert.ToInt32(table.Status) == (int)EnumUtility.TableStatus.Clean)
                        {
                            imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Clean.jpg"));
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#cf2e0e"));
                        }
                        else if (Convert.ToInt32(table.Status) == (int)EnumUtility.TableStatus.Occupied)
                        {
                            imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Occupied.jpg"));
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#dce309"));
                        }
                        else
                        {
                            imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Pending.jpg"));
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#32a836"));
                        }

                        StackPanel tableListPanel = new StackPanel();
                        tableListPanel.Orientation = Orientation.Vertical;
                        tableListPanel.VerticalAlignment = VerticalAlignment.Top;
                        tableListPanel.Width = 140;
                        tableListPanel.Height = 165;
                        tableListPanel.Background = solidColorBrush;
                        tableListPanel.Margin = new Thickness(1);

                        //Table Id
                        TextBlock txtbTableId = new TextBlock();
                        txtbTableId.Text = table.Id.ToString();
                        txtbTableId.Name = "txtbTableId_" + table.Id;
                        txtbTableId.FontSize = 5;
                        txtbTableId.Visibility = Visibility.Hidden;
                        tableListPanel.Children.Add(txtbTableId);

                        //Table Name
                        TextBlock txtbTableName = new TextBlock();
                        txtbTableName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000"));
                        txtbTableName.Text = "#" + table.TableName;
                        txtbTableName.FontSize = 16;
                        txtbTableName.FontWeight = FontWeights.Bold;
                        txtbTableName.Name = "txtbTableName" + table.Id;
                        txtbTableName.Margin = new Thickness(2);
                        tableListPanel.Children.Add(txtbTableName);

                        //Table Image
                        imgTableStatus.Width = 75;
                        imgTableStatus.Height = 75;
                        imgTableStatus.Margin = new Thickness(2);
                        imgTableStatus.Stretch = Stretch.UniformToFill;
                        imgTableStatus.Name = "imgTableStatus" + table.Status;
                        tableListPanel.Children.Add(imgTableStatus);

                        //Table Status
                        TextBlock txtbTableStatus = new TextBlock();
                        txtbTableStatus.Text = table.StatusDescription;
                        txtbTableStatus.Name = "txtbTableStatus" + table.Status;
                        txtbTableStatus.FontSize = 18;
                        txtbTableStatus.FontWeight = FontWeights.Bold;
                        txtbTableStatus.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000"));
                        txtbTableStatus.Margin = new Thickness(2);
                        tableListPanel.Children.Add(txtbTableStatus);

                        //Table Capacity
                        TextBlock txtbTableCapacity = new TextBlock();
                        txtbTableCapacity.Text = "Capacity : " + table.AllocatedPerson.ToString() + "/" + table.PersonCapacity.ToString();
                        txtbTableCapacity.Name = "txtbTableCapacity" + table.Id;
                        txtbTableCapacity.FontSize = 16;
                        txtbTableCapacity.FontWeight = FontWeights.Bold;
                        txtbTableCapacity.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000"));
                        txtbTableCapacity.Margin = new Thickness(2);
                        tableListPanel.Children.Add(txtbTableCapacity);

                        //Table Status Id
                        TextBlock txtbTableStatusId = new TextBlock();
                        txtbTableStatusId.Text = table.Status;
                        txtbTableStatusId.Visibility = Visibility.Hidden;
                        tableListPanel.Children.Add(txtbTableStatusId);

                        //Table Order Id
                        TextBlock txtbTableOrderId = new TextBlock();
                        txtbTableOrderId.Text = table.OrderId.ToString();
                        txtbTableOrderId.Visibility = Visibility.Hidden;
                        tableListPanel.Children.Add(txtbTableOrderId);

                        tableListPanel.Name = "tableListPanel" + table.Id.ToString();
                        tableListPanel.MouseDown += PerformTableOperation;
                        //Add to panel
                        wpTableView.Children.Add(tableListPanel);
                    }
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
            }
        }
        private void PerformTableOperation(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TableViewModel tableViewModel = new TableViewModel();
                var tableListPanel = sender as StackPanel;
                var txtbTableId = tableListPanel.Children[0] as TextBlock;
                var txtbTableName = tableListPanel.Children[1] as TextBlock;
                var txtbTableStatusId = tableListPanel.Children[3] as TextBlock;
                var txtbTableOrderId = tableListPanel.Children[6] as TextBlock;

                if ((txtbTableStatusId.Text) == EnumUtility.TableStatus.Clean.ToString())
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInTitle, StatusMessages.DineInTableIsClean, MessageBoxButton.YesNo, EnumUtility.MessageBoxImage.Question);
                    if (messageBoxResult.ToString() == "Yes")
                    {
                        tableViewModel.UpdateTableStatus(txtbTableId.Text, (int)EnumUtility.TableStatus.Open);
                        GetDineInTableList();
                    }
                }

                if ((txtbTableStatusId.Text) == EnumUtility.TableStatus.Open.ToString())
                {
                    DineTable.TableId = Convert.ToInt32(txtbTableId.Text);
                    ((MainWindow)this.Owner).OrderCall(Convert.ToInt32(txtbTableId.Text), (int)EnumUtility.TableStatus.Open, Convert.ToInt32(txtbTableId.Text));
                    this.Close();

                }

                if ((txtbTableStatusId.Text) == EnumUtility.TableStatus.Occupied.ToString())
                {
                    if (!string.IsNullOrEmpty(txtbTableOrderId.Text))
                    {
                        DineTable.OrderId = Convert.ToInt32(txtbTableOrderId.Text);
                        DineTable.TableId = Convert.ToInt32(txtbTableId.Text);

                        if (DineTable.OrderId == 0)
                        {
                            //Froce Update Table Status
                            tableViewModel.UpdateTableStatus(txtbTableId.Text, (int)EnumUtility.TableStatus.Clean);
                        }
                        else
                        {
                            ((MainWindow)this.Owner).OrderCall(Convert.ToInt32(txtbTableOrderId.Text), (int)EnumUtility.TableStatus.Occupied, Convert.ToInt32(txtbTableId.Text));
                        }
                        this.Close();
                    }
                }
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
