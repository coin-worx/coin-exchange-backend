using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Application.DepositServices
{
    /// <summary>
    /// Deposit Application Service
    /// </summary>
    public class DepositApplicationService : IDepositApplicationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IFundsValidationService _fundsValidationService;
        private ICoinClientService _coinClientService;
        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private IDepositAddressRepository _depositAddressRepository;
        // NOTE: The balanceRepository is here for initial testing of Funds, it must be removed once the Exchange is
        // in link with the bank accounts to deposit USD
        private IBalanceRepository _balanceRepository;
        private IDepositRepository _depositRepository;
        private IDepositLimitRepository _depositLimitRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        private DepositApplicationService(IFundsValidationService fundsValidationService, ICoinClientService coinClientService,
            IFundsPersistenceRepository fundsPersistenceRepository, IDepositAddressRepository depositAddressRepository,
            IBalanceRepository balanceRepository, IDepositRepository depositRepository, IDepositLimitRepository depositLimitRepository)
        {
            _fundsValidationService = fundsValidationService;
            _coinClientService = coinClientService;
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _depositAddressRepository = depositAddressRepository;
            _balanceRepository = balanceRepository;
            _depositRepository = depositRepository;
            _depositLimitRepository = depositLimitRepository;

            _coinClientService.DepositArrived += OnDepositArrival;
            _coinClientService.DepositConfirmed += OnDepositConfirmed;
        }

        /// <summary>
        /// Event handler for event raised in the result of a new confirmation for a deposit
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="confirmations"></param>
        public void OnDepositConfirmed(string transactionId, int confirmations)
        {
            // Get all deposits
            Deposit deposit = _depositRepository.GetDepositByTransactionId(new TransactionId(transactionId));

            if (deposit != null)
            {
                // Set the confirmations
                deposit.SetConfirmations(confirmations);
                // If enough confirmations are not available for the current deposit yet
                if (deposit.Confirmations < 7)
                {
                    // Save in database
                    _fundsPersistenceRepository.SaveOrUpdate(deposit);
                }
                    // If enough confirmations are available, forward to the FundsValidationService to proceed with the 
                    // ledger transation of this deposit
                else if (deposit.Confirmations >= 7)
                {
                    _fundsValidationService.DepositConfirmed(deposit);
                }
            }
        }

        /// <summary>
        /// Handles event raised in result when new transacitons are available. 
        /// Item1 = Address, Item2 = TransactionId, Item3 = Amount, Item4 = Category
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        public void OnDepositArrival(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
        {
            // Get all the deposit addresses to get the AccountId of the user who created this address. These
            // addresses are created whenever a new address is requested from the bitcoin network
            List<DepositAddress> allDepositAddresses = _depositAddressRepository.GetAllDepositAddresses();
            for (int i = 0; i < newTransactions.Count; i++)
            {
                Deposit deposit = _depositRepository.GetDepositByTransactionId(new TransactionId(newTransactions[i].Item2));
                if (deposit != null)
                {
                    continue;
                }
                if (newTransactions[i].Item4 == BitcoinConstants.ReceiveCategory)
                {
                    foreach (var depositAddress in allDepositAddresses)
                    {
                        // Confirm if the user has the permissions to perform the current transaction
                        if (_fundsValidationService.IsTierVerified(depositAddress.AccountId.Value, true).Item1)
                        {
                            // If any of the new transactions' addresses matches any deposit addresses
                            if (depositAddress.BitcoinAddress.Value == newTransactions[i].Item1)
                            {
                                // Create a new deposit for this transaction
                                ValidateDeposit(currency, newTransactions[i].Item1, newTransactions[i].Item3,
                                                depositAddress.AccountId.Value, newTransactions[i].Item2,
                                                TransactionStatus.Pending);

                                // Change the status of the deposit address to Used and save
                                depositAddress.StatusUsed();
                                _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                            }
                        }
                        else
                        {
                            Log.Error(string.Format("FATAL ERROR: Tier Level not enough for submitting deposits: Account ID: {0}",
                                depositAddress.AccountId.Value));
                            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(currency, true),
                                depositAddress.AccountId);
                            if (balance != null)
                            {
                                balance.FreezeAccount();
                                _fundsPersistenceRepository.SaveOrUpdate(balance);
                            }
                            else
                            {
                                balance = new Balance(new Currency(currency, true), depositAddress.AccountId);
                                balance.FreezeAccount();
                                _fundsPersistenceRepository.SaveOrUpdate(balance);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new Deposit instance if not already present, or updates the deposit confirmations otherwise and 
        /// sends to FundsValidationService for further validation
        /// </summary>
        public void ValidateDeposit(string currency, string address, decimal amount, int accountId, string transactionId, 
            TransactionStatus transactionStatus)
        {
            Deposit deposit = new Deposit(new Currency(currency, true), Guid.NewGuid().ToString(),
                    DateTime.Now, DepositType.Default, amount, 0, transactionStatus, new AccountId(accountId),
                    new TransactionId(transactionId), new BitcoinAddress(address));
            _fundsPersistenceRepository.SaveOrUpdate(deposit);
        }


        /// <summary>
        /// Get deposits for the given currency
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"> </param>
        /// <returns></returns>
        public List<DepositRepresentation> GetRecentDeposits(string currency, int accountId)
        {
            List<DepositRepresentation> depositRepresentations = new List<DepositRepresentation>(); ;
            List<Deposit> deposits = _depositRepository.GetDepositByCurrencyAndAccountId(currency, new AccountId(accountId));
            if (deposits != null && deposits.Any())
            {
                foreach (var deposit in deposits)
                {
                    depositRepresentations.Add(new DepositRepresentation(deposit.Currency.Name, "", deposit.DepositId, 
                        deposit.Date, deposit.Amount, deposit.Status.ToString(), (deposit.BitcoinAddress == null) ? null : 
                        deposit.BitcoinAddress.Value, (deposit.TransactionId == null) ? null : deposit.TransactionId.Value));
                }
            }
            return depositRepresentations;
        }

        /// <summary>
        /// Get a new address from the Bitcoin Client
        /// </summary>
        /// <param name="generateNewAddressCommand"></param>
        /// <returns></returns>
        public DepositAddressRepresentation GenarateNewAddress(GenerateNewAddressCommand generateNewAddressCommand)
        {
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(generateNewAddressCommand.Currency),
                new AccountId(generateNewAddressCommand.AccountId));
            if (balance != null)
            {
                if (balance.IsFrozen)
                {
                    throw new InvalidOperationException(string.Format("Account balance Frozen for Account ID = {0}, Currency = {1}",
                        generateNewAddressCommand.AccountId, generateNewAddressCommand.Currency));
                }
            }
            //Check if the required Tier Level for this operation is verified i.e., Tier 1
            if (_fundsValidationService.IsTierVerified(generateNewAddressCommand.AccountId, true).Item1)
            {
                List<DepositAddress> depositAddresses = _depositAddressRepository.
                    GetDepositAddressByAccountIdAndCurrency(
                        new AccountId(generateNewAddressCommand.AccountId), generateNewAddressCommand.Currency);

                if (depositAddresses != null && depositAddresses.Any())
                {
                    // Cannot allow more than 5 New Unused addresses at a time, so will raise exception if count exceeds or reaches 5
                    int counter = 0;
                    foreach (DepositAddress depositAddress1 in depositAddresses)
                    {
                        if (depositAddress1.Status == AddressStatus.New)
                        {
                            counter++;
                        }
                    }
                    if (counter >= 5)
                    {
                        throw new InvalidOperationException("Too many New addresses");
                    }
                }
                string address = _coinClientService.CreateNewAddress(generateNewAddressCommand.Currency);
                DepositAddress depositAddress = new DepositAddress(new Currency(generateNewAddressCommand.Currency),
                                                                   new BitcoinAddress(address), AddressStatus.New,
                                                                   DateTime.Now,
                                                                   new AccountId(generateNewAddressCommand.AccountId));
                _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                return new DepositAddressRepresentation(address, AddressStatus.New.ToString());
            }
            throw new InvalidOperationException(string.Format("Tier 1 is not verified: Account ID = {0}", 
                generateNewAddressCommand.AccountId));
        }

        /// <summary>
        /// Get the Threshold Limits for the given account and currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public DepositLimitThresholdsRepresentation GetThresholdLimits(int accountId, string currency)
        {
            AccountDepositLimits depositLimitThresholds = _fundsValidationService.GetDepositLimitThresholds(new AccountId(accountId), new Currency(currency));
            return new DepositLimitThresholdsRepresentation(depositLimitThresholds.Currency.Name,
                                                  depositLimitThresholds.AccountId.Value,
                                                  depositLimitThresholds.DailyLimit,
                                                  depositLimitThresholds.DailyLimitUsed,
                                                  depositLimitThresholds.MonthlyLimit,
                                                  depositLimitThresholds.MonthlyLimitUsed,
                                                  depositLimitThresholds.CurrentBalance,
                                                  depositLimitThresholds.MaximumDeposit);
        }

        /// <summary>
        /// Get the Bitcoin addresses saved against this user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"> </param>
        /// <returns></returns>
        public IList<DepositAddressRepresentation> GetAddressesForAccount(int accountId, string currency)
        {
            List<DepositAddressRepresentation> depositAddressRepresentations = new List<DepositAddressRepresentation>();
            List<DepositAddress> depositAddresses = _depositAddressRepository.GetDepositAddressByAccountIdAndCurrency(
                new AccountId(accountId), currency);
            foreach (var depositAddress in depositAddresses)
            {
                // If the address has been used, there is a 7 day limit for its expiration from first use
                if (depositAddress.Status == AddressStatus.Used)
                {
                    if (depositAddress.DateUsed.AddDays(7) < DateTime.Now)
                    {
                        // If 7 dayshave passed after first use, mark this address expired
                        depositAddress.StatusExpired();
                        continue;
                    }
                }
                // Only pick addresses that are either New or Used, but not expired
                if (depositAddress.Status != AddressStatus.Expired)
                {
                    depositAddressRepresentations.Add(new DepositAddressRepresentation(depositAddress.BitcoinAddress.Value,
                                                         depositAddress.Status.ToString()));
                }
            }
            return depositAddressRepresentations;
        }

        /// <summary>
        /// Make the deposit
        /// </summary>
        /// <param name="makeDepositCommand"> </param>
        /// <returns></returns>
        public bool MakeDeposit(MakeDepositCommand makeDepositCommand)
        {
            if (_fundsValidationService.IsTierVerified(makeDepositCommand.AccountId, makeDepositCommand.IsCryptoCurrency).Item1)
            {
                Balance balance =
                    _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(makeDepositCommand.Currency),
                                                                        new AccountId(makeDepositCommand.AccountId));
                if (balance == null)
                {
                    balance = new Balance(new Currency(makeDepositCommand.Currency),
                                          new AccountId(makeDepositCommand.AccountId), makeDepositCommand.Amount,
                                          makeDepositCommand.Amount);
                }
                else
                {
                    balance.AddAvailableBalance(makeDepositCommand.Amount);
                    balance.AddCurrentBalance(makeDepositCommand.Amount);
                }
                _fundsPersistenceRepository.SaveOrUpdate(balance);

                if (makeDepositCommand.IsCryptoCurrency)
                {
                    Deposit deposit = new Deposit(new Currency(makeDepositCommand.Currency, true),
                                                  Guid.NewGuid().ToString(),
                                                  DateTime.Now, DepositType.Default, makeDepositCommand.Amount, 0,
                                                  TransactionStatus.Confirmed,
                                                  new AccountId(makeDepositCommand.AccountId), null, null);
                    _fundsPersistenceRepository.SaveOrUpdate(deposit);
                }
                return true;
            }
            throw new InvalidOperationException("Require Tier Level is not verified.");
        }

        /// <summary>
        /// Get the Daily and Monthly Tier limits for Deposit
        /// </summary>
        /// <returns></returns>
        public DepositTierLimitRepresentation GetDepositTiersLimits()
        {
            decimal tier0DailyLimit = 0;
            decimal tier0MonthlyLimit = 0;

            decimal tier1DailyLimit = 0;
            decimal tier1MonthlyLimit = 0;

            decimal tier2DailyLimit = 0;
            decimal tier2MonthlyLimit = 0;

            decimal tier3DailyLimit = 0;
            decimal tier3MonthlyLimit = 0;

            decimal tier4DailyLimit = 0;
            decimal tier4MonthlyLimit = 0;

            IList<DepositLimit> allDepositLimits = _depositLimitRepository.GetAllDepositLimits();
            if (allDepositLimits != null && allDepositLimits.Any())
            {
                foreach (DepositLimit depositLimit in allDepositLimits)
                {
                    if (depositLimit.TierLevel.Equals("Tier 0"))
                    {
                        tier0DailyLimit = depositLimit.DailyLimit;
                        tier0MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 1"))
                    {
                        tier1DailyLimit = depositLimit.DailyLimit;
                        tier1MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 2"))
                    {
                        tier2DailyLimit = depositLimit.DailyLimit;
                        tier2MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 3"))
                    {
                        tier3DailyLimit = depositLimit.DailyLimit;
                        tier3MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                    if (depositLimit.TierLevel.Equals("Tier 4"))
                    {
                        tier4DailyLimit = depositLimit.DailyLimit;
                        tier4MonthlyLimit = depositLimit.MonthlyLimit;
                    }
                }
                return new DepositTierLimitRepresentation(
                    tier0DailyLimit, tier0MonthlyLimit, tier1DailyLimit, tier1MonthlyLimit, tier2DailyLimit, tier2MonthlyLimit,
                    tier3DailyLimit, tier3MonthlyLimit, tier4DailyLimit, tier4MonthlyLimit);

            }
            return null;
        }
    }
}
