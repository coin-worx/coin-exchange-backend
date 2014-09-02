using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private IFundsValidationService _fundsValidationService;
        private ICoinClientService _coinClientService;
        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private IDepositAddressRepository _depositAddressRepository;
        // NOTE: The balanceRepository is here for initial testing of Funds, it must be removed once the Exchange is
        // in link with the bank accounts to deposit USD
        private IBalanceRepository _balanceRepository;
        private IDepositRepository _depositRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        private DepositApplicationService(IFundsValidationService fundsValidationService, ICoinClientService coinClientService,
            IFundsPersistenceRepository fundsPersistenceRepository, IDepositAddressRepository depositAddressRepository,
            IBalanceRepository balanceRepository, IDepositRepository depositRepository)
        {
            _fundsValidationService = fundsValidationService;
            _coinClientService = coinClientService;
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _depositAddressRepository = depositAddressRepository;
            _balanceRepository = balanceRepository;
            _depositRepository = depositRepository;

            _coinClientService.DepositArrived += OnDepositArrival;
            _coinClientService.DepositConfirmed += OnDepositConfirmed;
        }

        /// <summary>
        /// Event handler for event raised in the result of a new confirmation for a deposit
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="confirmations"></param>
        private void OnDepositConfirmed(string transactionId, int confirmations)
        {
            // Get all deposits
            List<Deposit> allDeposits = _depositRepository.GetAllDeposits();
            foreach (var deposit in allDeposits)
            {
                deposit.SetConfirmations(confirmations);
                // If enough confirmations are not available for the current deposit yet
                if (deposit.Confirmations < 7)
                {
                    // Set the confirmations
                    deposit.SetConfirmations(confirmations);
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
        private void OnDepositArrival(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
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
                if (newTransactions[i].Item4 == "receive")
                {
                    foreach (var depositAddress in allDepositAddresses)
                    {
                        // If any of the new transactions' addresses matches any deposit addresses
                        if (depositAddress.BitcoinAddress.Value == newTransactions[i].Item1)
                        {
                            // Make sure this address hasn't been used earlier
                            if (depositAddress.Status != AddressStatus.Used &&
                                depositAddress.Status != AddressStatus.Expired)
                            {
                                // Create a new deposit for this transaction
                                ValidateDeposit(currency, newTransactions[i].Item1, newTransactions[i].Item3,
                                                depositAddress.AccountId.Value, newTransactions[i].Item2,
                                                TransactionStatus.Pending);

                                // Change the status of the deposit address to Used and save
                                depositAddress.StatusUsed();
                                _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
                            }
                            else
                            {
                                // Create a new deposit for this transaction but mark it suspended
                                ValidateDeposit(currency, newTransactions[i].Item1, newTransactions[i].Item3,
                                                depositAddress.AccountId.Value, newTransactions[i].Item2,
                                                TransactionStatus.Suspended);
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
            List<DepositRepresentation> depositRepresentations = null;
            List<Deposit> deposits = _depositRepository.GetDepositByCurrencyAndAccountId(currency, new AccountId(accountId));
            if (deposits != null && deposits.Any())
            {
                depositRepresentations = new List<DepositRepresentation>();
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
            List<DepositAddress> depositAddresses = _depositAddressRepository.GetDepositAddressByAccountIdAndCurrency(
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
                    throw new InvalidOperationException("Too many addresses");
                }
            }
            string address = _coinClientService.CreateNewAddress(generateNewAddressCommand.Currency);
            DepositAddress depositAddress = new DepositAddress(new Currency(generateNewAddressCommand.Currency), 
                new BitcoinAddress(address), AddressStatus.New, DateTime.Now, 
                new AccountId(generateNewAddressCommand.AccountId));
            _fundsPersistenceRepository.SaveOrUpdate(depositAddress);
            return new DepositAddressRepresentation(address, AddressStatus.New.ToString());
        }

        /// <summary>
        /// Get the Threshold Limits for the given account and currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public DepositLimitRepresentation GetThresholdLimits(int accountId, string currency)
        {
            AccountDepositLimits depositLimitThresholds = _fundsValidationService.GetDepositLimitThresholds(new AccountId(accountId), new Currency(currency));
            return new DepositLimitRepresentation(depositLimitThresholds.Currency.Name,
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
                depositAddressRepresentations.Add(new DepositAddressRepresentation(depositAddress.BitcoinAddress.Value,
                    depositAddress.Status.ToString()));
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
            Balance balance = _balanceRepository.GetBalanceByCurrencyAndAccountId(new Currency(makeDepositCommand.Currency), new AccountId(makeDepositCommand.AccountId));
            if (balance == null)
            {
                balance = new Balance(new Currency(makeDepositCommand.Currency), 
                    new AccountId(makeDepositCommand.AccountId), makeDepositCommand.Amount, makeDepositCommand.Amount);
            }
            else
            {
                balance.AddAvailableBalance(makeDepositCommand.Amount);
                balance.AddCurrentBalance(makeDepositCommand.Amount);
            }
            _fundsPersistenceRepository.SaveOrUpdate(balance);

            if (makeDepositCommand.IsCryptoCurrency)
            {
                Deposit deposit = new Deposit(new Currency(makeDepositCommand.Currency, true), Guid.NewGuid().ToString(),
                    DateTime.Now, DepositType.Default, makeDepositCommand.Amount, 0, TransactionStatus.Confirmed, 
                    new AccountId(makeDepositCommand.AccountId), null, null);
                _fundsPersistenceRepository.SaveOrUpdate(deposit);
            }
            return true;
        }
    }
}
