using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Represents the Permissions for a pair of API Key <-> SecretKey
    /// </summary>
    public class SecurityKeyPairPermission
    {
        /// <summary>
        /// parameterized Constructor
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="permissionId"></param>
        /// <param name="isAllowed"></param>
        public SecurityKeyPairPermission(ApiKey apiKey, Permission permissionId, bool isAllowed)
        {
            ApiKey = apiKey;
            PermissionId = permissionId;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// API Key
        /// </summary>
        public ApiKey ApiKey { get; private set; }

        /// <summary>
        /// Permission ID
        /// </summary>
        public Permission PermissionId { get; private set; }

        /// <summary>
        /// IsAllowed
        /// </summary>
        public bool IsAllowed { get; private set; }
    }
}
