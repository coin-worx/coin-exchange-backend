using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Handles the change in the state of any order and publishes to the output disruptor
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
            OutputDisruptor.Publish(order);
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
            /*OutputDisruptor.Publish(order);
            Log.Debug("Order Accepted: " + order.ToString() + " | Published to output Disruptor");*/
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
            /*// Publish the order that was just received from the user and got filled
            OutputDisruptor.Publish(inboundOrder);
            Log.Debug("Order Filled: " + inboundOrder.ToString() + " | Published to output Disruptor.");

            // Publish the order that was on the order book and got matched with the incoming order
            OutputDisruptor.Publish(matchedOrder);
            Log.Debug("Order Filled: " + matchedOrder.ToString() + " | Published to output Disruptor.");*/
        }

        /// <summary>
        /// handles the event in case an order gets cancelled
        /// </summary>
        /// <param name="order"> </param>
        public void OnOrderCancelled(Order order)
        {
            /*OutputDisruptor.Publish(order);
            Log.Debug("Order Cancelled: " + order.ToString() + " | Published to output Disruptor.");*/
        }
    }
}
