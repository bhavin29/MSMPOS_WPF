using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class CustomerBillModel
    {
        public int OutletId { get; set; }
        public int CustomerOrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal VatableAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int BillStatus { get; set; }
        public int OutletRegisterId { get; set; }
        public int UserId { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentNumber { get; set; }
    }
}
