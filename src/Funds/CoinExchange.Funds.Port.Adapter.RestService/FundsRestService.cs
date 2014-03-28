using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model.Entities;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Funds.Infrastructure.Services;

namespace CoinExchange.Funds.Port.Adapter.RestService
{
    /// <summary>
    /// Rest service for serving requests related to Trades
    /// </summary>
    public class FundsRestService
    {
        /// <summary>
        /// Gives access to the service responsible for carrying out operations for the Private Controller
        /// </summary>
        private FundsService _fundsRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FundsRestService()
        {
            _fundsRepository = new FundsService();
        }

        /// <summary>
        /// Returns the Account balance for a number of currencies for a particular user
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. AssetClass(string): e.g., currency (optional)
        /// 3. asset(string): e.g., LTC (optional)
        /// </summary>
        /// <returns></returns>
        public AccountBalance[] AccountBalance(TraderId traderId, string assetClass = null, string asset = null)
        {
            return _fundsRepository.GetAccountBalance();
        }

        /// <summary>
        /// Returns the Trade balance for a number of currencies for a particular user
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. AssetClass(string): e.g., currency (optional)
        /// 3. asset(string): e.g., LTC (optional)
        /// </summary>
        /// <returns></returns>
        public TradeBalance TradeBalance(TraderId traderId, string assetClass = null, string asset = null)
        {
            return _fundsRepository.GetTradeBalance();
        }

        /// <summary>
        /// Returns the queried Ledger Info for all or the specified ledger for the given Trader
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. LedgerInfoRequest ledger[FromUri]: Member = AssetClass, Asset, Type, StartTime, EndTime, Offset (optional)
        /// </summary>
        /// <returns></returns>
        public LedgerInfo[] LedgerInfo(TraderId traderId, LedgerInfoRequest ledger)
        {
            return _fundsRepository.GetLedgers();
        }

        /// <summary>
        /// Accepts a comma separated list of ledgers, if no id is provided, returns all the ledgers. If no ID is given, 
        /// behaves the same as a LedgersInfo call.
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody] : Contains an Id of the trader, used for authentication of the trader(required)
        /// 2. LedgerIds(int): comma separated list of ledgers (optional)
        /// </summary>
        /// <returns></returns>
        public LedgerInfo[] FetchLedgers([FromBody]TraderId traderId, string ledgerIds = "")
        {
            // ToDo: In the next sprint related to business logc behind RESTful calls, need to split the ledgersIds comma
            // separated list
            return _fundsRepository.GetLedgers();
        }
    }
}
