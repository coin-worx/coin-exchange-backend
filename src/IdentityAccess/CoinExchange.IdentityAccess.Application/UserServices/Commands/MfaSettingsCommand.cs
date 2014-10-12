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
        public MfaSettingsCommand(string apiKey, List<Tuple<string, string, bool>> mfaSettingsList)
        {
            ApiKey = apiKey;
            MfaSettingsList = mfaSettingsList;
        }

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
