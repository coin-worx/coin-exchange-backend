using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    class UserApplicationServiceUnitTests
    {
        [Test]
        [Category("Unit")]
        public void ConfirmPasswordSuccessTest_ChecksIfThePasswordIsChangedSuccessfully_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository);

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", "linkinpark", true));

            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(new User("linkinpark@rock.com", "linkinpark",
                passwordEncryptionService.EncryptPassword("burnitdown"), "USA", TimeZone.CurrentTimeZone, "", ""));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            // Give the API key that is already stored in the Security keys repository mentioned with the User Name
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0,0,0,100));

            bool changeSuccessful = userApplicationService.ChangePassword(userValidationEssentials, "burnitdown", 
                "burnitdowntwice", "burnitdowntwice");

            Assert.IsTrue(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreNotEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Unit")]
        public void ConfirmPasswordFailTest_ChecksThePasswordDoesNotGetChangedWhenInvalidOldPasswordIsGiven_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository);

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", "linkinpark", true));

            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(new User("linkinpark@rock.com", "linkinpark",
                passwordEncryptionService.EncryptPassword("burnitdown"), "USA", TimeZone.CurrentTimeZone, "", ""));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            // Give the API key that is already stored in the Security keys repository mentioned with the User Name
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 100));

            // Wrong password given
            bool changeSuccessful = userApplicationService.ChangePassword(userValidationEssentials, "burnitdow",
                "burnitdowntwice", "burnitdowntwice");

            Assert.IsFalse(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }

        [Test]
        [Category("Unit")]
        public void ConfirmPasswordFailTest_ChecksThePasswordDoesNotGetChangedWhenConfirmationPasswordIsNotTheSame_VeririesThroughTheReturnedValue()
        {
            IUserRepository userRepository = new MockUserRepository();
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            IPasswordEncryptionService passwordEncryptionService = new PasswordEncryptionService();
            IIdentityAccessPersistenceRepository persistenceRepository = new MockPersistenceRepository(false);
            UserApplicationService userApplicationService = new UserApplicationService(userRepository, securityKeysRepository,
                passwordEncryptionService, persistenceRepository);

            // Store the Securiyty Keys with the Username of the User at hand
            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                new ApiKey("123456789").Value, new SecretKey("987654321").Value, "desc", "linkinpark", true));

            // We need to encrypt the password in the test case ourselves, as we are not registering the user through 
            // the proper service here
            (userRepository as MockUserRepository).AddUser(new User("linkinpark@rock.com", "linkinpark",
                passwordEncryptionService.EncryptPassword("burnitdown"), "USA", TimeZone.CurrentTimeZone, "", ""));

            User userBeforePasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordBeforeChange = userBeforePasswordChange.Password;

            // Give the API key that is already stored in the Security keys repository mentioned with the User Name
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 100));

            // Wrong password given
            bool changeSuccessful = userApplicationService.ChangePassword(userValidationEssentials, "burnitdown",
                "burnitdowntwice", "burnitdowntwice2");

            Assert.IsFalse(changeSuccessful);
            User userAfterPasswordChange = userRepository.GetUserByUserName("linkinpark");
            string passwordAfterChange = userAfterPasswordChange.Password;

            // Verify the old and new password do not match
            Assert.AreEqual(passwordBeforeChange, passwordAfterChange);
        }
    }
}
