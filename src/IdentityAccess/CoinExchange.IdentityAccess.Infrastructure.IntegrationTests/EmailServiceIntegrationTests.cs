using System;
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
            bool sendMail = emailService.SendMail("waqas.syed@hotmail.com", "BlancRock", EmailContents.GetActivationKeyMessage(
                "Bruce Wayne", "123456787654"));
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendMail);
        }

        [Test]
        [Category("Integration")]
        public void PostSignUpEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendPostSignUpEmail = emailService.SendPostSignUpEmail("waqas.syed@hotmail.com", "Bruce Wayne", "123456789");
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendPostSignUpEmail);
        }

        [Test]
        [Category("Integration")]
        public void ForgotUsernameEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendForgotUsernameEmail("waqas.syed@hotmail.com",
                "Bruce Wayne");
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }

        [Test]
        [Category("Integration")]
        public void ReActivationEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendReactivaitonNotificationEmail("waqas.syed@hotmail.com", 
                "Tony Stark");
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }

        [Test]
        [Category("Integration")]
        public void CancelActivationEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendCancelActivationEmail("waqas.syed@hotmail.com",
                "Peter Parker");
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }
    }
}
