using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    class MfaEmailServiceIntegrationTests
    {
        [Test]
        public void InitializationTest_TestsThatTheServiceIsInitializedUsingSpringDi_VerifiesThroughInstanceVariable()
        {
            IMfaCodeSenderService smsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            Assert.IsNotNull(smsService);
        }

        [Test]
        public void EmailSendingService_VerifiesThatTheServiceSendsEmailAsExpected_VerifiesByTheReturnedValue()
        {
            MfaEmailService codeSenderService = new MfaEmailService();

            string email = "crypgocoinexchange@gmail.com";
            string tfaCode = "TFA Code";
            bool sendCode = codeSenderService.SendCode(email, tfaCode);
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.WaitOne(10000);
            Assert.IsTrue(sendCode);
        }
    }
}
