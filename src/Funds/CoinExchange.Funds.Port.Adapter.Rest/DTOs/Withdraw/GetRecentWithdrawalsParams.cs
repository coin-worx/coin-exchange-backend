
namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Withdraw
{
    /// <summary>
    /// Parameters for getting the recent withdrawals for a currency and account ID
    /// </summary>
    public class GetRecentWithdrawalsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetRecentWithdrawalsParams(int accountId, string currency)
        {
            AccountId = accountId;
            Currency = currency;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }
    }
}
