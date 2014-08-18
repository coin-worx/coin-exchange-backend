using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.DepositServices.Representations
{
    /// <summary>
    /// Represents the Deposit Address
    /// </summary>
    public class DepositAddressRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositAddressRepresentation(string address, string status)
        {
            Address = address;
            Status = status;
        }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; private set; }
    }
}
