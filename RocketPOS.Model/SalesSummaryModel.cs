using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class SalesSummaryModel
    {
        public string FoodMenuCategoryName { get; set; }
        public decimal TotalQty { get; set; }
        public decimal NetSalesAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal ValuePercentage { get; set; }
    }

    public class SalesSummaryByFoodCategoryFoodMenuModel
    {
        public string FoodMenuCategoryName { get; set; }
        public string FoodMenuName { get; set; }
        public decimal TotalQty { get; set; }
        public decimal NetSalesAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal ValuePercentage { get; set; }
    }

    public class SalesSummaryBySectionModel
    {
        public string SectionName { get; set; }
        public string Orderdate { get; set; }
        public decimal TotalInvoice { get; set; }
        public decimal NetSalesAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalGrossAmount { get; set; }
    }

    public class SalesSummaryByWeek
    {
        public string WeekStartDate { get; set; }
        public decimal TotalInvoice { get; set; }
        public decimal NetSalesAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalGrossAmount { get; set; }
    }
}
