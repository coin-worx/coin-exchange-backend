using System;
using System.Configuration;
using System.Security.Authentication;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
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
        [Category("Integration")]
        public void LoginServiceInitializationAndInjectiontest_ChecksIfTheServiceGetsInitializedUsingSpring_FailsIfNot()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
        }

        [Test]
        [Category("Integration")]
        public void LoginSuccessfulTest_TestsifTheLoginisSuccessfulAfterProvidingValidCredentials_VerifiesThroughThereturnedResult()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];;

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", "Bob", "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice"));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);
        }

        [Test]
        [Category("Integration")]
        public void LoginFailTest_TestsifTheLoginisSuccessfulAfterProvidingInvalidUsername_VerifiesThroughTheReturnedResult()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"]; ;

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", "Bob", "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);
            bool exceptionRaised = false;
            try
            {
                loginApplicationService.Login(new LoginCommand("bobby", "alice"));
            }
            catch (InvalidCredentialException e)
            {
                exceptionRaised = true;
            }

            Assert.IsTrue(exceptionRaised);
        }

        [Test]
        [Category("Integration")]
        public void LoginFailTest_TestsifTheLoginisSuccessfulAfterProvidingInvalidPassword_VerifiesThroughTheReturnedResult()
        {
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService) _applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService) _applicationContext["RegistrationApplicationService"];
            ;

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                                                                         "bob@alice.com", "Bob", "alice", "Wonderland",
                                                                         TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            bool exceptionRaised = false;
            try
            {
                loginApplicationService.Login(new LoginCommand("Bob", "khaleesi"));
            }
            catch (InvalidCredentialException e)
            {
                exceptionRaised = true;
            }

            Assert.IsTrue(exceptionRaised);
        }
    }
}
