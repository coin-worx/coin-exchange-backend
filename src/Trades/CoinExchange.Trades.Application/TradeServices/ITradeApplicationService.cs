using System.Collections.Generic;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Application.TradeServices
{
    /// <summary>
    /// Interface for Trade Services
    /// </summary>
    public interface ITradeApplicationService
    {
        object GetTradesHistory(TraderId traderId, string start = "", string end = "");

        object QueryTrades(TraderId traderId, string txId = "", bool includeTrades = false);
        IList<object> GetRecentTrades(string pair, string since);
        TradeVolumeRepresentation TradeVolume(string pair);
        IList<CurrencyPair> GetTradeableCurrencyPairs();
    }
}
