
namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Represents the daily and monthly limits for withdrawal
    /// </summary>
    public class WithdrawLimit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawLimit(string tierLevel, double monthlyLimit, double dailyLimit)
        {
            TierLevel = tierLevel;
            MonthlyLimit = monthlyLimit;
            DailyLimit = dailyLimit;
        }

        /// <summary>
        /// Tier Level
        /// </summary>
        public string TierLevel { get; private set; }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public double MonthlyLimit { get; private set; }

        /// <summary>
        /// Daily Limit
        /// </summary>
        public double DailyLimit { get; private set; }
    }
}
