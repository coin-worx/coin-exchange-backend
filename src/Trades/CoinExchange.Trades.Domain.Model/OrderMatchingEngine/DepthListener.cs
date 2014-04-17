using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the changes in the depth of the Order Book
    /// </summary>
    public class DepthListener : IDepthListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Implementation of IDepthListener

        public void OnDepthChanged(Depth depth)
        {
            Log.Debug("Depth changed for currency pair: " + depth.CurrencyPair);
        }

        #endregion
    }
}
