using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.DigitalSignature;
using CoinExchange.IdentityAccess.Application.Registration;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    class DigitalSignatureServiceTests
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
            IDigitalSignatureApplicationService registrationService =
                (IDigitalSignatureApplicationService)_applicationContext["DigitalSignatureApplicationService"];
            Assert.IsNotNull(registrationService);
        }

        [Test]
        public void SecurityKeyGenerationService_TestsIfTheSecurityKeysAreGenerated_VerifiesThroughTheReturnedValues()
        {
            IDigitalSignatureApplicationService registrationService =
                (IDigitalSignatureApplicationService)_applicationContext["DigitalSignatureApplicationService"];

            SecurityKeys securityKeys = registrationService.GetNewKey();

            Assert.IsNotNull(securityKeys);
            Assert.IsNotNull(securityKeys.ApiKey);
            Assert.IsNotNull(securityKeys.SecretKey);
        }
    }
}
