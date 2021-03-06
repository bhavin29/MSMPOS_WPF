﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class ClientModel
    {
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Logo { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public int CurrencyId { get; set; }
        public string TimeZone { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Footer1 { get; set; }
        public string Footer2 { get; set; }
        public string Footer3 { get; set; }
        public string Footer4 { get; set; }

    }
    public class ClientSettingModel
    {
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Footer1 { get; set; }
        public string Footer2 { get; set; }
        public string Footer3 { get; set; }
        public string Footer4 { get; set; }
        public string Website { get; set; }
        public string OrderPrefix { get; set; }
        public string ReceiptPrefix { get; set; }
        public string HeaderMarqueeText { get; set; }
        public string DeliveryList { get; set; }
        public string Powerby { get; set; }
        public bool IsItemOverright { get; set; }
        public string LinkedServer { get; set; }
        public string WebAppUrl { get; set; }
        public string CurrentOutletId { get; set; }
        public string InvoiceTerms { get; set; }

    }
}
