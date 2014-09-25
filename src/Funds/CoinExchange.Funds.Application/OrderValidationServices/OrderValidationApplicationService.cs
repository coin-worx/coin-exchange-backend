using System;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;

namespace CoinExchange.Funds.Application.OrderValidationServices
{
    /// <summary>
    /// Service for Validating if the user has enough balance to send the current order
    /// </summary>
    public class OrderValidationApplicationService : IOrderValidationApplicationService
    {
        private IFundsValidationService _fundsValidationService = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderValidationApplicationService(IFundsValidationService fundsValidationService)
        {
            _fundsValidationService = fundsValidationService;
        }

        /// <summary>
        /// Validates the funds before sending an order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool ValidateFundsForOrder(int accountId, string baseCurrency, bool baseCurrencyIsCrypto, 
            string quoteCurrency, bool quoteCurrencyIsCrypto, decimal volume, decimal price, string orderSide, 
            string orderId)
        {
            return _fundsValidationService.ValidateFundsForOrder(new AccountId(accountId), 
                                                                 new Currency(baseCurrency, baseCurrencyIsCrypto),
                                                                 new Currency(quoteCurrency, quoteCurrencyIsCrypto),
                                                                 volume, price, orderSide, orderId);
        }

        /// <summary>
        /// Updates the balance after sending the order
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
        public bool TradeExecuted(string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency, 
            bool quoteCurrencyIsCrypto, decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId,
            int buyAccountId, int sellAccountId, string buyOrderId, string sellOrderId)
        {
            return _fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, tradeVolume, price, executionDateTime,
                                                  tradeId, buyAccountId, sellAccountId, buyOrderId, sellOrderId);
        }

        /// <summary>
        /// Restores the balance when an order is cancelled based on the order's open quantity
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="accountId"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <param name="openQuantity"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool OrderCancelled(string baseCurrency, string quoteCurrency, int accountId, string orderSide, string orderId,
            decimal openQuantity, decimal price)
        {
            return _fundsValidationService.OrderCancelled(new Currency(baseCurrency), new Currency(quoteCurrency),
                                                          new AccountId(accountId), orderSide, orderId, openQuantity, price);
        }
    }
}
