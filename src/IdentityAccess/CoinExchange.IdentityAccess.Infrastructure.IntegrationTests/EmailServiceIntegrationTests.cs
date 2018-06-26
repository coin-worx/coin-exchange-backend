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
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    class EmailServiceIntegrationTests
    {
        [Test]
        public void EmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendMail = emailService.SendMail("waqasshah047@gmail.com", "BlancRock", EmailContents.GetActivationKeyMessage(
                "Bruce Wayne", "123456787654"), true);
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendMail);
        }

        [Test]
        [Category("Integration")]
        public void PostSignUpEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendPostSignUpEmail = emailService.SendPostSignUpEmail("waqasshah047@gmail.com", "Bruce Wayne", "123456789", true);
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendPostSignUpEmail);
        }

        [Test]
        [Category("Integration")]
        public void ForgotUsernameEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendForgotUsernameEmail("waqasshah047@gmail.com",
                "Bruce Wayne", true);
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }

        [Test]
        [Category("Integration")]
        public void ReActivationEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendReactivaitonNotificationEmail("waqasshah047@gmail.com", 
                "Tony Stark", true);
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }

        [Test]
        [Category("Integration")]
        public void CancelActivationEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendCancelActivationEmail("waqasshah047@gmail.com",
                "Peter Parker", true);
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }

        [Test]
        [Category("Integration")]
        public void MultipleEmailAsyncCheckTest_ChecksWhetherTheAsyncOperationsOfSendingEmailsBackToBackDontCauseException_SendsEmailSuccessfully()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            bool sendForgotUsernameEmail = emailService.SendForgotUsernameEmail("waqasshah047@gmail.com", "Waqas Shah", true);
            Assert.IsTrue(sendForgotUsernameEmail);

            bool sendForgotUsernameEmail2 = emailService.SendForgotUsernameEmail("waqasshah047@gmail.com", "Waqas Shah", true);
            Assert.IsTrue(sendForgotUsernameEmail2);

            bool activationEmail = emailService.SendCancelActivationEmail("waqasshah047@gmail.com", "Waqas Shah", true);
            Assert.IsTrue(activationEmail);

            bool passwordChangedEmail = emailService.SendPasswordChangedEmail("waqasshah047@gmail.com", "Waqas Shah", true);
            Assert.IsTrue(passwordChangedEmail);

            bool welcomeEmail = emailService.SendWelcomeEmail("waqasshah047@gmail.com", "Waqas Shah", true);
            Assert.IsTrue(welcomeEmail);

            bool postSignUpEmail = emailService.SendPostSignUpEmail("waqasshah047@gmail.com", "Waqas Shah", "123", true);
            Assert.IsTrue(postSignUpEmail);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(6000);
        }
    }
}
