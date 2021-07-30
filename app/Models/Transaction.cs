using System;

namespace authorize.Models
{
    public class Transaction
    {
        public string merchant { get; }
        public int amount { get; }
        public DateTime time { get; }

        public Transaction(string merchant, int amount, DateTime time)
        {
            this.merchant = merchant;
            this.amount = amount;
            this.time = time;
        }
    }
}
