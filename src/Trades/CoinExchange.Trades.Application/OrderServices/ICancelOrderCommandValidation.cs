using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Commands;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Cancel order command validation service
    /// </summary>
    public interface ICancelOrderCommandValidation
    {
        bool ValidateCancelOrderCommand(CancelOrderCommand orderCommand);

    }
}
