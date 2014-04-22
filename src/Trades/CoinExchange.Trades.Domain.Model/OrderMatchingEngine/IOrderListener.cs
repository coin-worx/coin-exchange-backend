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
    }
}
