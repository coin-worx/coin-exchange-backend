using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Authentication;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.Services;
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

            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);

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

            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice"));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);

            User user = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Email, email);
            Assert.AreEqual(user.ActivationKey, activationKey);
            Assert.AreEqual(userValidationEssentials.SessionLogoutTime, user.AutoLogout);
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

            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);

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

            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(userValidationEssentials.ApiKey);
            Assert.IsNotNull(securityKeysPair);
            Assert.AreEqual(userValidationEssentials.SecretKey, securityKeysPair.SecretKey);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ActivateAccountFailTest_ChecksIfUserCannotLoginUntilAccountIsNotActivated_VerifiesByExpectingException()
        {
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            string username = "linkinpark";
            string email = "waqasshah047@gmail.com";
            string password = "burnitdown";
            registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));

            loginApplicationService.Login(new LoginCommand(username, password));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ActivateAccountFailThenSeccussfulTest_ChecksIfUserCannotLoginUntilAccountIsNotActivatedAndTriesToActivateAgainAndThenLogsIn_VerifiesByExpectingExceptionAndReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];

            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)_applicationContext["SecurityKeysPairRepository"];

            string username = "linkinpark";
            string email = "waqasshah047@gmail.com";
            string password = "burnitdown";
            string activationKey = registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, password, "USA", TimeZone.CurrentTimeZone, ""));

            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, password));
            Assert.IsNull(validationEssentials);

            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, username, password));

            Assert.IsTrue(accountActivated);
            User userByUserName = userRepository.GetUserByUserName(username);
            Assert.IsNotNull(userByUserName);
            Assert.IsTrue(userByUserName.IsActivationKeyUsed.Value);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand(username, password));
            Assert.IsNotNull(userValidationEssentials);
            SecurityKeysPair securityKeysPair = securityKeysRepository.GetByApiKey(userValidationEssentials.ApiKey);
            Assert.IsNotNull(securityKeysPair);
            User receivedUser = userRepository.GetUserByUserName(username);
            Assert.IsTrue(receivedUser.IsActivationKeyUsed.Value);
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
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);
            
            loginApplicationService.Login(new LoginCommand("bobby", "alice"));            
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LoginFailTest_TestsifTheLoginisFailsAfterProvidingBlankUsername_VerifiesThroughTheReturnedResult()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"]; ;

            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", "Bob", "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));

            Assert.IsNotNull(activationKey);
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));

            Assert.IsTrue(accountActivated);
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
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);

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
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);

            loginApplicationService.Login(new LoginCommand("Bob", ""));
        }

        [Test]
        [Category("Integration")]
        public void LoginMfaAuthorizationTest_ChecksIfMfaAuthorizationIsDoneSuccessfully_VerifiesThroughReturnValue()
        {
            ILoginApplicationService loginApplicationService = (ILoginApplicationService)_applicationContext["LoginApplicationService"];
            ILogoutApplicationService logoutApplicationService = (ILogoutApplicationService)_applicationContext["LogoutApplicationService"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)_applicationContext["MfaSubscriptionRepository"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            Assert.IsNotNull(loginApplicationService);
            IRegistrationApplicationService registrationService = (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            string username = "Bob";
            string activationKey = registrationService.CreateAccount(new SignupUserCommand(
                "bob@alice.com", username, "alice", "Wonderland", TimeZone.CurrentTimeZone, ""));
            Assert.IsNotNull(activationKey);

            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            bool accountActivated = userApplicationService.ActivateAccount(new ActivationCommand(activationKey, "Bob", "alice"));
            Assert.IsTrue(accountActivated);

            UserValidationEssentials userValidationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice"));
            Assert.IsNotNull(userValidationEssentials);
            Assert.IsNotNull(userValidationEssentials.ApiKey);
            Assert.IsNotNull(userValidationEssentials.SecretKey);
            Assert.IsNotNull(userValidationEssentials.SessionLogoutTime);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();
            List<Tuple<string,string,bool>> mfaSubscriptions = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                mfaSubscriptions.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId, subscription.MfaSubscriptionName,
                    true));
            }
            SubmitMfaSettingsResponse submitMfaSettingsResponse = userApplicationService.SubmitMfaSettings(
                new MfaSettingsCommand(false, null, userValidationEssentials.ApiKey, mfaSubscriptions));
            Assert.IsTrue(submitMfaSettingsResponse.Successful);

            logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));

            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice", null));
            Assert.IsFalse(validationEssentials.LoginSuccessful);
            Assert.IsNull(validationEssentials.ApiKey);

            // As we are using the stub implementation for MfaCodeGenerationService, the service returns only the same code every 
            // time. So we can use the code in this test case

            validationEssentials = loginApplicationService.Login(new LoginCommand("Bob", "alice", mfaCodeGenerationService.GenerateCode()));
            Assert.IsTrue(validationEssentials.LoginSuccessful);
            Assert.IsNotNull(validationEssentials.ApiKey);
            Assert.IsNotNull(validationEssentials.SecretKey);
        }
    }
}
