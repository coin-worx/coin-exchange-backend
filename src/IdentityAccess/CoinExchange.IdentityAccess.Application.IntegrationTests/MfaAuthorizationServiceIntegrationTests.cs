using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Application.MfaServices;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    internal class MfaAuthorizationServiceIntegrationTests
    {
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
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
        public void ServiceInitilizationTest_ChecksIfTheServiceInstanceIsInitializedThroughSpringAsExpected_VerifiesThroughVariableValue()
        {
            IMfaAuthorizationService mfaAuthorizationService = (IMfaAuthorizationService) ContextRegistry.GetContext()["MfaAuthorizationService"];
            Assert.IsNotNull(mfaAuthorizationService);
        }

        [Test]
        [Category("Integration")]
        public void MfaAuthorizationSuccessfulTest_ChecksThatServiceReturnsTrueIfNoMfaSubscriptionIsSubscribed_VerifiesThroughReturnsValue()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            ISecurityKeysRepository securityKeysPairRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaCodeSenderService mfaSmsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            IMfaCodeSenderService mfaEmailService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaEmailService"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            IMfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(persistenceRepository,
                userRepository, securityKeysPairRepository, mfaSmsService, mfaEmailService, mfaCodeGenerationService);

            string apiKey = "123";
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");
            persistenceRepository.SaveUpdate(user);

            user = userRepository.GetUserByUserName(userName);
            Assert.IsNotNull(user);
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(user.Id, apiKey, "secret123", true, "#1");
            persistenceRepository.SaveUpdate(securityKeysPair);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, "");
            Assert.IsTrue(authorizeAccess.Item1);
        }

        [Test]
        [Category("Integration")]
        public void MfaAuthorizationFailTest_ChecksThatServiceReturnsFalseIfMfaCodeIsNotProvidedAfterSubscription_VerifiesThroughReturnsValue()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            ISecurityKeysRepository securityKeysPairRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaCodeSenderService mfaSmsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            IMfaCodeSenderService mfaEmailService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaEmailService"];
            IMfaCodeGenerationService mfaCodeGenerationService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];
            IMfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(persistenceRepository,
                userRepository, securityKeysPairRepository, mfaSmsService, mfaEmailService, mfaCodeGenerationService);

            string apiKey = "123";
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";

            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");
            persistenceRepository.SaveUpdate(user);

            user = userRepository.GetUserByUserName(userName);
            Assert.IsNotNull(user);
            SecurityKeysPair securityKeysPair = new SecurityKeysPair(user.Id, apiKey, "secret123", true, "#1");
            persistenceRepository.SaveUpdate(securityKeysPair);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, "");
            Assert.IsTrue(authorizeAccess.Item1);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();

            IList<Tuple<string, string, bool>> mfaSubscriptions = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                mfaSubscriptions.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId, 
                    subscription.MfaSubscriptionName, true));
            }
            user.AssignMfaSubscriptions(mfaSubscriptions);
            persistenceRepository.SaveUpdate(user);

            authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, MfaConstants.Deposit, null);
            Assert.IsFalse(authorizeAccess.Item1);
        }
    }
}