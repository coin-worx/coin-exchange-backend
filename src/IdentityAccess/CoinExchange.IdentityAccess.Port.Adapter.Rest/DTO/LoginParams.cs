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
        public string UserName { get; private set; }
        public string Password { get; private set; }

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
        /// custom to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("UserName:{0},Password{1}", UserName, Password.GetHashCode());
        }
    }
}
