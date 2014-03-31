namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// Specifies the type of the order e.g., Limit, Stop
    /// </summary>
    public enum OrderType
    {
        Limit,
        Stop,
        Market,
        StopLimit
    }
}
