using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class SaleItemModel
    {
        public string FoodMenuId { get; set; }
        public string Product { get; set; }
        public double Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Discount { get; set; }
        public double Total { get; set; }
    }
}
