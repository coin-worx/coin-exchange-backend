using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// Response DTO after changing password
    /// </summary>
    public class ChangePasswordResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ChangePasswordResponse(bool changeSuccessful, string message)
        {
            ChangeSuccessful = changeSuccessful;
            Message = message;
        }

        /// <summary>
        /// Change Successful
        /// </summary>
        public bool ChangeSuccessful { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Message { get; private set; }
    }
}
