/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.MfaServices;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    class MfaAuthorizationServiceTests
    {
        [Test]
        [Category("Unit")]
        public void MfaAuthorizationNotSubscribedTest_TestsIfResultIsTrueWhenMfaIsNotSubscribedForAnyAction_VerifiesByReturnValue()
        {
            MockPersistenceRepository mockPersistenceRepository = new MockPersistenceRepository(true);
            MockUserRepository mockUserRepository = new MockUserRepository();            
            MockMfaEmailService mockMfaEmailService = new MockMfaEmailService();
            MockSmsService mockSmsService = new MockSmsService();
            MockMfaCodeGenerationService mockMfaCodeGenerationService = new MockMfaCodeGenerationService();

            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");

            MockSecurityKeysRepository mockSecurityKeysRepository = new MockSecurityKeysRepository();
            string apiKey = "apikey123";
            // Add Api Key to mock implementation
            mockSecurityKeysRepository.AddSecurityKeysPair(new SecurityKeysPair(user.Id, apiKey, "secretkey123", true, "Desc"));
            // Add user to mock implementation
            mockUserRepository.AddUser(user);

            MfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(mockPersistenceRepository,
                mockUserRepository, mockSecurityKeysRepository, mockSmsService, mockMfaEmailService, mockMfaCodeGenerationService);
            Tuple<bool, string> authorizeAccess = mfaAuthorizationService.AuthorizeAccess(apiKey, "Login", null);
            Assert.IsTrue(authorizeAccess.Item1);
        }

        [Test]
        [Category("Unit")]
        public void MfaAuthorizationSubscribedTest_ChecksThatTheResponseIsTrueWhenMfaCodeMatches_VerifiesByReturnValue()
        {
            // The first time of authorization, the user des not have the MFA code present, but the second time it is present
            // as it has been sent to the user
            MockPersistenceRepository mockPersistenceRepository = new MockPersistenceRepository(false);
            MockUserRepository mockUserRepository = new MockUserRepository();
            MockMfaEmailService mockMfaEmailService = new MockMfaEmailService();
            MockSmsService mockSmsService = new MockSmsService();
            MockMfaCodeGenerationService mockMfaCodeGenerationService = new MockMfaCodeGenerationService();

            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");

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

            MockSecurityKeysRepository mockSecurityKeysRepository = new MockSecurityKeysRepository();
            string apiKey = "apikey123";
            // Add Api Key to mock implementation
            mockSecurityKeysRepository.AddSecurityKeysPair(new SecurityKeysPair(user.Id, apiKey, "secretkey123", true, "Desc"));
            // Add user to mock implementation
            mockUserRepository.AddUser(user);

            MfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(mockPersistenceRepository,
                mockUserRepository, mockSecurityKeysRepository, mockSmsService, mockMfaEmailService, mockMfaCodeGenerationService);

            // Login MFA
            Tuple<bool, string> authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, loginSubscription.Item2, null);
            Assert.IsFalse(authorizeAccess1.Item1);
            // This time the code should be assigned to the user, so verify that
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, loginSubscription.Item2, user.MfaCode);
            Assert.IsTrue(authorizeAccess1.Item1);
            // MFA Code in the user should be null
            Assert.IsNull(user.MfaCode);

            // Deposit MFA
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, depositSubscription.Item2, null);
            Assert.IsFalse(authorizeAccess1.Item1);
            // This time the code should be assigned to the user, so verify that
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, depositSubscription.Item2, user.MfaCode);
            Assert.IsTrue(authorizeAccess1.Item1);
            // MFA Code in the user should be null
            Assert.IsNull(user.MfaCode);

            // Withdraw MFA
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, withdrawSubscription.Item2, null);
            Assert.IsFalse(authorizeAccess1.Item1);
            // This time the code should be assigned to the user, so verify that
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, withdrawSubscription.Item2, user.MfaCode);
            Assert.IsTrue(authorizeAccess1.Item1);
            // MFA Code in the user should be null
            Assert.IsNull(user.MfaCode);

            // Place Order MFA
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, placeOrderSubscription.Item2, null);
            Assert.IsFalse(authorizeAccess1.Item1);
            // This time the code should be assigned to the user, so verify that
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, placeOrderSubscription.Item2, user.MfaCode);
            Assert.IsTrue(authorizeAccess1.Item1);
            // MFA Code in the user should be null
            Assert.IsNull(user.MfaCode);
        }

        [Test]
        [Category("Unit")]
        public void MfaAuthorizationSubscribedFailsTest_ChecksThatResponseIsFalseWhenMfaCodeIsNotPresent_VerifiesByReturnValue()
        {
            // The first time of authorization, the user des not have the MFA code present, but the second time it is present
            // as it has been sent to the user
            MockPersistenceRepository mockPersistenceRepository = new MockPersistenceRepository(false);
            MockUserRepository mockUserRepository = new MockUserRepository();
            MockMfaEmailService mockMfaEmailService = new MockMfaEmailService();
            MockSmsService mockSmsService = new MockSmsService();
            MockMfaCodeGenerationService mockMfaCodeGenerationService = new MockMfaCodeGenerationService();

            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");

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

            MockSecurityKeysRepository mockSecurityKeysRepository = new MockSecurityKeysRepository();
            string apiKey = "apikey123";
            // Add Api Key to mock implementation
            mockSecurityKeysRepository.AddSecurityKeysPair(new SecurityKeysPair(user.Id, apiKey, "secretkey123", true, "Desc"));
            // Add user to mock implementation
            mockUserRepository.AddUser(user);

            MfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(mockPersistenceRepository,
                mockUserRepository, mockSecurityKeysRepository, mockSmsService, mockMfaEmailService, mockMfaCodeGenerationService);

            // Login MFA
            Tuple<bool, string> authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, loginSubscription.Item2, null);
            Assert.IsFalse(authorizeAccess1.Item1);
        }

        [Test]
        [Category("Unit")]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void MfaAuthorizationSubscribedFailtTest_ChecksThatExceptionIsRaisedWhenMfaCodesDontMatch_VerifiesByReturnValue()
        {
            MockPersistenceRepository mockPersistenceRepository = new MockPersistenceRepository(false);
            MockUserRepository mockUserRepository = new MockUserRepository();
            MockMfaEmailService mockMfaEmailService = new MockMfaEmailService();
            MockSmsService mockSmsService = new MockSmsService();
            MockMfaCodeGenerationService mockMfaCodeGenerationService = new MockMfaCodeGenerationService();

            string userName = "NewUser";
            string phoneNumber = "2233344";
            string email = "user88@gmail.com";
            User user = new User(userName, "asdf", "12345", "xyz", email, Language.English, TimeZone.CurrentTimeZone,
                new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", phoneNumber, "1234");

            Tuple<string, string, bool> loginSubscription = new Tuple<string, string, bool>("LOG", "Login", true);
            IList<Tuple<string, string, bool>> subscriptionsList = new List<Tuple<string, string, bool>>();
            subscriptionsList.Add(loginSubscription);
            user.AssignMfaSubscriptions(subscriptionsList);

            MockSecurityKeysRepository mockSecurityKeysRepository = new MockSecurityKeysRepository();
            string apiKey = "apikey123";
            // Add Api Key to mock implementation
            mockSecurityKeysRepository.AddSecurityKeysPair(new SecurityKeysPair(user.Id, apiKey, "secretkey123", true, "Desc"));
            // Add user to mock implementation
            mockUserRepository.AddUser(user);

            MfaAuthorizationService mfaAuthorizationService = new MfaAuthorizationService(mockPersistenceRepository,
                mockUserRepository, mockSecurityKeysRepository, mockSmsService, mockMfaEmailService, mockMfaCodeGenerationService);

            // Login MFA
            Tuple<bool, string> authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, loginSubscription.Item2, null);
            Assert.IsFalse(authorizeAccess1.Item1);
            // This time the code should be assigned to the user, so verify that
            authorizeAccess1 = mfaAuthorizationService.AuthorizeAccess(apiKey, loginSubscription.Item2, user.MfaCode + "1");
            Assert.IsFalse(authorizeAccess1.Item1);
        }
    }
}
