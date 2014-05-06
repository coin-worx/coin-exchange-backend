using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Cancel order command validation service
    /// </summary>
    public interface ICancelOrderCommandValidation
    {
        bool ValidateCancelOrderCommand(CancelOrderCommand orderCommand);
        string GetCurrencyPair(OrderId orderId);
    }
}
