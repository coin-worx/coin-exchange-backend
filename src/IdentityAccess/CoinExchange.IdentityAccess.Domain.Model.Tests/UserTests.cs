using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Domain.Model.Tests
{
    [TestFixture]
    class UserTests
    {
        [Test]
        [Category("Unit")]
        public void UserMfaSubscriptionsTest_VerifiesThatTheSubscriptionsAreAddedAsExpected_VerifiesThroughReturnedValue()
        {
            string userName = "NewUser";
            User user = new User(userName, "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");

            Tuple<string, string, bool> loginSubscription = new Tuple<string, string, bool>("LOG", "Login", true);
            Tuple<string, string, bool> depositSubscription = new Tuple<string, string, bool>("DEP", "Deposit", true);
            Tuple<string, string, bool> withdrawSubscription = new Tuple<string, string, bool>("WD", "Withdraw", true);
            Tuple<string, string, bool> placeOrderSubscription = new Tuple<string, string, bool>("PO", "PlaceOrder", true);
            Tuple<string, string, bool> cancelOrderSubscription = new Tuple<string, string, bool>("CO", "CancelOrder", true);
            IList<Tuple<string, string, bool>> subscriptionsList = new List<Tuple<string, string, bool>>();
            subscriptionsList.Add(loginSubscription);
            subscriptionsList.Add(depositSubscription);
            subscriptionsList.Add(withdrawSubscription);
            subscriptionsList.Add(placeOrderSubscription);
            subscriptionsList.Add(cancelOrderSubscription);
            user.AssignMfaSubscriptions(subscriptionsList);

            Assert.NotNull(user);
            bool mfaSubscription1 = user.CheckMfaSubscriptions(subscriptionsList[0].Item2);
            Assert.IsTrue(mfaSubscription1);
            bool mfaSubscription2 = user.CheckMfaSubscriptions(subscriptionsList[1].Item2);
            Assert.IsTrue(mfaSubscription2);
            bool mfaSubscription3 = user.CheckMfaSubscriptions(subscriptionsList[2].Item2);
            Assert.IsTrue(mfaSubscription3);
            bool mfaSubscription4 = user.CheckMfaSubscriptions(subscriptionsList[3].Item2);
            Assert.IsTrue(mfaSubscription4);
            bool mfaSubscription5 = user.CheckMfaSubscriptions(subscriptionsList[4].Item2);
            Assert.IsTrue(mfaSubscription5);
        }

        [Test]
        [Category("Unit")]
        public void UserMfaSubscriptionAndUnsuscriptionTest_VerifiesThatTheSubscriptionsAreAnabledThenDisabledAsExpected_VerifiesThroughReturnedValue()
        {
            string userName = "NewUser";
            User user = new User(userName, "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");

            Tuple<string, string, bool> loginSubscription = new Tuple<string, string, bool>("LOG", "Login", true);
            Tuple<string, string, bool> depositSubscription = new Tuple<string, string, bool>("DEP", "Deposit", true);
            Tuple<string, string, bool> withdrawSubscription = new Tuple<string, string, bool>("WD", "Withdraw", true);
            Tuple<string, string, bool> placeOrderSubscription = new Tuple<string, string, bool>("PO", "PlaceOrder", true);
            Tuple<string, string, bool> cancelOrderSubscription = new Tuple<string, string, bool>("CO", "CancelOrder", true);
            IList<Tuple<string, string, bool>> subscriptionsList = new List<Tuple<string, string, bool>>();
            subscriptionsList.Add(loginSubscription);
            subscriptionsList.Add(depositSubscription);
            subscriptionsList.Add(withdrawSubscription);
            subscriptionsList.Add(placeOrderSubscription);
            subscriptionsList.Add(cancelOrderSubscription);
            user.AssignMfaSubscriptions(subscriptionsList);

            Assert.NotNull(user);
            bool mfaSubscription1 = user.CheckMfaSubscriptions(subscriptionsList[0].Item2);
            Assert.IsTrue(mfaSubscription1);
            bool mfaSubscription2 = user.CheckMfaSubscriptions(subscriptionsList[1].Item2);
            Assert.IsTrue(mfaSubscription2);
            bool mfaSubscription3 = user.CheckMfaSubscriptions(subscriptionsList[2].Item2);
            Assert.IsTrue(mfaSubscription3);
            bool mfaSubscription4 = user.CheckMfaSubscriptions(subscriptionsList[3].Item2);
            Assert.IsTrue(mfaSubscription4);
            bool mfaSubscription5 = user.CheckMfaSubscriptions(subscriptionsList[4].Item2);
            Assert.IsTrue(mfaSubscription5);

            subscriptionsList = new List<Tuple<string, string, bool>>();
            loginSubscription = new Tuple<string, string, bool>("LOG", "Login", true);
            depositSubscription = new Tuple<string, string, bool>("DEP", "Deposit", false);
            withdrawSubscription = new Tuple<string, string, bool>("WD", "Withdraw", true);
            placeOrderSubscription = new Tuple<string, string, bool>("PO", "PlaceOrder", false);
            cancelOrderSubscription = new Tuple<string, string, bool>("CO", "CancelOrder", true);
            subscriptionsList.Add(loginSubscription);
            subscriptionsList.Add(depositSubscription);
            subscriptionsList.Add(withdrawSubscription);
            subscriptionsList.Add(placeOrderSubscription);
            subscriptionsList.Add(cancelOrderSubscription);
            user.AssignMfaSubscriptions(subscriptionsList);
            user.AssignMfaSubscriptions(subscriptionsList);

            Assert.NotNull(user);
            mfaSubscription1 = user.CheckMfaSubscriptions(loginSubscription.Item2);
            Assert.IsTrue(mfaSubscription1);
            mfaSubscription2 = user.CheckMfaSubscriptions(depositSubscription.Item2);
            Assert.IsFalse(mfaSubscription2);
            mfaSubscription3 = user.CheckMfaSubscriptions(withdrawSubscription.Item2);
            Assert.IsTrue(mfaSubscription3);
            mfaSubscription4 = user.CheckMfaSubscriptions(placeOrderSubscription.Item2);
            Assert.IsFalse(mfaSubscription4);
            mfaSubscription5 = user.CheckMfaSubscriptions(cancelOrderSubscription.Item2);
            Assert.IsTrue(mfaSubscription5);
        }

        [Test]
        [Category("Unit")]
        public void AssignTfaModeTest_ChecksIfAssignmentOfEMailOrSmsTfaIsDoneAsExpected_VerifiesThroughUserValues()
        {
            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");

            user.SubscribeToEmailMfa();

            Assert.IsTrue(user.IsEmailMfaEnabled().Item1);
            Assert.AreEqual(email, user.Email);

            user.SubscribeToSmsMfa();

            Assert.IsFalse(user.IsEmailMfaEnabled().Item1);
            Assert.AreEqual(phoneNumber, user.PhoneNumber);
        }
    }
}
