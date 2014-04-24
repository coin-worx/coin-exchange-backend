using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Interface for handling Trades when they are executed
    /// </summary>
    public interface ITradeListener
    {
        void OnTrade(Trade trade);

        List<Trade> Trades { get; }
    }
}
