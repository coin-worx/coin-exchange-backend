using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations
{
    /// <summary>
    /// VO to represent security key pair details
    /// </summary>
    public class SecurityKeyRepresentation
    {
        public string KeyDescritpion { get; private set; }
        public bool EnableStartDate { get; private set; }
        public bool EnableEndDate { get; private set; }
        public bool EnableExpirationDate { get; private set; }
        public string EndDateTime { get; private set; }
        public string StartDateTime { get; private set; }
        public string ExpirationDateTime { get; private set; }
        public SecurityKeyPermissionsRepresentation[] SecurityKeyPermissions { get; private set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="keyDescritpion"></param>
        /// <param name="enableStartDate"></param>
        /// <param name="enableEndDate"></param>
        /// <param name="enableExpirationDate"></param>
        /// <param name="endDateTime"></param>
        /// <param name="startDateTime"></param>
        /// <param name="expirationDateTime"></param>
        /// <param name="securityKeyPermissions"></param>
        public SecurityKeyRepresentation(string keyDescritpion, bool enableStartDate, bool enableEndDate, bool enableExpirationDate, string endDateTime, string startDateTime, string expirationDateTime, SecurityKeyPermissionsRepresentation[] securityKeyPermissions)
        {
            KeyDescritpion = keyDescritpion;
            EnableStartDate = enableStartDate;
            EnableEndDate = enableEndDate;
            EnableExpirationDate = enableExpirationDate;
            EndDateTime = endDateTime;
            StartDateTime = startDateTime;
            ExpirationDateTime = expirationDateTime;
            SecurityKeyPermissions = securityKeyPermissions;
        }
    }
}
