using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Services
{
    public interface IMarketDataQueryService
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
