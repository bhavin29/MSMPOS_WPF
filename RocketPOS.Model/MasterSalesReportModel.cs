using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class MasterSalesReportModel
    {
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string SalesInvoiceNumber { get; set; }
        public string FoodMenuName { get; set; }
        public decimal FoodMenuRate { get; set; }
        public decimal FoodMenuQty { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal GrossAmount { get; set; }
        public string FoodMenuCategoryName { get; set; }
    }
}
