using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Listens to any changes in state of an Order
    /// </summary>
    public interface IOrderListener
    {
        /// <summary>
        /// Handles the event raised due to the change in the state of an Order
        /// </summary>
        /// <param name="order"></param>
        void OnOrderChanged(Order order);

        /// <summary>
        /// Handles the event in case an order gets accepted
        /// </summary>
        /// <param name="order"></param>
        /// <param name="matchedPrice"></param>
        /// <param name="matchedVolume"></param>
        void OnOrderAccepted(Order order, Price matchedPrice, Volume matchedVolume);

        /// <summary>
        /// Handles the event in case an order gets filled
        /// </summary>
        /// <param name="inboundOrder"></param>
        /// <param name="matchedOrder"> </param>
        /// <param name="fillFlags"> </param>
        /// <param name="filledPrice"></param>
        /// <param name="filledVolume"></param>
        void OnOrderFilled(Order inboundOrder, Order matchedOrder, FillFlags fillFlags, Price filledPrice, Volume filledVolume);

        /// <summary>
        /// Handles the event in case the order gets cancelled
        /// </summary>
        /// <param name="order"> </param>
        void OnOrderCancelled(Order order);
    }
}
