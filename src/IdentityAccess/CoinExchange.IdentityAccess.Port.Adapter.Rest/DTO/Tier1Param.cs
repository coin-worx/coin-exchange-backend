using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// VO to take tier 1 input params
    /// </summary>
    public class Tier1Param
    {
        public Tier1Param()
        {
            
        }

        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="phoneNumber"></param>
        public Tier1Param(string fullName, string dateOfBirth, string phoneNumber)
        {
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Custom to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("FullName:{0}, DateOfBirth{1}, PhoneNumber{2}", FullName, DateOfBirth, PhoneNumber);
        }
    }
}
