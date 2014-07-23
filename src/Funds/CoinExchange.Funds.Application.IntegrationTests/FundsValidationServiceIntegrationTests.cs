using System.Configuration;
using CoinExchange.Common.Tests;
using NUnit.Framework;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class FundsValidationServiceIntegrationTests
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


    }
}
