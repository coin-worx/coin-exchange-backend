using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Authentication;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Evaluates limits keeping in check what type of currency the limits need to be depicted
    /// </summary>
    public class LimitsConfigurationService : ILimitsConfigurationService
    {
        private string _adminDefinedLimitsCurrency = null;
        private IDepositLimitRepository _depositLimitRepository = null;
        private IWithdrawLimitRepository _withdrawLimitRepository = null;
        private IDepositLimitEvaluationService _depositLimitEvaluationService = null;
        private IWithdrawLimitEvaluationService _withdrawLimitEvaluationService = null;
        private IBboCrossContextService _bboCrossContextService = null;
        private Tuple<decimal, decimal> _bestBidBestAsk = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LimitsConfigurationService(IDepositLimitRepository depositLimitRepository, 
            IDepositLimitEvaluationService depositLimitEvaluationService, IWithdrawLimitRepository withdrawLimitRepository, 
            IWithdrawLimitEvaluationService withdrawLimitEvaluationService, IBboCrossContextService bboCrossContextService)
        {
            _depositLimitRepository = depositLimitRepository;
            _depositLimitEvaluationService = depositLimitEvaluationService;
            _withdrawLimitRepository = withdrawLimitRepository;
            _withdrawLimitEvaluationService = withdrawLimitEvaluationService;
            _bboCrossContextService = bboCrossContextService;

            ConfigureCurrencyType();
        }

        /// <summary>
        /// Configure using configuration which currency type is going to be used to evalute limits
        /// </summary>
        public void ConfigureCurrencyType()
        {
            string adminDefinedLimitsCurrency = ConfigurationManager.AppSettings.Get("LimitsCurrencyType");
            string[] limitCurrencies = Enum.GetNames(typeof (LimitsCurrency));
            // Check if such currency exists as defined by the admin
            bool limitCurrencyFound = false;
            foreach (var limitCurrency in limitCurrencies)
            {
                if (limitCurrency == adminDefinedLimitsCurrency)
                {
                    limitCurrencyFound = true;
                }
            }
            if (!limitCurrencyFound)
            {
                throw new InvalidCredentialException(string.Format("User defined limits currency not found: Currency = {0}",
                    adminDefinedLimitsCurrency));
            }
            else
            {
                _adminDefinedLimitsCurrency = adminDefinedLimitsCurrency;
            }
        }

        /// <summary>
        /// Evaluate the limits for Deposit
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="amount"></param>
        /// <param name="depositLedgers"></param>
        /// <param name="baseCurrency"> </param>
        /// <returns></returns>
        public bool EvaluateDepositLimits(string baseCurrency, string tierLevel, decimal amount,
                                            IList<Ledger> depositLedgers)
        {
            // If the admin has specified the limits to be calculated as the currency itself
            if (_adminDefinedLimitsCurrency == LimitsCurrency.Default.ToString())
            {
                // Get the Current Deposit limits for this user
                DepositLimit depositLimit = _depositLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel, 
                    _adminDefinedLimitsCurrency);
                if (depositLimit != null)
                {
                    // Check if the current Deposit transaction is within the Deposit limits
                    return _depositLimitEvaluationService.EvaluateDepositLimit(amount, depositLedgers, depositLimit);
                }
                throw new InvalidOperationException(string.Format("No Deposit Limit found for Tier: Tier Level = {0} & " +
                                                                  "CurrencyType = {1}", tierLevel, _adminDefinedLimitsCurrency));
            }
            // If the admin has specified that limtis must be calculated in a FIAT currency
            else
            {
                // Get the Current Deposit limits for this user which will be in a FIAT currency
                DepositLimit depositLimit = _depositLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel,
                    _adminDefinedLimitsCurrency);

                if (depositLimit != null)
                {
                    // Convert the currency to FIAT
                    decimal amountInFiat = ConvertCurrencyToFiat(baseCurrency, amount);
                    // Return reponse that if the user is allowed to deposit the specified limit of not
                    // NOTE: If there is no best bid and best ask, then the limits will be speficied in the defauly crypto currency itself
                    return _depositLimitEvaluationService.EvaluateDepositLimit(amountInFiat, depositLedgers,
                                                                               depositLimit,
                                                                               _bestBidBestAsk.Item1,
                                                                               _bestBidBestAsk.Item2);
                }
                throw new InvalidOperationException(string.Format("No Deposit Limit found for Tier: Tier Level = {0} & " +
                                                                  "CurrencyType = {1}", tierLevel, _adminDefinedLimitsCurrency));
            }
        }

        /// <summary>
        /// Evaluate the threshold limits and limits used for withdraw
        /// </summary>
        /// <returns></returns>
        public bool EvaluateWithdrawLimits(string baseCurrency, string tierLevel, decimal amount,
            IList<Withdraw> withdrawals, decimal availableBalance, decimal currentBalance)
        {
            // If the admin has specified the limits to be calculated as the currency itself
            if (_adminDefinedLimitsCurrency == LimitsCurrency.Default.ToString())
            {
                // Get the Current Withdraw limits for this user
                WithdrawLimit withdrawLimit = _withdrawLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel, 
                    _adminDefinedLimitsCurrency);

                if (withdrawLimit != null)
                {
                    // Check if the current Withdraw transaction is within the Withdraw limits
                    return _withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(amount, withdrawals,
                                                                                        withdrawLimit,
                                                                                        availableBalance, currentBalance);
                }
                throw new InvalidOperationException(string.Format("No Withdraw Limit found for Tier: Tier Level = {0} & " +
                                                                  "CurrencyType = {1}", tierLevel, _adminDefinedLimitsCurrency));
            }
            // If the admin has specified that limits must be calculated in a FIAT currency
            else
            {
                // Get the Current Withdraw limits for this user which will be in a FIAT currency
                WithdrawLimit withdrawLimit = _withdrawLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel,
                    _adminDefinedLimitsCurrency);

                if (withdrawLimit != null)
                {
                    // Convert the currency to FIAT
                    decimal amountInFiat = ConvertCurrencyToFiat(baseCurrency, amount);
                    // Check if the current Withdraw transaction is within the Withdraw limits
                    // NOTE: If there is no best bid and best ask, then the limits will be specified in the default crypto currency 
                    // itself
                    return _withdrawLimitEvaluationService.EvaluateMaximumWithdrawLimit(amountInFiat, withdrawals,
                                                                                        withdrawLimit,
                                                                                        availableBalance, currentBalance,
                                                                                        _bestBidBestAsk.Item1,
                                                                                        _bestBidBestAsk.Item2);
                }
                throw new InvalidOperationException(string.Format("No Withdraw Limit found for Tier: Tier Level = {0} & " +
                                                                  "CurrencyType = {1}", tierLevel, _adminDefinedLimitsCurrency));
            }
        }

        /// <summary>
        /// Assign the deposit limits
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="tierLevel"></param>
        /// <param name="depositLedgers"></param>
        public void AssignDepositLimits(string baseCurrency, string tierLevel, IList<Ledger> depositLedgers)
        {
            // If the admin has specified the limits to be calculated as the currency itself
            if (_adminDefinedLimitsCurrency == LimitsCurrency.Default.ToString())
            {
                // Get the Current Deposit limits for this user
                DepositLimit depositLimit = _depositLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel, 
                    _adminDefinedLimitsCurrency);
                if (depositLimit != null)
                {
                    // Check if the current Deposit transaction is within the Deposit limits
                    _depositLimitEvaluationService.AssignDepositLimits(depositLedgers, depositLimit);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("No Deposit Limit found for Tier: Tier Level = " + tierLevel));
                }
            }
            // If the admin has specified that limtis must be calculated in a FIAT currency
            else
            {
                // Get the Current Deposit limits for this user which will be in a FIAT currency
                DepositLimit depositLimit = _depositLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel,
                    _adminDefinedLimitsCurrency);

                if (depositLimit != null)
                {
                    // Get the best bid and best ask
                    Tuple<decimal, decimal> bestBidBestAsk = _bboCrossContextService.GetBestBidBestAsk(baseCurrency, _adminDefinedLimitsCurrency);

                    // Return reponse that if the user is allowed to deposit the specified limit of not
                    // NOTE: If there is no best bid and best ask, then the limits will be speficied in the defauly crypto currency itself
                    _depositLimitEvaluationService.AssignDepositLimits(depositLedgers, depositLimit,
                                                                       bestBidBestAsk.Item1, bestBidBestAsk.Item2);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("No Deposit Limit found for Tier: Tier Level = " + tierLevel));
                }
            }
        }

        /// <summary>
        /// Assign The Withdraw Limits
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="tierLevel"></param>
        /// <param name="withdrawals"></param>
        /// <param name="availableBalance"></param>
        /// <param name="currentBalance"></param>
        public void AssignWithdrawLimits(string baseCurrency, string tierLevel,
            IList<Withdraw> withdrawals, decimal availableBalance, decimal currentBalance)
        {
            // If the admin has specified the limits to be calculated as the currency itself
            if (_adminDefinedLimitsCurrency == LimitsCurrency.Default.ToString())
            {
                // Get the Current Withdraw limits for this user
                WithdrawLimit withdrawLimit = _withdrawLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel,
                    _adminDefinedLimitsCurrency);

                if (withdrawLimit != null)
                {
                    // Check if the current Withdraw transaction is within the Withdraw limits
                    _withdrawLimitEvaluationService.AssignWithdrawLimits(withdrawals, withdrawLimit,
                                                                         availableBalance, currentBalance);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("No Withdraw Limit found for Tier: Tier Level = {0} & " +
                                      "CurrencyType = {1}", tierLevel, _adminDefinedLimitsCurrency));
                }
            }
            // If the admin has specified that limits must be calculated in a FIAT currency
            else
            {
                // Get the Current Withdraw limits for this user which will be in a FIAT currency
                WithdrawLimit withdrawLimit = _withdrawLimitRepository.GetLimitByTierLevelAndCurrency(tierLevel,
                    _adminDefinedLimitsCurrency);

                if (withdrawLimit != null)
                {
                    // Get the best bid and best ask
                    Tuple<decimal, decimal> bestBidBestAsk = _bboCrossContextService.GetBestBidBestAsk(baseCurrency, _adminDefinedLimitsCurrency);
                    // Assign the withdraw limits in FIAT amounts
                    _withdrawLimitEvaluationService.AssignWithdrawLimits(withdrawals, withdrawLimit,
                                                                         availableBalance, currentBalance,
                                                                         bestBidBestAsk.Item1, bestBidBestAsk.Item2);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("No Withdraw Limit found for Tier: Tier Level = {0} & " +
                                      "CurrencyType = {1}", tierLevel, _adminDefinedLimitsCurrency));
                }
            }
        }

        /// <summary>
        /// Convert a currency to it's equivalent in FIAT. The FIAT currency is the one specified by the admin at start-up time
        /// </summary>
        /// <param name="currency"> </param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal ConvertCurrencyToFiat(string currency, decimal amount)
        {
            _bestBidBestAsk = _bboCrossContextService.GetBestBidBestAsk(currency, _adminDefinedLimitsCurrency);
            if (_bestBidBestAsk.Item1 > 0 && _bestBidBestAsk.Item2 > 0)
            {
                decimal sum = (amount*_bestBidBestAsk.Item1) + (amount*_bestBidBestAsk.Item2);
                decimal midPoint = sum/2;
                return midPoint;
            }
            return 0;
        }
    }
}
