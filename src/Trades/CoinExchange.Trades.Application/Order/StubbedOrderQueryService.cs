using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Application.Order
{
    /// <summary>
    /// Handles the query requests related to Orders
    /// </summary>
    public class StubbedOrderQueryService:IOrderQueryService
    {
        /// <summary>
        /// Gets the list for the Open orders
        /// </summary>
        /// <returns></returns>
        public List<Domain.Model.Order.Order> GetOpenOrders(TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            List<Domain.Model.Order.Order> orderList = new List<Domain.Model.Order.Order>();
            orderList.Add(new Domain.Model.Order.Order()
            {
                TxId = "EEER342",
                UserRefId = "WREDF342",
                Pair = "XBTUSD",
                Status = OrderStatus.Open,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });
            orderList.Add(new Domain.Model.Order.Order()
            {
                TxId = "EEER342",
                UserRefId = "YIO468S",
                Pair = "XBTEURS",
                Status = OrderStatus.Open,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });
            orderList.Add(new Domain.Model.Order.Order()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderStatus.Open,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });

            return orderList;
        }

        /// <summary>
        /// Gets the list for the Closed orders
        /// </summary>
        /// <returns></returns>
        public List<Domain.Model.Order.Order> GetClosedOrders(TraderId traderId, bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            List<Domain.Model.Order.Order> orderList = new List<Domain.Model.Order.Order>();
            orderList.Add(new Domain.Model.Order.Order()
            {
                TxId = "EEER342",
                UserRefId = "WREDF342",
                Pair = "XBTUSD",
                Status = OrderStatus.Canceled,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new Domain.Model.Order.Order()
            {
                TxId = "EEER342",
                UserRefId = "YIO468S",
                Pair = "XBTEURS",
                Status = OrderStatus.Expired,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new Domain.Model.Order.Order()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderStatus.Closed,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });

            return orderList;
        }

        /// <summary>
        /// Returns the Order Book
        /// </summary>
        /// <returns></returns>
        public List<object> GetOrderBook(string symbol, int count)
        {
            List<object> list = new List<object>();
            list.Add(symbol);
            list.Add("asks");
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "32", "1000", "204832014" });
            list.Add(new object[] { "34", "1000", "204832014" });

            list.Add("bids");
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "23", "1000", "204832014" });

            return list;
        }
    }
}
