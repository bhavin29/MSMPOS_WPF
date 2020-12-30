using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class CustomerOrderModel : CustomerOrderItemModel
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public string SalesInvoiceNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int WaiterEmployeeId { get; set; }
        public string WaiterName { get; set; }
        public int OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public string TableId { get; set; }
        public string TockenNumber { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal CustomerPaid { get; set; }
        public string CustomerNote { get; set; }
        public int OrderStatus { get; set; }
        public string AnyReason { get; set; }
        public int UserIdInserted { get; set; }
        public DateTime DateInserted { get; set; }

        public List<CustomerOrderItemModel> CustomerOrderItemModels = new List<CustomerOrderItemModel>();
    }

    public class OrderDetailModel  : CustomerOrderModel 
    {

    }
}
