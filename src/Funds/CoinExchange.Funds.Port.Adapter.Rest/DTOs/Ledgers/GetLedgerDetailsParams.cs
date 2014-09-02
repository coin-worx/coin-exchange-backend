
namespace CoinExchange.Funds.Port.Adapter.Rest.DTOs.Ledgers
{
    /// <summary>
    /// Parameters for getting the ledger parameters
    /// </summary>
    public class GetLedgerDetailsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GetLedgerDetailsParams(string ledgerId)
        {
            LedgerId = ledgerId;
        }

        /// <summary>
        /// Ledger Id
        /// </summary>
        public string LedgerId { get; private set; }
    }
}
