using System;
using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Validates the fudns and updates the balance for the necessary operation
    /// </summary>
    public class FundsValidationService : IFundsValidationService
    {
        #region Private Fields

        private List<Balance> _balanceList = new List<Balance>();
        private ITransactionService _transactionService = null;
        private IFundsPersistenceRepository _fundsPersistenceRepository = null;
        private IBalanceRepository _balanceRepository = null;
        private IDepositRepository _depositRepository = null;
        private IFeeCalculationService _feeCalculationService = null;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public FundsValidationService(ITransactionService transactionService, IFundsPersistenceRepository
            fundsPersistenceRepository, IBalanceRepository balanceRepository, IDepositRepository depositRepository, 
            IFeeCalculationService feeCalculationService)
        {
            _transactionService = transactionService;
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _balanceRepository = balanceRepository;
            _depositRepository = depositRepository;
            _feeCalculationService = feeCalculationService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Validates the Funds before sending an order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="quoteCurrency"> </param>
        /// <param name="volume"></param>
        /// <param name="price"> </param>
        /// <param name="orderSide"></param>
        /// <param name="baseCurrency"> </param>
        /// <returns></returns>
        [Transaction]
        public bool ValidateFundsForOrder(AccountId accountId, Currency baseCurrency, Currency quoteCurrency, 
            double volume, double price, string orderSide)
        {
            Balance baseCurrencyBalance = GetBalanceForAccountId(accountId, baseCurrency);
            Balance quoteCurrencyBalance = GetBalanceForAccountId(accountId, quoteCurrency);
            if (baseCurrencyBalance != null && quoteCurrency != null)
            {
                if (orderSide.Equals("buy", StringComparison.OrdinalIgnoreCase))
                {
                    // If the order is a buy order, then we need to figure out if we have enough balance in the quote 
                    // currency of the currency pairto carry out the order. If we are trading XBT/USD,
                    // If 1 XBT = 101 USD, then if we want to buy 8 XBT coins, then, 8 XBT = 8*101 USD
                    // So we need askPrice(XBT/USD) * Volume(Volume of the order)
                    // Also, we need to check the Curret Balance(balance that does not contain pending balance)
                    if (quoteCurrencyBalance.CurrentBalance >= price * volume)
                    {
                        quoteCurrencyBalance.AddAvailableBalance(-(price*volume));
                        _fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
                        return true;
                    }
                }
                else if (orderSide.Equals("sell", StringComparison.OrdinalIgnoreCase))
                {
                    // If we are trading for XBT/USD and want to sell XBT, then we need to check if the current
                    // balance(does not contain pending balance) for XBT is enough for the order to take place
                    if (baseCurrencyBalance.CurrentBalance >= volume)
                    {
                        baseCurrencyBalance.AddAvailableBalance(-volume);
                        _fundsPersistenceRepository.SaveOrUpdate(baseCurrencyBalance);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Validates that enough funds exist for the requested withdrawal to be made
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool ValidateFundsForWithdrawal(AccountId accountId, Currency currency, double amount)
        {
            Balance currencyBalance = GetBalanceForAccountId(accountId, currency);
            if (currencyBalance != null)
            {
                if (currencyBalance.AvailableBalance >= amount)
                {
                    currencyBalance.AddAvailableBalance(-amount);
                    _fundsPersistenceRepository.SaveOrUpdate(currencyBalance);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the event that withdraw has been confirmed and takes the necessary steps
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="transactionId"></param>
        /// <param name="bitcoinAddress"></param>
        /// <returns></returns>
        public bool WithdrawalExecuted(AccountId accountId, Currency currency, double amount, TransactionId transactionId, BitcoinAddress bitcoinAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the event that a Deposit has been made and performes the necesssary options
        /// </summary>
        /// <returns></returns>
        public bool DepositConfirmed(Deposit deposit)
        {
            if (deposit != null && deposit.Confirmations >= 7)
            {
                Balance balance = GetBalanceForAccountId(deposit.AccountId, deposit.Currency);

                if (balance == null)
                {
                    balance = new Balance(deposit.Currency, deposit.AccountId, deposit.Amount, deposit.Amount);
                }
                else
                {
                    balance.AddAvailableBalance(deposit.Amount);
                    balance.AddCurrentBalance(deposit.Amount);
                }
                _fundsPersistenceRepository.SaveOrUpdate(balance);

                return _transactionService.CreateDepositTransaction(deposit, balance.CurrentBalance);
            }
            return false;
        }

        /// <summary>
        /// Handles the event that a trade has been executed and performs the necessay steps to update the balance and 
        /// create a transaction record
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"> </param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        /// <returns></returns>
        public bool TradeExecuted(string baseCurrency, string quoteCurrency, double tradeVolume, double price, DateTime executionDateTime,
            string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, string sellOrderId)
        {
            // First we update balance for Buy Account's Base Currency
            if (InitializeBalanceLedgerUpdate(new Currency(baseCurrency), new AccountId(buyAccountId), tradeVolume,
                                          executionDateTime, buyOrderId, tradeId, false))
            {
                // Then, for Buy Account's Quote Currency
                if (InitializeBalanceLedgerUpdate(new Currency(quoteCurrency), new AccountId(buyAccountId),
                                              -(tradeVolume * price),
                                              executionDateTime, buyOrderId, tradeId, false))
                {
                    // Then, for the Seller Account's Base Currency
                    if (InitializeBalanceLedgerUpdate(new Currency(baseCurrency), new AccountId(sellAccountId),
                                                        -tradeVolume, executionDateTime, sellOrderId, tradeId, false))
                    {
                        // Last, for the seller account's quote currency
                        return InitializeBalanceLedgerUpdate(new Currency(quoteCurrency), new AccountId(sellAccountId),
                                                      tradeVolume*price,executionDateTime, sellOrderId, tradeId, false);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Calls methods that update the balance and create a ledger transaction in the database
        /// </summary>
        /// <returns></returns>
        private bool InitializeBalanceLedgerUpdate(Currency currency, AccountId accountId, double amount, DateTime 
            executionDateTime, string orderId, string tradeId, bool includeFee)
        {
            // Update the balance
            double balance = UpdateBalanceAfterTrade(currency, accountId, amount);
            // Create Ledger and save in database
            return CreatePostTradeTransaction(currency, accountId, amount, balance, executionDateTime, tradeId, orderId,
                                       includeFee);
        }

        /// <summary>
        /// Updates the balance for the given currency and Account ID
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        private double UpdateBalanceAfterTrade(Currency currency, AccountId accountId, double volume)
        {
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            if (balance != null)
            {
                balance.AddAvailableBalance(volume);
                balance.AddCurrentBalance(volume);
                _fundsPersistenceRepository.SaveOrUpdate(balance);
                return balance.CurrentBalance;
            }
            return 0;
        }

        /// <summary>
        /// Creates a ledger transaction for a single currency and single order side of a trades by calling the 
        /// TransactionService
        /// </summary>
        /// <returns></returns>
        private bool CreatePostTradeTransaction(Currency currency, AccountId accountId, double amount, double balance,
            DateTime executionDateTime, string tradeId, string orderId, bool includeFee)
        {
            double fee = 0;
            if (includeFee)
            {
                fee = _feeCalculationService.GetFee(currency, amount);
            }
            return _transactionService.CreateLedgerEntry(currency, amount, fee, balance, executionDateTime, orderId, tradeId,
                                                  accountId);
        }

        #endregion Methods

        #region Helper Methods

        /// <summary>
        /// Checks if the balance for the Account Id is already present in the record kept by this service
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"> </param>
        /// <returns></returns>
        private Balance GetBalanceForAccountId(AccountId accountId, Currency currency)
        {
            return _balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
        }

        /// <summary>
        /// Creates a new balance entity to be kept as a record in the database
        /// </summary>
        /// <returns></returns>
        private Balance CreateNewBalanceRecord(AccountId accountId, Currency currency)
        {
            return new Balance(currency, accountId);
        }

        #endregion Helper Methods
    }
}
