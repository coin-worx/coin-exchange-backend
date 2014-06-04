using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands
{
    /// <summary>
    /// Create command for user generated security pair
    /// </summary>
    public class CreateUserGeneratedSecurityKeyPair
    {
        public CreateUserGeneratedSecurityKeyPair()
        {
            
        }

        public string KeyDescritpion { get; set; }
        public bool EnableStartDate { get; set; }
        public bool EnableEndDate { get; set; }
        public bool EnableExpirationDate { get; set; }
        public string EndDateTime { get; set; }
        public string StartDateTime { get; set; }
        public string ExpirationDateTime { get; set; }
        public SecurityKeyPermissionsRepresentation[] SecurityKeyPermissions { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="securityKeyPermissions"></param>
        /// <param name="expirationDateTime"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="enableExpirationDate"></param>
        /// <param name="enableEndDate"></param>
        /// <param name="enableStartDate"></param>
        /// <param name="keyDescritpion"></param>
        public CreateUserGeneratedSecurityKeyPair(SecurityKeyPermissionsRepresentation[] securityKeyPermissions, string expirationDateTime, string startDateTime, string endDateTime, bool enableExpirationDate, bool enableEndDate, bool enableStartDate, string keyDescritpion)
        {
            SecurityKeyPermissions = securityKeyPermissions;
            ExpirationDateTime = expirationDateTime;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            EnableExpirationDate = enableExpirationDate;
            EnableEndDate = enableEndDate;
            EnableStartDate = enableStartDate;
            KeyDescritpion = keyDescritpion;
           // SystemGeneratedApiKey = systemgeneratedApiKey;
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
