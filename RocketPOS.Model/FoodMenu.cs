using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class FoodMenu
    {
        public List<FoodList> FoodList { get; set; }
    }
    public class FoodList
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public List<SubCategory> SubCategory { get; set; }
    }
    public class SubCategory
    {
        public double SalesPrice { get; set; }
        public string SmallThumb { get; set; }
        public string SmallName { get; set; }
        public int FoodCategoryId { get; set; }
    }
}
