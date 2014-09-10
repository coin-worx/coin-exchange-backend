using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.BalanceAggregate
{
    /// <summary>
    /// Specifies which type of Entity that has the balance pending
    /// </summary>
    public enum PendingTransactionType
    {
        Withdraw,
        Order
    }
}
