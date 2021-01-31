using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class DetailedDailyReportModel
    {
        public string RegisterTitle { get; set; }
        public string RegisterValue { get; set; }
    }

    public class ProductWiseSalesReportModel
    {
        public string FoodMenuCategoryName { get; set; }
        public string Id { get; set; }
        public string RowNumber { get; set; }
        public string FoodMenuName { get; set; }
        public string SalesPrice { get; set; }
        public string FoodMenuQty { get; set; }
        public string Total { get; set; }
    }

}
