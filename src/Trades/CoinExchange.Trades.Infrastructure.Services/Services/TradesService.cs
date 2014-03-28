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
            orderList.Add(new Order()
                              {
                                  TxId = "EEER342",
                                  UserRefId = "WREDF342",
                                  Pair = "XBTUSD",
                                  Status = OrderStatus.Open,
                                  OpenTime = DateTime.Now.AddHours(-2),
                                  ExpireTime = DateTime.Now.AddHours(3),
                                  Volume = 3000,
                                  Cost = 0,
                                  Fee = (decimal?) 0.25,
                                  Price = (decimal?) 491.23,
                                  StopPrice = 0,
                                  LimitPrice = 0,
                                  OFlags = "DUMMY",
                                  Trades = "23453,1764,1554,2134",
                                  Opened = "045643.23"
                              });
            orderList.Add(new Order()
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
                                Price = (decimal?)491.23,
                                StopPrice = 0,
                                LimitPrice = 0,
                                OFlags = "DUMMY",
                                Trades = "23453,1764,1554,2134",
                                Opened = "045643.23"
                            });
            orderList.Add(new Order()
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
                                Price = (decimal?)491.23,
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
        public List<Order> GetClosedOrders()
        {
            List<Order> orderList = new List<Order>();
            orderList.Add(new Order()
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new Order()
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new Order()
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
                Price = (decimal?)491.23,
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
        /// Gets the list for the Open orders
        /// </summary>
        /// <returns></returns>
        public List<Order> GetTradesHistory()
        {
            List<Order> orderList = new List<Order>();
            orderList.Add(new Order()
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Executed = "045643.23",
                VolumeExecuted = 2000
            });
            orderList.Add(new Order()
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Executed = "045643.23",
                VolumeExecuted = 2000
            });
            orderList.Add(new Order()
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Executed = "045643.23",
                VolumeExecuted = 2000
            });

            return orderList;
        }
    }
}
