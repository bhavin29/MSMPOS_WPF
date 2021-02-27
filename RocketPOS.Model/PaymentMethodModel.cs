using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class PaymentMethodModel
    {
        public int Id { get; set; }
        public string PaymentMethodName { get; set; }
        //public decimal Amount { get; set; }
    }

    public class PaymentMethodSettingModel
    {
        public int Id { get; set; }
        public string PaymentMethodName { get; set; }
        public string TallyLedgerName { get; set; }
        public string TallyLedgerNamePark { get; set; }
        public string TallyBillPostfix { get; set; }
    }
}
