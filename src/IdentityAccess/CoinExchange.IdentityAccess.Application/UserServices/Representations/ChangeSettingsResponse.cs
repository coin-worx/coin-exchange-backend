using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// Response after changing the settings for a user
    /// </summary>
    public class ChangeSettingsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ChangeSettingsResponse(bool changeSuccessful, string mesage)
        {
            ChangeSuccessful = changeSuccessful;
            Mesage = mesage;
        }

        public bool ChangeSuccessful { get; private set; }
        public string Mesage { get; private set; }
    }
}
