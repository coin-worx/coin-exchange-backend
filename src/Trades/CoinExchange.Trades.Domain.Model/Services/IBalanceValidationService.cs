using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.Services
{
    /// <summary>
    /// Interface for the service interacting with the Funds BC for validating the funds before sending an order
    /// </summary>
    public interface IBalanceValidationService
    {
        /// <summary>
        /// Checks and confirms if enough funds are present in the user's account for this currency
        /// </summary>
        /// <returns></returns>
        bool FundsConfirmation(string accountId, string baseCurrency, string quoteCurrency,
            decimal volume, decimal price, string orderSide, string orderId);

        /// <summary>
        /// Informs the Funds BC that a trade has executed, which in turn updates the balance
        /// </summary>
        bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price,
            DateTime executionDateTime, string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, 
            string sellOrderId);
    }
}
