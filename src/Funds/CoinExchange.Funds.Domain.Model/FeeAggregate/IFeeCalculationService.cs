using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.FeeAggregate
{
    /// <summary>
    /// Interface for matters related to the calculation of Fee
    /// </summary>
    public interface IFeeCalculationService
    {
        /// <summary>
        /// Gets the fee for the given currency and the amount
        /// </summary>
        /// <param name="quoteCurrency"> </param>
        /// <param name="baseCurrency"> </param>
        /// <param name="volume"> </param>
        /// <param name="price"> </param>
        /// <param name="accountId"> </param>
        /// <returns></returns>
        decimal GetFee(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, decimal volume, decimal price);
    }
}
