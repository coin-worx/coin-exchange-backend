using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    public class LoginApplicationServiceTests
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
        public void LoginServiceInitializationAndInjectiontest_ChecksIfTheServiceGetsInitializedUsingSpring_FailsIfNot()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
        }
    }
}
