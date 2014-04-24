using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Services
{
    public interface ITradeQueryService
    {
        /// <summary>
        /// Public call for getting recent trades
        /// </summary>
        /// <param name="lastId"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        List<object> GetRecentTrades(string lastId,string pair);

        /// <summary>
        /// To get traders trade history
        /// </summary>
        /// <param name="traderId"></param>
        /// <returns></returns>
        List<TradeReadModel> GetTraderTradeHistory(string traderId);

    }
}
