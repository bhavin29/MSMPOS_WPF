using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class CustomerOrderItemModel
    {
        public long CustomerOrderId { get; set; }
        public int FoodMenuId { get; set; }
        public decimal FoodMenuRate { get; set; }
        public decimal FoodMenuQty { get; set; }
        public int AddonsId { get; set; }
        public decimal AddonsQty { get; set; }
        public int VarientId { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
    }
}
