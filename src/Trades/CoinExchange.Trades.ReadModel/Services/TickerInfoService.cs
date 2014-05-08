using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.MemoryImages;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.Services
{
    /// <summary>
    /// Ticker info service for getting ticker and appending bbo to it
    /// </summary>
    public class TickerInfoService
    {
        private ITickerInfoRepository _tickerInfoRepository;
        private BBOMemoryImage _bboMemoryImage;
        public TickerInfoService(ITickerInfoRepository tickerInfoRepository, BBOMemoryImage bboMemoryImage)
        {
            _tickerInfoRepository = tickerInfoRepository;
            _bboMemoryImage = bboMemoryImage;
        }

        /// <summary>
        /// Method that wil append get ticker info and append bbo to it
        /// </summary>
        /// <param name="currenyPair"></param>
        /// <returns></returns>
        public TickerInfoReadModel GetTickerInfo(string currenyPair)
        {
            TickerInfoReadModel model=_tickerInfoRepository.GetTickerInfoByCurrencyPair(currenyPair);
            BBORepresentation bboRepresentation=_bboMemoryImage.BBORepresentationList.Contains(currenyPair);
            if (bboRepresentation != null)
            {
                model.UpdateBboInTickerInfo(bboRepresentation);
            }
            return model;
        }
    }
}
