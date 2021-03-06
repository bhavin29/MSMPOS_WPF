﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class OutletRegisterModel
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public int USerID { get; set; }
        public DateTime OpenDate { get; set; }
        public float OpeningBalance { get; set; }

        public DateTime CloseDate { get; set; }

    }

    public class OutletUserRegister
    {
        public string RegisterTitle { get; set; }
        public string RegisterValue { get; set; }
    }
}
