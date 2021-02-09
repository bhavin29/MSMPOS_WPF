using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RocketPOS.Core.Constants
{
    public class EnumUtility
    {
        public enum OrderType
        {
            DineIN = 1,
            TakeAway = 2,
            Delivery = 3,
            All = 4
        }
        public enum OrderPaidStatus
        {
            Pending = 1,
            Hold = 2,
            PartialPaid = 3,
            FullPaid = 4,
            Cancelled = 5
        }
        public enum KOTStatus
        {
            [Display(Name = "Pending")]
            Pending = 1,
            [Display(Name = "Cooking")]
            Cooking = 2,
            [Display(Name = "Ready")]
            Ready = 3,
            [Display(Name = "Served")]
            Served = 4,
            [Display(Name = "Completed")]
            Completed = 5
        }
        public enum TableStatus
        {
            [Display(Name = "Open")]
            Open = 1,
            [Display(Name = "Occupied")]
            Occupied = 2,
            [Display(Name = "Clean")]
            Clean = 3,
        }

        public enum MessageBoxType
        {
            ConfirmationWithYesNo = 0,
            ConfirmationWithYesNoCancel,
            Information,
            Error,
            Warning
        }
        public enum MessageBoxImage
        {
            Warning = 0,
            Question,
            Information,
            Error,
            None
        }
        public enum RoleTypeId
        {
            [Display(Name = "Admin")]
            Admin = 1,
            [Display(Name = "Manager")]
            Manager = 2,
            [Display(Name = "POS")]
            POS = 3,
            [Display(Name = "Waiter")]
            Waiter = 4,
            [Display(Name = "KitchenView")]
            KitchenView = 5,
        }

        public enum TaxType
        {
            Vat = 1,
            Exempted = 2,
            Zero = 3,
        }

    }
}
