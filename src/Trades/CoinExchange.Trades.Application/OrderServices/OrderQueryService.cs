using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Representation;
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

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderQueryService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        #region Implementation of IOrderQueryService

        /// <summary>
        /// Gets a List of open orders represented as ReadModel
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="includeTrades"></param>
        /// <param name="userRefId"></param>
        /// <returns></returns>
        public object GetOpenOrders(TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            return _orderRepository.GetOpenOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets a List of closed orders represented as ReadModel
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="includeTrades"></param>
        /// <param name="userRefId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="offset"></param>
        /// <param name="closetime"></param>
        /// <returns></returns>
        public object GetClosedOrders(TraderId traderId, bool includeTrades = false, string userRefId = "", string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            return _orderRepository.GetClosedOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
