using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for generating the ID for Deposit
    /// </summary>
    public interface IDepositIdGeneratorService
    {
        string GenerateId();
    }
}
