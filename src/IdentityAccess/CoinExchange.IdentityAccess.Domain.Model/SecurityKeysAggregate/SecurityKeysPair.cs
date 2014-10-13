using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

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
        private int _userId;
        private IList<SecurityKeysPermission> _securityKeysPermissions { get; set; }
        private IList<MfaSubscriptionStatus> _mfaSubscriptions { get; set; }

        #endregion Private Fields

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SecurityKeysPair()
        {
            _securityKeysPermissions=new List<SecurityKeysPermission>();
            _mfaSubscriptions = new List<MfaSubscriptionStatus>();
        }

        /// <summary>
        /// Constructor for system generated security key pair
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="keyDescription"></param>
        /// <param name="userId"></param>
        /// <param name="systemGenerated"></param>
        public SecurityKeysPair(string apiKey, string secretKey, string keyDescription, int userId, bool systemGenerated)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
            KeyDescription = keyDescription;
            UserId = userId;
            SystemGenerated = systemGenerated;
            CreationDateTime = DateTime.Now;
            LastModified = DateTime.Now;
            _mfaSubscriptions = new List<MfaSubscriptionStatus>();
        }

        /// <summary>
        /// Constructor for user generated key pair
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="keyDescription"></param>
        /// <param name="userName"></param>
        /// <param name="systemGenerated"></param>
        public SecurityKeysPair(string apiKey, string secretKey, string keyDescription, int userId, bool systemGenerated,List<SecurityKeysPermission> permissions )
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
            KeyDescription = keyDescription;
            UserId = userId;
            SystemGenerated = systemGenerated;
            _securityKeysPermissions = permissions;
            CreationDateTime = DateTime.Now;
            _mfaSubscriptions = new List<MfaSubscriptionStatus>();
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
        public SecurityKeysPair(int userId, string apiKey, string secretKey, bool isSystemGenerated, string keyDescription)
        {
            UserId = userId;
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
        public SecurityKeysPair(string keyDescription, string apiKey, string secretKey, int userId, DateTime expirationDate, DateTime startDate, DateTime endDate, DateTime lastModified, bool systemGenerated, IList<SecurityKeysPermission> securityKeysPermissions)
        {
            KeyDescription = keyDescription;
            _apiKey = apiKey;
            _secretKey = secretKey;
            UserId = userId;
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
        /// Update security key pair
        /// </summary>
        public void UpdateSecuritykeyPair(string keyDescritpion, bool enableStartDate, bool enableEndDate, bool enableExpirationDate, string endDateTime, string startDateTime,string expirationDateTime)
        {
            KeyDescription = keyDescritpion;
            EnableStartDate = enableStartDate;
            EnableEndDate = enableEndDate;
            if (EnableExpirationDate)
            {
                ExpirationDate = Convert.ToDateTime(expirationDateTime);
            }
            else
            {
                ExpirationDate = null;
            }
            if (EnableStartDate)
            {
                StartDate = Convert.ToDateTime(startDateTime);
            }
            else
            {
                StartDate = null;
            }
            if (EnableEndDate)
            {
                EndDate = Convert.ToDateTime(endDateTime);
            }
            else
            {
                EndDate = null;
            }
            LastModified = DateTime.Now;
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

        /// <summary>
        /// Get All permissions
        /// </summary>
        /// <returns></returns>
        public SecurityKeysPermission[] GetAllPermissions()
        {
            IList<SecurityKeysPermission> permissions=new List<SecurityKeysPermission>();
            for (int i = 0; i < _securityKeysPermissions.Count; i++)
            {
                permissions.Add(_securityKeysPermissions[i].Clone() as SecurityKeysPermission);
            }
            return permissions.ToArray();
        }

        /// <summary>
        /// Assigns the Mfa Pass code to this security keys pair instance
        /// </summary>
        /// <returns></returns>
        public bool AssignMfaCode(string mfaPassCode)
        {
            if (!string.IsNullOrEmpty(mfaPassCode))
            {
                MfaCode = mfaPassCode;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check the MFA Subscription for the current action
        /// </summary>
        /// <param name="currentAction"></param>
        /// <returns></returns>
        public bool CheckMfaSubscriptions(string currentAction)
        {
            // Check in each element of the list to see if the current action is enabled
            foreach (var mfaSubscriptionStatus in _mfaSubscriptions)
            {
                if (mfaSubscriptionStatus.MfaSubscription.MfaSubscriptionName == currentAction)
                {
                    return mfaSubscriptionStatus.Enabled;
                }
            }
            return false;
        }

        /// <summary>
        /// Marks the MFA Subscriptions as enabled, whichever are present in the parameter list
        /// Item1 = MfaSUbscriptionId, Item2 = MfaSubscriptionName, Item3 = Enabled/Disabled
        /// </summary>
        /// <param name="mfaSubscriptions"></param>
        /// <returns></returns>
        public void AssignMfaSubscriptions(IList<Tuple<string, string, bool>> mfaSubscriptions)
        {
            IList<Tuple<string, string, bool>> firstTimeSubscriptions = new List<Tuple<string, string, bool>>();
            // As this list of incoming MFA subscriptions is the list that is taken as master data from the database, displayed
            // on front-end, and then picked by user and sent here, the subscription may or may not be present to the user. So,
            // if a subscription is not present yet, it is safe to add it as it is master data
            foreach (var mfaSubscription in mfaSubscriptions)
            {
                bool subscriptionPresent = false;
                foreach (var mfaSubscriptionStatus in _mfaSubscriptions)
                {
                    if (mfaSubscriptionStatus.MfaSubscription.MfaSubscriptionName == mfaSubscription.Item2)
                    {
                        subscriptionPresent = true;
                        mfaSubscriptionStatus.Enabled = mfaSubscription.Item3;
                    }
                }
                // If this subscription is not alreaady present, add it to the temporary list and add to MFASubscriptions list later
                if (!subscriptionPresent)
                {
                    firstTimeSubscriptions.Add(new Tuple<string, string, bool>(mfaSubscription.Item1, mfaSubscription.Item2,
                        mfaSubscription.Item3));
                }
            }

            // For each of the subscriptions in the temporary list, we need to add them in the MFA Subscriptions list, these are
            // the new elements that we need to add
            foreach (var firstTimeSubscription in firstTimeSubscriptions)
            {
                _mfaSubscriptions.Add(new MfaSubscriptionStatus(ApiKey, new MfaSubscription(firstTimeSubscription.Item1,
                    firstTimeSubscription.Item2), firstTimeSubscription.Item3));
            }
        }

        /// <summary>
        /// Evaluate if the provided code is real one-time code for this user
        /// </summary>
        /// <returns></returns>
        public bool VerifyMfaCode(string mfaCode)
        {
            // Verify the code
            if (this.MfaCode == mfaCode)
            {
                // Make the code null, so it can verified the next time
                this.MfaCode = null;
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
            set
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
        public int UserId
        {
            get { return _userId; }
            private set
            {
                _userId = value;
            }
        }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public DateTime? ExpirationDate { get;  set; }

        /// <summary>
        /// Enable expiration date
        /// </summary>
        public bool EnableExpirationDate { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime? StartDate { get;  set; }

        /// <summary>
        /// Enable Start date
        /// </summary>
        public bool EnableStartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime? EndDate { get;  set; }

        /// <summary>
        /// Enable end date
        /// </summary>
        public bool EnableEndDate { get; set; }

        /// <summary>
        /// LastModified
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Creation DateTime
        /// </summary>
        public DateTime CreationDateTime { get; private set; }

        /// <summary>
        /// SystemGenerated
        /// </summary>
        public bool SystemGenerated { get; private set; }

        /// <summary>
        /// Soft Delete
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Password for MFA
        /// </summary>
        public string MfaCode { get; private set; }
    }
}
