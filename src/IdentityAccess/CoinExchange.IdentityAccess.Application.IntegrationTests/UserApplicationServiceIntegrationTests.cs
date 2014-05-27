using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
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
        public void ConfirmPasswordSuccessTest_ChecksIfThePasswordIsChangedSuccessfully_VeririesThroughTheReturnedValue()
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

            bool changeSuccessful = userApplicationService.ChangePassword(validationEssentials, "burnitdown",
                "burnitdowntwice", "burnitdowntwice");

            Assert.IsTrue(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreNotEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        public void ConfirmPasswordFailTest_ChecksIfThePasswordIsNotChangedIfOldPasswordisIncorrect_VeririesThroughTheReturnedValue()
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

            bool changeSuccessful = userApplicationService.ChangePassword(validationEssentials, "burnitdowner",
                "burnitdowntwice", "burnitdowntwice");

            Assert.IsFalse(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Integration")]
        public void ConfirmPasswordFailTest_ChecksIfThePasswordIsNotChangedIfConfirmPasswordDoesNotMatch_VeririesThroughTheReturnedValue()
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

            bool changeSuccessful = userApplicationService.ChangePassword(validationEssentials, "burnitdown",
                "burnitdowntwice", "burnitdowntwice2");

            Assert.IsFalse(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }
    }
}
