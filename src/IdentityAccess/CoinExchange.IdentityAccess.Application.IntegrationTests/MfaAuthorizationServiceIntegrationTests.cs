using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.IntegrationTests
{
    [TestFixture]
    internal class MfaAuthorizationServiceIntegrationTests
    {
        [Test]
        [Category("Integration")]
        public void ServiceInitilizationTest_ChecksIfTheServiceInstanceIsInitializedThroughSpringAsExpected_VerifiesThroughVariableValue()
        {
            IMfaAuthorizationService mfaAuthorizationService = (IMfaAuthorizationService) ContextRegistry.GetContext()["MfaAuthorizationService"];
            Assert.IsNotNull(mfaAuthorizationService);
        }
    }
}