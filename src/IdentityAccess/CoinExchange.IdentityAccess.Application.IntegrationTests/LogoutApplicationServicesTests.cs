using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class LogoutApplicationServicesTests
    {
        private IApplicationContext _applicationContext;
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            _applicationContext = ContextRegistry.GetContext();
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void TearDown()
        {
            ContextRegistry.Clear();
            _databaseUtility.Create();
        }

        [Test]
        public void LogoutServiceInitializationAndInjectiontest_ChecksIfTheServiceGetsInitializedUsingSpring_FailsIfNot()
        {
            ILogoutApplicationService logoutApplicationService = (ILogoutApplicationService)_applicationContext["LogoutApplicationService"];
            Assert.IsNotNull(logoutApplicationService);
        }
    }
}
