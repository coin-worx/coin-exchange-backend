using System;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Address to make a deposit to
    /// </summary>
    public class DepositAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositAddress(BitcoinAddress bitcoinAddress, AddressStatus status, DateTime creationDateTime, AccountId accountId)
        {
            BitcoinAddress = bitcoinAddress;
            Status = status;
            CreationDateTime = creationDateTime;
            AccountId = accountId;
        }

        /// <summary>
        /// Database primary key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public BitcoinAddress BitcoinAddress { get; set; }

        /// <summary>
        /// Status of the Address
        /// </summary>
        public AddressStatus Status { get; set; }

        /// <summary>
        /// DateTime when this address was generated
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>
        public AccountId AccountId { get; set; }

        /// <summary>
        /// Sets the Status set to Expired
        /// </summary>
        public void StatusExpired()
        {
            if (Status == AddressStatus.New || Status == AddressStatus.Used)
            {
                Status = AddressStatus.Expired;
            }
        }

        /// <summary>
        /// Sets the Status set to Used
        /// </summary>
        public void StatusUsed()
        {
            if (Status == AddressStatus.New)
            {
                Status = AddressStatus.Used;
            }
        }
    }
}
