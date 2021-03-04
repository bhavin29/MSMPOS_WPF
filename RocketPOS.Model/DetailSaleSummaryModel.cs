using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class DetailSaleSummaryModel
    {
        public string OrderDate { get; set; }
        public string FoodMenuName { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalNetAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalTaxPercentage { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalBillGrossAmount { get; set; }
        public decimal CashPayment { get; set; }
        public decimal CardPayment { get; set; }
    }
}
