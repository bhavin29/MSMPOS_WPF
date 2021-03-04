using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class SalesByCategoryProductModel
    {
        public string SectionName { get; set; }
        public string FoodMenuCategoryName { get; set; }
        public string FoodMenuName { get; set; }
        public decimal TotalUnitPrice { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalGrossAmount { get; set; }
    }
}
