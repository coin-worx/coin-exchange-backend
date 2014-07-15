namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Bank Account
    /// </summary>
    public class BankAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BankAccount(string bankName, string accountAddress, string swiftCode)
        {
            BankName = bankName;
            AccountAddress = accountAddress;
            SwiftCode = swiftCode;
        }

        /// <summary>
        /// BankName
        /// </summary>
        public string BankName { get; private set; }

        /// <summary>
        /// AccountAddress
        /// </summary>
        public string AccountAddress { get; private set; }

        /// <summary>
        /// SwiftCode
        /// </summary>
        public string SwiftCode { get; private set; }
    }
}
