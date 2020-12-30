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
using RocketPOS.Model;
using RocketPOS.ViewModels;
namespace RocketPOS.Helpers.Reports
{
    /// <summary>
    /// Interaction logic for OrderList.xaml
    /// </summary>
    public partial class CustomerOrderHistoryList : Window
    {
        List<CustomerOrderHistoryModel> customerOrderHistoryModel = new List<CustomerOrderHistoryModel>();
        public CustomerOrderHistoryList()
        {
            CustomerOrderViewModel customerOrderViewModel = new CustomerOrderViewModel();

            InitializeComponent();

            DateTime dt1 = new DateTime(2020,12,12);
            DateTime dt2 = new DateTime(2021, 12, 31);

            customerOrderHistoryModel =  customerOrderViewModel.GetCustomerOrderHistoryList(dt1,dt2);

            this.dgOrderList.ItemsSource = customerOrderHistoryModel;
        }

        private void dgOrderList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
