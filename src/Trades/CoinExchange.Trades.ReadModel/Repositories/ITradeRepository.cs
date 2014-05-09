using System;
using System.Collections.Generic;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Repositories
{
    public interface ITradeRepository
    {
        /// <summary>
        /// Public call for getting recent trades
        /// </summary>
        /// <param name="lastId"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        IList<object> GetRecentTrades(string lastId,string pair);

        /// <summary>
        /// To get traders trade history
        /// </summary>
        /// <param name="traderId"></param>
        /// <returns></returns>
        IList<object> GetTraderTradeHistory(string traderId);

        /// <summary>
        /// Get trades between specified date
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IList<object> GetTraderTradeHistory(string traderId, DateTime start, DateTime end);

        /// <summary>
        /// Get by Trade Id
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        TradeReadModel GetById(string tradeId);

        /// <summary>
        /// Get trades between dates
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        IList<TradeReadModel> GetTradesBetweenDates(DateTime end, DateTime start,string currencyPair);

        /// <summary>
        /// Get custom calculated data for ticker info calculation
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        object GetCustomDataBetweenDates(DateTime end, DateTime start,string currencyPair);

    }
}
