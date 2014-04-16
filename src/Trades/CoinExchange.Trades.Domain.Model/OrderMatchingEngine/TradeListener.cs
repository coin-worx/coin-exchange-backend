using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the Trades that are executed by the Order Book
    /// </summary>
    public class TradeListener
    {
        /// <summary>
        /// Event handler for the event of Trade Execution by the Order Book
        /// </summary>
        public void OnTradeExecuted(Trade trade)
        {
            
        }
    }
}
