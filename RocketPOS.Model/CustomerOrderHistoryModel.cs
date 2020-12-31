using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class CustomerOrderHistoryModel
    {
        public string SalesInvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalPayable { get; set; }
        public string OrderStatus { get; set; }
    }
}
