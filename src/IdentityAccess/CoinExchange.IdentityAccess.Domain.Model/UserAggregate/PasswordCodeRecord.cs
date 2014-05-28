using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Stores the Forgotten Password Code and all the related information
    /// </summary>
    public class PasswordCodeRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordCodeRecord"/> class.
        /// </summary>
        public PasswordCodeRecord(string forgotPasswordCode, DateTime expirationDateTime, DateTime creationDateTime)
        {
            ForgotPasswordCode = forgotPasswordCode;
            ExpirationDateTime = expirationDateTime;
            CreationDateTime = creationDateTime;
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
    }
}
