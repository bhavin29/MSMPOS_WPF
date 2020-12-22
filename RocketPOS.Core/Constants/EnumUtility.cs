using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Core.Constants
{
    public class EnumUtility
    {
        public enum OrderType
        {
            DineIN = 1,
            TakeAway = 2,
            Delivery = 3
        }

        public enum OrderPaidStatus
        {
            Pending = 1,
            Hold = 2,
            PartialPaid = 3,
            FullPaid = 4
        }

        public enum KOTStatus
        {
            Pending = 1,
            Cooking = 2,
            Completed = 3
        }
    }
}
