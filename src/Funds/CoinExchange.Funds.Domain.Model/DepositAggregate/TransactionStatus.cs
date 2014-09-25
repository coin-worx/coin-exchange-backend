namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Represents the Status of a Deposit or Withdrawal
    /// </summary>
    public enum TransactionStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Frozen
    }
}
