using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class KitchenModel
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int PersonCapacity { get; set; }
        public int AllocatedPerson { get; set; }
      
        public string Position { get; set; }
        public int CustomerOrderId { get; set; }
        public List<KOTStatusList> kotStatusList { get; set; }
    }
    public class KOTStatusList
    {
        public int CustomerOrderId { get; set; }

        public string CustomerOrderNo { get; set; }
        public string KOTNumber { get; set; }
        public int KOTId { get; set; }
        public int KOTItemId { get; set; }
        public string FoodMenuName { get; set; }
        public string FoodMenuQty { get; set; }
        public string KOTStatus { get; set; }
    }
    public class KitchenStatusDetail
    {
        public int CustomerOrderId { get; set; }
        public string CustomerOrderNo { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public string KOTNumber { get; set; }
        public int KOTId { get; set; }
        public int KOTItemId { get; set; }
        public string FoodMenuName { get; set; }
        public string FoodMenuQty { get; set; }
        public string KOTStatus { get; set; }
    }
}
