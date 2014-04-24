using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class TickerInfoReadModel
    {
        public int Id;
        public decimal AskPrice;
        public decimal AskVolume;
        public decimal BidPrice;
        public decimal BidVolume;
        public decimal TardePrice;
        public decimal TradeVolume;
        public decimal TodaysVolume;
        public decimal Last24HourVolume;
        public decimal TodaysVolumeWeight;
        public decimal Last24HourVolumeWeight;
        public long TodaysTrades;
        public long Last24HourTrades;
        public decimal TodaysLow;
        public decimal Last24HoursLow;
        public decimal TodaysHigh;
        public decimal Last24HoursHigh;
    }
}
