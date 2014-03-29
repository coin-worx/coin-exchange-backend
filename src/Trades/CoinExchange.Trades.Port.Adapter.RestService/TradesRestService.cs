using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Trades.Domain.Model.Entities;
using CoinExchange.Trades.Domain.Model.VOs;
using CoinExchange.Trades.Infrastructure.Services.Services;

namespace CoinExchange.Trades.Port.Adapter.RestService
{
    /// <summary>
    /// Rest service for serving requests related to Trades
    /// </summary>
    public class TradesRestService : ApiController
    {
        private TradesService _tradesService;

        public TradesRestService()
        {
            _tradesService = new TradesService();
        }

        /// <summary>
        /// Returns orders that have not been executed but those that have been accepted on the server. Exception can be 
        /// provided in the second parameter
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        public List<Order> OpenOrderList(TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            // ToDo: In the next sprint related to business logic behind RESTful calls, need to split the ledgersIds comma
            // separated list
            return _tradesService.GetOpenOrders();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        public List<Order> GetClosedOrders(TraderId traderId, bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            return _tradesService.GetClosedOrders();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="offset">Result offset</param>
        /// <param name="type">Type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        /// <param name="trades">Whether or not to include trades related to position in output (optional.  default = false)</param>
        /// <param name="start">Starting unix timestamp or trade tx id of results (optional.  exclusive)</param>
        /// <param name="end">Ending unix timestamp or trade tx id of results (optional.  inclusive)</param>
        /// </summary>
        /// <returns></returns>
        public List<Order> GetTradeHistory([FromBody]TraderId traderId, string offset = "", string type = "all",
            bool trades = false, string start = "", string end = "")
        {
            return _tradesService.GetTradesHistory();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="traderId">Trader ID</param>
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        public List<Order> FetchQueryTrades([FromBody]TraderId traderId, string txId = "", bool includeTrades = false)
        {
            return _tradesService.GetTradesHistory();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="traderId">Trader ID</param>
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        public List<Order> TradeBalance([FromBody]TraderId traderId, string txId = "", bool includeTrades = false)
        {
            return _tradesService.GetTradesHistory();
        }

        /// <summary>
        /// Recent trades info
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        public TradeInfo GetRecentTrades(string pair, string since)
        {
            List<TradeEntries> list = new List<TradeEntries>();
            TradeEntries entries = new TradeEntries(123.33m, 200, "Buy", "Limit", "", DateTime.UtcNow.ToString());
            for (int i = 0; i < 5; i++)
            {
                list.Add(entries);
            }
            TradeInfo tradeInfo=new TradeInfo("234",list,pair);
            return tradeInfo;

        }

        /// <summary>
        /// Cancel the order of user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CancelOrderResponse CancelOrder(CancelOrderRequest request)
        {
            return new CancelOrderResponse(true,2);
            
        }

        /// <summary>
        /// Trade volum request handler
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TradeVolumeResponse TradeVolume(TradeVolumeRequest request)
        {
            Fees fees = new Fees(100m, 234m, 34.5m, 25.5m, 23.5m, 0.005m);
            TradeVolumeResponse response=new TradeVolumeResponse(fees,1000,"ZUSD");
            return response;
        }

        public List<TradeableAsset> TradeabelAssetPair(string info, string pair)
        {
            AssetFee fee=new AssetFee(0.002m,100);
            List<AssetFee> fees=new List<AssetFee>();
            for (int i = 0; i < 10; i++)
            {
                fees.Add(fee);
            }
            TradeableAsset asset=new TradeableAsset(10,10,"USD",fees,null,10,2,2,"Unit","XXDG","currency","currency","LTCXDG");
            List<TradeableAsset> assets=new List<TradeableAsset>();
            for (int i = 0; i < 10; i++)
            {
                assets.Add(asset);
            }
            return assets;
        }
    }
}
