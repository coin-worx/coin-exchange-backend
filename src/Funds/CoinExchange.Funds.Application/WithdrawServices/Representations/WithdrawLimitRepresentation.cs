namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Represents the Withdrawawl limit thresholds and balance information
    /// </summary>
    public class WithdrawLimitRepresentation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountId"> </param>
        /// <param name="dailyLimit"></param>
        /// <param name="dailyLimitUsed"></param>
        /// <param name="monthlyLimit"></param>
        /// <param name="monthlyLimitUsed"></param>
        /// <param name="currentBalance"></param>
        /// <param name="maximumWithdraw"></param>
        /// <param name="maximumWithdrawInUsd"> </param>
        /// <param name="withheld"> </param>
        /// <param name="withheldInUsd"> </param>
        /// <param name="fee"> </param>
        public WithdrawLimitRepresentation(string currency, int accountId, decimal dailyLimit, decimal dailyLimitUsed,
            decimal monthlyLimit, decimal monthlyLimitUsed, decimal currentBalance, decimal maximumWithdraw,
            decimal maximumWithdrawInUsd, decimal withheld, decimal withheldInUsd, decimal fee)
        {
            Currency = currency;
            AccountId = accountId;
            DailyLimit = dailyLimit;
            DailyLimitUsed = dailyLimitUsed;
            MonthlyLimit = monthlyLimit;
            MonthlyLimitUsed = monthlyLimitUsed;
            CurrentBalance = currentBalance;
            MaximumWithdraw = maximumWithdraw;
            MaximumWithdrawInUsd = maximumWithdrawInUsd;
            Withheld = withheld;
            WithheldInUsd = withheldInUsd;
            Fee = fee;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// DailyLimit
        /// </summary>
        public decimal DailyLimit { get; private set; }

        /// <summary>
        /// DailyLimit that has been Used
        /// </summary>
        public decimal DailyLimitUsed { get; private set; }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public decimal MonthlyLimit { get; private set; }

        /// <summary>
        /// Monthly limit that has already been used
        /// </summary>
        public decimal MonthlyLimitUsed { get; private set; }

        /// <summary>
        /// Current balance for this currency of this user
        /// </summary>
        public decimal CurrentBalance { get; private set; }

        /// <summary>
        /// Maximum Withdraw threshold that the user can commit at the moment
        /// </summary>
        public decimal MaximumWithdraw { get; private set; }

        /// <summary>
        /// Maximum Withdraw amount in US Dollars
        /// </summary>
        public decimal MaximumWithdrawInUsd { get; private set; }

        /// <summary>
        /// Withheld Amount
        /// </summary>
        public decimal Withheld { get; private set; }

        /// <summary>
        /// Withheld amount in US Dollars
        /// </summary>
        public decimal WithheldInUsd { get; private set; }

        /// <summary>
        /// Fee for the withdrawal
        /// </summary>
        public decimal Fee { get; private set; }
    }
}
