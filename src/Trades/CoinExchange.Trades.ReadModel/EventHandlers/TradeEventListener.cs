using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
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
        private TickerInfoCalculation _tickerInfoCalculation;
        private IBalanceValidationService _balanceValidationService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="persistanceRepository"></param>
        /// <param name="ohlcCalculation"></param>
        /// <param name="tickerInfoCalculation"></param>
        /// <param name="balanceValidationService"></param>
        public TradeEventListener(IPersistanceRepository persistanceRepository,
            OhlcCalculation ohlcCalculation, TickerInfoCalculation tickerInfoCalculation,
            IBalanceValidationService balanceValidationService)
        {
            _persistanceRepository = persistanceRepository;
            _ohlcCalculation = ohlcCalculation;
            _tickerInfoCalculation = tickerInfoCalculation;
            _balanceValidationService = balanceValidationService;
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
            _tickerInfoCalculation.CalculateTickerInfo(trade);
            Tuple<string, string> currencies = CurrencySplitterService.SplitCurrencyPair(trade.CurrencyPair);
            // Update the balance on hte Funds BC
            _balanceValidationService.TradeExecuted(currencies.Item1, currencies.Item2, trade.ExecutedVolume.Value,
                trade.ExecutionPrice.Value, trade.ExecutionTime, trade.TradeId.Id, trade.BuyOrder.TraderId.Id, 
                trade.SellOrder.TraderId.Id, trade.BuyOrder.OrderId.Id, trade.SellOrder.OrderId.Id);
        }
    }
}
