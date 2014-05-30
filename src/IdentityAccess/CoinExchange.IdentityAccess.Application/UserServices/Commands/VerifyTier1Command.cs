using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// VO for tier 0 requirements
    /// </summary>
    public class VerifyTier1Command
    {
        public string PhoneNumber { get; private set; }
        public string SystemGeneratedApiKey { get; private set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="systemGeneratedApiKey"></param>
        public VerifyTier1Command(string phoneNumber, string systemGeneratedApiKey)
        {
            AssertionConcern.AssertNullOrEmptyString(systemGeneratedApiKey,"Api key must be provided");
            AssertionConcern.AssertNullOrEmptyString(phoneNumber, "Phone number cannot be null or empty");
            PhoneNumber = phoneNumber;
            SystemGeneratedApiKey = systemGeneratedApiKey;
        }
    }
}
