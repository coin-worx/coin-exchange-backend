using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Configures in which currency the Depsoit/Withdraw limits are to be shown
    /// </summary>
    public interface ILimitsConfigurationService
    {
        /// <summary>
        /// Configure using configuration which currency type is going to be used to evalute limits
        /// </summary>
        void ConfigureCurrencyType();

        /// <summary>
        /// Evaluate the limits for Deposit
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="amount"></param>
        /// <param name="depositLedgers"></param>
        /// <param name="baseCurrency"> </param>
        /// <returns></returns>
        bool EvaluateDepositLimits(string baseCurrency, string tierLevel, decimal amount, 
                                   IList<Ledger> depositLedgers);

        /// <summary>
        /// Evaluate the Limits for Withdraw
        /// </summary>
        /// <returns></returns>
        bool EvaluateWithdrawLimits(string baseCurrency, string tierLevel, decimal amount, IList<Withdraw> withdrawals, 
            decimal availableBalance, decimal currentBalance);

        /// <summary>
        /// Assign the limits for Deposit
        /// </summary>
        void AssignDepositLimits(string baseCurrency, string tierLevel, IList<Ledger> depositLedgers);

        /// <summary>
        /// Assign the limtis for Withdraw
        /// </summary>
        void AssignWithdrawLimits(string baseCurrency, string tierLevel, IList<Withdraw> withdrawals, decimal availableBalance, 
            decimal currentBalance);

        /// <summary>
        /// Covnert the currency to the current FIAT currency
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        decimal ConvertCurrencyToFiat(string currency, decimal amount);
    }
}
