using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Trading Fee for currency pair
    /// </summary>
    public class Fee
    {
        public string CurrencyPair { get; private set; }
        public string PercentageFee { get; private set; }
        public string Amount { get; private set; }
    }
}
