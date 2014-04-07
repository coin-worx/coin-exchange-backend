using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.Order.Commands;
using CoinExchange.Trades.Application.Order.Representation;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.Order
{
    public interface IOrderApplicationService
    {
        CancelOrderResponse CancelOrder(string txid);
        NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand);
    }
}
