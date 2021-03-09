using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class PrintReceiptModel
    {
        public int CustomerOrderId { get; set; }
        public int BillId { get; set; }
        public DateTime BillDateTime { get; set; }
        public string OutletName { get; set; }
        public string SalesInvoiceNumber { get; set; }
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public float GrossAmount { get; set; }
        public float TaxAmount { get; set; }
        public float VatableAmount { get; set; }
        public float NonVatableAmount { get; set; }
        public float Discount { get; set; }
        public float ServiceCharge { get; set; }
        public float TotalAmount { get; set; }
        public string PaymentMethodName { get; set; }
        public float BillAmount { get; set; }
        public float RewardAmount { get; set; }


    }

    public class PrintReceiptItemModel
    {
        public string FoodMenuName { get; set; }

        public float FoodMenuQty { get; set; }
        public float FoodMenuRate { get; set; }
        public float Price { get; set; }
        public string FoodVat { get; set; }
    }
    public class PrintKOTModel
    {
        public int CustomerOrderId { get; set; }
        public int BillId { get; set; }
        public DateTime BillDateTime { get; set; }
        public string OutletName { get; set; }
        public string SalesInvoiceNumber { get; set; }
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public float GrossAmount { get; set; }
        public float TaxAmount { get; set; }
        public float VatableAmount { get; set; }
        public float NonVatableAmount { get; set; }
        public float Discount { get; set; }
        public float ServiceCharge { get; set; }
        public float TotalAmount { get; set; }
        public string PaymentMethodName { get; set; }
        public float BillAmount { get; set; }
        public float RewardAmount { get; set; }


    }
    public class PrintKOTItemModel
    {
        public int CustomerOrderId { get; set; }
        public int BillId { get; set; }
        public DateTime BillDateTime { get; set; }
        public string OutletName { get; set; }
        public string KOTNumber { get; set; }
        public string TableName { get; set; }
        public string OrderType { get; set; }
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public string FoodMenuName { get; set; }
        public DateTime KOTDateTime { get; set; }
        public float FoodMenuQty { get; set; }
        public float FoodMenuRate { get; set; }
        public float Price { get; set; }
        public string FoodVat { get; set; }
    }
}
