using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for accessing Deposit objects from repository
    /// </summary>
    public interface IDepositRepository
    {
        Deposit GetDepositById(string depositId);
        Deposit GetDepositByDate(DateTime dateTime);
        Deposit GetDepositByCurrencyName(string currency);
        Deposit GetDepositByDepositId(string currency);
    }
}
