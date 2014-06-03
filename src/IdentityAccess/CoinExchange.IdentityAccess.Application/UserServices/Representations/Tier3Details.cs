using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// VO to represeent tier 3 detils of the user
    /// </summary>
    public class Tier3Details
    {
        public string Ssn { get; private set; }
        public string Nin { get; private set; }
        
        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="socialSecurityNumber"></param>
        /// <param name="nin"></param>
        /// <param name="documentType"></param>
        public Tier3Details(string socialSecurityNumber, string nin)
        {
            Ssn = socialSecurityNumber;
            Nin = nin;
        }
    }
}
