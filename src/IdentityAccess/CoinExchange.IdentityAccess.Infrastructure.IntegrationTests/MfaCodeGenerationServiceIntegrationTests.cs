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
    class MfaCodeGenerationServiceIntegrationTests
    {
        [Test]
        public void InitializationTest_ChecksIftheInstanceOfTheServiceIsInitializedAsExpectedUsingSpring_VerifiesThroughTheInstanceVariable()
        {
            IMfaCodeGenerationService smsService = (IMfaCodeGenerationService)ContextRegistry.GetContext()["MfaCodeGenerationService"];
            Assert.IsNotNull(smsService);
        }

        [Test]
        public void MfaCodeUniquenessTest_TestsTheMfaCodeGenerationServiceToGenerateUniqueNumbers_VerifiesByComparingOlderCreatedValues()
        {
            MfaCodeGenerationService mfaCodeGenerationService = new MfaCodeGenerationService();

            List<string> storedCodes = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                // We keep a distance of 10 seconds between two consecutive number generations, as the random generator is seeded
                // from the system clock
                Thread.Sleep(10);
                string theCode = mfaCodeGenerationService.GenerateCode();
                if (storedCodes.Contains(theCode))
                {
                    throw new InvalidOperationException("Code already present: Codes Count = " + storedCodes.Count);
                }
                storedCodes.Add(theCode);
            }
        }
    }
}
