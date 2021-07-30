using authorize.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace authorize
{

    public class Program
    {

        static void Main()
        {
            // State management
            Account account = null;
            string line;

            while ((line = Console.In.ReadLine()) != null)
            {
                // Deserialize input
                JsonInOut json_in = JsonConvert.DeserializeObject<JsonInOut>(line);

                switch (line)
                {
                    // Account
                    case string when line.Contains("account"):
                        {
                            JsonInOut json_out = ValidateAccount(account, json_in);
                            account = json_out.account;
                            Console.WriteLine(JsonConvert.SerializeObject(json_out));
                        }
                        break;
                    // Transaction
                    case string when line.Contains("transaction"):
                        {
                            JsonInOut json_out = AuthorizeTransaction(json_in.transaction.merchant, json_in.transaction.amount, json_in.transaction.time, account);
                            account = json_out.account;
                            Console.WriteLine(JsonConvert.SerializeObject(json_out));
                        }
                        break;
                }
            }
        }


        // Creates a new account
        public static Account CreateAccount(bool active_card, int available_limit, List<Transaction> history)
        {
            return new Account(active_card, available_limit, history);
        }


        // Validate the state of the account
        public static JsonInOut ValidateAccount(Account account, JsonInOut json_in)
        {
            List<string> violations = new();

            if (account is null)
            {
                account = CreateAccount(json_in.account.active_card, json_in.account.available_limit, new List<Transaction>());
            }
            else
            {
                violations.Add("account-already-initialized");
            }

            return new JsonInOut(account, violations);
        }


        // Process a transaction
        public static JsonInOut AuthorizeTransaction(string merchant, int amount, DateTime time, Account account)
        {
            time = DateTime.Now;
            List<string> violations = new();

            if (account is null)
            {
                violations.Add("account-not-initialized");
                return new JsonInOut(null, violations);
            }

            List<Transaction> transactions = account.history.FindAll(x => x.time >= DateTime.Now.AddMinutes(-2));

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
    }
}
