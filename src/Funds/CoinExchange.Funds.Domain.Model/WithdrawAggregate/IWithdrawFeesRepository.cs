using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Interface for Withdraw Fee Repository
    /// </summary>
    public interface IWithdrawFeesRepository
    {
        /// <summary>
        /// Gets the Withdraw Fees by specifying the database primary key ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WithdrawFees GetWithdrawFeesById(int id);

        /// <summary>
        /// Gets the Withraw Fees object by specifying the currency name
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        WithdrawFees GetWithdrawFeesByCurrencyName(string currency);
    }
}
