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
                throw;
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
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7D001A"));
                        }
                        else if (Convert.ToInt32(table.Status) == (int)EnumUtility.TableStatus.Occupied)
                        {
                            imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Occupied.jpg"));
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00265B"));
                        }
                        else
                        {
                            imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Pending.jpg"));
                            solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D9BA41"));
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
                        txtbTableName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        txtbTableName.Text = "#" + table.TableName;
                        txtbTableName.FontSize = 16;
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
                        txtbTableStatus.Text = "Status : " + table.StatusDescription;
                        txtbTableStatus.Name = "txtbTableStatus" + table.Id;
                        txtbTableStatus.FontSize = 16;
                        txtbTableStatus.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        txtbTableStatus.Margin = new Thickness(2);
                        tableListPanel.Children.Add(txtbTableStatus);

                        //Table Capacity
                        TextBlock txtbTableCapacity = new TextBlock();
                        txtbTableCapacity.Text = "Capacity : " + table.PersonCapacity.ToString();
                        txtbTableCapacity.Name = "txtbTableCapacity" + table.Id;
                        txtbTableCapacity.FontSize = 16;
                        txtbTableCapacity.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                        txtbTableCapacity.Margin = new Thickness(2);
                        tableListPanel.Children.Add(txtbTableCapacity);

                        //Table Status Id
                        TextBlock txtbTableStatusId = new TextBlock();
                        txtbTableStatusId.Text = table.Status;
                        txtbTableStatusId.Visibility = Visibility.Hidden;
                        tableListPanel.Children.Add(txtbTableStatusId);

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
                throw;
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
                var txtbTableStatusId = tableListPanel.Children[5] as TextBlock;

                if (Convert.ToInt32(txtbTableStatusId.Text) == (int)EnumUtility.TableStatus.Clean)
                {
                    var messageBoxResult = WpfMessageBox.Show(StatusMessages.DineInTitle, StatusMessages.DineInTableIsClean, MessageBoxButton.YesNo, EnumUtility.MessageBoxImage.Question);
                    if (messageBoxResult.ToString() == "Yes")
                    {
                        tableViewModel.UpdateTableStatus(txtbTableId.Text, (int)EnumUtility.TableStatus.Open);
                        GetDineInTableList();
                    }
                }

                if (Convert.ToInt32(txtbTableStatusId.Text) == (int)EnumUtility.TableStatus.Open)
                {
                    MainWindow mainWin = new MainWindow();
                    mainWin.rdbDineInOrderType.IsChecked = true;
                    mainWin.txtbDineInTableId.Text = txtbTableId.Text.ToString();
                    tableViewModel.UpdateTableStatus(txtbTableId.Text, (int)EnumUtility.TableStatus.Occupied);
                    mainWin.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                SystemError.Register(ex);
                throw;
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
                throw;
            }
        }

    }
}
