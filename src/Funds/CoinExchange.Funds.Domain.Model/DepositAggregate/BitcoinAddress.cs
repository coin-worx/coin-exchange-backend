using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Bitcoin Address to make Deposits to or Withdrawals from
    /// </summary>
    public class BitcoinAddress
    {
        public string Value { get; set; }
    }
}
