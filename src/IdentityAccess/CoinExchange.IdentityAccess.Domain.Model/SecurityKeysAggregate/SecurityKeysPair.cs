using System;
using System.Collections;
using System.Collections.Generic;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Digital Signature Info(Api key and Secret key)
    /// </summary>
    public class SecurityKeysPair
    {
        #region Private Fields

        private string _apiKey;
        private string _secretKey;
        private string _keyDescription;
        private string _userName;
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
        /// Constructor for system generated key pair
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="keyDescription"></param>
        /// <param name="userName"></param>
        /// <param name="systemGenerated"></param>
        public SecurityKeysPair(string apiKey, string secretKey, string keyDescription, string userName, bool systemGenerated)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
            KeyDescription = keyDescription;
            UserName = userName;
            SystemGenerated = systemGenerated;
            CreationDateTime = DateTime.Now;
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public SecurityKeysPair(string apiKey, string secretKey)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public SecurityKeysPair(string username, string apiKey, string secretKey, bool isSystemGenerated, string keyDescription)
        {
            this.UserName = username;
            _apiKey = apiKey;
            _secretKey = secretKey;
            this.KeyDescription = keyDescription;

            if (isSystemGenerated)
            {
                SystemGenerated = true;
            }
            else
            {
                SystemGenerated = false;
            }
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
        public SecurityKeysPair(string keyDescription, string apiKey, string secretKey, string userName, DateTime expirationDate, DateTime startDate, DateTime endDate, DateTime lastModified, bool systemGenerated, IList<SecurityKeysPermission> securityKeysPermissions)
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
            // Update last modified date time.
            LastModified = DateTime.Now;
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
        public bool ChangeApiKeyValue(string apiKey)
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
        public bool ChangeSecretKeyValue(string secretKey)
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
        private bool IsApiKeyValid(string apiKey)
        {
            if (apiKey != null && !string.IsNullOrEmpty(apiKey))
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
        private bool IsSecretKeyValid(string secretKey)
        {
            if (secretKey!= null && !string.IsNullOrEmpty(secretKey))
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
            get { return _keyDescription; }
            private set
            {
                AssertionConcern.AssertNullOrEmptyString(value,"Key description cannot be null");
                _keyDescription = value;
            }
        }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get { return _apiKey; } set { _apiKey = value; } }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get { return _secretKey; } set { _secretKey = value; } }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            private set
            {
                AssertionConcern.AssertNullOrEmptyString(value, "Username cannot be null or empty");
                _userName = value;
            }
        }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public DateTime ExpirationDate { get;  set; }

        /// <summary>
        /// Enable expiration date
        /// </summary>
        public bool EnableExpirationDate { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get;  set; }

        /// <summary>
        /// Enable Start date
        /// </summary>
        public bool EnableStartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { get;  set; }

        /// <summary>
        /// Enable end date
        /// </summary>
        public bool EnableEndDate { get; set; }

        /// <summary>
        /// LastModified
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Creation DateTime
        /// </summary>
        public DateTime CreationDateTime { get; private set; }

        /// <summary>
        /// SystemGenerated
        /// </summary>
        public bool SystemGenerated { get; private set; }
    }
}
