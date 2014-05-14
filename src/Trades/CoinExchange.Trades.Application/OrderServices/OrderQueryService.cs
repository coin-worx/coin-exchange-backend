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
            if (includeTrades)
            {
                for (int i = 0; i < orders.Count;i++)
                {
                    orders[i].Trades = _tradeRepository.GetTradesByorderId(orders[i].OrderId);
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
            List<OrderReadModel> orders=_orderRepository.GetClosedOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
            if (includeTrades)
            {
                for (int i = 0; i < orders.Count; i++)
                {
                    orders[i].Trades = _tradeRepository.GetTradesByorderId(orders[i].OrderId);
                }
            }
            return orders;
        }

        #endregion
        
        public object GetOrderById(TraderId traderId,OrderId orderId)
        {
            return _orderRepository.GetOrderById(traderId,orderId);
        }
    }
}
