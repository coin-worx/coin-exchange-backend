using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using CoinExchange.Trades.ReadModel.Services;

namespace CoinExchange.Trades.ReadModel.EventHandlers
{
    /// <summary>
    /// Event listener for Trade
    /// </summary>
    public class TradeEventListener
    {
        private IPersistanceRepository _persistanceRepository;
        private OhlcCalculation _ohlcCalculation;

        public TradeEventListener(IPersistanceRepository persistanceRepository,OhlcCalculation ohlcCalculation)
        {
            _persistanceRepository = persistanceRepository;
            _ohlcCalculation = ohlcCalculation;
            TradeEvent.TradeOccured += OnTradeArrived;
        }

        /// <summary>
        /// Receives trade when events are raised
        /// </summary>
        /// <param name="trade"></param>
        void OnTradeArrived(Trade trade)
        {
            _persistanceRepository.SaveOrUpdate(ReadModelAdapter.GetTradeReadModel(trade));
            _ohlcCalculation.CalculateAndPersistOhlc(trade);
        }
    }
}
