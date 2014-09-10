using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Infrastructure.Services
{
    /// <summary>
    /// Stub Implementation for the service that communicates cross Bounded context to validate the balance before sending
    /// an order and after execution of a trade
    /// </summary>
    public class StubBalanceValidationService : IBalanceValidationService
    {
        /// <summary>
        /// Confirms balance before sending an order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool FundsConfirmation(string accountId, string baseCurrency, string quoteCurrency, decimal volume, decimal price, string orderSide, string orderId)
        {
            return true;
        }

        public bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, string sellOrderId)
        {
            return true;
        }
    }
}
