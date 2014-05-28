using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class SecurityKeysApplicationServiceIntegrationTests
    {
        private IApplicationContext _applicationContext;
        private DatabaseUtility _databaseUtility;
        private ISecurityKeysRepository _securityKeysRepository;

        [SetUp]
        public void Setup()
        {
            _applicationContext = ContextRegistry.GetContext();
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _securityKeysRepository = _applicationContext["SecurityKeysPairRepository"] as ISecurityKeysRepository;
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

            Tuple<ApiKey, SecretKey> securityKeys = registrationService.CreateSystemGeneratedKey("Bob");

            Assert.IsNotNull(securityKeys);
            Assert.IsNotNull(securityKeys.Item1);
            Assert.IsNotNull(securityKeys.Item2);
        }

        [Test]
        public void CreateSystemGeneratedSecurityPair_IfUserNameIsProvided_VerifyKeyPairIsReturnedAndPersistedSuccessfully()
        {
             ISecurityKeysApplicationService registrationService =
                (ISecurityKeysApplicationService)_applicationContext["SecurityKeysApplicationService"];
            var keys = registrationService.CreateSystemGeneratedKey("user1");
            Assert.NotNull(keys);
            Assert.IsNotNullOrEmpty(keys.Item1.Value);
            Assert.IsNotNullOrEmpty(keys.Item2.Value);
            SecurityKeysPair persistedKeysPair = _securityKeysRepository.GetByApiKey(keys.Item1.Value);
            Assert.NotNull(persistedKeysPair);
            Assert.AreEqual(persistedKeysPair.UserName, "user1");
            Assert.AreEqual(persistedKeysPair.SystemGenerated, true);
            Assert.AreEqual(persistedKeysPair.ApiKey, keys.Item1.Value);
            Assert.AreEqual(persistedKeysPair.SecretKey,keys.Item2.Value);
            Assert.IsNotNullOrEmpty(persistedKeysPair.KeyDescription);
            Assert.IsNotNullOrEmpty(persistedKeysPair.CreationDateTime.ToString());
        }
    }
}
