using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.RegistrationServices;
using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    public class UserTierLevelApplicationServiceTests
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
        [ExpectedException(typeof(NullReferenceException))]
        public void ApplyForTier1()
        {
            IUserApplicationService userApplicationService = (IUserApplicationService)_applicationContext["UserApplicationService"];
            IRegistrationApplicationService registrationApplicationService =
                (IRegistrationApplicationService)_applicationContext["RegistrationApplicationService"];
            IUserRepository userRepository = (IUserRepository)_applicationContext["UserRepository"];
            IPasswordEncryptionService passwordEncryption =
                (IPasswordEncryptionService)_applicationContext["PasswordEncryptionService"];

            string username = "linkinpark";
            string email = "waqas.syed@hotmail.com";
            registrationApplicationService.CreateAccount(new SignupUserCommand(email, username, "burnitdown", "USA", TimeZone.CurrentTimeZone, ""));
            IUserTierLevelApplicationService tierLevelApplicationService =
                _applicationContext["UserTierLevelApplicationService"] as IUserTierLevelApplicationService;
            tierLevelApplicationService.ApplyForTier1Verification(new VerifyTier1Command("1234","123","123",DateTime.Today));
            //TODO: have to modify the test and test case name.
        }
    }
}
