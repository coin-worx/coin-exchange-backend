using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for Funds validation
    /// </summary>
    public interface IFundsValidationService
    {
        /// <summary>
        /// Validates the Funds before sending an order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="curerncyPair"></param>
        /// <param name="amount"></param>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        bool ValidateFunds(AccountId accountId, string curerncyPair, double amount, string orderSide);
    }
}
