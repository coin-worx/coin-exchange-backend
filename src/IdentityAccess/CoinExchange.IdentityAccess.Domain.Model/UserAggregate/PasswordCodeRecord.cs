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

        private int _userId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordCodeRecord()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCodeRecord"/> class.
        /// </summary>
        public PasswordCodeRecord(string forgotPasswordCode, DateTime expirationDateTime, DateTime creationDateTime,int userId)
        {
            ForgotPasswordCode = forgotPasswordCode;
            ExpirationDateTime = expirationDateTime;
            CreationDateTime = creationDateTime;
            UserId = userId;
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
        public int UserId 
        {
            get { return _userId; }
            private set
            {
                _userId = value;
            } 
        }
    }
}
