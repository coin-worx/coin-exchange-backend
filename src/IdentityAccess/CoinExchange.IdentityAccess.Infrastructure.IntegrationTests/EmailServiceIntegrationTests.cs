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
            emailService.SendMail("waqas.syed@hotmail.com", "BlancRock",
                                  EmailContents.GetActivationKeyMessage("123456787654"));
            manualResetEvent.WaitOne(5000);
        }
    }
}
