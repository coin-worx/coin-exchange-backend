using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies the user and their related information after the ysign up for CoinExchange
    /// </summary>
    public class User
    {
        private string _username;
        private string _password;
        private string _publicKey;
        private Address _address;
        private string _email;
        private Language _language;
        private TimeZone _timeZone;
        private TimeSpan _autoLogout;
        private DateTime _lastLogin;
        private TierStatusList _tierStatusList;
        private UserDocumentsList _userDocumentsList;
        
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="publicKey"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="language"></param>
        /// <param name="timeZone"></param>
        /// <param name="autoLogout"></param>
        /// <param name="lastLogin"></param>
        public User(string username, string password, string publicKey, Address address, string email, Language language,
            TimeZone timeZone, TimeSpan autoLogout, DateTime lastLogin)
        {
            _username = username;
            _password = password;
            _publicKey = publicKey;
            _address = address;
            _email = email;
            _language = language;
            _timeZone = timeZone;
            _autoLogout = autoLogout;
            _lastLogin = lastLogin;

            _tierStatusList = new TierStatusList();
            _userDocumentsList = new UserDocumentsList();
        }

        /// <summary>
        /// Add User Tier Status
        /// </summary>
        /// <param name="userTierStatus"></param>
        /// <returns></returns>
        public bool AddTierStatus(UserTierStatus userTierStatus)
        {
            _tierStatusList.AddTierStatus(userTierStatus);
            return true;
        }

        /// <summary>
        /// Remove a Tier status for a user
        /// </summary>
        /// <param name="userTierStatus"></param>
        /// <returns></returns>
        public bool RemoveTierStatus(UserTierStatus userTierStatus)
        {
            _tierStatusList.RemoveTierStatus(userTierStatus);
            return true;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get { return _username; } set { _username = value; } }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get { return _password; } set { _password = value; } }

        /// <summary>
        /// Public Key
        /// </summary>
        public string PublicKey { get { return _publicKey; } set { _publicKey = value; } }

        /// <summary>
        /// Address
        /// </summary>
        public Address Address { get { return _address; } set { _address = value; } }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get { return _email; } set { _email = value; } }

        /// <summary>
        /// Language
        /// </summary>
        public Language Language { get { return _language; } set { _language = value; } }

        /// <summary>
        /// TimeZone
        /// </summary>
        public TimeZone TimeZone { get { return _timeZone; } set { _timeZone = value; } }

        /// <summary>
        /// AutoLogout
        /// </summary>
        public TimeSpan AutoLogout { get { return _autoLogout; } set { _autoLogout = value; } }

        /// <summary>
        /// Last Login
        /// </summary>
        public DateTime LastLogin { get { return _lastLogin; } set { _lastLogin = value; } }

        /// <summary>
        /// List of the Tier associated with this user
        /// </summary>
        public TierStatusList TierStatusList { get { return _tierStatusList; } private set { _tierStatusList = value; } }

        /// <summary>
        /// List of Ueser Docuemnts associated with this User
        /// </summary>
        public UserDocumentsList UserDocumentsList { get; set; }
    }
}