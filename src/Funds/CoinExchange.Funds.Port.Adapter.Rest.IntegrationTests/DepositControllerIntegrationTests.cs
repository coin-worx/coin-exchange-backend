using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    class DepositControllerIntegrationTests
    {
        [Test]
        public void DepositControllerInitializationTest_ChecksIfTheControllerInitializesAsExpected_VerifiesThroughInstance()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            Assert.IsNotNull(depositController);
        }
    }
}
