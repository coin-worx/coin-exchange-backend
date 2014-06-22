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
                Tuple<decimal, DateTime?> data = CalculateAveragePrice(trades);
                orders[i].AveragePrice = data.Item1;
                if (orders[i].Status == OrderState.Complete.ToString())
                {
                    orders[i].ClosingDateTime = data.Item2;
                }
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
                Tuple<decimal, DateTime?> data = CalculateAveragePrice(trades);
                orders[i].AveragePrice = data.Item1;
                if (orders[i].Status == OrderState.Complete.ToString())
                {
                    orders[i].ClosingDateTime = data.Item2;
                }
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
            Tuple<decimal, DateTime?> data = CalculateAveragePrice(_tradeRepository.GetTradesByorderId(orderId.Id));
            order.AveragePrice = data.Item1;
            if (order.Status == OrderState.Complete.ToString())
            {
                order.ClosingDateTime = data.Item2;
            }
            return order;
        }

        /// <summary>
        /// Calculate avergae price from trades.
        /// </summary>
        /// <returns></returns>
        private Tuple<decimal,DateTime?> CalculateAveragePrice(IList<object> trades)
        {
            decimal price = 0;
            DateTime? closingDateTime=null;
            if (trades != null)
            {
                if (trades.Count > 0)
                {
                    for (int i = 0; i < trades.Count; i++)
                    {
                        object[] trade = trades[i] as object[];
                        price = price + (decimal) trade[2];
                        if (i == 0)
                        {
                            closingDateTime = (DateTime) trade[1];
                        }
                        else
                        {
                            if ((DateTime) trade[1] >= closingDateTime)
                            {
                                closingDateTime = (DateTime) trade[1];
                            }
                        }
                    }
                    price = price/trades.Count;
                }
            }
            return new Tuple<decimal, DateTime?>(price,closingDateTime);
        }
    }
}
