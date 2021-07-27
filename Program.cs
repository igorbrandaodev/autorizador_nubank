using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace authorize
{
    class Account
    {
        [JsonProperty("active-card")]
        public bool active_card { get; }

        [JsonProperty("available-limit")]
        public int available_limit { get; }

        [JsonIgnore]
        public List<Transaction> history { get; }

        public Account(bool active_card, int available_limit, List<Transaction> history)
        {
            this.active_card = active_card;
            this.available_limit = available_limit;
            this.history = history;
        }
    }

    class Transaction
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

    class JsonInOut
    {
        public Account account { get; set; }
        public Transaction transaction { internal get; set; }
        public List<string> violations { get; set; }

        public JsonInOut(Account account, List<string> violations)
        {
            this.account = account;
            this.violations = violations;
        }
    }

    class Program
    {

        static void Main()
        {
            Account account = null;
            string line;

            while ((line = Console.In.ReadLine()) != null)
            {
                JsonInOut json_in = JsonConvert.DeserializeObject<JsonInOut>(line);

                if (line.Contains("account"))
                {
                    List<string> violations = new();

                    if (account is null)
                    {
                        List<Transaction> new_history = new();
                        account = CreateAccount(json_in.account.active_card, json_in.account.available_limit, new_history);
                    }
                    else
                    {
                        violations.Add("account-already-initialized");
                    }

                    PrintObject(new JsonInOut(account, violations));
                }
                else
                {
                    JsonInOut json_out = AuthorizeTransaction(json_in.transaction.merchant, json_in.transaction.amount, json_in.transaction.time, account);
                    account = json_out.account;
                    PrintObject(json_out);
                }
            }
        }

        static void PrintObject(object json)
        {
            Console.WriteLine(JsonConvert.SerializeObject(json));
        }

        static Account CreateAccount(bool active_card, int available_limit, List<Transaction> history)
        {
            return new Account(active_card, available_limit, history);
        }

        static JsonInOut AuthorizeTransaction(string merchant, int amount, DateTime time, Account account)
        {
            time = DateTime.Now;
            List<string> violations = new();
            List<Transaction> transactions = account.history.FindAll(x => x.time >= DateTime.Now.AddMinutes(-2));

            if (account is null)
            {
                violations.Add("account-not-initialized");
            }

            if (!account.active_card)
            {
                violations.Add("card-not-active");
            }

            if (account.available_limit < amount)
            {
                violations.Add("insufficient-limit");
            }

            if (transactions.Count >= 3)
            {
                violations.Add("highfrequency-small-interval");
            }

            if (transactions.Any(x => x.amount == amount && x.merchant == merchant))
            {
                violations.Add("double-transaction");
            }

            if (violations.Count == 0)
                account.history.Add(new Transaction(merchant, amount, time));

            int new_amount = violations.Count == 0 ? (account.available_limit - amount) : account.available_limit;

            return new JsonInOut(CreateAccount(account.active_card, new_amount, account.history), violations);

        }


        // Unit tests
        // Integration tests
    }
}
