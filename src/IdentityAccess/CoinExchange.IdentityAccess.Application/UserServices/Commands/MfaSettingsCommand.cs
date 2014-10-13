using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command for submitting MFaSettings
    /// </summary>
    public class MfaSettingsCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSettingsCommand(bool apiKeyMfa, string apiKeyPassword, string apiKey, List<Tuple<string, string, bool>> mfaSettingsList)
        {
            ApiKeyMfa = apiKeyMfa;
            ApiKeyPassword = apiKeyPassword;
            ApiKey = apiKey;
            MfaSettingsList = mfaSettingsList;
        }

        /// <summary>
        /// Are the subscriptions for API key or for the user
        /// </summary>
        public bool ApiKeyMfa { get; private set; }

        /// <summary>
        /// Password for the API key
        /// </summary>
        public string ApiKeyPassword { get; private set; }

        /// <summary>
        /// Api Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Mfa Subcriptions list
        /// Tuple Items: Item1 = MfaSubscriptionId, Item2 = MfaSusbcriptionName, Item3 = Enabled
        /// </summary>
        public List<Tuple<string,string,bool>> MfaSettingsList { get; private set; }
    }
}
