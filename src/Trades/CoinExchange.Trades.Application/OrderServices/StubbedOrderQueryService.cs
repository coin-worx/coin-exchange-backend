using System;
using System.Collections.Generic;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Application.OrderServices
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
        public List<OrderRepresentation> GetOpenOrders(TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            List<OrderRepresentation> orderList = new List<OrderRepresentation>();
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "WREDF342",
                Pair = "XBTUSD",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "YIO468S",
                Pair = "XBTEURS",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
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
        public List<OrderRepresentation> GetClosedOrders(TraderId traderId, bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            List<OrderRepresentation> orderList = new List<OrderRepresentation>();
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "WREDF342",
                Pair = "XBTUSD",
                Status = OrderState.Cancelled,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "YIO468S",
                Pair = "XBTEURS",
                Status = OrderState.Expired,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
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
