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
            bool sendMail = emailService.SendMail("waqasshah047@gmail.com", "BlancRock", EmailContents.GetActivationKeyMessage(
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
            bool sendPostSignUpEmail = emailService.SendPostSignUpEmail("waqasshah047@gmail.com", "Bruce Wayne", "123456789");
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
            bool sendForgotUsernameEmail = emailService.SendReactivaitonNotificationEmail("waqasshah047@gmail.com", 
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
            bool sendForgotUsernameEmail = emailService.SendCancelActivationEmail("waqasshah047@gmail.com",
                "Peter Parker");
            manualResetEvent.WaitOne(6000);

            Assert.IsTrue(sendForgotUsernameEmail);
        }

        [Test]
        [Category("Integration")]
        public void MultipleEmailAsyncCheckTest_ChecksWhetherTheAsyncOperationsOfSendingEmailsBackToBackDontCauseException_SendsEmailSuccessfully()
        {
            IEmailService emailService = (IEmailService)ContextRegistry.GetContext()["EmailService"];
            bool sendForgotUsernameEmail = emailService.SendForgotUsernameEmail("waqasshah047@gmail.com", "Waqas Shah");
            Assert.IsTrue(sendForgotUsernameEmail);

            bool sendForgotUsernameEmail2 = emailService.SendForgotUsernameEmail("waqasshah047@gmail.com", "Waqas Shah");
            Assert.IsTrue(sendForgotUsernameEmail2);

            bool activationEmail = emailService.SendCancelActivationEmail("waqasshah047@gmail.com", "Waqas Shah");
            Assert.IsTrue(activationEmail);

            bool passwordChangedEmail = emailService.SendPasswordChangedEmail("waqasshah047@gmail.com", "Waqas Shah");
            Assert.IsTrue(passwordChangedEmail);

            bool welcomeEmail = emailService.SendWelcomeEmail("waqasshah047@gmail.com", "Waqas Shah");
            Assert.IsTrue(welcomeEmail);

            bool postSignUpEmail = emailService.SendPostSignUpEmail("waqasshah047@gmail.com", "Waqas Shah", "123");
            Assert.IsTrue(postSignUpEmail);

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(6000);
        }
    }
}
