using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Stores the Forgotten Password Code and all the related information
    /// </summary>
    public class PasswordCodeRecord
    {
        /// <summary>
        /// For datatabse primary key
        /// </summary>
        private int _id { get; set; }

        private string _username;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordCodeRecord()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCodeRecord"/> class.
        /// </summary>
        public PasswordCodeRecord(string forgotPasswordCode, DateTime expirationDateTime, DateTime creationDateTime,string userName)
        {
            ForgotPasswordCode = forgotPasswordCode;
            ExpirationDateTime = expirationDateTime;
            CreationDateTime = creationDateTime;
            Username = userName;
        }

        /// <summary>
        /// Mark that this code was used before it's expiration
        /// </summary>
        public void MarkCodeUsage()
        {
            this.IsUsed = true;
        }

        /// <summary>
        /// The Code
        /// </summary>
        public string ForgotPasswordCode { get; private set; }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public DateTime ExpirationDateTime { get; private set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime CreationDateTime { get; private set; }

        /// <summary>
        /// Has this Code been used to reset password before it's expiration?
        /// </summary>
        public bool IsUsed { get; private set; }

        /// <summary>
        /// UserName of the User
        /// </summary>
        public string Username 
        {
            get { return _username; }
            private set
            {
                AssertionConcern.AssertNullOrEmptyString(value,"UserName cannot be null or empty");
                _username = value;
            } 
        }
    }
}
