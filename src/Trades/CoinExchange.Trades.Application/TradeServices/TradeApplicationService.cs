using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.TradeServices
{
    public class TradeApplicationService : ITradeApplicationService
    {
        private ITradeRepository _tradeRepository;
        private ICurrencyPairRepository _currencyPairRepository;

        public TradeApplicationService(ITradeRepository tradeRepository,ICurrencyPairRepository currencyPairRepository)
        {
            _tradeRepository = tradeRepository;
            _currencyPairRepository = currencyPairRepository;
        }

        public object GetTradesHistory(TraderId traderId, string offset = "", string type = "all", bool trades = false, string start = "", string end = "")
        {
            if (start == "" || end == "")
            {
                return QueryTrades(traderId);
            }
            return _tradeRepository.GetTraderTradeHistory(traderId.Id.ToString(), Convert.ToDateTime(start),
                Convert.ToDateTime(end));
        }

        public object QueryTrades(TraderId traderId, string txId = "", bool includeTrades = false)
        {
            return _tradeRepository.GetTraderTradeHistory(traderId.Id.ToString());
        }

        public IList<object> GetRecentTrades(string pair, string since)
        {
            return _tradeRepository.GetRecentTrades(since, pair);
        }

        public TradeVolumeRepresentation TradeVolume(string pair)
        {
            throw new NotImplementedException();
        }


        public IList<CurrencyPair> GetTradeableCurrencyPairs()
        {
            return _currencyPairRepository.GetTradeableCurrencyPairs();
        }
    }
}
