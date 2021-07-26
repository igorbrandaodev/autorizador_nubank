using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace authorize
{
    class Account
    {
        [JsonProperty("active-card")]
        public bool active_card { get; set; }

        [JsonProperty("available-limit")]
        public int available_limit { get; set; }

        [JsonIgnore]
        public List<Transaction> history { get; set; }

        public Account(bool active_card, int available_limit)
        {
            this.active_card = active_card;
            this.available_limit = available_limit;
            this.history = null;
        }
    }

    class Transaction
    {
        public string merchant { get; set; }
        public int amount { get; set; }
        public DateTime time { get; set; }
    }

    class Json
    {
        public Account account { get; set; }
        public Transaction transaction { internal get; set; }
        public List<string> violations { get; set; }
    }

    class Program
    {

        static void Main()
        {
            // State management
            Account account = null;


            // Program
            string line;
            while ((line = Console.In.ReadLine()) != null)
            {
                Json json = JsonConvert.DeserializeObject<Json>(line);

                // Decision
                if (line.Contains("account"))
                {

                    // account-already-initialized
                    if (account is null)
                    {
                        account = create(json.account.active_card, json.account.available_limit);
                        json.account = account;
                        json.violations = new List<string>();
                        Console.WriteLine(JsonConvert.SerializeObject(json));
                    }
                    else
                    {
                        json.account = account;
                        json.violations = new List<string> { "account-already-initialized" };
                        Console.WriteLine(JsonConvert.SerializeObject(json));
                    }

                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(authorize(json.transaction.merchant, json.transaction.amount, json.transaction.time, account)));

                }

            }

        }

        // Create
        // in: available-limit, active-card
        // out: account
        static Account create(bool active_card, int available_limit)
        {
            return new Account(active_card, available_limit);
        }

        // Authorizate
        // in: merchant, amount, time
        // out: account
        static Json authorize(string merchant, int amount, DateTime time, Account account)
        {
            time = DateTime.Now;

            // account-not-initialized
            if (account is null)
            {
                return new Json { account = account, violations = new List<string> { "account-not-initialized" } };
            }

            // card-not-active
            if (!account.active_card)
            {
                return new Json { account = account, violations = new List<string> { "card-not-active" } };
            }

            // insufficient-limit
            if (account.available_limit < amount)
            {
                return new Json { account = account, violations = new List<string> { "insufficient-limit" } };
            }


            if (account.history is not null)
            {
                List<Transaction> transactions = account.history.FindAll(x => x.time >= DateTime.Now.AddMinutes(-2));


                // highfrequency-small-interval
                if (transactions.Count() >= 3)
                {
                    return new Json { account = account, violations = new List<string> { "highfrequency-small-interval" } };
                }

                // double-transaction
                if (transactions.Any(x => x.amount == amount && x.merchant == merchant))
                {
                    return new Json { account = account, violations = new List<string> { "double-transaction" } };
                }

                account.history.Add(new Transaction { merchant = merchant, amount = amount, time = time });
            }
            else
            {
                account.history = new List<Transaction> { new Transaction { merchant = merchant, amount = amount, time = time } };
            }


            // Processing
            account.available_limit -= amount;

            return new Json { account = account, violations = new List<string>() };

        }


        // Unit tests
        // Integration tests
    }
}
