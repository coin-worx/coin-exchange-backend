using System;
using System.Collections.Generic;
using CoinExchange.Trades.Application.MarketDataServices.Representation;
using CoinExchange.Trades.ReadModel.MemoryImages;

namespace CoinExchange.Trades.Application.MarketDataServices
{
    public interface IMarketDataQueryService
    {
        object GetTickerInfo(string pairs);
        OhlcRepresentation GetOhlcInfo(string pair, int interval, string since);
        //Tuple<OrderRepresentationList, OrderRepresentationList> GetOrderBook(string symbol, int count);
        object GetOrderBook(string symbol, int count);
        //Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> GetDepth(string currencyPair);
        object GetDepth(string currencyPair);
        Rate GetRate(string currencyPair);
        RatesList GetAllRates();
    }
}
