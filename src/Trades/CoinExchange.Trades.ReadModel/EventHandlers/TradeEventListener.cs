using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.EventHandlers
{
    /// <summary>
    /// Event listener for Trade
    /// </summary>
    public class TradeEventListener
    {
        private IPersistanceRepository _persistanceRepository;

        public TradeEventListener(IPersistanceRepository persistanceRepository)
        {
            _persistanceRepository = persistanceRepository;
            TradeEvent.TradeOccured += OnTradeArrived;
        }

        /// <summary>
        /// Receives trade when events are raised
        /// </summary>
        /// <param name="trade"></param>
        void OnTradeArrived(Trade trade)
        {
            _persistanceRepository.SaveOrUpdate(ReadModelAdapter.GetTradeReadModel(trade));
        }
    }
}
