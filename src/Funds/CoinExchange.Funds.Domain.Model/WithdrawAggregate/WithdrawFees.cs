using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// withdraw fees
    /// </summary>
    public class WithdrawFees
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawFees()
        {
        }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="minAmount"></param>
        /// <param name="fee"></param>
        public WithdrawFees(Currency currency, double minAmount, double fee)
        {
            Currency = currency;
            MinimumAmount = minAmount;
            Fee = fee;
        }

        /// <summary>
        /// Primary Key for database ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; private set; }

        /// <summary>
        /// Minimum Amount
        /// </summary>
        public double MinimumAmount { get; private set; }

        /// <summary>
        /// Fee
        /// </summary>
        public double Fee { get; private set; }
    }
}
