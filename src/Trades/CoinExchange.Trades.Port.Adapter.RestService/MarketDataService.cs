using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.VOs;

namespace CoinExchange.Trades.Port.Adapter.RestService
{
    /// <summary>
    /// Market Data Service class rest expose
    /// </summary>
    public class MarketDataService
    {
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
    }
}
