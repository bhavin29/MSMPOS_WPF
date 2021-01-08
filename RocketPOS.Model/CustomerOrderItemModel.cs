using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class CustomerOrderItemModel
    {
        public int CustomerOrderItemId { get; set; }
        public int FoodMenuId { get; set; }
        public string FoodMenuName { get; set; }
        public decimal FoodMenuRate { get; set; }
        public decimal FoodMenuQty { get; set; }
        public int AddonsId { get; set; }
        public decimal AddonsQty { get; set; }
        public int VarientId { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal FoodVat { get; set; }
        public decimal Foodcess { get; set; }
        public decimal TaxPercentage { get; set; }
        public int IsVatable { get; set; }
    }
}
