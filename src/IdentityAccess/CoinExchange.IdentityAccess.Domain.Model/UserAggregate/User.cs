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
        private string _pgpPublicKey;
        private Address _address;
        private string _email;
        private Language _language;
        private TimeZone _timeZone;
        private TimeSpan _autoLogout;
        private DateTime _lastLogin;
        private TierStatusList _tierStatusList;
        private UserDocumentsList _userDocumentsList;
        private string _phoneNumber;
        private string _country;
        private string _state;
        private string _activationKey;

        //default constructor
        public User()
        {
            
        }

        /// <summary>
        /// Constructor for Sign Up
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="country"></param>
        /// <param name="timeZone"></param>
        /// <param name="pgpPublicKey"></param>
        /// <param name="activationKey"></param>
        public User(string email, string username, string password, string country, TimeZone timeZone,
            string pgpPublicKey, string activationKey)
        {
            _username = username;
            _password = password;
            _pgpPublicKey = pgpPublicKey;
            _country = country;
            _email = email;
            _timeZone = timeZone;
            _activationKey = activationKey;

            _tierStatusList = new TierStatusList();
            _userDocumentsList = new UserDocumentsList();
        }

        /// <summary>
        /// Constructor for filling in all the information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="pgpPublicKey"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="language"></param>
        /// <param name="timeZone"></param>
        /// <param name="autoLogout"></param>
        /// <param name="lastLogin"></param>
        /// <param name="country"> </param>
        /// <param name="state"> </param>
        /// <param name="phoneNumber"> </param>
        /// <param name="activationKey"> </param>
        public User(string username, string password, string pgpPublicKey, Address address, string email, Language language,
            TimeZone timeZone, TimeSpan autoLogout, DateTime lastLogin, string country, string state, string phoneNumber, 
            string activationKey)
        {
             Username = username;
            _password = password;
            _pgpPublicKey = pgpPublicKey;
            _address = address;
            _email = email;
            _language = language;
            _timeZone = timeZone;
            _autoLogout = autoLogout;
            _lastLogin = lastLogin;
            Country = country;
            PhoneNumber = phoneNumber;
            State = state;
            ActivationKey = activationKey;

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
        /// Username (Once set cannot be changed)
        /// </summary>
        public string Username { get { return _username; } private set { _username = value; } }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get { return _password; } set { _password = value; } }

        /// <summary>
        /// Public Key
        /// </summary>
        public string PublicKey { get { return _pgpPublicKey; } set { _pgpPublicKey = value; } }

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
        /// Country State of user
        /// </summary>
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Residence Country
        /// </summary>
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        /// <summary>
        /// Phone Number
        /// </summary>
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string ActivationKey
        {
            get { return _activationKey; }
            set
            {
                _activationKey = value;
            }
        }

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