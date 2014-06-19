using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// VO to represent tier 2 details of the user
    /// </summary>
    public class Tier2Details
    {
        public string AddressLine1 {get; private set;}
        public string AddressLine2 { get; private set; }
        public string AddressLine3 { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="zipCode"></param>
        public Tier2Details(string country, string addressLine1, string addressLine2, string addressLine3, string state, string city, string zipCode)
        {
            Country = country;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressLine3 = addressLine3;
            State = state;
            City = city;
            ZipCode = zipCode;
        }
    }
}
