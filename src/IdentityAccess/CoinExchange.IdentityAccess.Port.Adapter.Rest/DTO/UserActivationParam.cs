using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// dto to get param needed for user account activation
    /// </summary>
    public class UserActivationParam
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string ActivationKey { get; private set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="activationKey"></param>
        public UserActivationParam(string userName, string password, string activationKey)
        {
            UserName = userName;
            Password = password;
            ActivationKey = activationKey;
        }

        /// <summary>
        /// custome to string method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("UserName:{0},Password:{1},ActivationKey{2}", UserName, Password.GetHashCode(),
                ActivationKey);
        }
    }
}
