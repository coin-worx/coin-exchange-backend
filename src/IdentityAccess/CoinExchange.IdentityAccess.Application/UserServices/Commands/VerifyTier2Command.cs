using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// VO to gather tier 1 requirements
    /// </summary>
    public class VerifyTier2Command
    {
        public string SystemGeneratedApiKey { get; private set; }
        public string AddressLine1 {get; private set;}
        public string AddressLine2 { get; private set; }
        public string AddressLine3 { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }
        public string ZipCode { get; private set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="systemGeneratedApiKey"></param>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="zipCode"></param>
        public VerifyTier2Command(string systemGeneratedApiKey, string addressLine1, string addressLine2, string addressLine3, string state, string city, string zipCode)
        {
            SystemGeneratedApiKey = systemGeneratedApiKey;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressLine3 = addressLine3;
            State = state;
            City = city;
            ZipCode = zipCode;
        }

        /// <summary>
        /// Validate command
        /// </summary>
        /// <returns></returns>
        public bool ValidateCommand()
        {
            if (string.IsNullOrEmpty(AddressLine1) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(State))
            {
                throw new ArgumentNullException("Provide all the necessary information");
            }
            return true;
        }
    }
}
