using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// supported ledger types
    /// </summary>
    public enum LedgerType
    {
        Deposit,
        Withdrawal,
        Trades
    }
}
