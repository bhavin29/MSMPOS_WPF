using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class KitchenModel
    {
        public List<KOTStatusList> kotStatusList { get; set; }
    }
    public class KOTStatusList
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public string KOTNumber { get; set; }
        public int KOTId { get; set; }
        public List<KOTItem> kOTItems { get; set; }
    }
    public class KOTItem
    {
        public int KOTItemId { get; set; }
        public string FoodMenuName { get; set; }
        public string FoodMenuQty { get; set; }
        public string KOTStatus { get; set; }
    }
    public class KitchenStatusDetail
    {
        public int OrderId { get; set; }
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
