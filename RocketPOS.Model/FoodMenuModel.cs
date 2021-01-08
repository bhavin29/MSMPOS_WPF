using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class FoodMenuModel
    {
        public List<FoodList> FoodList { get; set; }
    }
    public class FoodList
    {
        public int Id { get; set; }
        public string FoodCategory { get; set; }
        public int IsFavourite { get; set; }
        public List<SubCategory> SubCategory { get; set; }
    }
    public class SubCategory
    {
        public int FoodMenuId { get; set; }
        public double SalesPrice { get; set; }
        public string SmallThumb { get; set; }
        public string SmallName { get; set; }
        public int FoodCategoryId { get; set; }
        public decimal FoodVat { get; set; }
        public decimal Foodcess { get; set; }
        public decimal TaxPercentage { get; set; }
        public int IsVatable { get; set; }
    }

    public class FoodMenu
    {
        public int Id { get; set; }
        public int FoodMenuId { get; set; }
        public string FoodCategory { get; set; }
        public int IsFavourite { get; set; }
        public int FoodCategoryId { get; set; }
        public string SmallName { get; set; }
        public double SalesPrice { get; set; }
        public string SmallThumb { get; set; }
        public decimal FoodVat { get; set; }
        public decimal Foodcess { get; set; }
        public decimal TaxPercentage { get; set; }
        public int IsVatable { get; set; }
    }
}
