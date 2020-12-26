﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class LoginModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public string OutletName { get; set; }
        public int OutletId { get; set; }
        public int RoleTypeId { get; set; }
        public int OutletRegisterStatus { get; set; }
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
}
