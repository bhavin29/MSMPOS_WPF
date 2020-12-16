using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class SaleItem
    {
        public string Product { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public int Discount { get; set; }
        public double Total { get; set; }
    }
}
