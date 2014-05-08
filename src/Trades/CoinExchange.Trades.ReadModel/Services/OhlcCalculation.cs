using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.MarketDataAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.Services
{
    /// <summary>
    /// Service for calculating one minute Ohlc on new trade arrival
    /// </summary>
    public class OhlcCalculation
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPersistanceRepository _persistanceRepository;
        private ITradeRepository _tradeRepository;
        private IOhlcRepository _ohlcRepository;
        
        public OhlcCalculation(IPersistanceRepository persistanceRepository,IOhlcRepository ohlcRepository,ITradeRepository tradeRepository)
        {
            _persistanceRepository = persistanceRepository;
            _ohlcRepository = ohlcRepository;
            _tradeRepository = tradeRepository;
        }

        public void CalculateAndPersistOhlc(Trade latestTrade)
        {
            DateTime ohlcdDateTime = latestTrade.ExecutionTime.AddSeconds(-1*latestTrade.ExecutionTime.Second);
            //IList<TradeReadModel> trades=_tradeRepository.GetTradesBetweenDates(latestTrade.ExecutionTime,ohlcdDateTime);
            OhlcReadModel model = _ohlcRepository.GetOhlcByDateTime(ohlcdDateTime.AddMinutes(1));
            if (model == null)
            {
                //means it is 1st trade of that minute
                OhlcReadModel newOhlcReadModel = new OhlcReadModel(latestTrade.CurrencyPair,ohlcdDateTime.AddMinutes(1), latestTrade.ExecutionPrice.Value,
                    latestTrade.ExecutionPrice.Value,
                    latestTrade.ExecutionPrice.Value, latestTrade.ExecutionPrice.Value, latestTrade.ExecutedVolume.Value);
                _persistanceRepository.SaveOrUpdate(newOhlcReadModel);
            }
            else
            {
               // IList<TradeReadModel> trades = _tradeRepository.GetTradesBetweenDates(latestTrade.ExecutionTime,
                 //   ohlcdDateTime);
                //decimal volume = CalculateVolume(trades);
                //update the ohlc
                model.UpdateOhlc(latestTrade);
                _persistanceRepository.SaveOrUpdate(model);
            }

        }

        /// <summary>
        /// Calculate average volume of trades
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        private decimal CalculateVolume(IList<TradeReadModel> trades)
        {
            decimal volume = 0;
            foreach (var tradeReadModel in trades)
            {
                volume += tradeReadModel.Volume;
            }
            volume = volume/trades.Count;
            return volume;
        }
        
    }
}
