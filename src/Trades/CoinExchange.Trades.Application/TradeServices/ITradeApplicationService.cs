using System.Collections.Generic;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Application.TradeServices
{
    /// <summary>
    /// Interface for Trade Services
    /// </summary>
    public interface ITradeApplicationService
    {
        List<Domain.Model.Order.Order> GetTradesHistory(TraderId traderId, string offset = "", string type = "all",
            bool trades = false, string start = "", string end = "");

        List<Domain.Model.Order.Order> QueryTrades(TraderId traderId, string txId = "", bool includeTrades = false);
        TradeListRepresentation GetRecentTrades(TraderId traderId, string pair, string since);
        TradeVolumeRepresentation TradeVolume(string pair);
    }
}
