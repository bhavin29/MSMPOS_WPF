using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class ModeofPaymentReportModel
    {
        public string BillDate { get; set; }
        public string PaymentMethodName { get; set; }
        public decimal Sales { get; set; }
        public decimal BillAmount { get; set; }
    }
}
