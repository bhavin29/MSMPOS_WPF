using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class TallySetupModel
    {
        public string Keyname { get; set; }
        public string LedgerName { get; set; }
    }

    public class TallySalesVoucherModel
    {
        public string BillDate { get; set; }
        public string TallyLedgerName { get; set; }
        public string TallyLedgerNamePark { get; set; }

        public string TallyBillPostfix { get; set; }

        public decimal Sales { get; set; }
        public decimal ExemptedSales { get; set; }
        public decimal OutputVAT { get; set; }
        public decimal CashSales { get; set; }

    }

    public class TallySetupSettingModel
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string LedgerName { get; set; }
    }
}
