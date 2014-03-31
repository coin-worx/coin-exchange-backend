using System;
using System.Collections.Generic;
using CoinExchange.Trades.Domain.Model.VOs;

namespace CoinExchange.Trades.Application.MarketData
{
    /// <summary>
    /// Service serving the iperations relatedto Market Data
    /// </summary>
    public class MarketDataApplicationService
    {
        /// <summary>
        /// Returns the Order Book
        /// </summary>
        /// <returns></returns>
        public List<object> GetOrderBook(string symbol)
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

        /// <summary>
        /// Get ticker info
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Pair[] GetTickerInfo(string pairs)
        {
            Domain.Model.VOs.MarketData marketData = new Domain.Model.VOs.MarketData(200, 200.33m);
            Pair pair = new Pair(pairs, marketData, marketData, marketData, new decimal[] { 200, 300 },
                new decimal[] { 200, 300 }, new[] { 200, 200 }, new decimal[] { 200, 300 }, new decimal[] { 200, 300 },
                200m);
            Pair[] PairsList = new Pair[3] { pair, pair, pair };
            return PairsList;
        }

        /// <summary>
        /// Get Ohlc info
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="interval"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        public OhlcInfo GetOhlcInfo(string pair, int interval, string since)
        {
            OhlcValues ohlcData = new OhlcValues(DateTime.UtcNow, 113, 111, 123, 114, 100, 1000, 23);
            List<OhlcValues> dataList = new List<OhlcValues>();
            for (int i = 0; i < 10; i++)
            {
                dataList.Add(ohlcData);
            }
            OhlcInfo info = new OhlcInfo(dataList, pair, 100);
            return info;
        }
    }
}
