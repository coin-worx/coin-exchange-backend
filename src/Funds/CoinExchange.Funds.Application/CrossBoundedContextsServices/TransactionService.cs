using System;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service for making transaction resulting in Ledgers
    /// </summary>
    public class TransactionService : ITransactionService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private ILedgerIdGeneraterService _ledgerIdGeneraterService;
        private ILedgerRepository _ledgerRepository;
        private IFeeCalculationService _feeCalculationService;
        private IBalanceRepository _balanceRepository;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="fundsPersistenceRepository"> </param>
        /// <param name="ledgerIdGeneratorService"></param>
        /// <param name="ledgerRepository"> </param>
        /// <param name="feeCalculationService"> </param>
        /// <param name="balanceRepository"> </param>
        public TransactionService(IFundsPersistenceRepository fundsPersistenceRepository, 
            ILedgerIdGeneraterService ledgerIdGeneratorService, ILedgerRepository ledgerRepository, 
            IFeeCalculationService feeCalculationService, IBalanceRepository balanceRepository)
        {
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _ledgerIdGeneraterService = ledgerIdGeneratorService;
            _ledgerRepository = ledgerRepository;
            _feeCalculationService = feeCalculationService;
            _balanceRepository = balanceRepository;
        }

        /// <summary>
        /// Creates a transaction as a result of a Trade
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"> </param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>        
        /// <returns></returns>
        public bool CreateTradeTransaction(string currencyPair, double tradeVolume, double price,
            DateTime executionDateTime, string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, 
            string sellOrderId)
        {
            Tuple<Currency, Currency> separateBaseQuoteCurrencies = SeparateBaseQuoteCurrency(currencyPair);
            if (BuyOrderLedger(separateBaseQuoteCurrencies.Item1, separateBaseQuoteCurrencies.Item2, tradeVolume, price,
                           executionDateTime, buyOrderId, tradeId, buyAccountId))
            {
                return SellOrderLedger(separateBaseQuoteCurrencies.Item1, separateBaseQuoteCurrencies.Item2, tradeVolume,
                            price, executionDateTime, sellOrderId, tradeId, sellAccountId);
            }
            return false;
        }

        /// <summary>
        /// Creates two ledgers for the user who has the buy order in the trade. Ledger 1 = Base currency stats, 
        /// Ledger 2 = Quote Currency Stats
        /// </summary>
        /// <returns></returns>
        private bool BuyOrderLedger(Currency baseCurrency, Currency quoteCurrency, double volume, double price, 
            DateTime executionDateTime, string buyOrderId, string tradeId, string accountId)
        {
            // First, we take the balance of that currency
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, new AccountId(accountId));
            if (balance != null)
            {
                double currentBalance = balance.CurrentBalance;
                // Then we give details to the CreateLedgerEntry method that creates the ledger transaction
                if (CreateLedgerEntry(baseCurrency, volume, 0.000, currentBalance + volume, executionDateTime,
                                      buyOrderId, tradeId,
                                      new AccountId(accountId)))
                {
                    // Then, we get the balance for the quote currency
                    balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, new AccountId(accountId));
                    if (balance != null)
                    {
                        currentBalance = balance.CurrentBalance;
                        // Finally, we create ledger entry for the quote currency. Fee is charged for the quote currency
                        // side
                        double fee = _feeCalculationService.GetFee(quoteCurrency, volume*price);
                        return CreateLedgerEntry(quoteCurrency, -(volume*price), fee, currentBalance - (volume*price),
                                                 executionDateTime, buyOrderId, tradeId, new AccountId(accountId));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a ledger for the user who has the sell order in the trade
        /// </summary>
        /// <returns></returns>
        private bool SellOrderLedger(Currency baseCurrency, Currency quoteCurrency, double volume, double price,
            DateTime executionDateTime, string orderId, string tradeId, string accountId)
        {
            // First, we take the balance of that currency
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(baseCurrency, new AccountId(accountId));
            if (balance != null)
            {
                double currenctBalance = balance.CurrentBalance;
                // Then we give details to the CreateLedgerEntry method that creates the ledger transaction
                if (CreateLedgerEntry(baseCurrency, -volume, 0.000, currenctBalance - volume, executionDateTime, orderId,
                                      tradeId, new AccountId(accountId)))
                {
                    // Afterwards, we get the balance for the quote currency
                    balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(quoteCurrency, new AccountId(accountId));
                    if (balance != null)
                    {
                        currenctBalance = balance.CurrentBalance;
                        // Finally, we create the ledger for the quote currency. Fee is charged for the quote currency side
                        double fee = _feeCalculationService.GetFee(quoteCurrency, volume*price);
                        return CreateLedgerEntry(quoteCurrency, volume*price, fee, currenctBalance + (volume*price),
                                                 executionDateTime, orderId, tradeId, new AccountId(accountId));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a ledger entry for one currency of one of the two order sids of a trade
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="fee"></param>
        /// <param name="balance"></param>
        /// <param name="executionDate"></param>
        /// <param name="orderId"></param>
        /// <param name="tradeId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool CreateLedgerEntry(Currency currency, double amount, double fee, double balance, DateTime executionDate,
            string orderId, string tradeId, AccountId accountId)
        {
            try
            {
                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), executionDate, LedgerType.Trade, 
                    currency, amount, fee, balance, tradeId, orderId, null, null, accountId);
                _fundsPersistenceRepository.SaveOrUpdate(ledger);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return false;
            }
        }

        /// <summary>
        /// Separates the base and quote currency from the currency pair. The Tuple contains: 
        /// 1 = Base Currency
        /// 2 = Quote Currency
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        private Tuple<Currency, Currency> SeparateBaseQuoteCurrency(string currencyPair)
        {
            // We will split the string from the 2nd index, so that XBTUSD becomes XBT and USD
            return new Tuple<Currency, Currency>(new Currency(currencyPair.Substring(0, 3)), 
                new Currency(currencyPair.Substring(3, 3)));
        }

        /// <summary>
        /// Creates a transaction in result of a Deposit
        /// </summary>
        /// <param name="deposit"> </param>
        /// <param name="balance"> </param>
        /// <returns></returns>
        public bool CreateDepositTransaction(Deposit deposit, double balance)
        {
            if (deposit != null)
            {
                // double currenctBalance = _ledgerRepository.GetBalanceForCurrency(deposit.Currency.Name, 
                //    new AccountId(deposit.AccountId.Value));

                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), DateTime.Now,
                                           LedgerType.Deposit, deposit.Currency, deposit.Amount, 0,
                                           balance, null, null, null, deposit.DepositId,
                                           deposit.AccountId);
                _fundsPersistenceRepository.SaveOrUpdate(ledger);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a transaction in result of a Withdraw
        /// </summary>
        /// <param name="withdraw"> </param>
        /// <returns></returns>
        public bool CreateWithdrawTransaction(Withdraw withdraw)
        {
            if (withdraw != null)
            {
                double currenctBalance = _ledgerRepository.GetBalanceForCurrency(withdraw.Currency.Name, 
                    new AccountId(withdraw.AccountId.Value));
                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), DateTime.Now,
                                           LedgerType.Withdrawal,
                                           withdraw.Currency, withdraw.Amount, withdraw.Fee,
                                           currenctBalance - withdraw.Amount, null, null, withdraw.WithdrawId,
                                           null, withdraw.AccountId);
                _fundsPersistenceRepository.SaveOrUpdate(ledger);
                return true;
            }
            return false;
        }
    }
}
