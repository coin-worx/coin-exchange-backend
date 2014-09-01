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
                foreach (var deposit in deposits)
                {
                    depositRepresentations = new List<DepositRepresentation>();
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
