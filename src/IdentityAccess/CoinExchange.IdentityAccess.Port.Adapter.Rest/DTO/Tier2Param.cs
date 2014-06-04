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
        public Tier2Param()
        {
            
        }

        public string AddressLine1 {get; set;}
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

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

        /// <summary>
        /// Custom to string method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                string.Format("AddressLine1:{0}, AddressLine2:{1}, AddressLine3:{2}, State:{3}, City:{4}, ZipCode:{5}",
                    AddressLine1, AddressLine2, AddressLine3, State, City, ZipCode);
        }
    }
}
