using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Infrastructure.Services
{
    public class FundsConfirmationService : IFundsConfirmationService
    {
        private dynamic _orderValidationApplicationService = null;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public FundsConfirmationService(dynamic orderValidationApplicationService)
        {
            _orderValidationApplicationService = orderValidationApplicationService;
        }

        /// <summary>
        /// Confirms if enough funds are present to send this order
        /// </summary>
        /// <returns></returns>
        public bool FundsConfirmation(string accountId, string baseCurrency, string quoteCurrency,
            decimal volume, decimal price, string orderSide, string orderId)
        {
            return _orderValidationApplicationService.ValidateFundsForOrder(accountId, baseCurrency, true/*Default: CryptoCurrency*/, 
            quoteCurrency, false/*Default: Fiat Currency*/, volume, price, orderSide, orderId);
        }

        /// <summary>
        /// Informs the Funds BC that a trade has executed which in turn updates the balancefor the involving currrencies
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"></param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        public void TradeExecuted(string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency, 
            bool quoteCurrencyIsCrypto, decimal tradeVolume, decimal price, DateTime executionDateTime, 
            string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, string sellOrderId)
        {
            _orderValidationApplicationService.TradeExecuted(baseCurrency, baseCurrencyIsCrypto,
                                                                    quoteCurrency, quoteCurrencyIsCrypto, tradeVolume,
                                                                    price, executionDateTime, tradeId, buyAccountId,
                                                                    sellAccountId, buyOrderId, sellOrderId);
        }
    }
}
