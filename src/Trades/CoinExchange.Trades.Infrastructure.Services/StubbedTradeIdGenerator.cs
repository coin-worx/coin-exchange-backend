using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Infrastructure.Services
{
    /// <summary>
    /// Stub implementation for trade id generation service
    /// </summary>
    public class StubbedTradeIdGenerator:ITradeIdGenerator
    {
        public TradeId GenerateTradeId()
        {
            return new TradeId((int)DateTime.Now.Ticks);
        }
    }
}
