using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.MarketData.Representation;

namespace CoinExchange.Trades.Application.MarketData
{
    public interface IMarketDataApplicationService
    {
        TickerRepresentation[] GetTickerInfo(string pairs);
        OhlcRepresentation GetOhlcInfo(string pair, int interval, string since);
        List<object> GetOrderBook(string symbol, int count);

    }
}
