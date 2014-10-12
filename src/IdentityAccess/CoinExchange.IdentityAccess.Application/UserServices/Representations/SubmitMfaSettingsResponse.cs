using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// Response to the MFA settings submission
    /// </summary>
    public class SubmitMfaSettingsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SubmitMfaSettingsResponse(bool successful, string message)
        {
            Successful = successful;
            Message = message;
        }

        /// <summary>
        /// Was operation successful
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; }
    }
}
