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
    public class TradeListener : ITradeListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Implementation of ITradeListener

        public void OnTrade(Trade trade)
        {
            Log.Debug("Trade received: " + trade.ToString());
        }

        #endregion
    }
}
