using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.Services
{
    /// <summary>
    /// Domain service for generating unique order ids
    /// </summary>
    public interface IOrderIdGenerator
    {
        OrderId GenerateOrderId();
    }
}
