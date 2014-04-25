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
        IList<TradeReadModel> GetTraderTradeHistory(string traderId);

        /// <summary>
        /// Get by Trade Id
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        TradeReadModel GetById(string tradeId);

    }
}
