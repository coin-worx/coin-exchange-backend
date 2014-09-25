using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// The Currency used to represent the deposit limits
    /// </summary>
    public enum LimitsCurrency
    {
        /// <summary>
        /// The Crypto surrency itself
        /// </summary>
        Default,
        /// <summary>
        /// US Dollars
        /// </summary>
        Usd,
        /// <summary>
        /// EURO
        /// </summary>
        Eur
    }
}
