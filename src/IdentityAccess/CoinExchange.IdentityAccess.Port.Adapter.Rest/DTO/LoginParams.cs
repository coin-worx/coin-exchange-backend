using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Login parameters
    /// </summary>
    public class LoginParams
    {
        public LoginParams()
        {
            
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string MfaCode { get; set; }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public LoginParams(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LoginParams(string userName, string password, string mfaCode)
        {
            UserName = userName;
            Password = password;
            MfaCode = mfaCode;
        }

        /// <summary>
        /// custom to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("UserName:{0},Password{1}", UserName, Password.GetHashCode());
        }
    }
}
