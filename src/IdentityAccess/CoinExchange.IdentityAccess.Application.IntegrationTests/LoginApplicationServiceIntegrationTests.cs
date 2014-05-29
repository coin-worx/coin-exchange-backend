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
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            
            string username = "Bob";
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", username, "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice"));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);
        }

        [Test]
        [Category("Integration")]
        public void LoginSuccessfulTest_TestsifTheLoginisSuccessfulAfterProvidingValidCredentials_VerifiesByGettingUserFromRepositoryAndCheckingCredentials()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryptionService = (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];
            string username = "Bob";
            string email = "bob@alice.com";
            string password = "alice";
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                email, username, password, "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice"));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);

            User user = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Email, email);
            Assert.AreEqual(user.ActivationKey, activationKey);
            Assert.IsTrue(passwordEncryptionService.VerifyPassword(password, user.Password));
        }

        [Test]
        [Category("Integration")]
        public void LoginSuccessfulAndCheckSecurityKeysPairTest_ChecksIfAfterUserLoginSecurityPairsValuesAreAsExpected_ChecksByGettingSecurityKeysFromRepo()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];
            
            string username = "Bob";
            string email = "bob@alice.com";
            string password = "alice";
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                email, username, password, "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand(
                                                                    username, password));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);

            User user = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(user);
            // Check that the user logged in this same minute and date, as we cannot check the seconds exactly
            Assert.AreEqual(user.LastLogin.Date, DateTime.Today.Date);
            Assert.AreEqual(user.LastLogin.Hour, DateTime.Now.Hour);
            Assert.AreEqual(user.LastLogin.Minute, DateTime.Now.Minute);
            Assert.AreEqual(userValidationEssentials.SessionLogoutTime, user.AutoLogout);

            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(userValidationEssentials.ApiKey.Value);
            Assert.IsNotNull(securityKeysPair);
            Assert.AreEqual(userValidationEssentials.SecretKey.Value, securityKeysPair.SecretKey);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailTest_TestsifTheLoginFailsAfterProvidingInvalidUsername_VerifiesThroughTheReturnedResult()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"]; ;

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", "Bob", "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);
            
            loginApplicationService.Login(new LoginCommand("bobby", "alice"));            
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailTest_TestsifTheLoginisFailsAfterProvidingBlankUsername_VerifiesThroughTheReturnedResult()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"]; ;

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", "Bob", "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            loginApplicationService.Login(new LoginCommand("", "alice"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailTest_TestsifTheLoginFailsAfterProvidingInvalidPassword_VerifiesThroughTheReturnedResult()
        {
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService) _applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService) _applicationContext["RegistrationApplicationService"];
            
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                                                                         "bob@alice.com", "Bob", "alice", "Wonderland",
                                                                         TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            loginApplicationService.Login(new LoginCommand("Bob", "khaleesi"));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailTest_TestsifTheLoginFailsAfterProvidingBlankPassword_VerifiesThroughTheReturnedResult()
        {
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                                                                         "bob@alice.com", "Bob", "alice", "Wonderland",
                                                                         TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);

            loginApplicationService.Login(new LoginCommand("Bob", ""));
        }
    }
}
