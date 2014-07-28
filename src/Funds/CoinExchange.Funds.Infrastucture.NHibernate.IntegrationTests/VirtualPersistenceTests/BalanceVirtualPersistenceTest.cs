using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using NUnit.Framework;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests.VirtualPersistenceTests
{
    [TestFixture]
    class BalanceVirtualPersistenceTest : AbstractConfiguration
    {
        private IFundsPersistenceRepository _persistanceRepository;
        private IBalanceRepository _balanceRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IBalanceRepository DepositLimitRepository
        {
            set { _balanceRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveDepositAddressesAndRetreiveByAccountIdTest_SavesObjectsToDatabase_ChecksIfTheyAreAsExpected()
        {
            Balance balance = new Balance(new Currency("LTC", true), new AccountId("123"), 4000, 5000);

            _persistanceRepository.SaveOrUpdate(balance);

            Balance retrievedDepositAddressList = _balanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.IsNotNull(retrievedDepositAddressList);

            Assert.AreEqual(balance.AvailableBalance, retrievedDepositAddressList.AvailableBalance);
            Assert.AreEqual(balance.CurrentBalance, retrievedDepositAddressList.CurrentBalance);
            Assert.AreEqual(balance.PendingBalance, retrievedDepositAddressList.PendingBalance);
        }

        [Test]
        public void SavePendingTransactionsTest_SavesObjectsToPendingTransactionsList_ChecksIfTheyAreAsExpected()
        {
            Balance balance = new Balance(new Currency("LTC", true), new AccountId("123"), 5000, 5000);
            bool addPendingTransaction = balance.AddPendingTransaction("withdrawid123", PendingTransactionType.Withdraw, -500);
            Assert.IsTrue(addPendingTransaction);

            _persistanceRepository.SaveOrUpdate(balance);

            Balance retrievedDepositAddressList = _balanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.IsNotNull(retrievedDepositAddressList);

            Assert.AreEqual(4500, retrievedDepositAddressList.AvailableBalance);
            Assert.AreEqual(5000, retrievedDepositAddressList.CurrentBalance);
            Assert.AreEqual(500, retrievedDepositAddressList.PendingBalance);

            balance.ConfirmPendingTransaction("withdrawid123", PendingTransactionType.Withdraw, -500);

            _persistanceRepository.SaveOrUpdate(balance);
            Balance retreivedBalance = _balanceRepository.GetBalanceByCurrencyAndAccountId(balance.Currency, balance.AccountId);
            Assert.IsNotNull(retreivedBalance);
            Assert.AreEqual(4500, retreivedBalance.AvailableBalance);
            Assert.AreEqual(4500, retreivedBalance.CurrentBalance);
            Assert.AreEqual(0, retreivedBalance.PendingBalance);
        }
    }
}
