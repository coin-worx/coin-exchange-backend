using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    /// <summary>
    /// MfaSubscriptionRepositoryIntegrationTests
    /// </summary>
    [TestFixture]
    public class MfaSubscriptionRepositoryIntegrationTests
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
        public void InstanceInitializationTest_ChecksIfInstanceIsCreatedUsingSpringDiAsExpected_VerifiesThroughInstanceValue()
        {
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];
            Assert.IsNotNull(mfaSubscriptionRepository);
        }

        [Test]
        [Category("Integration")]
        public void GetAllSubscriptionsTest_GetsTheListOfAllTheSubscriptions_VerifiesThroughDatabaseQuery()
        {
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];
            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();
            Assert.IsNotNull(allSubscriptions);
            Assert.AreEqual(5, allSubscriptions.Count);
            Assert.AreEqual("Login", allSubscriptions[0].MfaSubscriptionName);
            Assert.AreEqual("Deposit", allSubscriptions[1].MfaSubscriptionName);
            Assert.AreEqual("Withdraw", allSubscriptions[2].MfaSubscriptionName);
            Assert.AreEqual("CreateOrder", allSubscriptions[3].MfaSubscriptionName);
            Assert.AreEqual("CancelOrder", allSubscriptions[4].MfaSubscriptionName);
        }
    }
}
