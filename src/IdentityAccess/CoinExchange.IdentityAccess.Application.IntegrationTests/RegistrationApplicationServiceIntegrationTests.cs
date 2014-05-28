using System;
using System.Configuration;
using System.Threading;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    public class RegistrationApplicationServiceIntegrationTests
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
        [Category("Integration")]
        public void CreateNewAccountTest_ChecksWhetherTheUserIsCreatedAndSavedToPersistence_VerifiesThroughUsingDatabaseresult()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "waqas.syed@hotmail.com", "Bob", "iamnotalice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            // Wait for the email to be sent
            manualResetEvent.WaitOne(5000);
            User receivedUser = userRepository.GetUserByUserName("Bob");
            Assert.NotNull(receivedUser);
            Assert.AreEqual("Bob", receivedUser.Username);
            Assert.IsTrue(passwordEncryption.VerifyPassword("iamnotalice", receivedUser.Password));
            Assert.AreEqual("", receivedUser.PublicKey);
            Assert.AreEqual(TimeZone.CurrentTimeZone.StandardName, receivedUser.TimeZone.StandardName);
            Assert.AreEqual(activationKey, receivedUser.ActivationKey);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewAccountFailTest_CredentialsNotGivenAndDatabaseSaveShouldNotHappen_VerifiesThroughUsingDatabaseResult()
        {
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            bool exceptionCaught = false;
            string exceptionName = string.Empty;
            try
            {
                string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                                                                             "", "", "", "Wonderland", TimeZone.CurrentTimeZone, ""));
            }
            catch (Exception e)
            {
                exceptionName = e.GetType().Name;                
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
            Assert.AreEqual("InvalidCredentialException", exceptionName);
        }
    }
}
