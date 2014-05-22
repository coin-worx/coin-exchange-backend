using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies the address for the User
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Address()
        {
            
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        public Address(string address1, string address2)
        {
            this.Address1 = address1;
            this.Address2 = address2;
        }
        /// <summary>
        /// Address1
        /// </summary>
        public string Address1 { get; private set; }

        /// <summary>
        /// Address2
        /// </summary>
        public string Address2 { get; private set; }

        /// <summary>
        /// Overriden equal method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Address)
            {
                Address address = obj as Address;
                return Address1.Equals(address.Address1, StringComparison.InvariantCultureIgnoreCase) &&
                       Address2.Equals(address.Address2, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }
    }
}
