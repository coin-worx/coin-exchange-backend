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
    class LedgerControllerIntegrationTests
    {
        [Test]
        public void LedgerControllerInitializationTest_ChecksIfTheControllerInitializesAsExpected_VerifiesThroughInstance()
        {
            LedgerController ledgerController = (LedgerController)ContextRegistry.GetContext()["LedgerController"];
            Assert.IsNotNull(ledgerController);
        }
    }
}
