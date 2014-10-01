using System.Threading;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices;
using NUnit.Framework;
using Spring.Context.Support;
using Twilio;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    /// <summary>
    /// SMS Service Integartion Tests
    /// </summary>
    [TestFixture]
    public class MfaSmsServiceIntegartionTests
    {
        [Test]
        public void InitializationTest_TestsThatTheServiceIsInitializedUsingSpringDi_VerifiesThroughInstanceVariable()
        {
            IMfaCodeSenderService smsService = (IMfaCodeSenderService)ContextRegistry.GetContext()["MfaSmsService"];
            Assert.IsNotNull(smsService);
        }

        [Test]
        public void SmsSendingService_VerifiesThatTheServiceSendsSmsAsExpected_VerifiesByTheReturnedValue()
        {
            MfaSmsService codeSenderService = new MfaSmsService();

            string phoneNumber = "+923325329974";
            string tfaCode = "TFA Code";
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Message receivedMessage = null;
            bool eventFired = false;
            codeSenderService.SmsCallback += delegate(Message message)
                                                 {
                                                     eventFired = true;
                                                     receivedMessage = message;
                                                     manualResetEvent.Set();
                                                 };
            bool sendCodeResponse = codeSenderService.SendCode(phoneNumber, tfaCode);
            manualResetEvent.WaitOne();
            Assert.IsTrue(eventFired);
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(phoneNumber, receivedMessage.To);
            Assert.AreEqual(tfaCode, receivedMessage.Body);
            Assert.IsTrue(sendCodeResponse);
        }
    }
}
