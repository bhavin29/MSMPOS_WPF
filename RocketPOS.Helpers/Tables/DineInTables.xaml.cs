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
            InitializeComponent();
            GetDineInTableList();
        }

        private void GetDineInTableList()
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
                    StackPanel tableListPanel = new StackPanel();
                    tableListPanel.Orientation = Orientation.Vertical;
                    tableListPanel.Width = 250;
                    tableListPanel.Height = 250;

                    //Table Id
                    TextBlock txtbTableId = new TextBlock();
                    //txtbTableName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    txtbTableId.Text = table.Id.ToString();
                    txtbTableId.Name = "txtbTableId_" + table.Id;
                    txtbTableId.Visibility = Visibility.Hidden;
                    tableListPanel.Children.Add(txtbTableId);


                    //Table Name
                    TextBlock txtbTableName = new TextBlock();
                    //txtbTableName.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    txtbTableName.Text = "#" + table.TableName;
                    txtbTableName.Name = "txtbTableName" + table.Id;
                    tableListPanel.Children.Add(txtbTableName);

                    //Table Image
                    Image imgTableStatus = new Image();
                    if (Convert.ToInt32(table.Status) == (int)EnumUtility.TableStatus.Clean)
                    {
                        imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Clean.jpg"));
                    }
                    else if (Convert.ToInt32(table.Status) == (int)EnumUtility.TableStatus.Occupied)
                    {
                        imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Occupied.jpg"));
                    }
                    else
                    {
                        imgTableStatus.Source = new BitmapImage(new System.Uri(rootPath + @"\Images\Pending.jpg"));
                    }
                    imgTableStatus.Width = 150;
                    imgTableStatus.Height = 150;
                    imgTableStatus.Margin = new Thickness(5, 5, 5, 5);
                    imgTableStatus.Stretch = Stretch.UniformToFill;
                    imgTableStatus.Name = "imgTableStatus" + table.Status;
                    tableListPanel.Children.Add(imgTableStatus);

                    //Table Status Id
                    TextBlock txtbTableStatusId = new TextBlock();
                    //txtbTableStatus.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    txtbTableStatusId.Text = table.Status;
                    txtbTableStatusId.Visibility = Visibility.Hidden;
                    tableListPanel.Children.Add(txtbTableStatusId);


                    //Table Status
                    TextBlock txtbTableStatus = new TextBlock();
                    //txtbTableStatus.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    txtbTableStatus.Text = "Status : " + table.StatusDescription;
                    txtbTableStatus.Name = "txtbTableStatus" + table.Id;
                    tableListPanel.Children.Add(txtbTableStatus);

                    //Table Capacity
                    TextBlock txtbTableCapacity = new TextBlock();
                    //txtbTableCapacity.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                    txtbTableCapacity.Text = "Capacity : " + table.PersonCapacity.ToString();
                    txtbTableCapacity.Name = "txtbTableCapacity" + table.Id;
                    tableListPanel.Children.Add(txtbTableCapacity);

                    tableListPanel.Name = "tableListPanel" + table.Id.ToString();
                    tableListPanel.MouseDown += PerformTableOperation;
                    //Add to panel
                    wpTableView.Children.Add(tableListPanel);
                }
            }
        }
        private void PerformTableOperation(object sender, MouseButtonEventArgs e)
        {
            TableViewModel tableViewModel = new TableViewModel();
            var tableListPanel = sender as StackPanel;
            var txtbTableId = tableListPanel.Children[0] as TextBlock;
            var txtbTableName = tableListPanel.Children[1] as TextBlock;
            var txtbTableStatusId = tableListPanel.Children[3] as TextBlock;

            if (Convert.ToInt32(txtbTableStatusId.Text)==(int)EnumUtility.TableStatus.Clean)
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
    }
}
