using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.Order.Commands;
using CoinExchange.Trades.Application.Order.Representation;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.Order
{
    /// <summary>
    /// Serves the operations for resources related to Order
    /// </summary>
    public class OrderApplicationService:IOrderApplicationService
    {
        /// <summary>
        /// Cancel the order of user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CancelOrderResponse CancelOrder(string txid)
        {
            return new CancelOrderResponse(true, 2);
        }

        /// <summary>
        /// Cancel the order of user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            return new NewOrderRepresentation(orderCommand.Price, orderCommand.Type, orderCommand.Side, orderCommand.Pair,
                DateTime.Now.ToString());
        }
    }
}
