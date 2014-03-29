using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.VOs;
using CoinExchange.Trades.Infrastructure.Services.Services;

namespace CoinExchange.Trades.Port.Adapter.RestService
{
    /// <summary>
    /// Market Data Service class rest expose
    /// </summary>
    public class MarketDataRestService
    {
        private MarketDataService _marketDataService = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketDataRestService()
        {
            _marketDataService = new MarketDataService();
        }

        /// <summary>
        /// Get ticker info
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Pair[] GetTickerInfo(string pairs)
        {
            MarketData marketData=new MarketData(200,200.33m);
            Pair pair = new Pair(pairs, marketData, marketData, marketData, new decimal[] {200, 300},
                new decimal[] {200, 300}, new[] {200, 200}, new decimal[] {200, 300}, new decimal[] {200, 300},
                200m);
            Pair[] PairsList = new Pair[3]{pair,pair,pair};
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
            List<OhlcValues> dataList=new List<OhlcValues>();
            for (int i = 0; i < 10; i++)
            {
                dataList.Add(ohlcData);
            }
            OhlcInfo info = new OhlcInfo(dataList, pair, 100);
            return info;
        }

        /// <summary>
        /// Returns orders that have not been executed but those that have been accepted on the server. Exception can be 
        /// provided in the second parameter
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        public List<object> OpenOrderList(string currencyPair, int count = 0)
        {
            return _marketDataService.GetOrderBook(currencyPair);
        }
    }
}
