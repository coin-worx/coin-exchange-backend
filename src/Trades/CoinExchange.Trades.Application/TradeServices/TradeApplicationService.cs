using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.TradeServices
{
    public class TradeApplicationService : ITradeApplicationService
    {
        private ITradeRepository _tradeRepository;
        private ICurrencyPairRepository _currencyPairRepository;
        private IOrderRepository _orderRepository;

        public TradeApplicationService(ITradeRepository tradeRepository,ICurrencyPairRepository currencyPairRepository,IOrderRepository orderRepository)
        {
            _tradeRepository = tradeRepository;
            _currencyPairRepository = currencyPairRepository;
            _orderRepository = orderRepository;
        }

        public object GetTradesHistory(TraderId traderId, string start = "", string end = "")
        {
            if (start == "" || end == "")
            {
                return _tradeRepository.GetTraderTradeHistory(traderId.Id.ToString());
            }
            return _tradeRepository.GetTraderTradeHistory(traderId.Id.ToString(), Convert.ToDateTime(start),
                Convert.ToDateTime(end));
        }

        public object QueryTrades(string orderId)
        {
            return _tradeRepository.GetTradesByorderId(orderId);
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

        /// <summary>
        /// Trade Details
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        public TradeDetailsRepresentation GetTradeDetails(string traderId, string tradeId)
        {
            TradeReadModel model = _tradeRepository.GetByIdAndTraderId(traderId,tradeId);
            OrderReadModel buyOrder = _orderRepository.GetOrderById(model.BuyOrderId);
            OrderReadModel sellOrder = _orderRepository.GetOrderById(model.SellOrderId);
            if (buyOrder.TraderId == traderId)
            {
                return new TradeDetailsRepresentation(buyOrder,model.ExecutionDateTime,model.Price,model.Volume,model.TradeId);
            }
            return new TradeDetailsRepresentation(sellOrder,model.ExecutionDateTime,model.Price,model.Volume,model.TradeId);
        }
    }
}
