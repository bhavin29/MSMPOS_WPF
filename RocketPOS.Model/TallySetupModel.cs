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
        public decimal Cash { get; set; }
        public decimal ExemptedSales { get; set; }
        public decimal OutputVAT { get; set; }
        public decimal CashSales { get; set; }

    }
}
