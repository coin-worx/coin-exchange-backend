using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.OrderServices
{
    public interface IOrderApplicationService
    {
        CancelOrderResponse CancelOrder(string txid);
        NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand);
    }
}
