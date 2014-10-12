using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    public class MfaSingleSettingParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSingleSettingParams(string mfaSubscriptionId, string mfaSubscriptionName, bool enabled)
        {
            MfaSubscriptionId = mfaSubscriptionId;
            MfaSubscriptionName = mfaSubscriptionName;
            Enabled = enabled;
        }

        public string MfaSubscriptionId { get; set; }
        public string MfaSubscriptionName { get; set; }
        public bool Enabled { get; set; }
    }
}
