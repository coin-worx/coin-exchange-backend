using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands
{
    /// <summary>
    /// Command to update user generated security params
    /// </summary>
    public class UpdateUserGeneratedSecurityKeyPair
    {
        public string ApiKey { get; set; }
        public string UserName { get; private set; }
        public string KeyDescritpion { get; private set; }
        public bool EnableStartDate { get; private set; }
        public bool EnableEndDate { get; private set; }
        public bool EnableExpirationDate { get; private set; }
        public string EndDateTime { get; private set; }
        public string StartDateTime { get; private set; }
        public string ExpirationDateTime { get; private set; }
        public SecurityKeyPermissionsRepresentation[] SecurityKeyPermissions { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="userName"></param>
        /// <param name="keyDescritpion"></param>
        /// <param name="enableStartDate"></param>
        /// <param name="enableEndDate"></param>
        /// <param name="enableExpirationDate"></param>
        /// <param name="endDateTime"></param>
        /// <param name="startDateTime"></param>
        /// <param name="securityKeyPermissions"></param>
        /// <param name="expirationDateTime"></param>
        public UpdateUserGeneratedSecurityKeyPair(string apiKey, string userName, string keyDescritpion, bool enableStartDate, bool enableEndDate, bool enableExpirationDate, string endDateTime, string startDateTime, SecurityKeyPermissionsRepresentation[] securityKeyPermissions, string expirationDateTime)
        {
            ApiKey = apiKey;
            UserName = userName;
            KeyDescritpion = keyDescritpion;
            EnableStartDate = enableStartDate;
            EnableEndDate = enableEndDate;
            EnableExpirationDate = enableExpirationDate;
            EndDateTime = endDateTime;
            StartDateTime = startDateTime;
            SecurityKeyPermissions = securityKeyPermissions;
            ExpirationDateTime = expirationDateTime;
        }

        /// <summary>
        /// Validate the command
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            for (int i = 0; i < SecurityKeyPermissions.Length; i++)
            {
                //validate atleast one permission is assigned
                if (SecurityKeyPermissions[i].Allowed)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
