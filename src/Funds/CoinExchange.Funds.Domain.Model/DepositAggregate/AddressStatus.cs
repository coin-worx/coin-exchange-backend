using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Status as whetehr a Botcion Address is new or expired
    /// </summary>
    public enum AddressStatus
    {
        New,
        Used,
        Expired
    }
}
