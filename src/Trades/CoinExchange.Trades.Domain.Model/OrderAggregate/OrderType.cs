using System;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Specifies the type of the order e.g., Limit, Stop
    /// </summary>
    [Serializable]
    public enum OrderType
    {
        Limit,
        Stop,
        Market,
        StopLimit
    }
}
