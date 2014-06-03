using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Utility;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Represents the Permissions for a pair of API Key <-> SecretKey
    /// </summary>
    [Serializable]
    public class SecurityKeysPermission:ICloneable
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
        /// <param name="apiKey"></param>
        /// <param name="permissionId"></param>
        /// <param name="isAllowed"></param>
        public SecurityKeysPermission(string apiKey, Permission permissionId, bool isAllowed)
        {
            ApiKey = apiKey;
            Permission = permissionId;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// KeyDescription
        /// </summary>
        public string ApiKey { get; private set; }
        /// <summary>
        /// Permission ID
        /// </summary>
        public Permission Permission { get; private set; }

        /// <summary>
        /// IsAllowed
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Deep copy
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
           // using (MemoryStream stream = new MemoryStream())
            //{
                if (this.GetType().IsSerializable)
                {
                    return StreamConversion.ByteArrayToObject(StreamConversion.ObjectToByteArray(this));
                }
                return null;
           // }
        }
    }
}
