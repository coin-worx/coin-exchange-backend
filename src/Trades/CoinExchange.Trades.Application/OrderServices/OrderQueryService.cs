using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Queries information for the orders
    /// </summary>
    public class OrderQueryService : IOrderQueryService
    {
        private IOrderRepository _orderRepository = null;
        private ITradeRepository _tradeRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderQueryService(IOrderRepository orderRepository,ITradeRepository tradeRepository)
        {
            _orderRepository = orderRepository;
            _tradeRepository = tradeRepository;
        }

        #region Implementation of IOrderQueryService

        /// <summary>
        /// Gets a List of open orders represented as ReadModel
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="includeTrades"></param>
        /// <returns></returns>
        public object GetOpenOrders(TraderId traderId, bool includeTrades = false)
        {
            List<OrderReadModel> orders = _orderRepository.GetOpenOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
            for (int i = 0; i < orders.Count; i++)
            {
                IList<object> trades = _tradeRepository.GetTradesByorderId(orders[i].OrderId);
                orders[i].AveragePrice = CalculateAveragePrice(trades);
                if (includeTrades)
                {
                    orders[i].Trades = trades;
                }
            }
            return orders;
        }

        /// <summary>
        /// Gets a List of closed orders represented as ReadModel
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="includeTrades"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public object GetClosedOrders(TraderId traderId, bool includeTrades = false, string startTime = "", string endTime = "")
        {
            List<OrderReadModel> orders;
            if (startTime == "" || endTime == "")
            {
                orders =_orderRepository.GetClosedOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                orders = _orderRepository.GetClosedOrders(traderId.Id.ToString(), Convert.ToDateTime(startTime),
                    Convert.ToDateTime(endTime));
            }
            for (int i = 0; i < orders.Count; i++)
            {
                IList<object> trades = _tradeRepository.GetTradesByorderId(orders[i].OrderId);
                orders[i].AveragePrice = CalculateAveragePrice(trades);
                if (includeTrades)
                {
                    orders[i].Trades = trades;
                }
            }
            return orders;
        }

        #endregion
        
        public object GetOrderById(TraderId traderId,OrderId orderId)
        {
            OrderReadModel order = _orderRepository.GetOrderById(traderId, orderId);
            order.AveragePrice = CalculateAveragePrice(_tradeRepository.GetTradesByorderId(orderId.Id));
            return order;
        }

        /// <summary>
        /// Calculate avergae price from trades.
        /// </summary>
        /// <returns></returns>
        private decimal CalculateAveragePrice(IList<object> trades)
        {
            decimal price = 0;
            if (trades != null)
            {
                if (trades.Count > 0)
                {
                    for (int i = 0; i < trades.Count; i++)
                    {
                        object[] trade = trades[i] as object[];
                        price = price + (decimal) trade[2];
                    }
                    price = price/trades.Count;
                }
            }
            return price;
        }
    }
}
