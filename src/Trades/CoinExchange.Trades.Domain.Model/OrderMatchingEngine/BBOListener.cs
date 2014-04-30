using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the best bid and best ask on the Order Book
    /// </summary>
    [Serializable]
    public class BBOListener : IBBOListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Implementation of IBBOListener

        /// <summary>
        /// Onn BBO changed event
        /// </summary>
        /// <param name="bbo"> </param>
        public void OnBBOChange(BBO bbo)
        {
            OutputDisruptor.Publish(bbo);
            Log.Debug("Best bid and offer received for currency pair: " + bbo.CurrencyPair);
        }

        #endregion
    }
}
