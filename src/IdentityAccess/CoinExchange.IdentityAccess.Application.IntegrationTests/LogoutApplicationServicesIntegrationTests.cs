using System;
using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class LogoutApplicationServicesIntegrationTests
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
        public void LogoutServiceInitializationAndInjectiontest_ChecksIfTheServiceGetsInitializedUsingSpring_FailsIfNot()
        {
            ILogoutApplicationService logoutApplicationService = (ILogoutApplicationService)_applicationContext["LogoutApplicationService"];
            Assert.IsNotNull(logoutApplicationService);
        }

        [Test]
        [Category("Integration")]
        public void LogoutSuccessTest_TestsIfAUserGetsLogoutAsExpected_FailsIfDoesNot()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"]; ;

            // Register
            string username = "Bob";
            string password = "alice";
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", username, password, "Wonderland", TimeZone.CurrentTimeZone, ""));
            Assert.IsNotNull(activationKey);

            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            // Activate account
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));
            Assert.IsTrue(accountActivated);
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);

            // Login
            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand(username, password));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);

            // Logout
            ILogoutApplicationService logoutApplicationService = 
                (ILogoutApplicationService)_applicationContext["LogoutApplicationService"];
            Assert.IsNotNull(logoutApplicationService);
            bool logout = logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey,
                userValidationEssentials.SecretKey));
            Assert.IsTrue(logout);

            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];
            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(userValidationEssentials.ApiKey);
            Assert.IsNull(securityKeysPair);
        }
    }
}
