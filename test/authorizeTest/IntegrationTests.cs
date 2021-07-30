
using authorize;
using authorize.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace authorizeTest
{
    class IntegrationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldCreateJsonOutputOfAccountState()
        {
            int expected_available_limit = 120;
            bool expected_active_card = true;
            List<Transaction> expected_history = new();

            Account acccount = new(true, 120, new List<Transaction>());

            JsonInOut json_out = new(acccount, new List<string>());

            Assert.NotNull(json_out);
            Assert.AreEqual(expected_available_limit, json_out.account.available_limit);
            Assert.AreEqual(expected_active_card, json_out.account.active_card);
            Assert.IsEmpty(json_out.account.history);
            Assert.Null(json_out.transaction);
            Assert.IsEmpty(json_out.violations);
        }

        [Test]
        public void ShouldCreateAccount()
        {
            string expectedOutput = @"{""account"":{""active-card"":true,""available-limit"":175},""violations"":[]}";

            Account acccount = Program.CreateAccount(true, 175, new List<Transaction>());
            JsonInOut json_out = new(acccount, new List<string>());
            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void ViolationAccountAlreadyInitialized()
        {
            string expectedOutput = @"{""account"":{""active-card"":true,""available-limit"":175},""violations"":[""account-already-initialized""]}";

            string input = @"{""account"":{""active-card"":true,""available-limit"":175},""violations"":[]}";
            JsonInOut json_in = JsonConvert.DeserializeObject<JsonInOut>(input);
            Account acccount = new(true, 175, new List<Transaction>());
            JsonInOut json_out = Program.ValidateAccount(acccount, json_in);
            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void ShouldProcessTransaction()
        {

            string expectedOutput = @"{""account"":{""active-card"":true,""available-limit"":150},""violations"":[]}";

            // Create account
            Account acccount = new(true, 175, new List<Transaction>());

            // Process a transaction
            string merchant = "Uber Eats";
            int amount = 25;
            DateTime time = DateTime.Now;

            JsonInOut json_out = Program.AuthorizeTransaction(merchant, amount, time, acccount);

            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);

        }
        [Test]
        public void ViolationAccountNotInitialized()
        {
            string expectedOutput = @"{""account"":null,""violations"":[""account-not-initialized""]}";

            // Process a transaction
            string merchant = "Uber Eats";
            int amount = 25;
            DateTime time = DateTime.Now;

            JsonInOut json_out = Program.AuthorizeTransaction(merchant, amount, time, null);

            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);

        }
        [Test]
        public void ViolationCardNotActive()
        {
            string expectedOutput = @"{""account"":{""active-card"":false,""available-limit"":100},""violations"":[""card-not-active""]}";

            // Create account
            Account acccount = new(false, 100, new List<Transaction>());

            // Process a transaction
            string merchant = "Uber Eats";
            int amount = 25;
            DateTime time = DateTime.Now;

            JsonInOut json_out = Program.AuthorizeTransaction(merchant, amount, time, acccount); 
            
            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);
        }
        [Test]
        public void ViolationInsuficcientLimit()
        {
            string expectedOutput = @"{""account"":{""active-card"":true,""available-limit"":200},""violations"":[""insufficient-limit""]}";

            // Create account
            Account acccount = new(true, 200, new List<Transaction>());

            // Process a transaction
            string merchant = "Vivara";
            int amount = 1250;
            DateTime time = DateTime.Now;

            JsonInOut json_out = Program.AuthorizeTransaction(merchant, amount, time, acccount); 
            
            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);
        }
        [Test]
        public void ViolationHighFrequencySmallInterval()
        {
            string expectedOutput = @"{""account"":{""active-card"":true,""available-limit"":115},""violations"":[""highfrequency-small-interval""]}";

            // Create account
            Account acccount = new(true, 150, new List<Transaction>());

            // Process 1° transaction
            string merchant = "Burguer King";
            int amount = 15;
            DateTime time = DateTime.Now;

            JsonInOut json_out = Program.AuthorizeTransaction(merchant, amount, time, acccount);

            // Process 2° transaction
            merchant = "Subway";
            amount = 5;
            time = DateTime.Now;

            json_out = Program.AuthorizeTransaction(merchant, amount, time, json_out.account);

            // Process 3° transaction
            merchant = "Mc Donalds";
            amount = 15;
            time = DateTime.Now;

            json_out = Program.AuthorizeTransaction(merchant, amount, time, json_out.account);

            // Process 4° transaction
            merchant = "Mc Donalds";
            amount = 5;
            time = DateTime.Now;

            json_out = Program.AuthorizeTransaction(merchant, amount, time, json_out.account);


            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);
        }
        [Test]
        public void ViolationDoubleTransaction()
        {
            string expectedOutput = @"{""account"":{""active-card"":true,""available-limit"":135},""violations"":[""double-transaction""]}";

            // Create account
            Account acccount = new(true, 150, new List<Transaction>());

            // Process 1° transaction
            string merchant = "Burguer King";
            int amount = 15;
            DateTime time = DateTime.Now;

            JsonInOut json_out = Program.AuthorizeTransaction(merchant, amount, time, acccount);

            // Process 2° transaction with the same merchant and amount
            json_out = Program.AuthorizeTransaction(merchant, amount, time, json_out.account);

            string output = JsonConvert.SerializeObject(json_out);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
