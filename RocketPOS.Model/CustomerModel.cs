using System;

namespace RocketPOS.Model
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public int CustomerTypeId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public int UserId { get; set; }
        public int BalancePoints { get; set; }

    }

    public class CustomerRewardModel
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Datetime { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Balance { get; set; }
    }
}
