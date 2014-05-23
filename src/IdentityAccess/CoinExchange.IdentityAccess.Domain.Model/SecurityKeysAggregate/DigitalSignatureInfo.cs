using System;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Digital Signature Info(Api key and Secret key)
    /// </summary>
    public class DigitalSignatureInfo
    {
        #region Private Fields

        private DigitalSignature _securityKeys;
        private PermissionsList _permissionsList;

        #endregion Private Fields

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DigitalSignatureInfo()
        {
            _permissionsList = new PermissionsList();
        }

        public DigitalSignatureInfo(string keyDescription, DigitalSignature securityKeys, string userName, DateTime expirationDate, DateTime startDate, DateTime endDate, DateTime lastModified, bool systemGenerated)
        {
            KeyDescription = keyDescription;
            SecurityKeys = securityKeys;
            UserName = userName;
            ExpirationDate = expirationDate;
            StartDate = startDate;
            EndDate = endDate;
            LastModified = lastModified;
            SystemGenerated = systemGenerated;

            _permissionsList = new PermissionsList();
        }

        #region Methods

        /// <summary>
        /// Adds the permission to the given list of permissions
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool AddPermission(DigitalSignaturePermission permission)
        {
            _permissionsList.AddPermission(permission);
            return true;
        }

        /// <summary>
        /// Removes the given permission from the allowed permision for this Digital Signature
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool RemovePermission(DigitalSignaturePermission permission)
        {
            _permissionsList.RemoveTierStatus(permission);
            return true;
        }

        /// <summary>
        /// Change the value of the API Key
        /// </summary>
        /// <param name="securityKeys"> </param>
        /// <returns></returns>
        public bool ChangeApiKeyValue(DigitalSignature securityKeys)
        {
            if (IsApiKeyValid(securityKeys.ApiKey) && IsSecretKeyValid(securityKeys.SecretKey))
            {
                _securityKeys = securityKeys;
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
            if (!string.IsNullOrEmpty(apiKey))
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
            if (!string.IsNullOrEmpty(secretKey))
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
        /// Security Keys
        /// </summary>
        public DigitalSignature SecurityKeys { get { return _securityKeys; } set { _securityKeys = value; } }

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
