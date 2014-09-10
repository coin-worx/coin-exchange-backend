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
    class WithdrawControllerIntegrationTests
    {
        [Test]
        public void WithdrawControllerInitializationTest_ChecksIfTheControllerInitializesAsExpected_VerifiesThroughInstance()
        {
            WithdrawController withdrawController = (WithdrawController)ContextRegistry.GetContext()["WithdrawController"];
            Assert.IsNotNull(withdrawController);
        }
    }
}
