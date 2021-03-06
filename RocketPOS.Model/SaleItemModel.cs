﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class SaleItemModel
    {
        public int RowId { get; set; }
        public int CustomerOrderItemId { get; set; }
        public string FoodMenuId { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal FoodVat { get; set; }
        public decimal Foodcess { get; set; }
        public decimal TaxPercentage { get; set; }
        public int IsVatable { get; set; }
    }
}
