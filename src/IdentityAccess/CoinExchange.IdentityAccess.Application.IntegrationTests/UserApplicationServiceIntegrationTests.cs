using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    class UserApplicationServiceIntegrationTests
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
        public void ConfirmPasswordSuccessTest_ChecksIfThePasswordIsChangedSuccessfully_VerifiesThroughTheReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IIdentityAccessPersistenceRepository persistenceRepository =
                (IIdentityAccessPersistenceRepository)_applicationContext["IdentityAccessPersistenceRepository"];

            string username = "linkinpark";
            registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));
            
            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            bool changeSuccessful = userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials, "burnitdown",
                "burnitdowntwice", "burnitdowntwice"));

            Assert.IsTrue(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreNotEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        public void ConfirmPasswordFailTest_ChecksIfThePasswordIsNotChangedIfOldPasswordisIncorrect_VerifiesThroughTheReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];
            IIdentityAccessPersistenceRepository persistenceRepository =
                (IIdentityAccessPersistenceRepository)_applicationContext["IdentityAccessPersistenceRepository"];

            string username = "linkinpark";
            registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            bool exceptionRaised = false;

            try
            {
                userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials, "burnitdowner",
                                                      "burnitdowntwice", "burnitdowntwice"));
            }
            catch (InvalidCredentialException e)
            {
                exceptionRaised = true;
            }
            Assert.IsTrue(exceptionRaised);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        public void ConfirmPasswordFailTest_ChecksIfThePasswordIsNotChangedIfConfirmPasswordDoesNotMatch_VerifiesThroughTheReturnedValue()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            IUserRepository userRepository =
                (IUserRepository)_applicationContext["UserRepository"];

            string username = "linkinpark";
            registrationApplicationService.CreateAccount(new SignupUserCommand("linkinpark@rock.com", "linkinpark", "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            UserValidationEssentials validationEssentials = loginApplicationService.Login(new LoginCommand(username, "burnitdown"));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            bool exceptionRaised = false;
            try
            {
                userApplicationService.ChangePassword(new ChangePasswordCommand(validationEssentials, "burnitdown", "burnitdowntwice", "burnitdowntwice2"));
            }
            catch (InvalidCredentialException e)
            {
                exceptionRaised = true;
            }
            Assert.IsTrue(exceptionRaised);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        public void ForgotPasswordRequestSuccessTest_ProvidesAValidEmailIdToSendTheMailTo_VerifiesByTheReturnedBoolean()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            ILoginApplicationService loginApplicationService =
                (ILoginApplicationService)_applicationContext["LoginApplicationService"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            // Wait for the asynchrnous email sending event to be completed otherwise no mre emails can be sent until
            // this operation finishes
            manualResetEvent.WaitOne(6000);
            
            string returnedUsername = userApplicationService.ForgotUsername(email);
            // Wait for the email to be sent and operation to be completed
            manualResetEvent.WaitOne(5000);
            Assert.AreEqual(username, returnedUsername);
        }

        [Test]
        [Category("Integration")]
        public void ForgotPasswordRequestFailTest_ProvidesAnInvalidEmailIdToSendTheMailTo_VerifiesByTheReturnedBoolean()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            // Wait for the asynchrnous email sending event to be completed otherwise no mre emails can be sent until
            // this operation finishes
            manualResetEvent.WaitOne(5000);

            string returnedUsername = userApplicationService.ForgotUsername(email + "1");
            // Wait for the email to be sent and operation to be completed
            manualResetEvent.WaitOne(5000);
            Assert.IsNull(returnedUsername);
        }
    }
}
