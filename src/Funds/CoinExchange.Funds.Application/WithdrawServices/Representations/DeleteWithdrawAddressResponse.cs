using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Response after deleting the given withdraw address
    /// </summary>
    public class DeleteWithdrawAddressResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DeleteWithdrawAddressResponse(bool deleteSuccessful, string description)
        {
            DeleteSuccessful = deleteSuccessful;
            Description = description;
        }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public bool DeleteSuccessful { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }
    }
}
