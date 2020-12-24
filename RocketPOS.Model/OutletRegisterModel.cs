using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class OutletRegisterModel
    {
        public int id { get; set; }
        public int OutletId { get; set; }
        public int USerID { get; set; }
        public DateTime OpenDate { get; set; }
        public float OpeningBalance { get; set; }

        public DateTime CloseDate { get; set; }

    }
}
