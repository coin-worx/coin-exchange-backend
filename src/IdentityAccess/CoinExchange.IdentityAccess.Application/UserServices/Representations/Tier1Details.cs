using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// VO to represent Tier 1 details of the user.
    /// </summary>
    public class Tier1Details
    {
        public string PhoneNumber { get; private set; }
        public string FullName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Country { get; private set; }
        
        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="phoneNumber"></param>
        public Tier1Details(string phoneNumber, string fullName, DateTime dateOfBirth, string country)
        {
            PhoneNumber = phoneNumber;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Country = country;
        }
    }
}
