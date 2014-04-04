using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using CoinExchange.Trades.Application.MarketData.Representation;
using CoinExchange.Trades.Domain.Model.MarketData;

namespace CoinExchange.Trades.Application.MarketData
{
    /// <summary>
    /// Service serving the iperations relatedto Market Data
    /// </summary>
    public class MarketDataApplicationService
    {
        /// <summary>
        /// Get ticker info
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public TickerRepresentation[] GetTickerInfo(string pairs)
        {
            Domain.Model.MarketData.MarketData marketData = new Domain.Model.MarketData.MarketData(200, 200.33m, "Bid");
            TickerRepresentation representation = new TickerRepresentation(pairs, marketData, marketData, marketData, new decimal[] { 200, 300 },
                new decimal[] { 200, 300 }, new[] { 200, 200 }, new decimal[] { 200, 300 }, new decimal[] { 200, 300 },
                200m);
            TickerRepresentation[] representations = new TickerRepresentation[3] { representation, representation, representation };
            return representations;
        }

        /// <summary>
        /// Get Ohlc info
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="interval"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        public OhlcRepresentation GetOhlcInfo(string pair, int interval, string since)
        {
            Ohlc ohlcData = new Ohlc(DateTime.UtcNow, 113, 111, 123, 114, 100, 1000, 23);
            List<Ohlc> dataList = new List<Ohlc>();
            for (int i = 0; i < 10; i++)
            {
                dataList.Add(ohlcData);
            }
            OhlcRepresentation representation = new OhlcRepresentation(dataList, pair, 100);
            return representation;
        }

        /// <summary>
        /// Returns the Order Book
        /// </summary>
        /// <returns></returns>
        public List<object> GetOrderBook(string symbol, int count)
        {
            List<object> list = new List<object>();
            list.Add(symbol);
            list.Add("asks");
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "32", "1000", "204832014" });
            list.Add(new object[] { "34", "1000", "204832014" });

            list.Add("bids");
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "23", "1000", "204832014" });

            return list;
        }
    }
}
