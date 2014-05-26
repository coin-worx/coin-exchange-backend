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
    public class SecurityKeysPermission
    {
        public int Id { get; private set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        public SecurityKeysPermission()
        {
            
        }

        /// <summary>
        /// parameterized Constructor
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="permissionId"></param>
        /// <param name="isAllowed"></param>
        public SecurityKeysPermission(string keyDescription, Permission permissionId, bool isAllowed)
        {
            KeyDescription = keyDescription;
            Permission = permissionId;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// KeyDescription
        /// </summary>
        public string KeyDescription { get; private set; }
        /// <summary>
        /// Permission ID
        /// </summary>
        public Permission Permission { get; private set; }

        /// <summary>
        /// IsAllowed
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
