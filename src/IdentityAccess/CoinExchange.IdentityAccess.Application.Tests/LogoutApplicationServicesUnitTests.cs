using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.AccessControlServices;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    class 
        LogoutApplicationServicesUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        [Category("Unit")]
        public void LogoutSuccessfulTest_ChecksIfTheTheUserProperlyLogsOutWhenCorrectCredentialsAreGiven_VerifiesTheReturnedValueToConfirm()
        {
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            ILogoutApplicationService logoutApplicationService = new LogoutApplicationService(securityKeysRepository);

            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair( 
                "123456789", "987654321", "1", 0, true));
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("123456789"), new SecretKey("987654321")), new TimeSpan(0,0,0,10));
            bool logout = logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));

            Assert.IsTrue(logout);
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void LogoutFailTest_ChecksIfLogoutFailsAsExpectedWhenWrongApiKeyIfGiven_VerifiesTheReturnedKeysToConfirm()
        {
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            ILogoutApplicationService logoutApplicationService = new LogoutApplicationService(securityKeysRepository);

            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                "123456789", "987654321", "1", 0, true));
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("12345678910"), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 10));
            logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LogoutFailTest_ChecksIfLogoutFailsWhenBlankApiKeyIsGiven_VerifiesTheReturnedKeysToConfirm()
        {
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            ILogoutApplicationService logoutApplicationService = new LogoutApplicationService(securityKeysRepository);

            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                "123456789", "987654321", "1", 0, true));
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey(""), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 10));
            logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void LogoutFailTest_ChecksIfLogoutFailsWhenInvalidApiKeyIsGiven_VerifiesTheReturnedKeysToConfirm()
        {
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            ILogoutApplicationService logoutApplicationService = new LogoutApplicationService(securityKeysRepository);

            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                "123456789", "987654321", "1", 0, true));
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("12345"), new SecretKey("987654321")), new TimeSpan(0, 0, 0, 10));
            logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void LogoutFailTest_ChecksIfLogoutFailsWhenBlankSecretKeyIsGiven_VerifiesTheReturnedKeysToConfirm()
        {
            ISecurityKeysRepository securityKeysRepository = new MockSecurityKeysRepository();
            ILogoutApplicationService logoutApplicationService = new LogoutApplicationService(securityKeysRepository);

            (securityKeysRepository as MockSecurityKeysRepository).AddSecurityKeysPair(new SecurityKeysPair(
                "123456789", "987654321", "1", 0, true));
            UserValidationEssentials userValidationEssentials = new UserValidationEssentials(new Tuple<ApiKey, SecretKey>(
                new ApiKey("123456789"), new SecretKey("")), new TimeSpan(0, 0, 0, 10));
            logoutApplicationService.Logout(new LogoutCommand(userValidationEssentials.ApiKey));
        }
    }
}
