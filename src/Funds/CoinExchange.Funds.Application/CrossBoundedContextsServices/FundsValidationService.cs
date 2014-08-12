using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Validates the fudns and updates the balance for the necessary operation
    /// </summary>
    public class FundsValidationService : IFundsValidationService
    {
        #region Private Fields

        private ITransactionService _transactionService = null;
        private IFundsPersistenceRepository _fundsPersistenceRepository = null;
        private IBalanceRepository _balanceRepository = null;
        private IDepositRepository _depositRepository = null;
        private IFeeCalculationService _feeCalculationService = null;
        private IWithdrawFeesRepository _withdrawFeesRepository = null;
        private IWithdrawIdGeneratorService _withdrawIdGeneratorService = null;
        private ILedgerRepository _ledgerRepository = null;
        private IDepositLimitEvaluationService _depositLimitEvaluationService = null;
        private IDepositLimitRepository _depositLimitRepository = null;
        private IWithdrawLimitEvaluationService _withdrawLimitEvaluationService = null;
        private IWithdrawLimitRepository _withdrawLimitRepository = null;
        private ITierLevelRetrievalService _tierLevelRetrievalService = null;
        private IBboRetrievalService _bboRetrievalService = null;
        
        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public FundsValidationService(ITransactionService transactionService, IFundsPersistenceRepository
            fundsPersistenceRepository, IBalanceRepository balanceRepository,
            IFeeCalculationService feeCalculationService, IWithdrawFeesRepository withdrawFeesRepository, 
            IWithdrawIdGeneratorService withdrawIdGeneratorService, ILedgerRepository ledgerRepository,
            IDepositLimitEvaluationService depositLimitEvaluationService, IDepositLimitRepository depositLimitRepository,
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService, IWithdrawLimitRepository withdrawLimitRepository,
            ITierLevelRetrievalService tierLevelRetrievalService, IBboRetrievalService bboRetrievalService)
        {
            _transactionService = transactionService;
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _balanceRepository = balanceRepository;
            _feeCalculationService = feeCalculationService;
            _withdrawFeesRepository = withdrawFeesRepository;
            _withdrawIdGeneratorService = withdrawIdGeneratorService;
            _ledgerRepository = ledgerRepository;
            _depositLimitEvaluationService = depositLimitEvaluationService;
            _depositLimitRepository = depositLimitRepository;
            _withdrawLimitEvaluationService = withdrawLimitEvaluationService;
            _withdrawLimitRepository = withdrawLimitRepository;
            _tierLevelRetrievalService = tierLevelRetrievalService;
            _bboRetrievalService = bboRetrievalService;
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
        /// <param name="orderId"> </param>
        /// <returns></returns>
        [Transaction]
        public bool ValidateFundsForOrder(AccountId accountId, Currency baseCurrency, Currency quoteCurrency, 
            decimal volume, decimal price, string orderSide, string orderId)
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
                    // Also, we need to check the Available Balance(balance that does not contain pending balance)
                    if (quoteCurrencyBalance.AvailableBalance >= price * volume)
                    {
                        quoteCurrencyBalance.AddPendingTransaction(orderId, PendingTransactionType.Order, -(price*volume));
                        _fundsPersistenceRepository.SaveOrUpdate(quoteCurrencyBalance);
                        return true;
                    }
                }
                else if (orderSide.Equals("sell", StringComparison.OrdinalIgnoreCase))
                {
                    // If we are trading for XBT/USD and want to sell XBT, then we need to check if the Available
                    // balance(does not contain pending balance) for XBT is enough for the order to take place
                    if (baseCurrencyBalance.AvailableBalance >= volume)
                    {
                        baseCurrencyBalance.AddPendingTransaction(orderId, PendingTransactionType.Order, -volume);
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
        /// <param name="transactionId"> </param>
        /// <param name="bitcoinAddress"> </param>
        /// <returns></returns>
        [Transaction]
        public Withdraw ValidateFundsForWithdrawal(AccountId accountId, Currency currency, decimal amount, TransactionId
            transactionId, BitcoinAddress bitcoinAddress)
        {
            WithdrawFees withdrawFees = _withdrawFeesRepository.GetWithdrawFeesByCurrencyName(currency.Name);
            if (withdrawFees != null)
            {
                // Check if the current withdrawal amount is greater than the amount required mentioned with the Fees
                if (amount >= withdrawFees.MinimumAmount)
                {
                    Balance balance = GetBalanceForAccountId(accountId, currency);
                    if (balance != null)
                    {
                        // Get all the Withdraw Ledgers
                        IList<Ledger> withdrawLedgers = GetWithdrawalLedgers();
                        // Get the Current Tier Level for this user using the cross bounded context Tier retrieval service
                        string currentTierLevel = _tierLevelRetrievalService.GetCurrentTierLevel(accountId.Value);
                        // Get the Current Withdraw limits for this user
                        WithdrawLimit withdrawLimit =
                            _withdrawLimitRepository.GetWithdrawLimitByTierLevel(currentTierLevel);

                        // Get the best bid and best ask form the Trades BC.
                        // ToDo: Implement and use real implementation later, rather than using the stub implementation being used right now
                        Tuple<decimal, decimal> bestBidBestAsk = _bboRetrievalService.GetBestBidBestAsk(currency.Name,
                                                                                                      "USD");
                        // Convert the amount and the fee to US Dollars, because the evaluation service accepts amounts
                        // in USD
                        decimal amountInUsd = ConvertCurrencyToUsd(amount, bestBidBestAsk.Item1, bestBidBestAsk.Item2);
                        decimal feeInUsd = ConvertCurrencyToUsd(withdrawFees.Fee, bestBidBestAsk.Item1,
                                                               bestBidBestAsk.Item2);
                        // Evaluate if the current withdrawal is within the limits of the maximum withdrawal allowed and 
                        // the balance available
                        if (_withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(amountInUsd + feeInUsd,
                            withdrawLedgers, withdrawLimit, bestBidBestAsk.Item1, bestBidBestAsk.Item2,
                            balance.AvailableBalance, balance.CurrentBalance))
                        {
                            Withdraw withdraw = new Withdraw(currency, _withdrawIdGeneratorService.GenerateNewId(),
                                                             DateTime.Now, WithdrawType.Default, amount,withdrawFees.Fee,
                                                             TransactionStatus.Pending, accountId, transactionId, 
                                                             bitcoinAddress);
                            _fundsPersistenceRepository.SaveOrUpdate(withdraw);
                            balance.AddPendingTransaction(withdraw.WithdrawId, PendingTransactionType.Withdraw, 
                                -(withdraw.Amount + withdrawFees.Fee));
                            _fundsPersistenceRepository.SaveOrUpdate(balance);
                            return withdraw;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Handles the event that withdraw has been confirmed and takes the necessary steps
        /// </summary>
        /// <param name="withdraw"> </param>
        /// <returns></returns>
        [Transaction]
        public bool WithdrawalExecuted(Withdraw withdraw)
        {
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(withdraw.Currency, withdraw.AccountId);


            bool addResponse = balance.ConfirmPendingTransaction(withdraw.WithdrawId,
                                                                 PendingTransactionType.Withdraw,
                                                                 -(withdraw.Amount + withdraw.Fee));
            if (addResponse)
            {
                _fundsPersistenceRepository.SaveOrUpdate(balance);
                return _transactionService.CreateWithdrawTransaction(withdraw, balance.CurrentBalance);
            }

            return false;
        }

        /// <summary>
        /// Handles the event that a Deposit has been made and performes the necesssary options
        /// </summary>
        /// <returns></returns>
        [Transaction]
        public bool DepositConfirmed(Deposit deposit)
        {
            if (deposit != null && deposit.Confirmations >= 7)
            {
                if (deposit.Currency.IsCryptoCurrency)
                {
                    // Get all the Deposit Ledgers
                    IList<Ledger> depositLedgers = GetDepositLedgers();
                    // Get the Current Tier Level for this user using the corss bounded context Tier retrieval service
                    // ToDo: Using the stub implementation for now, upgrade to the real cross bounded context service
                    // once completed
                    string currentTierLevel = _tierLevelRetrievalService.GetCurrentTierLevel(deposit.AccountId.Value);
                    // Get the Current Deposit limits for this user
                    DepositLimit depositLimit = _depositLimitRepository.GetDepositLimitByTierLevel(currentTierLevel);

                    // Get the best bid and best ask form the Trades BC.
                    // NOTE: Implement adn use real implementation later, rather than using the stub implementation being used right now
                    Tuple<decimal, decimal> bestBidBestAsk =
                        _bboRetrievalService.GetBestBidBestAsk(deposit.Currency.Name,
                                                               "USD");

                    deposit.SetAmountInUsd(ConvertCurrencyToUsd(deposit.Amount, bestBidBestAsk.Item1,
                                                                bestBidBestAsk.Item2));
                    // Check if the current Deposit transaction is within the Deposit limits
                    if (_depositLimitEvaluationService.EvaluateDepositLimit(deposit.AmountInUsd, depositLedgers,
                                                                            depositLimit, bestBidBestAsk.Item1,
                                                                            bestBidBestAsk.Item2))
                    {
                        return UpdateBalance(deposit);
                    }
                }
                else
                {
                    return UpdateBalance(deposit);
                }
            }
            return false;
        }

        private bool UpdateBalance(Deposit deposit)
        {
            // Check if balance instance has been created for this user already
            Balance balance = GetBalanceForAccountId(deposit.AccountId, deposit.Currency);

            // If balance instance is not initiated for this currency for the current user, create now
            if (balance == null)
            {
                balance = new Balance(deposit.Currency, deposit.AccountId, deposit.Amount, deposit.Amount);
            }
            // Otherwise, update the balance for the current user's currency
            else
            {
                balance.AddAvailableBalance(deposit.Amount);
                balance.AddCurrentBalance(deposit.Amount);
            }
            _fundsPersistenceRepository.SaveOrUpdate(balance);

            return _transactionService.CreateDepositTransaction(deposit, balance.CurrentBalance);
        }

        /// <summary>
        /// Converts the specified currency to US Dollars based on the given Currency - USD best bid and best ask
        /// </summary>
        /// <returns></returns>
        private decimal ConvertCurrencyToUsd(decimal currencyAmount, decimal bestBid, decimal bestAsk)
        {
            return (((currencyAmount * bestBid) + (currencyAmount * bestAsk)) / 2);
        }

        /// <summary>
        /// Gets all the Deposit Ledger transactions
        /// </summary>
        /// <returns></returns>
        private IList<Ledger> GetDepositLedgers()
        {
            IList<Ledger> depositLedgers = new List<Ledger>();
            IList<Ledger> allLedgers = _ledgerRepository.GetAllLedgers();
            foreach (var ledger in allLedgers)
            {
                if (ledger.LedgerType == LedgerType.Deposit)
                {
                    depositLedgers.Add(ledger);
                }
            }
            if (!depositLedgers.Any())
            {
                depositLedgers = null;
            }
            return depositLedgers;
        }

        /// <summary>
        /// Gets all the Withdraw Ledger transactions
        /// </summary>
        /// <returns></returns>
        private IList<Ledger> GetWithdrawalLedgers()
        {
            IList<Ledger> withdrawLedgers = new List<Ledger>();
            IList<Ledger> allLedgers = _ledgerRepository.GetAllLedgers();
            foreach (var ledger in allLedgers)
            {
                if (ledger.LedgerType == LedgerType.Withdrawal)
                {
                    withdrawLedgers.Add(ledger);
                }
            }
            if (!withdrawLedgers.Any())
            {
                withdrawLedgers = null;
            }
            return withdrawLedgers;
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
        [Transaction]
        public bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price, DateTime executionDateTime,
            string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, string sellOrderId)
        {

            decimal buySideFee = _feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                                                               new AccountId(buyAccountId), tradeVolume, price);
            decimal sellSideFee = _feeCalculationService.GetFee(new Currency(baseCurrency), new Currency(quoteCurrency),
                                                               new AccountId(sellAccountId), tradeVolume, price);
            // As in case of buy order, the current and available balances differ in the case of quote currency, we 
            // evaluate the quote currency first
            if (InitializeBalanceLedgerUpdate(new Currency(quoteCurrency), new AccountId(buyAccountId),
                                              -(tradeVolume*price), 0,
                                              executionDateTime, buyOrderId, tradeId, true, true, buySideFee, true))
            {
                // Then, we update balance for Buy Account's Base Currency
                if (InitializeBalanceLedgerUpdate(new Currency(baseCurrency), new AccountId(buyAccountId), tradeVolume, 0,
                                                  executionDateTime, buyOrderId, tradeId, false, false, 0, false))
                {
                    // In case of a sell order, the available balance is deducted from the base currency, so we validate the 
                    // base currency first
                    if (InitializeBalanceLedgerUpdate(new Currency(baseCurrency), new AccountId(sellAccountId),
                                                      -tradeVolume, 0, executionDateTime, sellOrderId, tradeId, false, true,
                                                      0, false))
                    {
                        // Lastly, for the seller account's quote currency
                        return InitializeBalanceLedgerUpdate(new Currency(quoteCurrency), new AccountId(sellAccountId),
                                                             tradeVolume*price, 0, executionDateTime, sellOrderId, tradeId,
                                                             true, false, sellSideFee, true);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the funds when an order is cancelled
        /// </summary>
        /// <param name="quoteCurrency"> </param>
        /// <param name="accountId"></param>
        /// <param name="orderside"> </param>
        /// <param name="orderId"></param>
        /// <param name="openQuantity"> </param>
        /// <param name="price"> </param>
        /// <param name="baseCurrency"> </param>
        /// <returns></returns>
        public bool OrderCancelled(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, string orderside, string orderId, decimal openQuantity, decimal price)
        {
            decimal amount = 0;
            Balance currencyBalance = null;
            // If the order is buy order, then we deducted price * volume from the quote currency. So now we add the 
            // openQuantity*price in the quote currency, as the order may be partially filled
            if (orderside.Equals("buy", StringComparison.OrdinalIgnoreCase))
            {
                amount = price * openQuantity;
                currencyBalance = GetBalanceForAccountId(accountId, quoteCurrency);
            }
            // If the order is a sell order, then we had deducted the volume from the base currency. So we now add the 
            // volume back in the base currency
            else if (orderside.Equals("sell", StringComparison.OrdinalIgnoreCase))
            {
                amount = openQuantity;
                currencyBalance = GetBalanceForAccountId(accountId, baseCurrency);
            }
            
            if (currencyBalance != null)
            {
                currencyBalance.CancelPendingTransaction(orderId, PendingTransactionType.Order, amount);
                _fundsPersistenceRepository.SaveOrUpdate(currencyBalance);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calls methods that update the balance and create a ledger transaction in the database
        /// </summary>
        /// <returns></returns>
        private bool InitializeBalanceLedgerUpdate(Currency currency, AccountId accountId, decimal amount,
            decimal amountInUsd, DateTime executionDateTime, string orderId, string tradeId, bool includeFee, 
            bool isPending, decimal fee, bool isBaseCurrencyInTrade)
        {
            // Update the balance
            decimal balance = UpdateBalanceAfterTrade(currency, accountId, amount, orderId, fee, isPending);
            // Create Ledger and save in database
            return CreatePostTradeTransaction(currency, accountId, amount, amountInUsd, balance, executionDateTime,
                tradeId, orderId, includeFee, fee, isBaseCurrencyInTrade);
        }

        /// <summary>
        /// Updates the balance for the given currency and Account ID
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"></param>
        /// <param name="volume"></param>
        /// <param name="orderId"> </param>
        /// <param name="fee"> </param>
        /// <param name="isPending"> </param>
        /// <returns></returns>
        private decimal UpdateBalanceAfterTrade(Currency currency, AccountId accountId, decimal volume, string orderId,
            decimal fee, bool isPending)
        {
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(currency, accountId);
            if (balance != null)
            {
                if (!isPending)
                {
                    balance.AddAvailableBalance(volume);
                    balance.AddCurrentBalance(volume);
                }
                else
                {
                    balance.ConfirmPendingTransaction(orderId, PendingTransactionType.Order, volume);
                }
                if (fee != 0.0M)
                {
                    balance.AddAvailableBalance(-fee);
                    balance.AddCurrentBalance(-fee);
                }
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
        private bool CreatePostTradeTransaction(Currency currency, AccountId accountId, decimal amount, decimal amountInUsd,
            decimal balance, DateTime executionDateTime, string tradeId, string orderId, bool includeFee, decimal fee, 
            bool isBaseCurrencyInTrade)
        {
            return _transactionService.CreateLedgerEntry(currency, amount, amountInUsd, fee, balance, executionDateTime,
                orderId, tradeId,  accountId, isBaseCurrencyInTrade);
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
