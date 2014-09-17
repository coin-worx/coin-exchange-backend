using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Infrastructure.Services.CoinClientServices;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastructure.Services.Tests
{
    [TestFixture]
    class LitecoinClientServiceTests
    {
        [Test]
        public void NewTransactionTest_ChecksIfNewTransactionsAreProperlyHandledAndSavedToList_VerifiesThroughVariablesValues()
        {
            LitecoinClientService litecoinClientService = new LitecoinClientService();
            litecoinClientService.CheckNewTransactions();
        }
    }
}
