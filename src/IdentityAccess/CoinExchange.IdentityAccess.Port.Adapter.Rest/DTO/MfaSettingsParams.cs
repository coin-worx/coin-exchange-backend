using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters for MFA Subscription settigns
    /// </summary>
    public class MfaSettingsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSettingsParams(bool apiKeyMfa, string apiKeyPassword, List<MfaSingleSettingParams> mfaSettingsList)
        {
            ApiKeyPassword = apiKeyPassword;
            ApiKeyMfa = apiKeyMfa;
            MfaSettingsList = mfaSettingsList;
        }

        public string ApiKeyPassword { get; set; }
        public bool ApiKeyMfa { get; set; }
        public List<MfaSingleSettingParams> MfaSettingsList { get; set; }
    }
}
