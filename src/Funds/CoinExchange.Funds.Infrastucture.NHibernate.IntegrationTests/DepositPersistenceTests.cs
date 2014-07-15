using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Infrastucture.NHibernate.IntegrationTests
{
    [TestFixture]
    class DepositPersistenceTests //: AbstractConfiguration
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [TearDown]
        public void Teardown()
        {
            
        }

        private IFundsPersistenceRepository _persistanceRepository;
        private IDepositRepository _depositRepository;

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IDepositRepository DepositRepository
        {
            set { _depositRepository = value; }
        }

        /// <summary>
        /// Spring's Injection using FundsAbstractConfiguration class
        /// </summary>
        public IFundsPersistenceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }

        [Test]
        public void SaveSepositAndRetreiveTest()
        {
            _depositRepository = ContextRegistry.GetContext()["DepositRepository"] as IDepositRepository;
            _persistanceRepository = ContextRegistry.GetContext()["FundsPersistenceRepository"] as IFundsPersistenceRepository;
            Deposit deposit = new Deposit(new Currency("LTC", true), "1234", DateTime.Now, "New", 2000, 0.005, TransactionStatus.Pending, 
                new AccountId("123"));

            _persistanceRepository.SaveOrUpdate(deposit);

            Deposit retrievedDeposit = _depositRepository.GetDepositByDepositId("1234");
            Assert.IsNotNull(retrievedDeposit);
            retrievedDeposit.Amount = 777;
            _persistanceRepository.SaveOrUpdate(retrievedDeposit);

            retrievedDeposit = _depositRepository.GetDepositByDepositId("1234");
            Assert.AreEqual(deposit.Currency.Name, retrievedDeposit.Currency.Name);
            Assert.AreEqual(deposit.DepositId, retrievedDeposit.DepositId);
            Assert.AreEqual(deposit.Type, retrievedDeposit.Type);
            Assert.AreEqual(777, retrievedDeposit.Amount);
            Assert.AreEqual(deposit.Fee, retrievedDeposit.Fee);
            Assert.AreEqual(deposit.Status, retrievedDeposit.Status);
            Assert.AreEqual(deposit.AccountId.Value, retrievedDeposit.AccountId.Value);
        }
    }
}
