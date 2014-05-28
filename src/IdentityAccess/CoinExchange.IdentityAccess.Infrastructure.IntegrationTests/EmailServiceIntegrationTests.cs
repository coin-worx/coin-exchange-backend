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
            bool sendMail = emailService.SendMail("waqas.syed@hotmail.com", "BlancRock", EmailContents.GetActivationKeyMessage("123456787654"));
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendMail);
        }

        [Test]
        public void PostSignUpEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendPostSignUpEmail = emailService.SendPostSignUpEmail("waqas.syed@hotmail.com", "ActivationKey:BlancRock");
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendPostSignUpEmail);
        }

        [Test]
        public void ForgotUsernameEmailSendingTest()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            bool sendForgotUsernameEmail = emailService.SendForgotUsernameEmail("waqas.syed@hotmail.com", "ActivationKey:BlancRock");
            manualResetEvent.WaitOne(5000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }
    }
}
