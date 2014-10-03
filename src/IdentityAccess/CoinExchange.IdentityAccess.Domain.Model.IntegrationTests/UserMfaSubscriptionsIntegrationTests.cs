using System;
using System.Collections.Generic;
using System.Configuration;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Domain.Model.IntegrationTests
{
    [TestFixture]
    class UserMfaSubscriptionsIntegrationTests
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
        public void UserMfaSUbscriptionsAdditionTest_ChecksIfAllTheSubscriptionsAreAddedToUserAsExpected_VerifiesThroughDatabaseQuery()
        {
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];

            string userName = "NewUser";
            User user = new User(userName, "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            persistenceRepository.SaveUpdate(user);
            User receivedUser = userRepository.GetUserByUserName(userName);
            Assert.NotNull(receivedUser);
            Assert.AreEqual(receivedUser.Username, receivedUser.Username);
            Assert.AreEqual(receivedUser.Password, receivedUser.Password);
            Assert.AreEqual(receivedUser.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(receivedUser.Language, receivedUser.Language);
            Assert.AreEqual(receivedUser.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(receivedUser.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(receivedUser.Country, receivedUser.Country);
            Assert.AreEqual(receivedUser.State, receivedUser.State);
            Assert.AreEqual(receivedUser.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(receivedUser.Address1, receivedUser.Address1);
            Assert.AreEqual(receivedUser.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(receivedUser.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(receivedUser.IsUserBlocked.Value, false);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();
            Assert.IsNotNull(allSubscriptions);
            Assert.AreEqual(5, allSubscriptions.Count);
            Assert.AreEqual("CancelOrder", allSubscriptions[0].MfaSubscriptionName);
            Assert.AreEqual("Deposit", allSubscriptions[1].MfaSubscriptionName);
            Assert.AreEqual("Login", allSubscriptions[2].MfaSubscriptionName);
            Assert.AreEqual("PlaceOrder", allSubscriptions[3].MfaSubscriptionName);
            Assert.AreEqual("Withdraw", allSubscriptions[4].MfaSubscriptionName);

            IList<Tuple<string,string, bool>> subscriptionsStringList = new List<Tuple<string,string, bool>>();
            foreach (var subscription in allSubscriptions)
            {
                subscriptionsStringList.Add(new Tuple<string, string, bool>(subscription.MfaSubscriptionId, 
                    subscription.MfaSubscriptionName, true));
            }

            receivedUser.AssignMfaSubscriptions(subscriptionsStringList);
            persistenceRepository.SaveUpdate(receivedUser);

            receivedUser = userRepository.GetUserByUserName(userName);
            Assert.AreEqual(receivedUser.Username, receivedUser.Username);
            Assert.AreEqual(receivedUser.Password, receivedUser.Password);
            Assert.AreEqual(receivedUser.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(receivedUser.Language, receivedUser.Language);
            Assert.AreEqual(receivedUser.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(receivedUser.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(receivedUser.Country, receivedUser.Country);
            Assert.AreEqual(receivedUser.State, receivedUser.State);
            Assert.AreEqual(receivedUser.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(receivedUser.Address1, receivedUser.Address1);
            Assert.AreEqual(receivedUser.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(receivedUser.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(receivedUser.IsUserBlocked.Value, false);

            bool mfaSubscription1 = receivedUser.CheckMfaSubscriptions(allSubscriptions[0].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription1);
            bool mfaSubscription2 = receivedUser.CheckMfaSubscriptions(allSubscriptions[1].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription2);
            bool mfaSubscription3 = receivedUser.CheckMfaSubscriptions(allSubscriptions[2].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription3);
            bool mfaSubscription4 = receivedUser.CheckMfaSubscriptions(allSubscriptions[3].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription4);
            bool mfaSubscription5 = receivedUser.CheckMfaSubscriptions(allSubscriptions[4].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription5);
        }

        [Test]
        [Category("Integration")]
        public void SubscribeToRandomMfaSubscriptions_ChecksIfSubscribedMfaSUbscriptionsCanBeUnSubscribedAsExpected_VerifiesThourghDatabaseQuery()
        {
            // Enables only some of the subscriptions, and disables others, and checks if the subscriptions are being handled properly
            IIdentityAccessPersistenceRepository persistenceRepository = (IIdentityAccessPersistenceRepository)ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            IMfaSubscriptionRepository mfaSubscriptionRepository = (IMfaSubscriptionRepository)ContextRegistry.GetContext()["MfaSubscriptionRepository"];

            string userName = "NewUser";
            User user = new User(userName, "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            persistenceRepository.SaveUpdate(user);
            User receivedUser = userRepository.GetUserByUserName(userName);
            Assert.NotNull(receivedUser);

            IList<MfaSubscription> allSubscriptions = mfaSubscriptionRepository.GetAllSubscriptions();
            Assert.IsNotNull(allSubscriptions);
            Assert.GreaterOrEqual(allSubscriptions.Count, 5);

            IList<Tuple<string, string, bool>> subscriptionsStringList = new List<Tuple<string, string,bool>>();
            // Only add alternative elements from the list, but not all
            subscriptionsStringList.Add(new Tuple<string, string, bool>(allSubscriptions[0].MfaSubscriptionId, allSubscriptions[0].MfaSubscriptionName, true));
            subscriptionsStringList.Add(new Tuple<string, string, bool>(allSubscriptions[2].MfaSubscriptionId, allSubscriptions[2].MfaSubscriptionName, true));
            subscriptionsStringList.Add(new Tuple<string, string, bool>(allSubscriptions[4].MfaSubscriptionId, allSubscriptions[4].MfaSubscriptionName, true));

            receivedUser.AssignMfaSubscriptions(subscriptionsStringList);
            persistenceRepository.SaveUpdate(receivedUser);

            receivedUser = userRepository.GetUserByUserName(userName);
            Assert.NotNull(receivedUser);
            bool mfaSubscription1 = receivedUser.CheckMfaSubscriptions(allSubscriptions[0].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription1);
            bool mfaSubscription2 = receivedUser.CheckMfaSubscriptions(allSubscriptions[1].MfaSubscriptionName);
            Assert.IsFalse(mfaSubscription2);
            bool mfaSubscription3 = receivedUser.CheckMfaSubscriptions(allSubscriptions[2].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription3);
            bool mfaSubscription4 = receivedUser.CheckMfaSubscriptions(allSubscriptions[3].MfaSubscriptionName);
            Assert.IsFalse(mfaSubscription4);
            bool mfaSubscription5 = receivedUser.CheckMfaSubscriptions(allSubscriptions[4].MfaSubscriptionName);
            Assert.IsTrue(mfaSubscription5);
        }
    }
}
