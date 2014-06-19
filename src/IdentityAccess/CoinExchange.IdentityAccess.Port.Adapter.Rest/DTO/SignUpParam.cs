using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters to be taken for signup
    /// </summary>
    public class SignUpParam
    {
        //default constructor
        public SignUpParam()
        {
            
        }
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="country"></param>
        /// <param name="timeZone"></param>
        /// <param name="pgpPublicKey"></param>
        public SignUpParam(string email, string username, string password, string country, TimeZone timeZone, string pgpPublicKey)
        {
            Email = email;
            Username = username;
            Password = password;
            Country = country;
            TimeZone = timeZone;
            PgpPublicKey = pgpPublicKey;
        }

        /// <summary>
        /// Email
        /// </summary>       
        public string Email { get;  set; }

        /// <summary>
        /// Username
        /// </summary>       
        public string Username { get;  set; }

        /// <summary>
        /// Password
        /// </summary>       
        public string Password { get;  set; }

        /// <summary>
        /// Country
        /// </summary>  
        public string Country { get;  set; }

        /// <summary>
        /// TimeZone
        /// </summary>       
        public TimeZone TimeZone { get;  set; }

        /// <summary>
        /// PGPPublicKey
        /// </summary>       
        public string PgpPublicKey { get;  set; }

        public override string ToString()
        {
            return string.Format("UserName:{0},Password{1},Email:{2},Country:{3},PgpPublic Key:{4},TimeZone:{5}",
                Username, Password.GetHashCode(), Email, Country, PgpPublicKey, TimeZone);
        }
    }
}
