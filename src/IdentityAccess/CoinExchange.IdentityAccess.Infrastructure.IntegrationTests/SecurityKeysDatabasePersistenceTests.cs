using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    class SecurityKeysDatabasePersistenceTests
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

        [Test]
        [Category("Integration")]
        public void SecurityKeysMfaVerification_ChecksIfMfaSubscriptionsAreAddedAsExpected_VerifiesByQueryingTheDatabase()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            ISecurityKeysRepository securityKeysRepository = (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];

            string apiKey = "123456";
            SecurityKeysPair originalPair = new SecurityKeysPair("1", apiKey, "secretkey", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, false, null);
            persistenceRepository.SaveUpdate(originalPair);

            SecurityKeysPair retrievedPair = securityKeysRepository.GetByApiKey(apiKey);
            Assert.IsNotNull(retrievedPair);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();
            Assert.IsNotNull(allSubscriptions);
            Assert.AreEqual(5, allSubscriptions.Count);
            Assert.AreEqual("CancelOrder", allSubscriptions[0].MfaSubscriptionName);
            Assert.AreEqual("Deposit", allSubscriptions[1].MfaSubscriptionName);
            Assert.AreEqual("Login", allSubscriptions[2].MfaSubscriptionName);
            Assert.AreEqual("PlaceOrder", allSubscriptions[3].MfaSubscriptionName);
            Assert.AreEqual("Withdraw", allSubscriptions[4].MfaSubscriptionName);

            IList<Tuple<string, string, bool>> subscriptionsStringList = new List<Tuple<string, string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                subscriptionsStringList.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId,
                    subscription.MfaSubscriptionName, true));
            }
            retrievedPair.AssignMfaSubscriptions(subscriptionsStringList);

            persistenceRepository.SaveUpdate(retrievedPair);

            retrievedPair = securityKeysRepository.GetByApiKey(apiKey);
            Assert.IsNotNull(retrievedPair);
        }
    }
}
