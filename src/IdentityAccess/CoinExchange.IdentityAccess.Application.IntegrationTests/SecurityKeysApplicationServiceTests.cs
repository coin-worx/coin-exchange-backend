using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class SecurityKeysApplicationServiceTests
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
        public void ServiceStartuptest_TestsIfTheServiceStartsAsExpectedThroughSpring_VerifiesThroughTheStateOfTheObject()
        {
            ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            Assert.IsNotNull(registrationService);
        }

        [Test]
        public void SecurityKeyGenerationService_TestsIfTheSecurityKeysAreGenerated_VerifiesThroughTheReturnedValues()
        {
            ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];

            SecurityKeysPair securityKeys = registrationService.GetNewKey();

            Assert.IsNotNull(securityKeys);
            Assert.IsNotNull(securityKeys.ApiKey);
            Assert.IsNotNull(securityKeys.SecretKey);
        }
    }
}
