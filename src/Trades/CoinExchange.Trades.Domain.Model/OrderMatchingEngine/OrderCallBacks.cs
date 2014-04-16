using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    public delegate void OrderAccepted(Order.Order order, int trasactionId);

    public delegate void OrderRejected(Order.Order order, int trasactionId, string reason);

    /// <summary>
    /// Defines all the Callbacks that will serve as the notifications to the client
    /// </summary>
    public class OrderCallBacks
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Signifies that the Order has been accepted successfully
        /// </summary>
        public void Accept(Order.Order order, int trasactionId)
        {
            Log.Debug("Order Accepted by Exchange. " + order.ToString());
            // ToDo: Send the notification back to the client
        }

        /// <summary>
        /// Signifies that the Order has been rejected by the Exchange
        /// </summary>
        public void Reject(Order.Order order, int trasactionId)
        {
            Log.Debug("Order rejected by Exchange. " + order.ToString());
        }
    }
}
