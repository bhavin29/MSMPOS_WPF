using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class KOTCustomerOrderDetail
    {
        public int Id { get; set; }
        public string CustomerOrderNo { get; set; }
        public string OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string WaiterName { get; set; }
        public string TableName { get; set; }
        public string OrderType { get; set; }
    }

    public class KOTHeaderDetail
    {
        public int Id { get; set; }
        public string KOTNumber { get; set; }
        public string KOTDateTime { get; set; }
        public string KOTStatus { get; set; }
    }

    public class KOTItemDetail
    {
        public int KOTItemId { get; set; }
        public string KOTNumber { get; set; }
        public DateTime KOTDateTime { get; set; }
        public string KOTItemStatus { get; set; }
        public string FoodMenuName { get; set; }
        public decimal FoodMenuQty { get; set; }
    }
}
