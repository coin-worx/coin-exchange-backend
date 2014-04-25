using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to the change in the state of any order
    /// </summary>
    [Serializable]
    public class OrderListener : IOrderListener
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Handles the OrderBook's event in case the state of an Order changes
        /// </summary>
        /// <param name="order"> </param>
        public void OnOrderChanged(Order order)
        {
            Log.Debug("Order change received: " + order.ToString());
        }

        /// <summary>
        /// When Order gets accepted
        /// </summary>
        /// <param name="order"></param>
        /// <param name="matchedPrice"></param>
        /// <param name="matchedVolume"></param>
        public void OnOrderAccepted(Order order, Price matchedPrice, Volume matchedVolume)
        {
            //ToDo: Create event for the order and publish the event on the output Disruptor
            throw new NotImplementedException();
        }

        /// <summary>
        /// When Order gets filled
        /// </summary>
        /// <param name="inboundOrder"></param>
        /// <param name="matchedOrder"></param>
        /// <param name="fillFlags"></param>
        /// <param name="filledPrice"></param>
        /// <param name="filledVolume"></param>
        public void OnOrderFilled(Order inboundOrder, Order matchedOrder, FillFlags fillFlags, Price filledPrice, Volume filledVolume)
        {
            // Dispatch both of the orders by creating events for them by using order instances themselves and then publish
            // both the events on the output disruptor
            throw new NotImplementedException();
        }

        /// <summary>
        /// handles the event in case an order gets cancelled
        /// </summary>
        /// <param name="order"> </param>
        public void OnOrderCancelled(Order order)
        {
            //ToDo: Create event for the order and publish the event on the output Disruptor
            throw new NotImplementedException();
        }
    }
}
