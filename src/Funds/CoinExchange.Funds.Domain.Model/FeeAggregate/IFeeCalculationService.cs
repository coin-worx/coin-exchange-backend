using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;

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
        /// <param name="amount"></param>
        /// <param name="baseCurrency"> </param>
        /// <returns></returns>
        double GetFee(Currency baseCurrency, Currency quoteCurrency, double amount);
    }
}
