using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.TradeServices
{
    public class TradeApplicationService:ITradeApplicationService
    {
        private ITradeRepository _tradeRepository;

        public List<OrderRepresentation> GetTradesHistory(TraderId traderId, string offset = "", string type = "all", bool trades = false, string start = "", string end = "")
        {
            throw new NotImplementedException();
        }

        public List<OrderRepresentation> QueryTrades(TraderId traderId, string txId = "", bool includeTrades = false)
        {
            throw new NotImplementedException();
        }

        public IList<object> GetRecentTrades(string pair, string since)
        {
            return _tradeRepository.GetRecentTrades(since, pair);
        }

        public TradeVolumeRepresentation TradeVolume(string pair)
        {
            throw new NotImplementedException();
        }
    }
}
