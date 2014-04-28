using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Domain service for trade id generation
    /// </summary>
    public interface ITradeIdGenerator
    {
        TradeId GenerateTradeId();
    }
}
