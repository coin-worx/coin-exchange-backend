using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.Tests
{
    [TestFixture]
    public class ActivationKeyGenerationServiceTests
    {
        [Test]
        public void GenerateNewKeyTest_TestsIfNewKeyIsGenerated_VerifiesThroughReturnedValue()
        {
            ActivationKeyGenerationService activationKeyService = new ActivationKeyGenerationService();
            string activationKey = activationKeyService.GenerateNewActivationKey();
            Assert.IsNotNull(activationKey);
        }
    }
}
