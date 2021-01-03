using RocketPOS.Core.Constants;
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
            TableViewModel tableViewModel = new TableViewModel();
            tablesList = tableViewModel.GetTables(LoginDetail.OutletId);
            lbDineInTables.ItemsSource = tablesList;
        }
    }
}
