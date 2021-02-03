using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class CessReportModel
    {
        public List<CessSummaryModel> CessSummaryList { get; set; }

        public List<CessDetailModel> CessDetailList { get; set; }
    }

    public class CessCategoryReportModel
    {
        public List<CessCategorySummaryModel> CessSummaryList { get; set; }

        public List<CessDetailModel> CessDetailList { get; set; }
    }
    public class CessDetailModel
    {
        public string BillDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal NetSales { get; set; }
        public decimal Vatable { get; set; }
        public decimal NonVatable { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public Decimal CateringLevy { get; set; }
    }

    public class CessSummaryModel
    {
        public string BillDate { get; set; }
        public decimal NetSales { get; set; }
        public decimal Vatable { get; set; }
        public decimal NonVatable { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public Decimal CateringLevy { get; set; }
    }
    public class CessCategorySummaryModel
    {
        public string BillDate { get; set; }
        public string FoodmenucategoryName { get; set; }
        public decimal NetSales { get; set; }
        public decimal Vatable { get; set; }
        public decimal NonVatable { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public Decimal CateringLevy { get; set; }
    }
}
