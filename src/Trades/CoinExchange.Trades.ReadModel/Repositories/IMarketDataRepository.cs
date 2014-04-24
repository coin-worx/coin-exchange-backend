using System.Collections.Generic;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Repositories
{
    public interface IMarketDataRepository
    {
        /// <summary>
        /// Get ohlc data for currency pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="interval">In minutes default is one</param>
        /// <param name="since">Id to poll next commited data</param>
        /// <returns></returns>
        List<object> GetOhlc(string currencyPair,int interval=1,string since="");

        /// <summary>
        /// Ticker info for a currency pair
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        TickerInfoReadModel GetTickerInfo(string currencyPair);
        
    }
}
