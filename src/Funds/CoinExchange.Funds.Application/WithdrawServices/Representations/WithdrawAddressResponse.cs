using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Response in return when a new withdraw address is saved
    /// </summary>
    public class WithdrawAddressResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawAddressResponse(bool saveSuccessful, string description)
        {
            SaveSuccessful = saveSuccessful;
            Description = description;
        }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public bool SaveSuccessful { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
    }
}
