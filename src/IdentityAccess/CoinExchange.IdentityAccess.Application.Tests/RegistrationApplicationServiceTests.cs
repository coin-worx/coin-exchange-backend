using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.Registration;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    public class RegistrationApplicationServiceTests
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
        public void InjectionTest_TestsWhetherSpringInitiatesAsExpectedAndInitializesRegistrationService_FailsIfNotInitialized()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            Assert.IsNotNull(registrationService);
        }

        [Test]
        public void CreateNewAccountTest_ChecksWhetherTheUserIsCreatedAndSavedToPersistence_VerifiesThroughUsingDatabaseresult()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            string activationKey = registrationService.CreateAccount("dummy@dumdum.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, "");

            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            Assert.AreEqual("Bob", receivedUser.Username);
            Assert.IsTrue(passwordEncryption.VerifyPassword("iamnotalice", receivedUser.Password));
            Assert.AreEqual("", receivedUser.PublicKey);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, receivedUser.TimeZone.StandardName);
            Assert.AreEqual(activationKey, receivedUser.ActivationKey);
        }
    }
}
