namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Representation of the withdrawal addresses
    /// </summary>
    public class WithdrawAddressRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawAddressRepresentation(string bitcoinAddress, string description)
        {
            BitcoinAddress = bitcoinAddress;
            Description = description;
        }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public string BitcoinAddress { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
    }
}
