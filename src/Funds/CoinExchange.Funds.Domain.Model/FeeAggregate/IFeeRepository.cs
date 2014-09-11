using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.FeeAggregate
{
    /// <summary>
    /// Interface for Fee Repository
    /// </summary>
    public interface IFeeRepository
    {
        /// <summary>
        /// Gets the Fee by providing the database primary key ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Fee GetFeeById(int id);

        /// <summary>
        /// Gets the Fee by providing the Currency Pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        List<Fee> GetFeeByCurrencyPair(string currencyPair);

        /// <summary>
        /// Gets the Fee corresponding to the given currency and the amount
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Fee GetFeeByCurrencyAndAmount(string currencyPair, int amount);

        /// <summary>
        /// Gets the fees for every currency pair
        /// </summary>
        /// <returns></returns>
        List<Fee> GetAllFees();
    }
}
