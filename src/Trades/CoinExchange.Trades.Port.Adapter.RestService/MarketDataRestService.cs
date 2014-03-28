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
            Pair pair=new Pair();
            pair.PairName = pairs;
            pair.Ask=new MarketData(){Price = 200.33m,Volume = 200};
            pair.Bid = new MarketData() { Price = 200.33m, Volume = 200 };
            pair.Last = new MarketData() { Price = 200.33m, Volume = 200 }; 
            pair.Volume=new decimal[]{200m,200m};
            pair.WeightedVolumeAverage = new[] {200m, 300m};
            pair.NumberOfTrades=new int[]{100,200};
            pair.Low=new decimal[]{300m,400m};
            pair.High=new decimal[]{400m,500m};
            pair.OpeningPrice = 100m;
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
            OhlcData ohlcData=new OhlcData();
            ohlcData.Time = DateTime.Now;
            ohlcData.Open = 100.3345m;
            ohlcData.High = 100.4567m;
            ohlcData.Low = 100.234m; 
            ohlcData.Close = 100.356m;
            ohlcData.vwap = 100.98765m;
            ohlcData.volume = 1000;
            ohlcData.count = 10;
            List<OhlcData> dataList=new List<OhlcData>();
            for (int i = 0; i < 10; i++)
            {
                dataList.Add(ohlcData);
            }
            OhlcInfo info=new OhlcInfo();
            info.Ohlc = dataList;
            info.PairName = pair;
            info.Last = 102200;
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
