using System.Collections.Generic;
using CoinExchange.Trades.Application.MarketDataServices.Representation;

namespace CoinExchange.Trades.Application.MarketDataServices
{
    public interface IMarketDataApplicationService
    {
        TickerRepresentation[] GetTickerInfo(string pairs);
        OhlcRepresentation GetOhlcInfo(string pair, int interval, string since);
        List<object> GetOrderBook(string symbol, int count);

    }
}
