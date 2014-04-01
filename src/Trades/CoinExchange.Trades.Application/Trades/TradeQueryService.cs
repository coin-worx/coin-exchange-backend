using System;
using System.Collections.Generic;
using CoinExchange.Trades.Application.Trades.Representation;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.Trades
{
    /// <summary>
    /// Service to serve operations related to Trades
    /// </summary>
    public class TradeQueryService
    {
        /// <summary>
        /// Gets the list for the Open orders
        /// </summary>
        /// <returns></returns>
        public List<Domain.Model.Order.Order> GetTradesHistory(string offset = "", string type = "all",
            bool trades = false, string start = "", string end = "")
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Executed = "045643.23",
                VolumeExecuted = 2000
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
                Price = (decimal?)491.23,
                StopPrice = 0,
                LimitPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Executed = "045643.23",
                VolumeExecuted = 2000
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

        /// <summary>
        /// Recent trades info
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        public TradeListRepresentation GetRecentTrades(string pair, string since)
        {
            List<TradeRecord> list = new List<TradeRecord>();
            TradeRecord entries = new TradeRecord(123.33m, 200, "Buy", "Limit", "", DateTime.UtcNow.ToString());
            for (int i = 0; i < 5; i++)
            {
                list.Add(entries);
            }
            TradeListRepresentation representation = new TradeListRepresentation("234", list, pair);
            return representation;
        }

        /// <summary>
        /// Trade volum request handler
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TradeVolumeRepresentation TradeVolume(string pair)
        {
            TradeFeeRepresentation fees = new TradeFeeRepresentation(100m, 234m, 34.5m, 25.5m, 23.5m, 0.005m);
            TradeVolumeRepresentation response = new TradeVolumeRepresentation(fees, 1000, "ZUSD");
            return response;
        }

        
    }
}
