using System;
using System.Collections;
using System.Collections.Generic;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Digital Signature Info(Api key and Secret key)
    /// </summary>
    public class SecurityKeysPair
    {
        #region Private Fields

        private ApiKey _apiKey;
        private SecretKey _secretKey;
        private IList<SecurityKeysPermission> _securityKeysPermissions { get; set; }

        #endregion Private Fields

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SecurityKeysPair()
        {
            _securityKeysPermissions=new List<SecurityKeysPermission>();
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public SecurityKeysPair(ApiKey apiKey, SecretKey secretKey)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="userName"></param>
        /// <param name="expirationDate"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="lastModified"></param>
        /// <param name="systemGenerated"></param>
        public SecurityKeysPair(string keyDescription, ApiKey apiKey, SecretKey secretKey, string userName, DateTime expirationDate, DateTime startDate, DateTime endDate, DateTime lastModified, bool systemGenerated, IList<SecurityKeysPermission> securityKeysPermissions)
        {
            KeyDescription = keyDescription;
            _apiKey = apiKey;
            _secretKey = secretKey;
            UserName = userName;
            ExpirationDate = expirationDate;
            StartDate = startDate;
            EndDate = endDate;
            LastModified = lastModified;
            SystemGenerated = systemGenerated;
            _securityKeysPermissions = securityKeysPermissions;
         }

        #region Methods

        /// <summary>
        /// Update security permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool UpdatePermissions(SecurityKeysPermission[] permissions)
        {
            for(int i=0;i<_securityKeysPermissions.Count;i++)
            {
                for (int j = 0; j < permissions.Length; j++)
                {
                    if (
                        _securityKeysPermissions[i].Permission.PermissionId.Equals(
                            permissions[j].Permission.PermissionId))
                    {
                        _securityKeysPermissions[i].IsAllowed = permissions[j].IsAllowed;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Validate permissions
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public bool ValidatePermission(string permissionId)
        {
            for (int i = 0; i < _securityKeysPermissions.Count; i++)
            {
                if (_securityKeysPermissions[i].Permission.PermissionId.Equals(permissionId))
                {
                    return _securityKeysPermissions[i].IsAllowed;
                }
            }
            return false;
        }

        /// <summary>
        /// Change the value of the API Key
        /// </summary>
        /// <param name="apiKey"> </param>
        /// <returns></returns>
        public bool ChangeApiKeyValue(ApiKey apiKey)
        {
            if (IsApiKeyValid(apiKey))
            {
                _apiKey = apiKey;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Change the value of the Secret Key
        /// </summary>
        /// <param name="secretKey"> </param>
        /// <returns></returns>
        public bool ChangeSecretKeyValue(SecretKey secretKey)
        {
            if (IsSecretKeyValid(secretKey))
            {
                _secretKey = secretKey;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if the given API Key is valid or not
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        private bool IsApiKeyValid(ApiKey apiKey)
        {
            if (apiKey != null && !string.IsNullOrEmpty(apiKey.Value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if the given Secret Key is valid or not
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        private bool IsSecretKeyValid(SecretKey secretKey)
        {
            if (secretKey!= null && !string.IsNullOrEmpty(secretKey.Value))
            {
                return true;
            }
            return false;
        }

        #endregion Methods

        /// <summary>
        /// Key Description
        /// </summary>
        public string KeyDescription
        {
            get; private set;
        }

        /// <summary>
        /// API Key
        /// </summary>
        public ApiKey ApiKey { get { return _apiKey; } set { _apiKey = value; } }

        /// <summary>
        /// Secret Key
        /// </summary>
        public SecretKey SecretKey { get { return _secretKey; } set { _secretKey = value; } }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public DateTime ExpirationDate { get; private set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get; private set; }
        
        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// LastModified
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// SystemGenerated
        /// </summary>
        public bool SystemGenerated { get; private set; }
    }
}
