using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastructure.Services.IntegrationTests
{
    [TestFixture]
    class LitecoinClientServiceIntegrationTests
    {
        [Test]
        public void CheckNewTransactionTest_ChecksTheServiceChecksForNewTransactionsAndAddsThemToInternalListProperly_VerifiesThroughVariablesValues()
        {
            ICoinClientService litecoinClientService = (ICoinClientService)ContextRegistry.GetContext()["LitecoinClientService"];
            litecoinClientService.CheckNewTransactions();
        }
    }
}
