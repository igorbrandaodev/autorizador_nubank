using authorize;
using authorize.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace authorizeTest
{
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldCreateValidAccount()
        {
            int expected_available_limit = 120;
            bool expected_active_card = true;
            List<Transaction> expected_history = new();

            Account acccount = new(true, 120, new List<Transaction>());

            Assert.AreEqual(expected_available_limit, acccount.available_limit);
            Assert.AreEqual(expected_active_card, acccount.active_card);
            Assert.AreEqual(expected_history, acccount.history);
        }

        [Test]
        public void ShouldCreateValidTransaction()
        {
            string expected_merchant = "Burguer King";
            int expected_amount = 5;

            Transaction transaction = new("Burguer King", 5, DateTime.Now);

            Assert.AreEqual(expected_merchant, transaction.merchant);
            Assert.AreEqual(expected_amount, transaction.amount);
            Assert.IsInstanceOf<DateTime>(transaction.time);

        }

        [Test]
        public void ShouldDeserializeValidAccount()
        {
            int expected_available_limit = 175;
            bool expected_active_card = true;

            string jsonAccountInput = @"{""account"": {""active-card"": true, ""available-limit"": 175}}";

            JsonInOut accountJson = JsonConvert.DeserializeObject<JsonInOut>(jsonAccountInput);

            Assert.NotNull(accountJson);
            Assert.AreEqual(expected_available_limit, accountJson.account.available_limit);
            Assert.AreEqual(expected_active_card, accountJson.account.active_card);
            Assert.Null(accountJson.account.history);
            Assert.Null(accountJson.transaction);
            Assert.Null(accountJson.violations);

        }

        [Test]
        public void ShouldDeserializeValidTransaction()
        {
            string expected_merchant = "Burger King";
            int expected_amount = 20;

            string jsonTransactionInput = @"{""transaction"": {""merchant"": ""Burger King"", ""amount"": 20, ""time"": ""2019-02-13T11:00:00.000Z""}}";

            JsonInOut transactionJson = JsonConvert.DeserializeObject<JsonInOut>(jsonTransactionInput);

            Assert.NotNull(transactionJson);
            Assert.AreEqual(expected_merchant, transactionJson.transaction.merchant);
            Assert.AreEqual(expected_amount, transactionJson.transaction.amount);
            Assert.IsInstanceOf<DateTime>(transactionJson.transaction.time);
            Assert.Null(transactionJson.account);
            Assert.Null(transactionJson.violations);
        }

    }
}