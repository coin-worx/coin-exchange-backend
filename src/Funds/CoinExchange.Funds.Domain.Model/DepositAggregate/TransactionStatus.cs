using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Common
{
    /// <summary>
    /// Represents the Status of a Deposit or Withdrawal
    /// </summary>
    public enum TransactionStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}
