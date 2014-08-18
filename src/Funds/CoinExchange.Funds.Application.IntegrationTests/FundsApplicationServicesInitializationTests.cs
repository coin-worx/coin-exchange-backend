using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.DepositServices;
using CoinExchange.Funds.Application.WithdrawServices;
using CoinExchange.Funds.Domain.Model.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class FundsApplicationServicesInitializationTests
    {
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void Teardown()
        {
            _databaseUtility.Create();
        }

        [Test]
        public void DepositApplicationServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            Assert.IsNotNull(depositApplicationService);
        }

        [Test]
        public void WithdrawApplicationServiceInitializationTest_ChecksIfTheServiceGetsInitializedAsExpected_VerifiesThroughInstanceValue()
        {
            IWithdrawApplicationService withdrawApplicationService = (IWithdrawApplicationService)ContextRegistry.GetContext()["WithdrawApplicationService"];
            Assert.IsNotNull(withdrawApplicationService);
        }
    }
}
