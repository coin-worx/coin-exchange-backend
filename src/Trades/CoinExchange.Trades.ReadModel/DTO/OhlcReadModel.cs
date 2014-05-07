using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class OhlcReadModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OhlcReadModel()
        {
            
        }

        public OhlcReadModel(DateTime dateTime, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            DateTime = dateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        /// <summary>
        /// Update ohlc to recent trade
        /// </summary>
        /// <param name="latestTrade"></param>
        public void UpdateOhlc(Trade latestTrade)
        {
            //update high
            if (latestTrade.ExecutionPrice.Value > High)
                High = latestTrade.ExecutionPrice.Value;
            //update low
            if (latestTrade.ExecutionPrice.Value < Low)
                Low = latestTrade.ExecutionPrice.Value;
            //assign new calculated volume
            Volume += latestTrade.ExecutedVolume.Value;
            //assign new close
            Close = latestTrade.ExecutionPrice.Value;
        }

        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
