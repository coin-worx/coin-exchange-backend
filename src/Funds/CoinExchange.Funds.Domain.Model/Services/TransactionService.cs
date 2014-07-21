using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
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

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="fundsPersistenceRepository"> </param>
        /// <param name="ledgerIdGeneratorService"></param>
        /// <param name="ledgerRepository"> </param>
        public TransactionService(IFundsPersistenceRepository fundsPersistenceRepository, 
            ILedgerIdGeneraterService ledgerIdGeneratorService, ILedgerRepository ledgerRepository)
        {
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _ledgerIdGeneraterService = ledgerIdGeneratorService;
            _ledgerRepository = ledgerRepository;
        }

        /// <summary>
        /// Creates a transaction as a result of a Trade
        /// </summary>
        /// <param name="currencyPair"></param>
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
            Tuple<Currency, Currency> baseQuoteCurencies = SeparateBaseQuoteCurrency(currencyPair);

            if (BuyOrderLedger(baseQuoteCurencies.Item1, baseQuoteCurencies.Item2, tradeVolume, price,
                           executionDateTime, buyOrderId, tradeId, buyAccountId))
            {
                return SellOrderLedger(baseQuoteCurencies.Item1, baseQuoteCurencies.Item2, tradeVolume, price,
                                executionDateTime, sellOrderId, tradeId, sellAccountId);
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
            // First we create a ledger for base currency
            double currentBalance = _ledgerRepository.GetBalanceForCurrency(baseCurrency.Name, new AccountId(accountId));
            if (CreateLedgerEntry(baseCurrency, volume, 0, currentBalance + volume, executionDateTime, buyOrderId, tradeId,
                accountId))
            {
                currentBalance = _ledgerRepository.GetBalanceForCurrency(quoteCurrency.Name, new AccountId(accountId));
                // Second, create ledger entry for the quote currency
                // Todo: Provide master data for fee in database and get it from the repository
                return CreateLedgerEntry(quoteCurrency, -(volume * price), 0 /* Update Me*/, currentBalance - (volume * price), 
                    executionDateTime, buyOrderId, tradeId, accountId);
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
            // First we create a ledger for base currency
            double currenctBalance = _ledgerRepository.GetBalanceForCurrency(baseCurrency.Name,  new AccountId(accountId));
            if (CreateLedgerEntry(baseCurrency, -volume, 0, currenctBalance - volume, executionDateTime, orderId,
                tradeId, accountId))
            {
                currenctBalance = _ledgerRepository.GetBalanceForCurrency(quoteCurrency.Name, new AccountId(accountId));
                // Secondly, we create the ledger for the quote currency
                return CreateLedgerEntry(quoteCurrency, volume*price, 0 /*Update Me*/, currenctBalance + (volume*price),
                                         executionDateTime, orderId, tradeId, accountId);
            }
            return false;
        }

        private bool CreateLedgerEntry(Currency currency, double amount, double fee, double balance, DateTime executionDate,
            string orderId, string tradeId, string accountId)
        {
            try
            {
                Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), executionDate, LedgerType.Trade, 
                    currency, amount, fee, balance, tradeId, orderId, null, null, new AccountId(accountId));
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
        /// <returns></returns>
        public Ledger CreateDepositTransaction(Deposit deposit)
        {
            if (deposit != null)
            {
                double currenctBalance = _ledgerRepository.GetBalanceForCurrency(deposit.Currency.Name, 
                    new AccountId(deposit.AccountId.Value));
                if (deposit.Confirmations >= 7)
                {
                    Ledger ledger = new Ledger(_ledgerIdGeneraterService.GenerateLedgerId(), DateTime.Now,
                                               LedgerType.Deposit, deposit.Currency, deposit.Amount, 0, 
                                               currenctBalance + deposit.Amount, null, null, null, deposit.DepositId,
                                               deposit.AccountId);
                    _fundsPersistenceRepository.SaveOrUpdate(ledger);
                    return ledger;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a transaction in result of a Withdraw
        /// </summary>
        /// <param name="withdraw"> </param>
        /// <returns></returns>
        public Ledger CreateWithdrawTransaction(Withdraw withdraw)
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
                return ledger;
            }
            return null;
        }

        /// <summary>
        /// Gets the balance by calculating the balance avaialble to the existing ledgers
        /// </summary>
        /// <returns></returns>
        /*private double GetBalance(Currency currency, AccountId accountId)
        {
            List<Ledger> ledgerByCurrencyName = _ledgerRepository.GetLedgerByCurrencyName(currency.Name);
            if (ledgerByCurrencyName != null && ledgerByCurrencyName.Any())
            {
                foreach (var ledger in ledgerByCurrencyName)
                {
                    
                }
            }
            else
            {
                return 0;
            }
        }*/
    }
}
