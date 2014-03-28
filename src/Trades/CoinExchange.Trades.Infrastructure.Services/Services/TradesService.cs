using System;
using System.Collections.Generic;
using CoinExchange.Trades.Domain.Model.Entities;
using CoinExchange.Trades.Domain.Model.Enums;

namespace CoinExchange.Trades.Infrastructure.Services.Services
{
    /// <summary>
    /// Service to serve operations related to Trades
    /// </summary>
    public class TradesService
    {
        /// <summary>
        /// Gets the list for the Open orders
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOpenOrders()
        {
            List<Order> orderList = new List<Order>();
            orderList.Add(new Order(1, "XBTUSD", 1000, (decimal)459.23));
            orderList.Add(new Order(2, "LTCUSD", 2000, (decimal)469.23));
            orderList.Add(new Order(3, "XXRTUSD", 3000, (decimal)479.23));

            return orderList;
        }

        /// <summary>
        /// Gets the list for the Open orders
        /// </summary>
        /// <returns></returns>
        public List<Order> GetClosedOrders()
        {
            List<Order> orderList = new List<Order>();
            orderList.Add(new Order()
            {
                UserRefId = "weqweew",
                Status = OrderStatus.Canceled,
                Reason = "Insufficient Volume",
                IsSell = true,
                Price = (decimal?)658.3422,
                Volume = 2,
                StartTime = DateTime.Now.AddDays(-1),
                ExpireTime = DateTime.Now.AddDays(1),
                OrderType = OrderType.Limit
            });

            orderList.Add(new Order()
            {
                UserRefId = "edeewa",
                Status = OrderStatus.Canceled,
                Reason = "Insufficient Volume",
                IsSell = true,
                Price = (decimal?)658.3422,
                Volume = 2,
                StartTime = DateTime.Now.AddDays(-1),
                ExpireTime = DateTime.Now.AddDays(1),
                OrderType = OrderType.Limit
            });

            orderList.Add(new Order()
            {
                UserRefId = "rtfdee",
                Status = OrderStatus.Canceled,
                Reason = "Insufficient Volume",
                IsSell = true,
                Price = (decimal?)658.3422,
                Volume = 2,
                StartTime = DateTime.Now.AddDays(-1),
                ExpireTime = DateTime.Now.AddDays(1),
                OrderType = OrderType.Limit
            });

            return orderList;
        }
    }
}
