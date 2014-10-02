using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies the user and their related information after the ysign up for CoinExchange
    /// </summary>
    public class User
    {
        public int Id { get; private set; }
        private string _username;
        private string _password;
        private string _pgpPublicKey;
        private string _address1;
        private string _address2;
        private string _email;
        private Language _language;
        private TimeZone _timeZone;
        private bool _isDefaultAutoLogout;
        private TimeSpan _autoLogout;
        private DateTime _lastLogin;
        private IList<UserTierLevelStatus> _tierLevelStatuses { get; set; } 
        private UserDocumentsList _userDocumentsList;
        private string _phoneNumber;
        private string _country;
        private string _state;
        private string _activationKey;
        private string _fullName;
        private DateTime _dateOfBirth;
        private IList<PasswordCodeRecord> _forgottenPasswordCodesList { get; set; }
        private bool _emailMfaEnabled = true;
        private MfaSubscriptions _mfaSubscriptions = new MfaSubscriptions();

        //default constructor
        public User()
        {
            _autoLogout = new TimeSpan(0, 0, 10, 0);
            IsActivationKeyUsed = new IsActivationKeyUsed(false);
            IsUserBlocked = new IsUserBlocked(false);

            _forgottenPasswordCodesList=new List<PasswordCodeRecord>();
            _tierLevelStatuses=new List<UserTierLevelStatus>();
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
            _autoLogout = new TimeSpan(0,0,10,0);
            IsActivationKeyUsed = new IsActivationKeyUsed(false);
            IsUserBlocked = new IsUserBlocked(false);

            _tierLevelStatuses=new List<UserTierLevelStatus>();
            _userDocumentsList = new UserDocumentsList();
            _forgottenPasswordCodesList=new List<PasswordCodeRecord>();
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
        public User(string username, string password, string pgpPublicKey, string address, string email, Language language,
            TimeZone timeZone, TimeSpan autoLogout, DateTime lastLogin, string country, string state, string phoneNumber, 
            string activationKey)
        {
             Username = username;
            _password = password;
            _pgpPublicKey = pgpPublicKey;
            Address1 = address;
            _email = email;
            _language = language;
            _timeZone = timeZone;
            _autoLogout = autoLogout;
            _lastLogin = lastLogin;
            Country = country;
            PhoneNumber = phoneNumber;
            State = state;
            ActivationKey = activationKey;
            IsActivationKeyUsed = new IsActivationKeyUsed(false);
            IsUserBlocked = new IsUserBlocked(false);

            _forgottenPasswordCodesList = new List<PasswordCodeRecord>();
            _tierLevelStatuses=new List<UserTierLevelStatus>();
            _userDocumentsList = new UserDocumentsList();
        }

        #region Methods

        /// <summary>
        /// Add User Tier Status
        /// </summary>
        /// <returns></returns>
        public bool AddTierStatus(Status status,Tier tier)
        {
            _tierLevelStatuses.Add(new UserTierLevelStatus(Id,tier,status));
            return true;
        }


        /// <summary>
        /// update user tier level status
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateTierStatus(string tierLevel, Status status)
        {
            for (int i = 0; i < _tierLevelStatuses.Count; i++)
            {
                if (_tierLevelStatuses[i].Tier.TierLevel.Equals(tierLevel))
                {
                    _tierLevelStatuses[i].Status = status;
                    return true;
                }
            }
            throw new ArgumentException("No tier level found");
        }

        /// <summary>
        /// Remove a Tier status for a user
        /// </summary>
        /// <param name="userTierStatus"></param>
        /// <returns></returns>
        public bool RemoveTierStatus(UserTierLevelStatus userTierStatus)
        {
            //_tierStatusList.RemoveTierStatus(userTierStatus);
            return true;
        }

        /// <summary>
        /// Update tier 1 information
        /// </summary>
        public void UpdateTier1Information(string fullName,DateTime dateOfBirth,string phoneNumber)
        {
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Update tier 2 information
        /// </summary>
        public void UpdateTier2Information(string city, string state, string addressLine1, string addressLine2,
            string zipCode)
        {
            City = city;
            State = state;
            Address1 = addressLine1;
            Address2 = addressLine2;
            ZipCode = int.Parse(zipCode);
        }

        /// <summary>
        /// Update tier 2 information
        /// </summary>
        public void UpdateTier3Information(string socialSecurityNumber,string nationalIdentificationNumber)
        {
            SocialSecurityNumber = socialSecurityNumber;
            NationalIdentificationNumber = nationalIdentificationNumber;
        }

        /// <summary>
        /// Get user speific tier level status
        /// </summary>
        /// <param name="tier"></param>
        /// <returns></returns>
        public string GetTierLevelStatus(Tier tier)
        {
            for (int i = 0; i < _tierLevelStatuses.Count; i++)
            {
                if (tier.TierLevel.Equals(_tierLevelStatuses[i].Tier.TierLevel))
                    return _tierLevelStatuses[i].Status.ToString();
            }
            throw new ArgumentException("Tier does not exist");
        }

        /// <summary>
        /// Get all tiers status of the user
        /// </summary>
        /// <returns></returns>
        public UserTierLevelStatus[] GetAllTiersStatus()
        {
            return _tierLevelStatuses.ToArray();
        }


        /// <summary>
        /// Adds a new Forgot Password code to this user
        /// </summary>
        /// <param name="forgotPasswordCode"></param>
        /// <returns></returns>
        public bool AddForgotPasswordCode(string forgotPasswordCode)
        {
            // Only assign the ForgotPasswordCode if this is the first time, or if the last ForgotPasswordCode has Expired
            if ((this.ForgotPasswordCode == null && this.ForgotPasswordCodeExpiration == null) ||
                this.ForgotPasswordCodeExpiration < DateTime.Now)
            {
                // Assign the latest code
                this.ForgotPasswordCode = forgotPasswordCode;
                this.ForgotPasswordCodeExpiration = DateTime.Now.AddHours(2);
                // Add this code to the list of the Forgotten Password Codes till date for this user
                _forgottenPasswordCodesList.Add(new PasswordCodeRecord(forgotPasswordCode,
                                                                       Convert.ToDateTime(
                                                                       this.ForgotPasswordCodeExpiration),
                                                                       DateTime.Now, Id));
                return true;
            }
            else
            {
                throw new InvalidOperationException("Last Forgot PasswordCode request hasn't expired yet.");
            }
        }

        /// <summary>
        /// Checks if this user's latest ForgotPasswordCode's validity period still remaining. Returns true if yes
        /// </summary>
        /// <returns></returns>
        public bool IsPasswordCodeValid(string resetPasswordCode)
        {
            if (!string.IsNullOrEmpty(this.ForgotPasswordCode))
            {
                if (this.ForgotPasswordCodeExpiration != null && this.ForgotPasswordCodeExpiration > DateTime.Now)
                {
                    // Iterate to see which Password code has been used
                    foreach (var passwordCodeRecord in _forgottenPasswordCodesList)
                    {
                        if (passwordCodeRecord.ExpirationDateTime.Equals(this.ForgotPasswordCodeExpiration) && 
                            passwordCodeRecord.ForgotPasswordCode.Equals(this.ForgotPasswordCode)&&ForgotPasswordCode.Equals(resetPasswordCode))
                        {
                            // No active ForgotPassword code at the moment
                            this.ForgotPasswordCode = null;
                            this.ForgotPasswordCodeExpiration = null;
                            // Mark that this Password Code has been used to reset the password
                            passwordCodeRecord.MarkCodeUsage();
                            return true;
                        }
                    }
                    throw new InvalidOperationException("ForgotPasswordCode in user does not match any Code in the FOrgottenPasswordCodes List.");
                }
                else
                {
                    // No active ForgotPassword code at the moment
                    this.ForgotPasswordCode = null;
                    this.ForgotPasswordCodeExpiration = null;
                    throw new InvalidOperationException("Timeout expired for resetting the password or no request has been made to reset password.");
                }
            }
            else
            {
                throw new NullReferenceException("Password Code is null or does not contain any value");
            }
        }

        /// <summary>
        /// Change the settings for this User
        /// </summary>
        /// <returns></returns>
        public bool ChangeSettings(string email, string pgpPublicKey, Language language, TimeZone timeZone, 
            bool isDefaultAutoLogout, int autoLogoutMinutes)
        {
            if (!string.IsNullOrEmpty(email))
            {
                this._email = email;
            }
            if (!string.IsNullOrEmpty(pgpPublicKey))
            {
                this._pgpPublicKey = pgpPublicKey;
            }
            _language = language;
            _timeZone = timeZone;
            if (isDefaultAutoLogout)
            {
                // If the Auto Logout time is set as default, assign it 10 minutes
                this.AssignAutoLogoutTime(10);
                return true;
            }
            else
            {
                if (autoLogoutMinutes > 0)
                {
                    // Else, assign the custom user defined time
                    this.AssignAutoLogoutTime(autoLogoutMinutes);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Assigns the Auto Logout Time
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        private bool AssignAutoLogoutTime(int minutes)
        {
            _autoLogout = new TimeSpan(0, 0, minutes, 0);
            return true;
        }

        /// <summary>
        /// Admin emails subscription
        /// </summary>
        /// <param name="adminEmailsSubcribed"></param>
        /// <returns></returns>
        public bool SetAdminEmailSubscription(bool adminEmailsSubcribed)
        {
            AdminEmailsSubscribed = adminEmailsSubcribed;
            return true;
        }

        /// <summary>
        /// NewsLetter email subscription
        /// </summary>
        /// <param name="adminEmailsSubcribed"></param>
        /// <returns></returns>
        public bool SetNewsletterEmailSubscription(bool adminEmailsSubcribed)
        {
            AdminEmailsSubscribed = adminEmailsSubcribed;
            return true;
        }

        #region MFA Methods

        /// <summary>
        /// Subscribe to MFA using Email Address
        /// </summary>
        /// <returns></returns>
        public bool SubscribeEmailMfa()
        {
            if (!string.IsNullOrEmpty(_email))
            {
                _emailMfaEnabled = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Subscribe to MFA using Phone Number
        /// </summary>
        /// <returns></returns>
        public bool SubscribeToSmsMfa(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                _emailMfaEnabled = false;
                _phoneNumber = phoneNumber;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Subscribe to Login MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeLoginMfa()
        {
            return _mfaSubscriptions.SubscribeLoginMfa();
        }

        /// <summary>
        /// Subscribe to Deposit MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeDepositMfa()
        {
            return _mfaSubscriptions.SubscribeDepositMfa();
        }

        /// <summary>
        /// Subscribe to Withdrawal MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeWithdrawalMfa()
        {
            return _mfaSubscriptions.SubscribeWithdrawalMfa();
        }

        /// <summary>
        /// Subscribe to Place Order MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribePlaceOrderMfa()
        {
            return _mfaSubscriptions.SubscribePlaceOrderMfa();
        }

        /// <summary>
        /// Subscribe to Cancel Order MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeCancelOrderMfa()
        {
            return _mfaSubscriptions.SubscribeCancelOrderMfa();
        }

        /// <summary>
        /// Check if the given method has been subscribed for MFA or not
        /// </summary>
        /// <returns></returns>
        public bool CheckMfaSubscription(string currentAction)
        {
            switch (currentAction)
            {
                // If current action is login, check if it has been subscribed for MFA by the user or not
                case MfaConstants.Login:
                    return _mfaSubscriptions.LoginMfaEnabled;
                // If current action is Deposit, check if it has been subscribed for MFA by the user or not
                case MfaConstants.Deposit:
                    return _mfaSubscriptions.DepositMfaEnabled;
                // If current action is Withdraw, check if it has been subscribed for MFA by the user or not
                case MfaConstants.Withdrawal:
                    return _mfaSubscriptions.WithdrawalMfaEnabled;
                // If current action is Placing a New Order, check if it has been subscribed for MFA by the user or not
                case MfaConstants.PlaceOrder:
                    return _mfaSubscriptions.PlaceOrderMfaEnabled;
                // If current action is Cancelling an order, check if it has been subscribed for MFA by the user or not
                case MfaConstants.CancelOrder:
                    return _mfaSubscriptions.CancelOrderMfaEnabled;
                // If no action found, return null;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Specifies whether the user has subscribed to TFA using Email or SMS. 
        /// Returns True + EmailAddress if email is subscribed
        /// Returns False + Phone Number if SMS is subscribed
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, string> IsEmailMfaEnabled()
        {
            // If Email TFA is enabled
            if (_emailMfaEnabled)
            {
                // Email must be present
                if (!string.IsNullOrEmpty(this.Email))
                {
                    return new Tuple<bool, string>(true, this.Email);
                }
                // Throw exception if no email is yet present
                else
                {
                    throw new InvalidOperationException(string.Format("No Email Provided for TFA Subscription: Username = {0}",
                        Username));
                }
            }
            // If SMS TFA is enabled
            else
            {
                // Phone number must be present
                if (!string.IsNullOrEmpty(_phoneNumber))
                {
                    return new Tuple<bool, string>(false, this._phoneNumber);
                }
                // Throw exception if no phone number is present
                else
                {
                    throw new InvalidOperationException(string.Format("No Phone Number Provided for TFA Subscription: Username = {0}",
                        Username));
                }
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

        /// <summary>
        /// Assign the given MFA code
        /// </summary>
        /// <param name="mfaCode"></param>
        /// <returns></returns>
        public bool AssignMfaCode(string mfaCode)
        {
            if (!string.IsNullOrEmpty(mfaCode))
            {
                this.MfaCode = mfaCode;
                return true;
            }
            return false;
        }

        #endregion MFA Methods

        #endregion Methods

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
        /// Address 1
        /// </summary>
        public string Address1 { get { return _address1; } set { _address1 = value; } }

        /// <summary>
        /// Address 2
        /// </summary>
        public string Address2 { get { return _address2; } set { _address2 = value; } }
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
        /// Specifies whether this user account uses the default logout time of 10 minutes or custom defined by user
        /// </summary>
        public bool IsDefaultAutoLogout
        {
            get
            {
                if (_autoLogout == new TimeSpan(0, 0, 10, 0))
                {
                    return true;
                }
                return false;
            }
            set
            {
                _isDefaultAutoLogout = value;
            }
        }

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

        /// <summary>
        /// Activation Key
        /// </summary>
        public string ActivationKey
        {
            get { return _activationKey; }
            set
            {
                _activationKey = value;
            }
        }

        /// <summary>
        /// Has the Activation Key already been used to activate this account
        /// </summary>
        public IsActivationKeyUsed IsActivationKeyUsed { get; set; }

        /// <summary>
        /// Is this User blocked
        /// </summary>
        public IsUserBlocked IsUserBlocked { get; set; }

        /// <summary>
        /// List of Ueser Docuemnts associated with this User
        /// </summary>
        public UserDocumentsList UserDocumentsList { get; set; }

        /// <summary>
        /// Is this user deleted softly from the database
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Code generated in case user has forgotten their password
        /// </summary>
        public string ForgotPasswordCode { get; set; }

        /// <summary>
        /// Represents an array of the forgotten password codes for this user
        /// </summary>
        public PasswordCodeRecord[] ForgottenPasswordCodes { get { return _forgottenPasswordCodesList.ToArray(); } }

        /// <summary>
        /// Validity Period of the latest ForgotPasswordCode
        /// </summary>
        public DateTime? ForgotPasswordCodeExpiration { get; private set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// Zip code
        /// </summary>
        public int ZipCode { get; private set; }

        /// <summary>
        /// SSN for US citizens only
        /// </summary>
        public string SocialSecurityNumber { get; private set; }

        /// <summary>
        /// Government Issued ID number
        /// </summary>
        public string NationalIdentificationNumber { get; private set; }

        /// <summary>
        /// Full Name of the User
        /// </summary>
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
            }
        }

        /// <summary>
        /// PGP Public Key
        /// </summary>
        public string PgpPublicKey { get { return _pgpPublicKey; } private set { _pgpPublicKey = value; } }

        /// <summary>
        /// Are Administrative emails subscribed
        /// </summary>
        public bool AdminEmailsSubscribed { get; private set; }

        /// <summary>
        /// Are Newsletter emails subscribed
        /// </summary>
        public bool NewsletterEmailsSubscribed { get; private set; }

        /// <summary>
        /// MFA Subscriptions instance
        /// </summary>
        public MfaSubscriptions MfaSubscriptions { get { return _mfaSubscriptions; } set { _mfaSubscriptions = value; } }

        /// <summary>
        /// One-Time Two Factor Authorization Code
        /// </summary>
        public string MfaCode { get; private set; }
    }
}