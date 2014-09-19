using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.DepositServices;
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Infrastructure.Services;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Application.IntegrationTests
{
    [TestFixture]
    class DepositApplicationServiceIntegrationTests
    {
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void Teardown()
        {
            _databaseUtility.Create();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DepositAddressCreationFailedTest_TestsIfDepositAddressIsActuallyNotCreatedIfTier1IsNotVerified_VerfiesThroughExceptionRaised()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];            
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            depositApplicationService.GenarateNewAddress(new GenerateNewAddressCommand(accountId.Value, currency.Name));
        }

        [Test]
        public void DepositAddressCreationSuccessTest_TestsIfDepositAddressIsActuallyCreatedIfTier1IsVerified_VerfiesThroughReturnValueAndDatabaseQuery()
        {
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            DepositAddressRepresentation addressResponse = depositApplicationService.GenarateNewAddress(new GenerateNewAddressCommand(accountId.Value, currency.Name));
            Assert.IsNotNull(addressResponse);
            Assert.AreEqual(AddressStatus.New.ToString(), addressResponse.Status);

            List<DepositAddress> depositAddresses = depositAddressRepository.GetDepositAddressByAccountIdAndCurrency(accountId, currency.Name);
            Assert.AreEqual(1, depositAddresses.Count);
            Assert.AreEqual(accountId.Value, depositAddresses.Single().AccountId.Value);
            Assert.AreEqual(currency.Name, depositAddresses.Single().Currency.Name);
            Assert.AreEqual(AddressStatus.New, depositAddresses.Single().Status);
        }

        [Test]
        public void DepositArrivedFailedTest_TestsIfTheOperationIsAbortedWhenTheTierLevelIsNotHighEnough_VerifiesThroughQueryingDatabaseIsntances()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];            
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            Assert.IsNotNull(tierLevelRetrievalService);
            tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel0);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            decimal amount = 1.02m;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now,
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNull(deposit);

            // Confirm that the balance for the user has been frozen for this account
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.IsTrue(balance.IsFrozen);
        }

        [Test]
        public void Deposit1ArrivedTest_TestsIfTheOperationProceedsAsExpectedWHenANewDepositArrives_VerifiesThroughQueryingDatabaseIsntances()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            decimal amount = 1.02m;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, 
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string,string,decimal,string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Thread.Sleep(2000);
            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(deposit.Amount, amount);
            Assert.AreEqual(deposit.Currency.Name, currency.Name);
            Assert.IsTrue(deposit.Currency.IsCryptoCurrency);
            Assert.AreEqual(deposit.TransactionId.Value, transactionId.Value);
            Assert.AreEqual(deposit.BitcoinAddress.Value, bitcoinAddress.Value);
            Assert.AreEqual(deposit.Status, TransactionStatus.Pending);

            depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(depositAddress.Status, AddressStatus.Used);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);
        }

        [Test]
        public void DepositConfirmedTest_TestsIfTheOperationProceedsAsExpectedWhenADepositIsConfirmed_VerifiesThroughQueryingDatabaseIsntances()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            ILedgerRepository ledgerRepository = (ILedgerRepository)ContextRegistry.GetContext()["LedgerRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            StubTierLevelRetrievalService tierLevelRetrievalService = (ITierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"] as StubTierLevelRetrievalService;

            if (tierLevelRetrievalService != null)
            {
                tierLevelRetrievalService.SetTierLevel(TierConstants.TierLevel1);
            }
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            decimal amount = 1.02m;
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now,
                accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);

            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            IList<Ledger> ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(ledgers.Count, 0);

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value, amount, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(deposit.Amount, amount);
            Assert.AreEqual(deposit.Currency.Name, currency.Name);
            Assert.IsTrue(deposit.Currency.IsCryptoCurrency);
            Assert.AreEqual(deposit.TransactionId.Value, transactionId.Value);
            Assert.AreEqual(deposit.BitcoinAddress.Value, bitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Pending, deposit.Status);

            depositAddress = depositAddressRepository.GetDepositAddressByAddress(bitcoinAddress);
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(AddressStatus.Used, depositAddress.Status);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(0, ledgers.Count);

            depositApplicationService.OnDepositConfirmed(transactionId.Value, 7);

            deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(deposit.Status, TransactionStatus.Confirmed);

            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(amount, balance.AvailableBalance);
            Assert.AreEqual(amount, balance.CurrentBalance);
            Assert.AreEqual(0, balance.PendingBalance);

            ledgers = ledgerRepository.GetLedgerByAccountIdAndCurrency(currency.Name, accountId);
            Assert.AreEqual(1, ledgers.Count);
            var ledger = ledgers.SingleOrDefault();
            Assert.IsNotNull(ledger);
            Assert.AreEqual(LedgerType.Deposit, ledger.LedgerType);
            Assert.AreEqual(accountId.Value, ledger.AccountId.Value);
            Assert.AreEqual(amount, ledger.Amount);
            Assert.AreEqual(deposit.DepositId, ledger.DepositId);
            Assert.AreEqual(0, ledger.Fee);
        }

        [Test]
        public void DepositArrivedFailTest_VerifiesThatNewDepositWithAMountGreaterThanLimitsIsSuspendedAndAccountBalanceFrozen_VerifiesThroughDatabaseQuery()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress =  new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            // Provide the amount which is greater than the daily limit
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value,
                depositLimit.DailyLimit + 0.001M, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(TransactionStatus.Frozen, deposit.Status);
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.IsTrue(balance.IsFrozen);
        }

        [Test]
        public void DepositArrivedSuccessfulTest_ChecksThatNewDepositInstanceIsCreatedAsExpectedWhenANewTrnasactionComesIn_VerifiesThroughDatabaseQuery()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();
            // Provide the amount which is greater than the daily limit
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value,
                depositLimit.DailyLimit - 0.01M, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, deposit.Amount);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Pending, deposit.Status);
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);
        }

        [Test]
        public void DepositArrivedSAndConfirmedTest_ChecksThatANewDepositInstanceIsCreatedAndConfirmedAsExpected_VerifiesThroughDatabaseQuery()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];
            IBalanceRepository balanceRepository = (IBalanceRepository)ContextRegistry.GetContext()["BalanceRepository"];
            IDepositRepository depositRepository = (IDepositRepository)ContextRegistry.GetContext()["DepositRepository"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];

            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            BitcoinAddress bitcoinAddress = new BitcoinAddress("bitcoinaddress1");
            TransactionId transactionId = new TransactionId("transactionid1");
            string category = BitcoinConstants.ReceiveCategory;

            DepositAddress depositAddress = new DepositAddress(currency, bitcoinAddress, AddressStatus.New, DateTime.Now, accountId);
            fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", "Default");
            Assert.IsNotNull(depositLimit, "DepositLimit used initially to compare later");

            List<Tuple<string, string, decimal, string>> transactionsList = new List<Tuple<string, string, decimal, string>>();

            // Provide the amount which is greater than the daily limit
            transactionsList.Add(new Tuple<string, string, decimal, string>(bitcoinAddress.Value, transactionId.Value,
                depositLimit.DailyLimit - 0.01M, category));
            depositApplicationService.OnDepositArrival(currency.Name, transactionsList);

            // Deposit Trnasction first arrival
            Deposit deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, deposit.Amount);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Pending, deposit.Status);
            Balance balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNull(balance);

            // Deposit Confirmation
            depositApplicationService.OnDepositConfirmed(transactionId.Value, 7);
            deposit = depositRepository.GetDepositByTransactionId(transactionId);
            Assert.IsNotNull(deposit);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, deposit.Amount);
            Assert.AreEqual(transactionId.Value, deposit.TransactionId.Value);
            Assert.AreEqual(accountId.Value, deposit.AccountId.Value);
            Assert.AreEqual(bitcoinAddress.Value, deposit.BitcoinAddress.Value);
            Assert.AreEqual(TransactionStatus.Confirmed, deposit.Status);

            // Balance instance now will have been created
            balance = balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            Assert.IsNotNull(balance);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, balance.AvailableBalance);
            Assert.AreEqual(depositLimit.DailyLimit - 0.01M, balance.CurrentBalance);
            Assert.AreEqual(0, balance.PendingBalance);
            Assert.IsFalse(balance.IsFrozen);
        }

        [Test]
        public void AssignDepositLimitsTest_ChecksThatDepositLimitsAreAssignedProperlyWhenLevel1IsVerified_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];

            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            DepositLimitThresholdsRepresentation depositLimitThresholds = depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
            Assert.IsNotNull(depositLimitThresholds);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, depositLimitThresholds.MonthlyLimit);
            Assert.AreEqual(0, depositLimitThresholds.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.MonthlyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.CurrentBalance);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.MaximumDeposit);
        }

        [Test]
        public void AssignDepositLimitsTest_ChecksThatDepositLimitsAreAssignedProperlyWhenLevel1IsVerifiedAndBalanceIsAlreadyPresent_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            IFundsPersistenceRepository fundsPersistenceRepository = (IFundsPersistenceRepository)ContextRegistry.GetContext()["FundsPersistenceRepository"];

            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 1", LimitsCurrency.Default.ToString());
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);
            decimal balanceAmount = 100;
            Balance balance = new Balance(currency, accountId, balanceAmount, balanceAmount);
            fundsPersistenceRepository.SaveOrUpdate(balance);

            DepositLimitThresholdsRepresentation depositLimitThresholds = depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
            Assert.IsNotNull(depositLimitThresholds);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, depositLimitThresholds.MonthlyLimit);
            Assert.AreEqual(0, depositLimitThresholds.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.MonthlyLimitUsed);
            Assert.AreEqual(balanceAmount, depositLimitThresholds.CurrentBalance);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.MaximumDeposit);
        }

        [Test]
        public void AssignDepositLimitsTest_ChecksThatDepositLimitsAreAssignedProperlyWhenLevel1IsNotVerifiedAndBalanceIsAlreadyPresent_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrieval = (StubTierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];

            tierLevelRetrieval.SetTierLevel("Tier 0");
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 0", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(depositLimit);
            Assert.AreEqual(0, depositLimit.DailyLimit);
            Assert.AreEqual(0, depositLimit.MonthlyLimit);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            DepositLimitThresholdsRepresentation depositLimitThresholds = depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
            Assert.IsNotNull(depositLimitThresholds);
            Assert.AreEqual(depositLimit.DailyLimit, depositLimitThresholds.DailyLimit);
            Assert.AreEqual(depositLimit.MonthlyLimit, depositLimitThresholds.MonthlyLimit);
            Assert.AreEqual(0, depositLimitThresholds.DailyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.MonthlyLimitUsed);
            Assert.AreEqual(0, depositLimitThresholds.CurrentBalance);
            Assert.AreEqual(0, depositLimitThresholds.MaximumDeposit);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AssignDepositLimitsTest_ChecksThatDepositLimitsAreNotAssignedWhenNoDepositLimitIsFound_VerifiesThroughReturnedValues()
        {
            IDepositApplicationService depositApplicationService = (IDepositApplicationService)ContextRegistry.GetContext()["DepositApplicationService"];
            IDepositLimitRepository depositLimitRepository = (IDepositLimitRepository)ContextRegistry.GetContext()["DepositLimitRepository"];
            StubTierLevelRetrievalService tierLevelRetrieval = (StubTierLevelRetrievalService)ContextRegistry.GetContext()["TierLevelRetrievalService"];

            // Specify invalid tier level
            tierLevelRetrieval.SetTierLevel("Tier 6");
            DepositLimit depositLimit = depositLimitRepository.GetLimitByTierLevelAndCurrency("Tier 0", LimitsCurrency.Default.ToString());
            Assert.IsNotNull(depositLimit);
            Assert.AreEqual(0, depositLimit.DailyLimit);
            Assert.AreEqual(0, depositLimit.MonthlyLimit);
            AccountId accountId = new AccountId(123);
            Currency currency = new Currency("BTC", true);

            // Exception occurs here because there is no such level as Tier 6
            depositApplicationService.GetThresholdLimits(accountId.Value, currency.Name);
        }
    }
}
