using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Interface for Withdraw addresses repository
    /// </summary>
    public interface IWithdrawAddressRepository
    {
        WithdrawAddress GetWithdrawAddressById(int id);
        List<WithdrawAddress> GetWithdrawAddressByAccountId(AccountId accountId);
    }
}
