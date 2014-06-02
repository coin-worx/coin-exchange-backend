using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// paramters to be taken for tier 2 verification
    /// </summary>
    public class Tier2Param
    { 
        public string AddressLine1 {get; private set;}
        public string AddressLine2 { get; private set; }
        public string AddressLine3 { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }
        public string ZipCode { get; private set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="addressLine1"></param>
        /// <param name="addressLine2"></param>
        /// <param name="addressLine3"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="zipCode"></param>
        public Tier2Param(string addressLine1, string addressLine2, string addressLine3, string state, string city, string zipCode)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressLine3 = addressLine3;
            State = state;
            City = city;
            ZipCode = zipCode;
        }
    }
}
