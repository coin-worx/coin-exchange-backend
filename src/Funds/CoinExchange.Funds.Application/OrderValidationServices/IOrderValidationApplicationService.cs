using System;

namespace CoinExchange.Funds.Application.OrderValidationServices
{
    /// <summary>
    /// Interface for the validation operations on the applciation layer level
    /// </summary>
    public interface IOrderValidationApplicationService
    {
        /// <summary>
        /// Confirm that the user has enough funds to send the current order to the exchange
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"> </param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"> </param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool ValidateFundsForOrder(int accountId, string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency,
                               bool quoteCurrencyIsCrypto, decimal volume, decimal price, string orderSide, string orderId);

        /// <summary>
        /// Informs that a trade has been executed and the Funds BC should update the corresponding balance
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"> </param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"> </param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        /// <returns></returns>
        bool TradeExecuted(string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency, bool quoteCurrencyIsCrypto, 
                        decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId, int buyAccountId, 
                        int sellAccountId, string buyOrderId, string sellOrderId);
    }
}
