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
        public List<SubCategory> SubCategory { get; set; }
    }
    public class SubCategory
    {
        public double SalesPrice { get; set; }
        public string SmallThumb { get; set; }
        public string SmallName { get; set; }
        public int FoodCategoryId { get; set; }
    }

    public class FoodMenu
    {
        public int Id { get; set; }
        public string FoodCategory { get; set; }
        public int FoodCategoryId { get; set; }
        public string SmallName { get; set; }
        public double SalesPrice { get; set; }
        public string SmallThumb { get; set; }
    }

}
